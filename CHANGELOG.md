# Changelog

Every release of this repository's packages has an entry here, newest first. The file was opened on
2026-10-16, at work package SJL-7.2 of the shared JavaScript libraries programme. Its
[convention 16](docs/conventions.md) asks each governed library for a changelog with "Observable changes"
and "Data" headings. Releases before it are recorded only by their packages on nuget.org.

All packages share one version, the preview stream [`eng/RELEASING.md`](eng/RELEASING.md) describes. They
follow semantic versioning:

- a public API removal or a binary-breaking change is a major version;
- a new API is a minor version;
- a correction toward a specification is a minor version, with its own entry under
  **Observable changes**;
- a data-version change is a minor version, with its own entry under **Data**.

While the stream is a preview it keeps its `0.1.0` prefix, and the preview number marks each release.
Each entry has the headings **Added**, **Changed**, **Observable changes** and **Data**, and says "none"
under a heading with nothing to report.

## 0.1.0-preview.4, 2026-10-16

*(Published to nuget.org by the Publish workflow, [run 38952273946](https://github.com/Broiler-Platform/Broiler.Unicode/actions/runs/38952273946), from `6e14f93`. The heading read "Unreleased" until then.)*

To be packed as the next preview the publish workflow selects.

### Added

- **`Broiler.Unicode.Algorithms`**, a new package (`net10.0`), the shared JavaScript libraries' Unicode
  area (milestone SJL-7, Broiler.VM's JSD-0074; adapter guide [`docs/adapter-guide-unicode.md`](docs/adapter-guide-unicode.md)).
  It carries no table: every type reads its tables through the `IUnicodeTables` the caller passes, and
  refuses a provider of another Unicode version.
  - `UnicodeProperties`:
    - exact lookups by the names and aliases ECMA-262's three property tables and
      `PropertyValueAliases.txt` admit, `Katakana_Or_Hiragana` included (roadmap U1);
    - immutable `CodePointSet` results (U2);
    - per-code-point classification by General_Category, ID_Start, ID_Continue, White_Space,
      Space_Separator and Soft_Dotted (U3).
  - `EcmaScriptLexical`: the identifier and white-space classes of ECMA-262 clause 12.
  - `UnicodeCaseFolding`: simple case folding, its orbits and the non-Unicode canonicalisation of
    ECMA-262 22.2.2.7.3.
  - `UnicodeCaseMapping`: the full case mappings with Final_Sigma and the default, Turkic and Lithuanian
    languages (`CaseLanguage`).
  - `UnicodeNormalization`: `Transform` to NFC, NFD, NFKC and NFKD (`NormalForm`), and
    `NormalizationQuickCheck`.
  - `UnicodeStringProperties`: the seven properties of strings the `v` flag admits, read without
    allocating through `StringPropertySequences` (U9).
  - The contract types `UnicodeWorkCharge`, a meter of the programme's `WorkCharge` shape, and
    `UnicodeStatus`; `UnicodeAlgorithms.UnicodeVersion`, `"17.0.0"`.

  Every transform takes a maximum result length and a caller-owned meter, charges before the work it
  charges for, and passes lone surrogates through (U8).
- **`Broiler.Unicode.Segmentation`**, a new optional package (`net10.0`). It finds UAX #29's grapheme,
  word and sentence boundaries over the break table the caller passes, with word statuses and a meter
  charged one unit per scanned code point. `UnicodeSegmentation.UnicodeVersion` is `"17.0.0"`.
- **`Broiler.Unicode.Tables`**, a new package (`net10.0`): this repository's own embedding of the
  generated tables, with `UnicodeTables.Instance`, for tests, probes and consumers other than the engines
  (U5). Neither engine references it, because each embeds its own tables (decision SJL-D-6 (c)).
- **`Broiler.Unicode.Generation`**, a new test-only package (`net10.0`): the generator.
  - `UnicodePin` parses a pin and verifies an archive against it by length, SHA-256 and version header.
  - `UnicodeTableGenerator` writes every table byte for byte, and `UnicodeTableSource` writes them as C#.
  - It is a port of Broiler.VM's `UnicodeDatabase` and `UnicodeTableGenerator`. It carries this
    repository's pin, so an engine's test can hold its own pin equal to the owner's (U4).
- `Broiler.Unicode.Properties`: `UnicodeProperties.UnicodeVersion`, `"17.0.0"` (U6).

### Changed

- `Broiler.Unicode.Properties`: the loose name matching of `UnicodeProperties`' four lookups is documented
  with a warning that points ECMAScript engines to the exact lookup (U1). The lookups themselves are
  unchanged and stay for their existing consumers.
- The emoji version the build, the data tool, the generator and the benchmarks default to is `17.0`, the
  version the generated `EmojiTrieData.g.cs` carries. It was `16.0` in `Directory.Build.props`, in the
  tools' defaults and in the README (U7).

### Observable changes

- `Broiler.Unicode.Properties`: `GetBinaryProperty`, `GetScript`, `GetScriptExtensions` and
  `GetGeneralCategory` return a new array on every call (U2). Before, they returned the same cached array,
  so a caller that wrote to it changed every later caller's answer. The ranges are unchanged; two calls no
  longer return the same reference, and each call allocates its copy.
- No other existing package changes behaviour. The new packages are new.

### Data

- **The pinned archive**, [`data/unicode/pins/`](data/unicode/pins/README.md), copied byte for byte from
  Broiler.VM at `7b44dc7`. It holds the Unicode Character Database 17.0.0 files the tables are generated
  from, the auxiliary break data and its tests, the emoji 17.0 sequence files, the Unicode licence text and
  ECMA-262's three property tables at `0248456c`. [`unicode.pin`](data/unicode/pins/unicode.pin) records
  each file's length and SHA-256, the same as Broiler.VM's pin.
- **The generated tables**, `src/Unicode.Tables/UnicodeTableData.g.cs`: 37 tables, 311,408 bytes, Unicode
  17.0.0.
  - The 31 tables Broiler.VM also carries are byte-identical to Broiler.VM's.
  - The other six carry, as tables, what Broiler.VM held as C# constants (the five layout tables) or did
    not need (Soft_Dotted).
  - The regeneration test holds every byte to the generator's output (the programme's rule SL7).
- The emoji package's data is unchanged: it was, and stays, emoji 17.0. Only the documents and defaults
  that named 16.0 are corrected.
