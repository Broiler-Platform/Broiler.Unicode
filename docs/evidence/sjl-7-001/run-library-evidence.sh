#!/usr/bin/env bash
# Bundle sjl-7-001's library evidence: decision-rule.md's parts A to D, run at one commit.
#
#   docs/evidence/sjl-7-001/run-library-evidence.sh <output-directory> <Broiler.Code checkout>
#
# The run happens in a detached git worktree of HEAD, so the bundle names exactly one commit and the
# witness mutations of part C never touch the caller's checkout. The tree must be clean. Every step's
# command is the first line of its log. The script stops at the first step that does not end as the rule
# expects: a check that fails, or a witness whose mutation is not caught.
set -euo pipefail

out=$(realpath -m "${1:?usage: run-library-evidence.sh <output-directory> <Broiler.Code checkout>}")
code=$(realpath "${2:?usage: run-library-evidence.sh <output-directory> <Broiler.Code checkout>}")
root=$(git rev-parse --show-toplevel)
cd "$root"

if [ -n "$(git status --porcelain --untracked-files=no)" ]; then
  echo "a tracked file is changed, and the bundle names a commit" >&2
  exit 2
fi

commit=$(git rev-parse HEAD)
mkdir -p "$out"
work=$(mktemp -d)
git worktree add --detach "$work/tree" "$commit" >/dev/null
trap 'cd "$root"; git worktree remove --force "$work/tree"; rm -rf "$work"' EXIT
cd "$work/tree"

# Part A's configuration: the invariant culture, and the globalization mode recorded.
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1

{
  echo "commit $commit"
  echo "taken $(date -u +%Y-%m-%dT%H:%M:%SZ)"
  echo "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=$DOTNET_SYSTEM_GLOBALIZATION_INVARIANT: the current culture is the invariant culture"
  echo "kernel $(uname -srm)"
  echo "sdk $(dotnet --version)"
  dotnet --list-runtimes
  echo "Broiler.Code $(git -C "$code" rev-parse HEAD)"
} > "$out/environment.txt"

# step <log> <command...>: the command must succeed.
step() {
  local log=$1
  shift
  echo "\$ $*" > "$out/$log.log"
  if ! "$@" >> "$out/$log.log" 2>&1; then
    echo "step $log failed; see $out/$log.log" >&2
    exit 1
  fi
}

# refuted <log> <command...>: the command must fail, because a witness's mutation is in place.
refuted() {
  local log=$1
  shift
  echo "\$ $*   # must fail" > "$out/$log.log"
  if "$@" >> "$out/$log.log" 2>&1; then
    echo "witness $log was not caught; see $out/$log.log" >&2
    exit 1
  fi
}

test_run() {
  local project=$1 filter=$2 name=$3
  dotnet test "$project" -c Release --no-build -f net10.0 --filter "$filter" \
    --logger "console;verbosity=detailed" --logger "trx;LogFileName=$name.trx" --results-directory "$out/trx"
}

review() {
  dotnet run --project "$code/src/Broiler.Code.Review.Cli" -c Release -- "$@"
}

step build dotnet build Broiler.Unicode.slnx -c Release

# Part A: Unicode's test files, every case (A1 to A5), and the sets against the UCD files (A6).
step part-a test_run tests/Unicode.Algorithms.Tests \
  "FullyQualifiedName~UnicodeTestFileTests|FullyQualifiedName~EcmaScriptTableTests|FullyQualifiedName~UcdPropertyTests" part-a
step part-a-probe-jit dotnet probes/Unicode.Probe/bin/Release/net10.0/Broiler.Unicode.Probe.dll data/unicode/pins --report "$out/probe-jit.txt"

# Part B: generation byte for byte, in two processes, with the pin against Broiler.VM's and the three
# negative controls (the Control_ tests change a byte of a table, a hash of the pin and a byte of an input).
step part-b-run-1 test_run tests/Unicode.Algorithms.Tests "FullyQualifiedName~RegenerationTests" part-b-run-1
step part-b-run-2 test_run tests/Unicode.Algorithms.Tests "FullyQualifiedName~RegenerationTests" part-b-run-2

# Part C: the contract, C-1 to C-5 in the test project and C-6 as rule SL4.
step part-c test_run tests/Unicode.Algorithms.Tests "FullyQualifiedName~ContractTests" part-c
step part-c-old-package test_run tests/UnicodeProperties.Tests "FullyQualifiedName~RangeLookupTests" part-c-old-package
step part-c-6 test_run tests/Unicode.Architecture.Tests "FullyQualifiedName~SL4_" part-c-6

