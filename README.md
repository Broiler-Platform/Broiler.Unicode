# Broiler.Unicode

Unicode property, emoji string-property, and CLDR locale-data components used by
Broiler.JS, and the Unicode character data and algorithms of the shared JavaScript libraries,
which both Broiler JavaScript engines consume. The
[shared libraries' packages](#the-shared-javascript-libraries-unicode-packages) are described
first; the rest of this README focuses on the `Broiler.Unicode.Emoji.StringProperties` package.

> **Preview status:** This is unstable, substantially AI-assisted first-preview software.
> Generated tables and their generators require human source and data-provenance review.
> Human-review approval is revision-scoped; consult
> [HUMAN_REVIEW.md](HUMAN_REVIEW.md) for the reviewed revision and conditions before
> describing the current checkout as approved.

## The shared JavaScript libraries' Unicode packages

The shared JavaScript libraries programme gives each JavaScript language algorithm and data set
one maintained implementation, in libraries that name neither JavaScript engine. Both Broiler.JS
and Broiler.VM's JavaScript profile consume them. Its Unicode area, milestone SJL-7, lives here:

- Broiler.VM's JSD-0074 placed all six sub-areas in this repository.
- This repository's [roadmap](docs/roadmap.md) carries its items and the owner conditions it adopted.
- The programme's plan lives in the Broiler.VM repository, under
  `docs/javascript-shared-libraries/`.

| Package | Holds |
|---|---|
| `Broiler.Unicode.Algorithms` | Exact property lookups by the names ECMA-262 admits, per-code-point classes, the ECMAScript lexical classes, case folding with its orbits, the full case mappings with Final_Sigma and the Turkic and Lithuanian languages, NFC, NFD, NFKC and NFKD, and the properties of strings. It carries **no table**: every type reads its tables through the `IUnicodeTables` its caller passes. |
| `Broiler.Unicode.Segmentation` | UAX #29's grapheme, word and sentence boundaries, with word statuses. Optional: only an engine with `Intl.Segmenter` reads it. No table either. |
| `Broiler.Unicode.Tables` | This repository's own embedding of the generated tables, with `UnicodeTables.Instance`, for tests, probes and consumers other than the engines. Neither engine references it: each embeds its own tables (decision SJL-D-6 (c)). |
| `Broiler.Unicode.Generation` | Test-only. The generator: it verifies a pinned archive by length, SHA-256 and version header and writes every table byte for byte. It carries this repository's pin, so an engine's test can hold its own pin equal to it. |

```csharp
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Tables;

var properties = new UnicodeProperties(UnicodeTables.Instance);
properties.TryResolve("Script_Extensions", "Grek", out var greek);   // true
properties.TryResolve("letter", out _);                               // false: names are matched exactly

var normalization = new UnicodeNormalization(UnicodeTables.Instance);
normalization.Transform("é", NormalForm.Nfc, maxLength: 1024, charge: null, out var nfc);
// UnicodeStatus.Success, nfc == "é"
```

**Scope.** The engines call these packages for:

- the property escapes of regular expressions, `v`-flag properties of strings included;
- the identifier classes of the lexical grammar;
- `String.prototype.normalize`;
- `toUpperCase`, `toLowerCase` and their locale forms;
- the case folding of `i`-flag matching;
- `Intl.Segmenter`.

What stays in each engine is argument conversion, locale negotiation, error construction and its own
resource policy. The [adapter guide](docs/adapter-guide-unicode.md) gives each operation's
contract.

**Pins.**

- **Data.** Unicode **17.0.0** (`UnicodeAlgorithms.UnicodeVersion`), from the archive
  [`data/unicode/pins/`](data/unicode/pins/README.md). Its [`unicode.pin`](data/unicode/pins/unicode.pin)
  hashes every file the generator reads, the same files by the same hashes as Broiler.VM's pin.
- **Names.** ECMA-262's three property tables, at `0248456c`, from the same archive.
- **Regeneration.** A test regenerates every table byte for byte (the programme's rule SL7).

**Conventions.** The four packages follow the programme's
[conventions](docs/conventions.md), in the owner's copy with the same numbering. Among them:

- caller-owned metering and explicit maximum lengths;
- failure as data;
- no host Unicode, culture or regular expressions;
- no mutable statics;
- Native AOT and trimming with no warning suppressed;
- an XML documentation comment on every public member.

The rules that decide them, SL1 to SL10, run in
[`tests/Unicode.Architecture.Tests`](tests/Unicode.Architecture.Tests/rules.register.json). The
Native AOT probe, [`probes/Unicode.Probe`](probes/Unicode.Probe), runs Unicode's own test files in CI.

**Consumers.** None yet. Broiler.VM's and Broiler.JS's cut-overs are milestone SJL-7's work
packages SJL-7.4 and SJL-7.5.

**Target framework:** `net10.0`. **Dependencies:** none. **Changes:** [`CHANGELOG.md`](CHANGELOG.md).

## The emoji string-property package

Managed, allocation-light matching of **Unicode RGI emoji string properties** (UTS #51) for .NET,
backed by a generated trie. No ICU, no native dependency, no runtime downloads, no giant regex.

- **Supported Unicode emoji version:** **17.0** *(corrected 2026-10-16, roadmap item U7: it read
  16.0, while the generated data has carried 17.0)*
- **Target frameworks:** `net8.0`, `net10.0`
- **Dependencies:** none (pure managed C#)

```csharp
using UnicodeEmoji.StringProperties;

EmojiStringProperties.IsRgiEmoji("👨‍👩‍👧‍👦");   // true  (family ZWJ sequence)
EmojiStringProperties.IsBasicEmoji("❤️");         // true  (U+2764 U+FE0F)
EmojiStringProperties.IsBasicEmoji("❤");          // false (missing U+FE0F presentation selector)
EmojiStringProperties.IsEmojiZwjSequence("👩🏽‍💻"); // true  (woman technologist, medium skin tone)
```

## Why a string property and not a code point property?

Emoji-ness is **not** a property of a single code point. The "woman technologist, medium skin tone"
emoji `👩🏽‍💻` is the sequence `U+1F469 U+1F3FD U+200D U+1F4BB` — four scalar values, six UTF-16
`char`s. Whether something is an emoji depends on the *whole sequence*: the presence of U+FE0F, the
zero-width joiners, regional-indicator pairs, tag characters, skin-tone modifiers, and so on.

`RGI_Emoji` (**R**ecommended **f**or **G**eneral **I**nterchange) is defined by UTS #51 as the union of
several **string** properties:

| Property                       | Example            | Shape                                        |
|--------------------------------|--------------------|----------------------------------------------|
| `Basic_Emoji`                  | `😀` `❤️`          | one emoji code point, optionally `+ U+FE0F`  |
| `Emoji_Keycap_Sequence`        | `1️⃣`              | `digit U+FE0F U+20E3`                         |
| `RGI_Emoji_Flag_Sequence`      | `🇩🇪`              | a pair of regional indicator symbols         |
| `RGI_Emoji_Tag_Sequence`       | `🏴󠁧󠁢󠁥󠁮󠁧󠁿`     | black flag `+` tag characters `+ U+E007F`     |
| `RGI_Emoji_Modifier_Sequence`  | `👋🏽`              | base emoji `+` skin-tone modifier            |
| `RGI_Emoji_ZWJ_Sequence`       | `👨‍👩‍👧‍👦` `🧑‍🤝‍🧑` | emoji joined with `U+200D`                |

```
RGI_Emoji = Basic_Emoji
          ∪ Emoji_Keycap_Sequence
          ∪ RGI_Emoji_Flag_Sequence
          ∪ RGI_Emoji_Tag_Sequence
          ∪ RGI_Emoji_Modifier_Sequence
          ∪ RGI_Emoji_ZWJ_Sequence
```

This library matches on **Unicode scalar values**, decoding surrogate pairs first, so it never assumes
one UTF-16 `char` equals one character.

## Installation

The repository ships seven packages, all on one version:

| Package | Purpose | Target frameworks |
|---|---|---|
| `Broiler.Unicode.Properties` | Unicode character properties, scripts, categories, and case mappings | `net8.0`, `net10.0` |
| `Broiler.Unicode.Emoji.StringProperties` | Unicode RGI emoji sequence matching | `net8.0`, `net10.0` |
| `Broiler.Unicode.Cldr.LocaleData` | CLDR locale data for Intl implementations | `net8.0`, `net10.0` |
| `Broiler.Unicode.Algorithms` | The shared JavaScript libraries' Unicode algorithms, over tables the caller passes | `net10.0` |
| `Broiler.Unicode.Segmentation` | UAX #29 boundaries, over a break table the caller passes | `net10.0` |
| `Broiler.Unicode.Tables` | The generated tables, and a provider over them | `net10.0` |
| `Broiler.Unicode.Generation` | Test-only: the table generator and this repository's pin | `net10.0` |

Select prerelease packages while the libraries are in preview:

```sh
dotnet add package Broiler.Unicode.Emoji.StringProperties --prerelease
```

For maintainer instructions, see [publishing previews](eng/RELEASING.md).

Earlier GitHub previews used the package IDs `UnicodeEmoji.StringProperties` and
`UnicodeCldr.LocaleData`. Update those `PackageReference` entries (and central
`PackageVersion` entries, if used) to the new IDs above. C# namespaces remain
`UnicodeEmoji.StringProperties` and `UnicodeCldr.LocaleData`.

## API

All exact-match APIs require the **entire** input to be a single, fully-qualified emoji. They come in
`string` and `ReadOnlySpan<char>` overloads.

```csharp
// Exact whole-string predicates
bool  EmojiStringProperties.IsRgiEmoji(string value);
bool  EmojiStringProperties.IsRgiEmoji(ReadOnlySpan<char> value);
bool  EmojiStringProperties.IsBasicEmoji(/* string | ReadOnlySpan<char> */);
bool  EmojiStringProperties.IsEmojiZwjSequence(/* string | ReadOnlySpan<char> */);

// Full property set for a single sequence (None if not an exact RGI emoji)
EmojiSequenceProperties EmojiStringProperties.GetProperties(/* string | ReadOnlySpan<char> */);

// Test for a specific combination of properties
bool  EmojiStringProperties.HasProperties(ReadOnlySpan<char> value, EmojiSequenceProperties mask);

// Allocation-free enumeration of emoji embedded in text (greedy / leftmost-longest)
EmojiSequenceEnumerator EmojiStringProperties.EnumerateRgiEmoji(ReadOnlySpan<char> text);
EmojiSequenceEnumerator EmojiStringProperties.Enumerate(ReadOnlySpan<char> text, EmojiSequenceProperties filter);

// The Unicode emoji version the tables were generated from
string EmojiStringProperties.UnicodeEmojiVersion; // "17.0"
```

### Enumeration without allocating strings

`EnumerateRgiEmoji` returns a `ref struct` enumerator that yields `EmojiMatch` values describing each
emoji by **index and length** (in UTF-16 code units). No substrings are allocated — slice the original
text yourself if you need the characters.

```csharp
ReadOnlySpan<char> text = "Hi 👋 family 👨‍👩‍👧‍👦 from 🇩🇪!";

foreach (EmojiMatch m in EmojiStringProperties.EnumerateRgiEmoji(text))
{
    ReadOnlySpan<char> emoji = text.Slice(m.Index, m.Length);
    Console.WriteLine($"{m.Index}..{m.End} -> {m.Properties}");
}
```

### Classifying a single value

```csharp
EmojiSequenceProperties p = EmojiStringProperties.GetProperties("🇩🇪");
// p == EmojiSequenceProperties.RgiEmojiFlagSequence
//   (and p has the RgiEmoji bits, since every sequence property is a subset of RGI_Emoji)
```

## How it works

The official Unicode data files are treated as the **source of truth** and are committed under
[`data/unicode/17.0/`](data/unicode/17.0), whose two sequence files carry the hashes
[`data/unicode/pins/unicode.pin`](data/unicode/pins/unicode.pin) records for them:

- `emoji-test.txt`
- `emoji-sequences.txt`
- `emoji-zwj-sequences.txt`

A build-time generator parses those files, normalizes every entry into an array of Unicode scalar
values, classifies it into one of the six properties above, and builds a **flattened trie** keyed on
scalar values. The trie is emitted as compact, diff-friendly C# arrays
([`EmojiTrieData.g.cs`](src/UnicodeEmoji.StringProperties/Generated/EmojiTrieData.g.cs)) that are
**committed to source control**. At runtime the library only walks those arrays — it never parses data
files, uses reflection, or emits code.

Matching uses leftmost-longest semantics so the longest valid emoji (e.g. the complete four-person
family) wins over its shorter prefixes, and exact-match APIs require the entire input to terminate on a
trie node, so partial sequences are never reported as RGI.

## Updating to a new Unicode version

The `DataTool` downloads the official files and regenerates the lookup tables:

```sh
# Download data/unicode/<version>/ and regenerate src/.../Generated/EmojiTrieData.g.cs
dotnet run --project src/UnicodeEmoji.StringProperties.DataTool -- update 17.0

# Or as separate steps:
dotnet run --project src/UnicodeEmoji.StringProperties.DataTool -- download 17.0
dotnet run --project src/UnicodeEmoji.StringProperties.DataTool -- generate 17.0
```

Then bump `UnicodeEmojiVersion` in [`Directory.Build.props`](Directory.Build.props), review the diff of
the generated file, run the tests, and commit. Data is fetched **only** by this tool — never during a
normal build or at runtime.

**This does not move the shared JavaScript libraries' Unicode version.** Their archive,
[`data/unicode/pins/`](data/unicode/pins/README.md), moves only by a dated decision that moves
Broiler.VM's pin in the same change, and its files are never downloaded again by a tool.

## Benchmarks

[`benchmarks/`](benchmarks) compares the generated trie against a naive `HashSet<string>` and a single
large compiled `Regex`, for both exact matching and enumeration:

```sh
dotnet run -c Release --project benchmarks/UnicodeEmoji.StringProperties.Benchmarks -- --filter '*'
```

The trie matches in time proportional to the input length (independent of the ~3,800-entry data set),
allocates nothing on the hot path, and avoids the large startup and memory cost of compiling a
giant alternation regex.

## Limitations

- **RGI only.** This library implements the *recommended-for-general-interchange* set. It does **not**
  recognize valid-but-not-recommended emoji ZWJ sequences, arbitrary tag sequences, or every
  emoji-presentation code point in isolation.
- **Exact match means fully-qualified.** Minimally-qualified / unqualified forms (those missing a
  required U+FE0F) are intentionally **not** reported as RGI by the exact-match APIs.
- **Not a grapheme segmenter.** `EnumerateRgiEmoji` finds RGI emoji; it is not a general Unicode
  text-segmentation (UAX #29) implementation.
- **Emoji presentation is matching-only.** The library reports properties of text; it does not render,
  normalize, or transform emoji.
- **Version-pinned.** The data is generated for a single Unicode version (currently 17.0). Use the
  `DataTool` to move to another version.

## Project layout

```
src/
  UnicodeEmoji.StringProperties/            # the NuGet library (net8.0; net10.0)
  UnicodeEmoji.StringProperties.Generator/  # parser + trie builder + source emitter
  UnicodeEmoji.StringProperties.DataTool/   # downloads data + drives the generator
  Unicode.Algorithms/                       # the shared libraries' algorithms (net10.0)
  Unicode.Segmentation/                     # UAX #29 boundaries (net10.0)
  Unicode.Tables/                           # the generated tables and a provider (net10.0)
  Unicode.Generation/                       # the pin's checks and the table generator (net10.0)
  assurance.config.json                     # the assurance gate over the four projects above
tests/
  UnicodeEmoji.StringProperties.Tests/      # golden + data-driven xUnit tests
  Unicode.Algorithms.Tests/                 # Unicode's test files, ECMA-262's tables, the contract, SL7
  Unicode.Architecture.Tests/               # the conventions' rules SL1 to SL10
probes/
  Unicode.Probe/                            # the Native AOT probe
benchmarks/
  UnicodeEmoji.StringProperties.Benchmarks/ # trie vs HashSet vs Regex
data/
  unicode/17.0/                             # committed official Unicode emoji data files
  unicode/pins/                             # the pinned archive the shared libraries' tables come from
docs/
  roadmap.md, conventions.md, adapter-guide-unicode.md, evidence/
```

## License

Broiler.Unicode source code is licensed under the
[Apache License 2.0](LICENSE). Unicode-provided data files and derived tables remain
© Unicode, Inc. and subject to the
[Unicode Terms of Use](https://www.unicode.org/terms_of_use.html); see
[THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md). The Apache license provides the
software on an “AS IS” basis without warranties or conditions.
