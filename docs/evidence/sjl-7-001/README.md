# Bundle SJL-7-001 - the Unicode area: the owner's rules and evidence

**Milestone:** SJL-7 of the shared JavaScript libraries programme. The milestone itself is
`docs/javascript-shared-libraries/milestones/sjl-7-unicode.md` in the Broiler.VM repository.

**Rules:**
- [`decision-rule.md`](decision-rule.md): parts A to D, the owner's component clauses 1 to 4.
- [`reference-arms.md`](reference-arms.md): the library against both engines' implementations.

Both were written at SJL-7.1, before any code of the milestone existed here, and neither has changed since
(this bundle's [`manifest.json`](manifest.json) lists their digests). The reference-arm rule has one dated
revision, written after its first run was seen:
[`reference-arms-revised-2026-10-16-after-first-run.md`](reference-arms-revised-2026-10-16-after-first-run.md).

**Records:**
- This repository's [roadmap](../../roadmap.md): the owner conditions adopted at SJL-7.1, and SJL-7.2's
  record.
- Broiler.VM's JSD-0074, the area verdict, with its correspondence `docs/evidence/sjl-7-001/correspondence.md`
  in that repository.

**Owner and reviewer:** the Broiler.Unicode maintainer. One person holds every role the rules name (EX-30), and
nobody independent has read this bundle.

**State, 2026-10-16, at the end of SJL-7.2.** *(Revised 2026-10-16, when the reference-arm comparison was taken.
The heading read "at the end of SJL-7.2's tests and gates", and its last item "The reference-arm comparison is
SJL-7.2's last part. It is not taken yet, and nothing here reads it.")*
- Parts A, B and C are **MET** at `378c2f0`.
- Part D's first reading holds for D-1, D-3 and D-4. D-2 holds when this branch reaches the default branch.
- Part D's second reading, at `f3d677a` at SJL-7.6, holds for D-1, D-3 and D-4 again; D-2 still waits on the
  merge (section 5). *(Added 2026-10-17, at SJL-7.6.)*
- The reference-arm comparison is **MET** at `3f7dbce`, under its rule as revised after its first run, which
  makes it weaker evidence (section 6).

## 1. The run

[`run-library-evidence.sh`](run-library-evidence.sh) took every part at one commit, in a detached worktree of
it, and stopped at the first step that did not end as the rule expects. It did not stop. Every step's command
is the first line of its log in [`library-run/`](library-run/), the test runs' result files are in
[`library-run/trx/`](library-run/trx/), and [`library-run/summary.txt`](library-run/summary.txt) lists each
step's outcome.

| | |
|---|---|
| Commit | `378c2f02e45072f9c6fbc1a13c8f921c7c9fa3df`, the commit that lands SJL-7.2's tests and gates ([`environment.txt`](library-run/environment.txt)) |
| Taken | 2026-10-16 from 16:21:32Z, for 3 minutes 48 seconds, on the programme's cloud machine (`linux-x64`, .NET SDK 10.0.401, runtime 10.0.12), with other work running beside it; nothing in these parts is timed |
| Globalization | `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`: the current culture is the invariant culture, as part A's configuration asks |
| Broiler.Code | `54ce0b9`, for the assurance tool |

**CI at the same commit:** [run 38931323297](https://github.com/Broiler-Platform/Broiler.Unicode/actions/runs/38931323297),
dispatched on this branch, succeeded in every job:
- Linux and Windows builds and tests, on `net8.0` and `net10.0`;
- pack and package validation of the seven packages, and the consumer restore;
- the Native AOT probe on `linux-x64` and `win-x64`, each report equal to the JIT's;
- the assurance check against a fresh checkout of Broiler.Code.

## 2. Part A - Unicode's test files: MET

Every file was read under its pin: the test project's `Archive` refuses a byte the pin does not record. Every
case of A1 to A5 ran, and every case passed ([`part-a.log`](library-run/part-a.log), 17 tests).

| Case | Population | Run | Failed |
|---|---|---|---|
| A1 | `NormalizationTest.txt`, parts 0 to 3, with the header's invariants | 20,034 lines, 400,680 comparisons | 0 |
| A1 | Part 1's closing claim: every code point not in its c1 column, surrogates included, unchanged by the four forms | 1,097,026 code points, in four forms | 0 |
| A2 | `GraphemeBreakTest.txt`, `WordBreakTest.txt`, `SentenceBreakTest.txt`, every line | 766, 1,944 and 512 lines | 0 |
| A3 | `CaseFolding.txt`'s C and S lines; every other code point folds to itself; every orbit | 1,512 lines; 1,114,112 code points | 0 |
| A4 | Every code point alone, upper and lower, in the default language | 1,114,112 code points | 0 |
| A4 | Every conditional line of `SpecialCasing.txt`, where its condition holds and where it does not | 12 lines (Azeri's read as Turkish's), 32 cases | 0 |
| A5 | ECMA-262's binary property names and aliases | 98 names | 0 |
| A5 | General_Category values and aliases, alone and under both property names | 80 value names | 0 |
| A5 | Script and Script_Extensions values and aliases, `Katakana_Or_Hiragana` and `Hrkt` included | 346 value names, under 4 property names | 0 |
| A5 | The seven properties of strings | 7 names | 0 |
| A5 | `reference-arms.md`'s refused set U2 | 1,105 lone names and 49 string-property names, all refused | 0 |

**Beside the rule:**
- **Test A6** compares every set the lookups answer with the set the UCD files state, range for range.
  It is roadmap item U3's test, and it passed in the same run.
- **The probe's JIT run** of the same files is [`probe-jit.txt`](library-run/probe-jit.txt): every case holds.

## 3. Part B - generation byte for byte, and the pin: MET

`RegenerationTests` ran twice, in two processes ([`part-b-run-1.log`](library-run/part-b-run-1.log),
[`part-b-run-2.log`](library-run/part-b-run-2.log)). Each run passed all 8 tests:

- **Every table regenerates byte for byte.** The generator's output from the pinned inputs equals the
  committed `UnicodeTableData.g.cs`, all 37 tables.
- **Two generations in one process write the same bytes**, and both runs compare with the same committed
  file, so the two runs agree with each other.
- **Every table the algorithms read is generated.**
- **The pin equals Broiler.VM's**, line by line, by file name, length and SHA-256: 26 entries, against the copy
  of Broiler.VM's pin at `data/unicode/pins/broiler-vm/`, taken at `7b44dc7`.
- **The generator package carries this pin.**

**The three negative controls**, each a test of its own:
1. One byte of a committed table changed: the comparison fails and names the table.
2. One hash of the pin changed: the generator refuses the line's file before writing anything.
3. One byte of a pinned input changed, in a scratch copy: the generator refuses it.

Each control changes an in-memory copy and asserts the failure. **Its revert is the same test's unchanged
input passing in the same run**, so no file is changed and none needs restoring.

## 4. Part C - the contract: MET

| Check | Test | Its witness, watched failing once | Run |
|---|---|---|---|
| C-1 exact names | A5 decides the exact lookup; `C1_the_loose_lookup_carries_its_documented_warning` | The warning's pointer to `Broiler.Unicode.Algorithms` removed: caught, 1 of 1 failed; reverted, passed | [`witness-C-1-*`](library-run/) |
| C-2 immutable results | `C2_no_public_member_hands_out_a_mutable_reference`, `C2_boundaries_are_a_new_array_per_call`, and `RangeLookupTests` for the old package | The old package's copy removed (`return ranges;`): caught, 1 of 2 failed; reverted, 2 passed | [`witness-C-2-*`](library-run/) |
| C-3 a public version | `C3_every_package_states_the_pin_s_version`, `C3_tables_of_another_version_are_refused` | `UnicodeVersion` set to `16.0.0`: caught, 2 of 2 failed; reverted, passed | [`witness-C-3-*`](library-run/) |
| C-4 metering and limits | `C4_*`: normalization's and casing's charges against Broiler.VM's units (N7, C13), segmentation's per code point, each limit on the whole result with the maximum itself admitted, an unchanged text answered as itself, a meter that throws | Normalization's three units per decomposed code point changed to two: caught, 4 of 6 failed; reverted, 6 passed | [`witness-C-4-*`](library-run/) |
| C-5 failure as data | `C5_an_unknown_name_is_a_false_and_a_lone_surrogate_passes_through` | `TryResolve` made to throw on an unknown name: caught, 1 of 1 failed; reverted, passed | [`witness-C-5-*`](library-run/) |
| C-6 no host data | Rule SL4 over the built packages' IL ([`part-c-6.log`](library-run/part-c-6.log)) | SL4's own witness, a source that calls the platform's globalization, which the rule's test watches failing in every run | `part-c-6` |

The contract tests passed at the commit: 23 in [`part-c.log`](library-run/part-c.log) and 2 in
[`part-c-old-package.log`](library-run/part-c-old-package.log).

**C-4's lookups**, as the rule asks: the lookups and folding take no meter. A name longer than the longest the
tables hold is refused before it is read: a 1,000-character name is refused in A5's refused-set test.

## 5. Part D - the four owner conditions: first reading, at the end of SJL-7.2

| Check | Reading |
|---|---|
| D-1 conventions | **Holds.** Every element the check names is in place: <br>- [`docs/conventions.md`](../../conventions.md), with the programme's numbering. <br>- Rules SL1 to SL10, Active in `tests/Unicode.Architecture.Tests`, each with its witness watched failing in its own test: 32 tests passed ([`part-d-rules.log`](library-run/part-d-rules.log)). <br>- The consumed packages target `net10.0` with nullable enabled and implicit usings disabled, and declare `IsAotCompatible` and `IsTrimmable` (SL5). <br>- The Native AOT probe publishes with no warning ([`part-d-aot-publish.log`](library-run/part-d-aot-publish.log)), its report equals the JIT's ([`part-d-aot-compare.log`](library-run/part-d-aot-compare.log)), and it runs in CI (run 38931323297). |
| D-2 the roadmap | **Holds when this branch merges.** The default branch has carried `docs/roadmap.md` with the milestone's items since `0faa1f9`. This milestone's records, SJL-7.1's owner conditions and SJL-7.2's, are on this branch, and reach the default branch with it. |
| D-3 the assurance gate | **Holds.** <br>- Every relevant unit of the four projects carries the annotation: 247 units, 232 of them ported from Broiler.VM. <br>- Broiler.Code's tool generated the artefacts under `src/`, and they are committed. <br>- `assurance check` reports no violations ([`part-d-assurance.log`](library-run/part-d-assurance.log)), and CI runs it. <br>- An injected drift was watched failing: one constant's expression changed without regenerating gave 5 violations, J3, J5 and J7 ([`part-d-assurance-drift-caught.log`](library-run/part-d-assurance-drift-caught.log)). Reverted, it passed. <br>- [`HUMAN_REVIEW.md`](../../../HUMAN_REVIEW.md) names the revision it reviews, `89f134ed`, and its addendum says that this revision covers none of the four new packages, whose reviewed revision is none. Their per-unit record, [`src/HUMAN_REVIEW.md`](../../../src/HUMAN_REVIEW.md), reads PENDING throughout. |
| D-4 pinned and hashed inputs | **Holds**: part B is MET. |

**Part D is judged MET only if every check holds at both readings.** The second reading is at the commit this
bundle names at SJL-7.6.

### Part D's second reading, at SJL-7.6

The same script took every part again, at `f3d677ab1f18096292f3eb03d00f13ea4d4aa39c`, this branch's head
when both engines' cut-overs had landed on `0.1.0-preview.4` as published from `6e14f93`. It did not stop. Its
logs, result files and summary are in [`library-run-second-reading/`](library-run-second-reading/), laid out
as `library-run/` is.

| | |
|---|---|
| Commit | `f3d677a` ([`environment.txt`](library-run-second-reading/environment.txt)). Against the first reading's `378c2f0` it changes no file under `src/`, `probes/` or `data/`: beside the documents, it adds the reference-arm harness `tests/Unicode.ReferenceArms/` and `scripts/reference-arms/`, the harness's line in `.gitattributes` and in the rule tests' list of projects, and the changelog |
| Taken | 2026-10-16 from 23:59:26Z, on the programme's cloud machine (`linux-x64`, .NET SDK 10.0.401, runtime 10.0.12), with other work running beside it; nothing in these parts is timed |
| Broiler.Code | `54ce0b9`, the first reading's |
| CI at the same commit | [run 38984596876](https://github.com/Broiler-Platform/Broiler.Unicode/actions/runs/38984596876), dispatched on this branch |

**Every step's outcome equals the first reading's**: [`summary.txt`](library-run-second-reading/summary.txt)
differs from [`library-run/summary.txt`](library-run/summary.txt) only in the latter's last line, which notes that
its summary was rewritten by the script's corrected summary step. The probe's report under the JIT is the first
reading's byte for byte. Parts A, B and C hold again at this commit, though this bundle judges them at
`378c2f0`.

| Check | Second reading |
|---|---|
| D-1 conventions | **Holds.** Rules SL1 to SL10 pass, 32 tests ([`part-d-rules.log`](library-run-second-reading/part-d-rules.log)); the Native AOT probe publishes with no warning ([`part-d-aot-publish.log`](library-run-second-reading/part-d-aot-publish.log)) and its report equals the JIT's ([`part-d-aot-compare.log`](library-run-second-reading/part-d-aot-compare.log)); the consumed packages' project files are the first reading's; and CI runs the probe |
| D-2 the roadmap | **Holds when this branch merges, as at the first reading.** The default branch is still `0faa1f9`, an ancestor of this branch: it carries `docs/roadmap.md` with the milestone's items, and this milestone's records reach it with this branch |
| D-3 the assurance gate | **Holds.** `assurance check` reports no violations ([`part-d-assurance.log`](library-run-second-reading/part-d-assurance.log)); the injected drift was caught again with the same 5 violations, J3, J5 and J7 ([`part-d-assurance-drift-caught.log`](library-run-second-reading/part-d-assurance-drift-caught.log)), and passed on its revert; the artefacts under `src/` and both `HUMAN_REVIEW.md` files are the first reading's |
| D-4 pinned and hashed inputs | **Holds**: part B's two runs pass again, 8 tests each ([`part-b-run-1.log`](library-run-second-reading/part-b-run-1.log), [`part-b-run-2.log`](library-run-second-reading/part-b-run-2.log)) |

**Part D is therefore MET for D-1, D-3 and D-4 at both readings, and D-2 waits on the merge of this branch** into
the default branch. Until then part D is not judged MET, and neither is the owner's clause 1.

## 6. The reference-arm comparison: MET under the rule as revised after its first run

**Read under [`reference-arms-revised-2026-10-16-after-first-run.md`](reference-arms-revised-2026-10-16-after-first-run.md).**
That revision was written after the first run had been seen, so this verdict is **weaker evidence** than one under
[`reference-arms.md`](reference-arms.md) as written. Under the rule as written, the first run was NOT MET:
- its classifier left 382,692 lines in class (c);
- 377,592 of them were charges, which the rule held to arm V's exactly even where it admits row N6.

The revision quotes the rule whole and gives each cause. **S's answer was the arbiter's on every line of both
runs, and no answer or charge of S changed between them.**

**The run that judges.** [`run-reference-arms.sh`](run-reference-arms.sh) took every step at `3f7dbce`, the commit
that carries the revision, in a detached worktree of it. Every step's command is the first line of its log in
[`reference-arms-run/`](reference-arms-run/), and [`summary.txt`](reference-arms-run/summary.txt) lists each run's
result.

| | |
|---|---|
| Commit | `3f7dbce`: S's sources are `378c2f0`'s; the harness `tests/Unicode.ReferenceArms` and classifier version 2 are the commit's ([`environment.txt`](reference-arms-run/environment.txt)) |
| Taken | 2026-10-16, from 18:37Z to 19:00Z, on the programme's cloud machine (`linux-x64`, four processors, .NET 10.0.12 under JIT), with other work beside it |
| Population | regenerated by `scripts/reference-arms/generate_population.py` with the rule's seeds ([`population-manifest.json`](reference-arms-run/population-manifest.json)); the arbiter's answers by `unicoderef.py` ([`reference-manifest.json`](reference-arms-run/reference-manifest.json)), each recorded answer checked against its direct one by the classifier |
| Host | ICU 74.2, carrying Unicode 15.1.0, and Python's `unicodedata` at 15.1.0, recorded in each `summary.json` and `classification.json` |

| Run | Differences | Classified | Listing, uncompressed, SHA-256 |
|---|---|---|---|
| The comparison, invariant culture | 16,022,956 | (a) 15,985,359; (a') 27,380; (b) 10,217; (c) none | `7d23a432…7ded5a`, the first run's, byte for byte |
| Arm J again under tr-TR | 234,322 | (a) 219,004; (a') 15,318; (c) none | `9b1af677…599445` |
| Arm J again under lt-LT | 183,238 | (a) 167,920; (a') 15,318; (c) none | `6b4ff2f2…835449` |
| The negative controls | 5,295,577 | all unclassified, both planted changes among them ([`planted.tsv`](reference-arms-run/negative-control/planted.tsv)) | `d24c6c87…8d132` |

The full digests and line counts are in each run's `classification.json`. The listings themselves are not
committed, as the rule says. Each is regenerated byte for byte from `3f7dbce` and the population: the first
comparison already was, by this run.

**The rule's MET line, as revised, clause by clause.**
- **Every operation ran over its whole population**, in S and in every arm that implements it. The comparison
  logs count, per operation and class:
  - the inputs and cases;
  - the cases an arm was not called for, each in an operation its `summary.json` names as not implemented by
    that arm (arm J under a maximum, arm V without one, and the others it lists);
  - the class U7 lines another operation's family holds.

  No run is void.
- **Every difference is in class (a), (a') or (b) and is listed** in its `classification.tsv.gz`. None is
  unnamed by the correspondence, which Broiler.VM revised for rows C2, C3, C6 and N1 before this run.
- **Every class (b) difference is in a vector at a named corpus revision.** These are the 10,217 word statuses
  of row G4. Broiler.JS's corpus revision 18 carries them in `unicode/segmenter-word-like`, as
  implementation-defined vectors with each engine's expectation.
- **No class (c) difference remains.**
- **Every charge of S for casing and normalization equals arm V's, but for those of class (a), row N6.** No
  casing charge differs. The 377,592 normalization charges that differ are all composed forms, each with arm V
  refusing after its first charge and S charging row N7's units in full.
- **Both negative controls were reported unclassified.**

**What it shows of each engine.**
- **Arm V differs from S only at the limits**, rows C12 and N6, and in those charges. Every other answer of
  Broiler.VM's copied implementation is S's.
- **Arm J's differences are the corrections and data policies** the correspondence lists for Broiler.JS, under
  rows P1 to P4, I1 to I3, W1, C1 to C8, N1, N2 and G1 to G4.

## 7. What this bundle does not yet hold

- **Part D's check D-2 at either reading**, which holds when this branch reaches the default branch. *(Revised
  2026-10-17, at SJL-7.6, when the second reading was taken. This item read "**Part D's second reading**, at
  SJL-7.6.")*