# Part C's witnesses: each check watched failing once against a mutation, then passing on the revert.
witness() {
  local name=$1 file=$2 from=$3 to=$4 project=$5 filter=$6
  grep -qF -- "$from" "$file" || { echo "witness $name: '$from' is not in $file" >&2; exit 1; }
  python3 -I -c 'import sys; p, a, b = sys.argv[1:4]; s = open(p, encoding="utf-8").read(); open(p, "w", encoding="utf-8", newline="\n").write(s.replace(a, b))' "$file" "$from" "$to"
  step "witness-$name-build" dotnet build "$project" -c Release
  refuted "witness-$name-caught" test_run "$project" "$filter" "witness-$name-caught"
  git checkout -- "$file"
  step "witness-$name-revert-build" dotnet build "$project" -c Release
  step "witness-$name-reverted" test_run "$project" "$filter" "witness-$name-reverted"
}

witness C-1 src/UnicodeProperties/UnicodeProperties.cs \
  "Broiler.Unicode.Algorithms</c>' exact lookup" "another package's lookup" \
  tests/Unicode.Algorithms.Tests "FullyQualifiedName~C1_"
witness C-2 src/UnicodeProperties/UnicodeProperties.cs \
  "return ranges.Length == 0 ? ranges : ((int Lo, int Hi)[])ranges.Clone();" "return ranges;" \
  tests/UnicodeProperties.Tests "FullyQualifiedName~RangeLookupTests"
witness C-3 src/Unicode.Algorithms/UnicodeAlgorithms.cs \
  'public const string UnicodeVersion = "17.0.0";' 'public const string UnicodeVersion = "16.0.0";' \
  tests/Unicode.Algorithms.Tests "FullyQualifiedName~C3_"
witness C-4 src/Unicode.Algorithms/UnicodeNormalization.cs \
  "Charge(charge, text.Length + (points * 3));" "Charge(charge, text.Length + (points * 2));" \
  tests/Unicode.Algorithms.Tests "FullyQualifiedName~C4_normalization_charges"
witness C-5 src/Unicode.Algorithms/UnicodeProperties.cs \
  "set = id < 0 ? default : Set(id);" 'set = id < 0 ? throw new ArgumentException("unknown", nameof(name)) : Set(id);' \
  tests/Unicode.Algorithms.Tests "FullyQualifiedName~C5_"

# Part D: the conventions' rules, the assurance gate with an injected drift, and the Native AOT probe,
# over a build of the commit itself again, every witness reverted.
step build-again dotnet build Broiler.Unicode.slnx -c Release
step part-d-rules test_run tests/Unicode.Architecture.Tests "FullyQualifiedName~Broiler.Unicode.Architecture.Tests" part-d-rules
step part-d-assurance review assurance check --root src
python3 -I -c 'import sys; p = sys.argv[1]; s = open(p, encoding="utf-8").read(); open(p, "w", encoding="utf-8", newline="\n").write(s.replace("public const int MaxCodePoint = 0x10FFFF;", "public const int MaxCodePoint = 0x10FFFF + 0;"))' src/Unicode.Algorithms/UnicodeAlgorithms.cs
refuted part-d-assurance-drift-caught review assurance check --root src
git checkout -- src/Unicode.Algorithms/UnicodeAlgorithms.cs
step part-d-assurance-drift-reverted review assurance check --root src

step part-d-aot-publish dotnet publish probes/Unicode.Probe -c Release -r linux-x64
if grep -E "warning [A-Z]+[0-9]+" "$out/part-d-aot-publish.log"; then
  echo "the Native AOT publish reported a warning" >&2
  exit 1
fi
step part-d-aot-run probes/Unicode.Probe/bin/Release/net10.0/linux-x64/publish/Broiler.Unicode.Probe data/unicode/pins --report "$out/probe-aot.txt"
step part-d-aot-compare cmp "$out/probe-aot.txt" "$out/probe-jit.txt"

# The run's verdict lines, read from the logs.
for log in "$out"/part-*.log "$out"/witness-*.log; do
  echo "$(basename "$log" .log): $(grep -hE '^ +(Passed|Failed):|Total tests:|no violations|[0-9]+ violations' "$log" | tr -s ' ' | tr '\n' ' ')"
  grep -hE '^ A[1-6] ' "$log" | sed 's/^ /  /' || true
done > "$out/summary.txt"
echo "every step ended as the rule expects" >> "$out/summary.txt"
