# Bundle SJL-7-001 - the Unicode area: the owner's rules and evidence

**Milestone:** SJL-7 of the shared JavaScript libraries programme. The milestone itself is
`docs/javascript-shared-libraries/milestones/sjl-7-unicode.md` in the Broiler.VM repository.

**Rules:**
- [`decision-rule.md`](decision-rule.md): parts A to D, the owner's component clauses 1 to 4.
- [`reference-arms.md`](reference-arms.md): the library against both engines' implementations.

Both were written at SJL-7.1, before any code of the milestone existed here, and neither has changed since
(this bundle's [`manifest.json`](manifest.json) lists their digests).

**Records:**
- This repository's [roadmap](../../roadmap.md): the owner conditions adopted at SJL-7.1, and SJL-7.2's
  record.
- Broiler.VM's JSD-0074, the area verdict, with its correspondence `docs/evidence/sjl-7-001/correspondence.md`
  in that repository.

**Owner and reviewer:** the Broiler.Unicode maintainer. One person holds every role the rules name (EX-30), and
nobody independent has read this bundle.

**State, 2026-10-16, at the end of SJL-7.2's tests and gates.**
- Parts A, B and C are **MET** at `378c2f0`.
- Part D's first reading holds for D-1, D-3 and D-4. D-2 holds when this branch reaches the default branch.
- The reference-arm comparison is SJL-7.2's last part. It is not taken yet, and nothing here reads it.

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

## 6. What this bundle does not yet hold

- **The reference-arm comparison**, [`reference-arms.md`](reference-arms.md). This includes:
  - its population, generator and arbiter;
  - arms V and J;
  - the classification;
  - the negative controls.

  It is SJL-7.2's last part. It joins this bundle with its own run directory, and the area's cut-overs wait
  for it.
- **Part D's second reading**, at SJL-7.6.
