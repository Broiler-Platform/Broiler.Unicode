# Broiler.Unicode roadmap

*Created 2026-10-12, at milestone SJL-0 of the shared JavaScript libraries programme. This repository
had no roadmap before. The programme's plan, ledger and decision register live in the Broiler.VM
repository, under `docs/javascript-shared-libraries/`: the area design is `areas/unicode.md`, and the
milestone that delivers these items is `milestones/sjl-7-unicode.md`.*

This file contains only unfinished work. What the packages do today is described by their code, their
tests and the README.

## Why this repository has a roadmap now

The programme gives each JavaScript language algorithm and data set one maintained implementation,
in libraries that name neither JavaScript engine, consumed by both Broiler.JS and Broiler.VM's
JavaScript profile. On 2026-10-11 it took two decisions that place work here:

- **SJL-D-1 (b):** Unicode character data and algorithms live in Broiler.Unicode, **under the owner
  conditions of work package SJL-7.1**: this repository adopts the programme's conventions, keeps
  this roadmap, gates on assurance, and pins and hashes its generator inputs. A sub-area whose
  conditions are not met goes to the Broiler.JS repository instead, and the record says why.
- **SJL-D-6 (c):** the generators and the pins live with the data's owner, and **each engine embeds
  the tables they generate**. The algorithms here read their tables through a provider the engine
  supplies, so neither engine references a package that carries tables.

**Locale data is not an item here.** Decision SJL-D-1 (b-ii) placed the CLDR generator and its pins in
the Broiler.JS repository, so `Broiler.Unicode.Cldr.LocaleData` is not extended for the programme.
**Collation is not an item here either**; it is milestone SJL-17's.

**Owner of every item: the Broiler.Unicode maintainer.** The JavaScript profile owner and the
Broiler.JS maintainer own the engines' cut-overs at SJL-7. All three roles are held by one person, and
no item here is independently confirmed by a second reader.

## The owner conditions, adopted 2026-10-16 (SJL-7.1)

*Added 2026-10-16, at work package SJL-7.1, by the Broiler.Unicode maintainer.* The programme's area
verdict for Unicode is Broiler.VM's JSD-0074
(`src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md`
in the Broiler.VM repository). It accepts all six sub-areas and places them here: properties and lexical
classes, case folding, case mapping, normalization, properties of strings, and segmentation. Broiler.JS's
shared-libraries plan records the same verdict.

**This repository adopts the four owner conditions** that decision SJL-D-1 (b) places the area here under.
It meets them at SJL-7.2, and the milestone's exit clause 1 judges them at SJL-7.6:

1. **The programme's conventions**, those of the programme's `architecture.md` section 4, as Broiler.JS's
   `Broiler.ECMAScript/docs/conventions.md` states them.
   - This repository files its own copy, with the same numbering.
   - What a test can decide is enforced by rules of the programme's `SL` shape in an architecture-test
     project of its own, each with a witness.
   - **Its contract types are its own.** The shared libraries may depend on this repository and never the
     reverse, so its meter is a delegate of the same shape as the programme's `WorkCharge`, defined here.
   - The packages the engines consume target `net10.0`, enable nullable, disable implicit usings, and
     declare `IsAotCompatible` and `IsTrimmable`, with a Native AOT probe in CI.
2. **A roadmap: met.** This file, filed at SJL-0 and merged into the default branch. Its items U1 to U10
   are SJL-7.2's work.
3. **An assurance gate**, from the first commit of any project the engines consume (decision SJL-D-5 (a)):
   - every relevant unit carries the Broiler Code Assurance annotation;
   - the artefacts are generated with Broiler.Code's command-line tool;
   - CI gates on them;
   - `HUMAN_REVIEW.md` names the revision it reviews, so that Broiler.VM's release gate can read it.
