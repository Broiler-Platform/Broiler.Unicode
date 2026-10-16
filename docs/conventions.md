# Conventions of the shared JavaScript libraries, in Broiler.Unicode

[← Roadmap](roadmap.md) · [Rules register](../tests/Unicode.Architecture.Tests/rules.register.json) ·
[Adapter guide](adapter-guide-unicode.md)

**This document is normative.** It makes the contract conventions of the shared JavaScript libraries
programme binding on the projects of this repository that the programme governs. Those conventions are
[section 4 of `architecture.md`](https://github.com/Broiler-Platform/Broiler.VM/blob/main/docs/javascript-shared-libraries/architecture.md#4-contract-conventions)
in the Broiler.VM repository, and Broiler.JS states them for its own shared libraries in
`Broiler.ECMAScript/docs/conventions.md`. **This is the owner's copy, with the same numbering**: convention
*n* here is convention *n* there. A change that breaks one of them is a defect, whoever makes it. Where
this document and `architecture.md` section 4 differ, that section wins and this document is corrected.

Filed 2026-10-16, at work package SJL-7.2, under the first owner condition this repository adopted at
SJL-7.1 ([roadmap](roadmap.md), "The owner conditions").

## Scope

The programme's work here is milestone SJL-7's: Unicode character data and algorithms, the area Broiler.VM's
JSD-0074 placed in this repository.

- **The libraries the engines consume** are held to every convention:
  - `src/Unicode.Algorithms` (`Broiler.Unicode.Algorithms`): exact property lookups, the ECMAScript
    lexical classes, case folding, case mapping, normalization and the properties of strings.
  - `src/Unicode.Segmentation` (`Broiler.Unicode.Segmentation`): UAX #29's boundaries. Optional,
    because only an engine with `Intl.Segmenter` reads it.

  Neither carries a table. Each reads its tables through the `IUnicodeTables` the engine passes
  (decision SJL-D-6 (c)).
- **The owner's table package**, `src/Unicode.Tables` (`Broiler.Unicode.Tables`), is held to every
  convention too. It is this repository's own embedding of the generated tables, for its tests, its probe
  and consumers other than the engines. Neither engine references it: each embeds its own tables from its
  own pin.
- **The generator**, `src/Unicode.Generation` (`Broiler.Unicode.Generation`), is test-only. It reaches each
  engine's tests as a pinned package and never a product closure.
  - Convention 8's I/O clause does not bind it, since generating is reading data. Even so it reads no file
    itself: the caller hands it the pin's text and the archive's bytes, and only its own pin is read, as an
    embedded resource.
  - It is not Native AOT code and declares neither flag of convention 12.
  - Every other convention holds for it.
- **The Native AOT probe**, `probes/Unicode.Probe`, is an executable. It reads the pinned archive from
  disk, so convention 8's I/O clause does not hold for it. Conventions 12 and 17 do.
- **Test projects** are held to convention 17's style rules, and to nothing that would stop xunit
  discovering them.
- **The packages that existed before the programme are not governed**: `Broiler.Unicode.Properties`,
  `Broiler.Unicode.Emoji.StringProperties` and `Broiler.Unicode.Cldr.LocaleData`, with their generators and
  data tools. They keep their own style and frameworks for their existing consumers. Neither engine calls
  them for the programme's work. The roadmap's items U1, U2, U6 and U9 say what was corrected in them.

## The conventions

Each convention is either decided by a test (the `SL` rule named) or a review obligation that the
[adapter guide](adapter-guide-unicode.md) names. A rule's register row, its test and its witness are in
[`rules.register.json`](../tests/Unicode.Architecture.Tests/rules.register.json).

| # | Convention | Decided by |
|---|---|---|
| 1 | **Neutral types only.** Public signatures use base class library types (`string`, `ReadOnlySpan<char>`, `Span<int>`, `ReadOnlySpan<byte>`, the integer types, `bool`) and types the library defines (structs, record structs, enums, the `IUnicodeTables` provider and the `UnicodeWorkCharge` meter). No public signature names an engine type, `object`, `dynamic`, `Type`, or a delegate that can run guest code. | Review; SL1 keeps engine types out of reach |
| 2 | **No coercion, no property access, no guest callbacks.** Inputs arrive converted: a string, a code point, a property name. An engine's `ToString`, its `RegExp` flags and its locale negotiation are the adapter's. The library takes the language a locale resolved to, not the locale. | Review, per adapter guide |
| 3 | **Failure is data.** An expected failure is a status (`UnicodeStatus`), a `false` from a `Try` member or an empty set. A library throws only when its caller breaks a documented precondition, such as a provider of another Unicode version or a missing table. Message text is not contract. | Review; the API baseline (SL6) shows every signature |
| 4 | **Work is charged by the caller's meter.** An operation whose work grows with its input takes a nullable `UnicodeWorkCharge` and charges before the work it charges for, in units it documents: normalization's and casing's are Broiler.VM's rows N7 and C13, segmentation's one per scanned code point (row G5, JSD-0074 decision 4). The meter may throw, and the library then leaves no shared state changed. `UnicodeWorkCharge` is this repository's own delegate of the programme's `WorkCharge` shape, because the shared libraries may depend on this repository and never the reverse. The library never reads a clock and never polls cancellation. | Review; SL3 and SL4 refuse the clock; the contract tests pin each operation's charges |
| 5 | **Limits are explicit.** A result's maximum length comes from the caller. Exceeding it is `UnicodeStatus.LimitExceeded`, never a crash, a silent "no match" or a shortened result. The limit is checked on the whole result (rows C12 and N6), and normalization refuses before its second charge when a bound it can compute exceeds it. **A limit is never the stack.** | Review; the contract tests; the probe runs every test file under Native AOT |
| 6 | **Determinism.** A result depends on its inputs, the tables passed in and the library version only: no culture, time zone or host Unicode, no randomness, environment or process history, no hash-table iteration order. | SL4, SL9 |
| 7 | **No ambient or mutable static state.** No mutable static field, settable static property, `[ThreadStatic]`, `AsyncLocal` or `ThreadLocal`. Static data is immutable: constant spans, constant strings sliced as spans, or lazily built immutable values. | SL2 |
| 8 | **No dynamic code and no hidden machinery.** Nothing on Broiler.VM's B5, B5b and B5c lists; no generic instantiation over a run-time `Type`, no `Enum.GetValues(Type)`, no `dynamic`, no reflection-based serialization, no file or network I/O, no environment read. Providers and data are handed in by the caller. | SL3 |
| 9 | **Platform independence.** No `string.Normalize` or `IsNormalized`, no `System.Text.RegularExpressions`, no `CultureInfo`, `TextInfo`, `CompareInfo`, `TimeZoneInfo` or `DateTime.Now`, no `CharUnicodeInfo`, `char.Is*` or `Rune.Is*` beyond ASCII. Every Unicode answer comes from the tables passed in. The surrogate tests `char.IsHighSurrogate` and `char.IsLowSurrogate` are code-unit arithmetic, not Unicode data, and are admitted. | SL4 |
| 10 | **Thread safety.** Every public member is safe to call concurrently. A lookup object holds only its provider and values derived from it once, at construction. | SL2, SL9 |
| 11 | **Allocation discipline.** Lookups return value types over the tables. Sequences are read into the caller's span. No cache grows without bound. A transform allocates only its result and a buffer bounded by the maximum length. | Review |
| 12 | **Native AOT and trimming.** `IsAotCompatible` and `IsTrimmable` for every library the engines consume and for the table package, with the trim, AOT and single-file analyzers reporting nothing, as errors. No such warning is suppressed. The probe publishes with Broiler.VM's root settings and runs Unicode's own test files. CI compares its report with the JIT's byte for byte. | SL5 |
| 13 | **Data versions are public**, as constants and package metadata: `UnicodeAlgorithms.UnicodeVersion`, `UnicodeSegmentation.UnicodeVersion`, `UnicodeTables.Version`, `UnicodePin.PinnedVersion`, and, on the packages that existed before, `UnicodeProperties.UnicodeVersion` and `EmojiStringProperties.UnicodeEmojiVersion`. Tables are `ReadOnlySpan<byte>` over constant data that regenerate byte for byte from the pinned archive. A provider of another version is refused. | SL7 |
| 14 | **Names that do not collide with engine scans.** No public member named `Normalize`, `IsNormalized` or `NormalizationForm`: Broiler.VM's N23 would refuse the adapter that calls it. Normalization's members are `UnicodeNormalization.Transform`, `NormalForm` and `NormalizationQuickCheck`. | Review; SL6's baseline lists every name |
| 15 | **Every public member cites what it implements**: the ECMA-262 clause, the Unicode standard's section or annex (UAX #15, #29, #31, #44, UTS #51), at the pinned version, in its XML documentation. A missing documentation comment is a build error here: the governed projects set their own `NoWarn`, so the packaging props' suppression of CS1591 does not reach them. | The build (CS1591 as an error); review for the citation |
| 16 | **Versioning and compatibility.** Semantic versioning on this repository's one lockstep stream; the API baseline compared in both directions; package validation against the last published version; a changelog entry for every observable behaviour change and every data-version change. | SL6; [`CHANGELOG.md`](../CHANGELOG.md) |
| 17 | **Frameworks and code style.** `net10.0`; nullable enabled; implicit usings disabled; warnings as errors; no preprocessor directives; LF line endings under `.gitattributes`. | The build (`Directory.Build.props` and each project file); SL10 |
| 18 | **Assurance.** Every relevant declaration carries the Broiler Code Assurance annotation, the generated artefacts under `src/` are current, and CI fails on drift from each project's first commit (SJL-D-5 (a)). | SL8; the `assurance` CI job |
| 19 | **Licensing and provenance.** Apache-2.0. Ported code records its origin (`Origin=Ported`); the generator and the algorithms are ported from Broiler.VM. No share-alike or unclear-licence code enters. Data notices travel with the tables: every package carries [`THIRD_PARTY_NOTICES.md`](../THIRD_PARTY_NOTICES.md), and the archive's licence text sits beside it. | Review; [`data/unicode/pins/README.md`](../data/unicode/pins/README.md) |

## How a convention is changed

The conventions are the programme's, not this repository's. A change is made first in Broiler.VM's
`architecture.md` section 4, under the programme's governance, then in Broiler.JS's copy and here, in
place, with a dated revision quoting the text it replaces. A rule that decides a convention changes in the
same commit as the convention, and its witness is watched failing and passing again.

## Every governed library also carries

Section 10 of the programme's `governance.md` in the Broiler.VM repository asks each governed library for
the following:

- a README with its scope, pins, conventions and consumers (this repository's [README](../README.md),
  "The shared JavaScript libraries' Unicode packages");
- a changelog with "Observable changes" and "Data" headings ([`CHANGELOG.md`](../CHANGELOG.md));
- XML documentation citing the clause of every public member;
- a provenance file for every data set ([`data/unicode/pins/README.md`](../data/unicode/pins/README.md));
- an adapter guide for every operation an engine calls ([`adapter-guide-unicode.md`](adapter-guide-unicode.md)).
