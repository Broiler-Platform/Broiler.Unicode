#!/usr/bin/env bash
# Bundle sjl-7-001's reference-arm comparison, run at one commit: reference-arms.md, as revised in
# reference-arms-revised-2026-10-16-after-first-run.md.
#
#   docs/evidence/sjl-7-001/run-reference-arms.sh <output-directory> <bundle-directory>
#
# The run happens in a detached git worktree of HEAD, so it names exactly one commit; the tree must be clean.
# Every step's command is the first line of its log. The large outputs (the population, the arbiter's answers
# and the four listings) stay in <output-directory>; the bundle directory receives only what the rule retains:
# each run's summary.json, classification.json and report.md, the population's manifest.json, the arbiter's
# reference-manifest.json, the negative controls' classification, the logs, and the arms' sources, which are
# the commit's own. The script stops at the first step that does not end as the rule expects.
set -euo pipefail

out=$(realpath -m "${1:?usage: run-reference-arms.sh <output-directory> <bundle-directory>}")
bundle=$(realpath -m "${2:?usage: run-reference-arms.sh <output-directory> <bundle-directory>}")
root=$(git rev-parse --show-toplevel)
cd "$root"

if [ -n "$(git status --porcelain --untracked-files=no)" ]; then
  echo "a tracked file is changed, and the bundle names a commit" >&2
  exit 2
fi

commit=$(git rev-parse HEAD)
mkdir -p "$out" "$bundle"
work=$(mktemp -d)
git worktree add --detach "$work/tree" "$commit" >/dev/null
trap 'cd "$root"; git worktree remove --force "$work/tree"; rm -rf "$work"' EXIT
cd "$work/tree"

# The rule's configuration: not invariant globalization (arm J reads the host's ICU); the comparison sets the
# invariant culture itself and records it.
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
unset DOTNET_SYSTEM_GLOBALIZATION_INVARIANT

{
  echo "commit $commit"
  echo "taken $(date -u +%Y-%m-%dT%H:%M:%SZ)"
  echo "kernel $(uname -srm)"
  echo "sdk $(dotnet --version)"
  dotnet --list-runtimes
  echo "python $(python3 -I -c 'import sys, unicodedata; print(sys.version.split()[0], "unicodedata", unicodedata.unidata_version)')"
} > "$bundle/environment.txt"

step() {
  local log=$1
  shift
  echo "\$ $*" > "$bundle/$log.log"
  if ! "$@" >> "$bundle/$log.log" 2>&1; then
    echo "step $log failed; see $bundle/$log.log" >&2
    exit 1
  fi
}

# classify <log> <run>: the classifier's exit status is 0 when nothing is class (c), 1 otherwise; both are kept.
classify() {
  local log=$1 run=$2
  echo "\$ python3 -I scripts/reference-arms/classify_reference_arms.py --run $run --reference $out/reference" > "$bundle/$log.log"
  local status=0
  python3 -I scripts/reference-arms/classify_reference_arms.py --run "$run" --reference "$out/reference" >> "$bundle/$log.log" 2>&1 || status=$?
  echo "exit $status" >> "$bundle/$log.log"
  if [ "$status" -gt 1 ]; then
    echo "step $log refused (exit $status); see $bundle/$log.log" >&2
    exit 1
  fi
}

arms="dotnet $out/build/bin/Unicode.ReferenceArms/release/Unicode.ReferenceArms.dll"

step build dotnet build tests/Unicode.ReferenceArms -c Release --artifacts-path "$out/build"
step script-tests python3 -m unittest discover -s scripts/reference-arms/tests
step population python3 -I scripts/reference-arms/generate_population.py --out "$out/population"
step reference python3 -I scripts/reference-arms/unicoderef.py --population "$out/population" --out "$out/reference"

step compare $arms compare --population "$out/population" --out "$out/run"
step compare-tr-TR $arms compare --population "$out/population" --out "$out/run-tr-TR" --culture tr-TR
step compare-lt-LT $arms compare --population "$out/population" --out "$out/run-lt-LT" --culture lt-LT
step negative-control $arms compare --population "$out/population" --out "$out/negative-control" --negative-control

classify classify "$out/run"
classify classify-tr-TR "$out/run-tr-TR"
classify classify-lt-LT "$out/run-lt-LT"
classify classify-negative-control "$out/negative-control"

# What the rule retains, beside the logs.
for run in run run-tr-TR run-lt-LT negative-control; do
  mkdir -p "$bundle/$run"
  cp "$out/$run/summary.json" "$out/$run/classification.json" "$out/$run/report.md" "$bundle/$run/"
done
# The two planted lines as the controls' classification lists them: each a difference, neither classified.
zcat "$out/negative-control/classification.tsv.gz" | awk -F'\t' 'NR == 1 ||
  ($1 == "lexical" && $3 == "0041" && $4 == "IdentifierStartChar" && $5 ~ /^J-control/) ||
  ($1 == "charge" && $2 == "U4" && $4 ~ /NFC;max=16777216/ && $5 ~ /^V-control/ && !seen++)' > "$bundle/negative-control/planted.tsv"
cp "$out/population/manifest.json" "$bundle/population-manifest.json"
cp "$out/reference/reference-manifest.json" "$bundle/reference-manifest.json"

# The verdict lines, read from the classifications.
for run in run run-tr-TR run-lt-LT negative-control; do
  echo "$run: $(python3 -I -c 'import json, sys; d = json.load(open(sys.argv[1])); print(d["differences"], "differences;", json.dumps(d["verdicts"]), "; unnamed", d["unnamed by the correspondence"], "; listing", d["differences.tsv"]["sha256"], d["differences.tsv"]["lines"], "lines")' "$bundle/$run/classification.json")"
done > "$bundle/summary.txt"
echo "every step ended" >> "$bundle/summary.txt"