4. **Pinned and hashed generator inputs.**
   - A pin file hashes the UCD 17.0.0 inputs, the auxiliary break data and its tests, the emoji sequence
     files and ECMA-262's three property tables. It starts as a copy of Broiler.VM's
     `src/tests/unicode/pins/unicode.pin`: the same files, by the same hashes.
   - The generators live here: ports of Broiler.VM's `UnicodeDatabase`, `UnicodeTableGenerator` and the
     Unicode parts of its `CldrTableGenerator`.
   - A test regenerates every table byte for byte (the programme's rule SL7).

**If a condition is not met by the end of SJL-7.2**, the sub-areas it reaches move to the Broiler.JS
repository, under `Broiler.ECMAScript`, by a dated record here and in Broiler.VM naming the condition. This
is SJL-D-1 (b)'s rule, and nothing here weakens it.

**What it found.** The area's correspondence from code is Broiler.VM's
`docs/evidence/sjl-7-001/correspondence.md`. It was read at this repository's `0faa1f9`, whose code is that
of its `0.1.0-preview.3` packages.
- **The data already agrees with Broiler.VM's pin.** This repository's four property generators and
  Broiler.Regex's case-folding generator, run against Broiler.VM's pinned UCD 17.0.0 files, reproduce
  their committed tables byte for byte.
- **The committed emoji inputs** carry the hashes that pin records.
- **One script value is missing**: `Hrkt`, `Katakana_Or_Hiragana`, has no code points, and
  `generate-scripts.py` emits keys only for scripts that have some (the correspondence's row P2).
- **What is missing is everything around the bytes**, which items U1 to U10 already name.

**The rules this repository's half of SJL-7 is judged by** are committed with this record, before any code
of the milestone: `docs/evidence/sjl-7-001/decision-rule.md` (Unicode's test files and generation byte for
byte) and `docs/evidence/sjl-7-001/reference-arms.md` (the library against both engines' implementations
before their cut-overs).

## SJL-7.2: the packages, the provider and the format, settled 2026-10-16

*Added 2026-10-16, at work package SJL-7.2, by the Broiler.Unicode maintainer.* Bundle `sjl-7-001`'s rule
leaves three things to "a dated record here, within JSD-0074's constraints": the packages' names, the
provider's binding and the tables' format. This is that record. It also states where each item U1 to U10
stands.

**The packages.** Each targets `net10.0` and is held to the [conventions](conventions.md). Each takes this
repository's one version stream.

| Package and namespace | What it is | Who references it |
|---|---|---|
| `Broiler.Unicode.Algorithms` | JSD-0074's code package: properties and lexical classes, case folding, case mapping, normalization and properties of strings. It carries no table. | Both engines, from the projects their cut-overs name |
| `Broiler.Unicode.Segmentation` | JSD-0074's optional package: UAX #29. It carries no table. | Only an engine's Intl assembly |
| `Broiler.Unicode.Tables` | This repository's embedding of the generated tables | This repository's tests and probe, and consumers other than the engines; neither engine |
| `Broiler.Unicode.Generation` | The pin's checks and the generator, ported from Broiler.VM's `UnicodeDatabase` and `UnicodeTableGenerator` | Each engine's tests, as a pinned test-only package; never a product closure |

**The provider.** It is `IUnicodeTables`, in `Broiler.Unicode.Algorithms`, in the shape of Broiler.VM's
`IJsIntlData`:

- a `UnicodeVersion` string;
- `ReadOnlySpan<byte> Table(UnicodeTable)`, for each of the 37 members of the `UnicodeTable` enumeration.

Every algorithm type reads its tables once at construction, refuses a provider of another version or one
that lacks a table it reads, and decodes nothing the provider has not handed it. An engine binds it by
implementing the interface over the tables it embeds. The members each engine passes are listed in the
[adapter guide](adapter-guide-unicode.md).

**The format: Broiler.VM's byte layouts, unchanged.** The generator writes 37 tables, 311,408 bytes:

- **31 tables are Broiler.VM's, byte for byte.** These are the tables Broiler.VM's `.g.cs` files and its
  Intl tables carry today: its properties, case folding, string properties, normalization, casing and
  segmentation break tables.
- **Five layout tables, 3 bytes per value, hold what Broiler.VM keeps as C# constants**, such as the
  longest decomposition and the property-set counts. A provider must hand them in, because the code no
  longer compiles them in.
- **One new table, Soft_Dotted's ranges**, serves the Lithuanian casing condition. Broiler.VM carries the
  same ranges in another form today: hexadecimal text in its Intl assembly's CLDR tables
  (`JsIntlTable.SoftDotted`).

Those six add 261 bytes. JSD-0074 asks for a format "chosen to fit N22's and N28's budgets, which sit close
to their limits today". The budgets need not move for the 31 shared tables. Whether the 261 bytes fit is
Broiler.VM's to measure at SJL-7.4, under its rules, with this record's figure as its input.

**Where the items stand at the end of SJL-7.2:**

| # | State | Where it is shown |
|---|---|---|
| U1 | **Met.** `UnicodeProperties.TryResolve` is the exact lookup. The loose lookups of `Broiler.Unicode.Properties` stay for their consumers and carry the warning. | Tests A5 and C1 |
| U2 | **Met.** The new lookups answer `CodePointSet`, a view over constant data. The old package's four lookups return a copy on every call: an observable change, in the [changelog](../CHANGELOG.md). | Tests C2 and `RangeLookupTests` |
| U3 | **Met.** `GeneralCategoryOf`, `IdStart`, `IdContinue`, `SpaceSeparator`, `WhiteSpace`, `SoftDotted` and the lexical classes, each compared range for range with the UCD files. | Test A6 |
| U4 | **Met.** The pin is [`data/unicode/pins/unicode.pin`](../data/unicode/pins/unicode.pin), equal to Broiler.VM's by file name, length and hash. The generator reads nothing it has not verified, and every table regenerates byte for byte. | The regeneration tests; bundle part B |
| U5 | **Met.** Every algorithm reads its tables through `IUnicodeTables`. The old packages keep their own tables. | The provider above |
| U6 | **Met.** `UnicodeAlgorithms.UnicodeVersion`, `UnicodeSegmentation.UnicodeVersion`, `UnicodeTables.Version` and `UnicodePin.PinnedVersion` are `17.0.0`, and the old property package gains `UnicodeProperties.UnicodeVersion`. | Test C3 |
| U7 | **Met.** The README, `Directory.Build.props`, the data tool's and generator's defaults and the benchmarks name 17.0, the version `EmojiTrieData.g.cs` carries. | The diff of this record's commit |
| U8 | **Met.** Every sub-area passes Unicode's own test files at the pin, every case: NFC, NFD, NFKC and NFKD; full case mappings with Final_Sigma and the default, Turkic and Lithuanian languages; simple folding, orbits and the non-Unicode canonicalisation; UAX #29's three granularities, in the optional package. Each transform takes a maximum length and a meter of `WorkCharge`'s shape, passes lone surrogates through and reads no host Unicode data. | Tests A1 to A4, C4 and C5; the probe, under Native AOT |
| U9 | **Met, in the code package rather than the emoji package.** The ECMAScript name map for the `v`-flag properties of strings is `UnicodeStringProperties`. Its sequences are read into the caller's span by a cursor that allocates nothing. JSD-0074's decision 1 put the properties of strings in the code package, which the engines consume; the emoji package keeps its own trie for its consumers. | Test A5; C2 |
| U10 | **Met here; CI's first run is the remaining evidence.** The repository now has: the tests above; the conventions in their owner's copy; SL1 to SL10 in `tests/Unicode.Architecture.Tests`, each with a witness; `IsAotCompatible` and `IsTrimmable` on the consumed packages; the probe and the assurance gate as CI jobs. The identifier series is U1 to U10. | Bundle parts C and D; [`ci.yml`](../.github/workflows/ci.yml) |

**The reference arms.** Arms V and J compare the library with both engines' implementations before their
cut-overs, under [`reference-arms.md`](evidence/sjl-7-001/reference-arms.md).
- **The comparison is met at `3f7dbce`**, under the rule as revised after its first run had been seen
  ([`reference-arms-revised-2026-10-16-after-first-run.md`](evidence/sjl-7-001/reference-arms-revised-2026-10-16-after-first-run.md)),
  which makes it weaker evidence. The bundle's section 6 records it.
- **The library's answer was the arbiter's on every input of both runs.**
- **What the revision changed:**
  - the classification of charges inside row N6's correction;
  - Broiler.JS's data-policy rows;
  - two defects of the classifier.

  Broiler.VM's correspondence named the Broiler.JS corrections it had missed before the run that judges.

**What is not settled here.** The preview package an engine admits is published when the owner chooses, from this
stream. *(Revised 2026-10-16, when the comparison was taken. The paragraph read: "Reference arms V and J compare
the library with both engines' implementations before their cut-overs, under
[`reference-arms.md`](evidence/sjl-7-001/reference-arms.md). They are SJL-7.2's last part and are recorded in the
bundle. The preview package an engine admits is published when the owner chooses, from this stream.")*

## SJL-7.6: the owner's evidence at the end of the milestone, 2026-10-17

*Added 2026-10-17, at work package SJL-7.6, by the Broiler.Unicode maintainer.* `0.1.0-preview.4` was published
to nuget.org from `6e14f93` on 2026-10-16, on the owner's instruction, and both engines' cut-overs are on it:
Broiler.VM's at its SJL-7.4 and Broiler.JS's at its SJL-7.5, each embedding tables this repository's generator
writes from a pin held equal to this one. Bundle `sjl-7-001` took part D's second reading at `f3d677a`
([section 5](evidence/sjl-7-001/README.md#part-ds-second-reading-at-sjl-76)): D-1, D-3 and D-4 hold again, every
step of the library run ending as at the first reading, and **D-2 holds only when this branch reaches the default
branch**, which still carries the roadmap as merged at `0faa1f9`. Until it does, the owner's clause 1 is not met.

## 1. Items

*(2026-10-16: each item's state at the end of SJL-7.2 is in the record above. The table is kept as SJL-0
wrote it, its "Today" column included, until SJL-7.6 judges the milestone and the met items leave this
file.)*

| # | Item | Today | Exit criterion |
|---|---|---|---|
| U1 | **Exact-name lookup.** A lookup that admits exactly the property names, values and aliases ECMA-262's tables admit, `Katakana_Or_Hiragana` included, for General_Category, the binary properties, Script and Script_Extensions. | `UnicodeProperties.GetBinaryProperty`, `GetScript`, `GetScriptExtensions` and `GetGeneralCategory` match names loosely (case-insensitive, ignoring `_`, spaces and `-`), which ECMA-262 does not. | The exact lookup exists beside the loose one, which stays for its existing consumers with a documented warning until they move; ECMA-262's property tables are its tests. |
| U2 | **Immutable results.** Lookups return immutable sets or spans over constant data. | They return `(int Lo, int Hi)[]` arrays from a shared cache, so one caller can change what the next one receives. | No lookup hands out a mutable reference to shared data. |
| U3 | **Code-point lookups** for the properties both engines classify by: General_Category, ID_Start and ID_Continue (with Pattern_Syntax subtracted, as UAX #31 requires), White_Space and Soft_Dotted. | Range lists by name only. | Per-code-point lookups with tests against the UCD. |
| U4 | **Pinned and hashed generator inputs, and the generators in the repository.** A pin file names the UCD 17.0.0 inputs by hash - the files Broiler.VM's `src/tests/unicode/pins/unicode.pin` names, the auxiliary break-test files included - and a byte-for-byte regeneration test (the programme's rule SL7). | `src/UnicodeProperties/tools/` holds Python generators whose README downloads the UCD files with `curl` and checks no hash; the emoji inputs are committed under `data/unicode/`. | The pin file exists, every generator reads only pinned inputs, and a test regenerates every table byte for byte. |
| U5 | **Tables reached through a provider** (SJL-D-6 (c)). The algorithms read their tables through a provider that hands them the generated bytes, so each engine can embed the tables the generator produces from the pinned inputs and pass them in. | The tables are compiled into each package. | Every algorithm the engines call reads its tables through the provider; the existing packages may keep shipping their own tables for their other consumers. |
| U6 | **Public data versions.** A public `UnicodeVersion` constant on every package that carries or reads Unicode data. | The emoji package exposes `UnicodeEmojiVersion`; the property package exposes no version. | Each package states its Unicode version publicly, and the programme's rule `I4` can read it. |
| U7 | **The emoji-version correction in the README.** | The README says the supported emoji version is 16.0, and `Directory.Build.props`, the data tool and the generator default to 16.0, while the generated `EmojiTrieData.g.cs` carries `"17.0"`. | The README, the defaults and the generated data name one version, the one the code carries. |
| U8 | **Normalization, case mapping, case folding and segmentation**, if this repository meets SJL-7.1's owner conditions: NFC, NFD, NFKC and NFKD; full case mappings with Final_Sigma and the default, Turkic and Lithuanian languages; simple case folding, its orbits and the non-Unicode canonicalisation; UAX #29 grapheme, word and sentence boundaries in an optional package. Each takes a maximum result length and a caller-owned meter (the programme's `WorkCharge` delegate), passes lone surrogates through, and falls back to no host Unicode data. Normalization's method names avoid the identifier tokens Broiler.VM's rule N23 rejects. | None of these exists here. `JSStringSpecialCasing` delegates its simple mappings to `ToUpperInvariant` and `ToLowerInvariant` and approximates Cased and Case_Ignorable from the runtime's categories. | Each sub-area has an area verdict at SJL-7.1. A sub-area placed here passes `NormalizationTest`, the three break tests, `CaseFolding` and `SpecialCasing` at the pin; a sub-area not placed here is recorded with the reason. *(2026-10-16: SJL-7.1 placed all of them here, so this item's "if" is met; the meter is this repository's own delegate of `WorkCharge`'s shape, by the conventions condition above. The item read "(the programme's `WorkCharge` delegate)".)* |
| U9 | **Properties of strings.** The ECMAScript name map for the `v`-flag properties of strings joins the emoji package, and `GetSequences` stops allocating on every call. | The name map sits in Broiler.Regex. | The map is here, and `GetSequences` returns constant data. |
| U10 | **Tests, conventions and an assurance gate.** Tests for every item above; the programme's contract conventions; `IsAotCompatible` and `IsTrimmable` declared with a Native AOT probe in CI; an identifier series for these items; and the Broiler Code Assurance scheme with a CI gate, from the first commit of any project the engines consume (decision SJL-D-5 (a)). | Tests exist for the three packages; CI builds, tests and packs; the packages publish under Native AOT without a warning but declare neither flag; no assurance annotation, generated artefact or gate exists. | Each runs in this repository's CI and is green, and the human-review state of each released revision is readable by Broiler.VM's release gate. |

## 2. What follows

When the items SJL-7.1 places here are met, SJL-7's cut-overs move both engines onto them: Broiler.JS
stops using host Unicode data, and Broiler.VM regenerates its embedded tables with this repository's
generators from its own pin, which its rule `I4` holds equal to the pin here. A fix to an algorithm
here reaches both engines through a release and each engine's own pin move; neither engine patches it
locally.
