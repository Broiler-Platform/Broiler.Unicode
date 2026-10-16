# Unicode.ReferenceArms: bundle sjl-7-001's reference-arm comparison

The library-level reference-arm comparison of
[`docs/evidence/sjl-7-001/reference-arms.md`](../../docs/evidence/sjl-7-001/reference-arms.md): the owner's
Unicode algorithms (arm **S**) against test-only copies of Broiler.VM's implementation at `24632b1` (arm **V**,
under `V/`) and of Broiler.JS's at `4d70821e` (arm **J**, under `J/`), over the population
`scripts/reference-arms/generate_population.py` writes. It **classifies nothing**: it writes `summary.json` and
`differences.tsv.gz`, and `scripts/reference-arms/classify_reference_arms.py` classifies them. The arms are copies
made for this comparison alone; nothing references this project, and the arms are retired at SJL-16.

The comparison's notation is the one `scripts/reference-arms/README.md` fixes in its section "`differences.tsv`:
the notation the classifier reads"; this file restates what the comparison writes.

## Commands

From the repository root, with the .NET 10 SDK on the path. Every build of this project goes to its own
artifacts directory, so that it never shares output with another build of the repository:

```sh
# Build: Release, JIT only (the project is never published, trimmed or compiled ahead of time).
dotnet build tests/Unicode.ReferenceArms -c Release --artifacts-path artifacts/reference-arms

ARMS="dotnet artifacts/reference-arms/bin/Unicode.ReferenceArms/release/Unicode.ReferenceArms.dll"

# The population and the arbiter's answers (scripts/reference-arms/README.md, "Commands").
python3 -I scripts/reference-arms/generate_population.py --out artifacts/reference-arms/population
python3 -I scripts/reference-arms/unicoderef.py --population artifacts/reference-arms/population --out artifacts/reference-arms/reference

# The comparison, under the invariant culture.
$ARMS compare --population artifacts/reference-arms/population --out artifacts/reference-arms/run

# The rule's second runs of arm J's culture-dependent members, under tr-TR and lt-LT.
$ARMS compare --population artifacts/reference-arms/population --out artifacts/reference-arms/run-tr-TR --culture tr-TR
$ARMS compare --population artifacts/reference-arms/population --out artifacts/reference-arms/run-lt-LT --culture lt-LT

# The negative controls, run once each.
$ARMS compare --population artifacts/reference-arms/population --out artifacts/reference-arms/negative-control --negative-control

# The classification, per run directory.
python3 -I scripts/reference-arms/classify_reference_arms.py --run artifacts/reference-arms/run --reference artifacts/reference-arms/reference
```

**The full listing is large** (16,022,956 lines on the population of 2026-10-16, 15.3 million of them arm V's
`;max=` lines of rows C12 and N6). The classifier's version 1 held a listing in memory and was stopped at 11.5 GB
on it; its version 2 reads and writes one line at a time, and classifies the whole listing in about eight minutes
in about 2 GB. Version 1's first classification, made in line ranges of 1,500,000 and summed, is retained in the
bundle only as the counts its rule revision quotes.

Options: `--population DIR` (without it, only U1 runs: the code point operations and U1's properties of
strings); `--out DIR` (required); `--culture NAME`; `--negative-control`; `--pins DIR` (the pinned tables the
names of U1's operations are read from; by default `data/unicode/pins` of the repository above the working
directory or the binary). `--limit LINES` (at most so many lines of each file), `--operations a,b` and
`--classes U1,U2` exist to try the comparison out: a run with any of them is **void** (`summary.json`:
`"void": true`), since the rule voids a run that stopped before the end of any operation's population.

## Configuration

- **Release, JIT only.** The project sets `PublishAot`, `IsAotCompatible`, `IsTrimmable` and `PublishTrimmed` to
  false; `summary.json` records `RuntimeFeature.IsDynamicCodeCompiled`.
- **Not in invariant globalization mode.** Arm J's host operations (`string.Normalize`, `CultureInfo`, a
  culture's `TextInfo`) run on the host's ICU, so the project sets `InvariantGlobalization` to false, and the
  run stops (exit 3) when the switch is set or a Turkish `TextInfo` does not map `i` to U+0130.
- **The culture.** Every thread of the run, the parallel workers' included, runs under the invariant culture
  (`CultureInfo.DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`, set in code), or under the
  culture `--culture` names for the rule's second runs. Every number the run writes is formatted invariantly.
- **ICU.** `summary.json`'s `environment` records the ICU the process loaded and the Unicode version it carries,
  each with how it was read: the version from the runtime itself, through `Interop.Globalization.GetICUVersion`
  in `System.Private.CoreLib` (by reflection; it packs major, minor, patch and build); the library's path from
  `/proc/self/maps`; and the Unicode version from ICU itself, `u_getUnicodeVersion_<major>` exported by that
  library. On the machine of the runs it was ICU 74.2 (`/usr/lib/x86_64-linux-gnu/libicuuc.so.74.2`) carrying
  Unicode 15.1.0. The runtime's own character data, which `char.GetUnicodeCategory` and `Rune` read (not ICU's),
  is recorded by two probes: U+1C89 (Unicode 16.0) and U+A7CE (Unicode 17.0).

## The arms

### Arm S

`src/Unicode.Algorithms`, `src/Unicode.Segmentation` and `src/Unicode.Tables` at the commit under test, as project
references, every table read through `UnicodeTables.Instance`: `UnicodeProperties`, `EcmaScriptLexical`,
`UnicodeCaseFolding`, `UnicodeCaseMapping`, `UnicodeNormalization`, `UnicodeStringProperties` and
`UnicodeSegmenter`. `summary.json` records each assembly's informational version, which carries its commit.

S is called as each engine's adapter will call it (adapter guide):

- **For arm V**, with a meter and Broiler.VM's string ceiling, 2^24 code units, as the maximum result length,
  written `;max=16777216` (or the U7 limit of another `;max=` case). For casing in a language other than the default, **the adapter's charge
  for the language pass** is added to S's total, before the call, as JSD-0074 decision 4 has Broiler.VM's
  adapter do: Broiler.VM's `LanguageCasing` (`JsRealm.String.cs:1166-1238`) runs only for `tr`, `az` and `lt`
  (:1168-1171) and charges `StringCharge(engine, text.Length)` (:1173), the input's length in UTF-16 code units,
  at least one unit (`StringCharge`, :973-974); so the adapter adds `max(1, length of the input)` for Turkic and
  Lithuanian and nothing for the default language (`Arms.LanguagePassCharge`).
- **For arm J**, with no meter and `int.MaxValue` as the maximum: Broiler.JS has no limit and no meter.

### Arm V: Broiler.VM at `24632b1`

Each file opens with a header naming its source path, its git blob, the members copied (with lines) and every
change; each change is also marked `ARM CHANGE` where it is made. The generated tables are copied unchanged:
every byte after the header's blank line is the blob's (checked with `git hash-object`).

| File | Source (`src/` of Broiler.VM) | Blob | Members | Changes |
|---|---|---|---|---|
| `V/JsUnicodeProperties.cs` | `Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.cs` | `9160add9` | the whole file: `JsUnicodeSet`, the lookup | none |
| `V/JsUnicodeProperties.g.cs` | `…Format/JsUnicodeProperties.g.cs` | `dd9186a5` | the whole file | none |
| `V/JsUnicodeLexical.cs` | `…Format/JsUnicodeLexical.cs` | `9909f968` | the whole file | none |
| `V/JsUnicodeCaseFolding.cs` | `…Format/JsUnicodeCaseFolding.cs` | `53c4de8d` | the whole file | none |
| `V/JsUnicodeCaseFolding.g.cs` | `…Format/JsUnicodeCaseFolding.g.cs` | `5ac4cb2c` | the whole file | none |
| `V/JsUnicodeStringProperties.cs` | `…Format/JsUnicodeStringProperties.cs` | `333c2fdf` | the whole file | none |
| `V/JsUnicodeStringProperties.g.cs` | `…Format/JsUnicodeStringProperties.g.cs` | `43fd263f` | the whole file | none |
| `V/JsUnicodeCasing.cs` | `Broiler.VM.Profile.JavaScript/JsUnicodeCasing.cs` | `4f8f0739` | the whole file | none |
| `V/JsUnicodeCasing.g.cs` | `…JavaScript/JsUnicodeCasing.g.cs` | `46b8e3c1` | the whole file | none |
| `V/JsUnicodeNormalization.cs` | `…JavaScript/JsUnicodeNormalization.cs` | `077c5e11` | the whole file | none |
| `V/JsUnicodeNormalization.g.cs` | `…JavaScript/JsUnicodeNormalization.g.cs` | `77f3a57d` | the whole file | none |
| `V/JsSegmenter.cs` | `…JavaScript/JsSegmenter.cs` | `3ac69898` | the whole file: `JsBreakData`'s decoding, `JsSegmentation` | none |
| `V/JsCldrTables.g.cs` | `Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs` | `ddc53aa2` | `SoftDotted` (:10312-10351), `SegmentBreakValues` (:14301-14312), `SegmentBreaks` (:14314-14929), unchanged | the other members left out; `SegmentBreakValues` copied as well, since `JsBreakData` cannot decode `SegmentBreaks` without it |
| `V/JsIntlTables.cs` | `…JavaScript/JsIntlTables.cs`; `…Format/JsIntlData.cs`; `…Intl/JsCldrData.cs` | `4316d98b`; `b85f88d4`; `ea0235db` | `IsSoftDotted`, `Breaks`, `Lines`, `ReadRanges` and the fields and constructor lines they need; `IJsIntlData.Table` and three `JsIntlTable` members; three arms of `JsCldrData.Table` | only those members, in one file |
| `V/JsRealm.String.cs` | `…JavaScript/JsRealm.String.cs` | `65b2fd7d` | `StringLengthCeiling`, the `normalize` built-in's call, `NormalizeText`, `IsNormalizedAlready`, `OrderCanonically`, `ComposeCanonically`, `WriteCodePoints`, `StringCharge`, `StringChangeCase`, `TransformCase`'s body, `LanguageCasing`, `AfterCapitalI`, `AfterSoftDotted`, `BeforeDot`, `MoreAbove`, `AppendCodePoint` | the ceiling a parameter (below); `TransformCase` takes the language (its locale resolution is the adapter's); visibility; `ArgOfString` not copied |
| `V/JsEngine.cs` | not a copy: stands for `JsEngine.Charge` (`JsEngine.Metering.cs:155`), `JsEngine.Error` (:270) and `JsEngine.Intl` (`JsEngine.cs:183`) | - | records every charge and its total; `JsThrow` by kind; Intl over the copied tables | - |

Package: `Broiler.ECMAScript.Algorithms` 0.1.0-preview.4 (Broiler.VM's pin at `24632b1`), whose
`Utf16.CodePointAt` `NormalizeText` calls.

**The ceiling.** `StringChangeCase`, `TransformCase` and `NormalizeText` take Broiler.VM's string ceiling as a
parameter where the source reads the constant `StringLengthCeiling` (1 << 24). The comparison passes the
constant for every case but the rule's U7 limit variants, where it passes the limit, so that Broiler.VM's own
enforcement (rows C12 and N6) runs at each limit: at the constant the copy is the source.

### Arm J: Broiler.JS at `4d70821e`

| File | Source (`Broiler.JS/` of Broiler.JS) | Blob | Members | Changes |
|---|---|---|---|---|
| `J/CharExtensions.cs` | `Broiler.JavaScript.Parser/CharExtensions.cs` | `9a5896d2` | the whole file: `IsIdentifierStart`, `IsIdentifierPart`, the hand lists, the Unicode 17 tables | the byte-order mark dropped |
| `J/FastScanner.cs` | `Broiler.JavaScript.Parser/FastScanner.cs` | `d39b2523` | the skip loop's white-space condition (:379) and its line-terminator test (:381-382) | each as a function of the code unit |
| `J/JSRegExp.cs` | `Broiler.JavaScript.BuiltIns/RegExp/JSRegExp.cs`; `…/RegExp/JSRegExp.UnicodeSets.cs` | `4859660c`; `a8e31b7e` | `IsRegExpIdentifierStart` and `IsRegExpIdentifierPart`; `TranslateUnicodeProperty` with `NormalizeKey`, `TranslateGeneralCategory`, `GeneralCategoryNames`, `EmojiStringPropertyNames`, `ExpandEmojiStringProperty`, `ExpandCodePointProperty`, `ComplementCodePointRanges`; the fall-through to .NET's `\p` (`NetFallThrough`); `ResolvePropertyOperand` with its helpers | LF line endings; a resolution answers its ranges or sequences where the source answers .NET pattern text; the in-class expansion not copied; errors through the arm's `JSEngine`; one nullable annotation |
| `J/JSStringPrototype.Transform.cs` | `Broiler.JavaScript.BuiltIns/String/JSStringPrototype.Transform.cs`; `…/Intl/JSIntl.cs` | `f346d9c9`; `83bd1413` | the bodies of `normalize`, `toLocaleLowerCase`, `toLocaleUpperCase`, `toLowerCase`, `toUpperCase`; the collator's `IsNormalized`/`Normalize` lines | the receiver's String and the locale (null for undefined) as parameters; **the current culture as a parameter**; the result unwrapped |
| `J/JSIntl.cs` | `Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs` | `83bd1413` | `Compute`, `ComputeGraphemes`, `ComputeSentences`, `ComputeWords`, `WordCat`, `WordCategory`, `IsNumericInfix`, `Joinable`, `IsWordLike` | a static class; visibility |
| `J/JSEngine.cs` | not a copy: stands for `JSEngine.NewSyntaxError` and `NewRangeError` (`Broiler.JavaScript.Engine/Core/JSEngine.cs:135, 151`) and `JSException` | - | the arm's recorded exception, by kind | - |

Packages, at the versions Broiler.JS resolved at `4d70821e` (its `Directory.Packages.props` pins `Broiler.Regex`
0.1.0-preview.4, whose nuspec depends on these two at 0.1.0-preview.3): `Broiler.Unicode.Properties`
0.1.0-preview.3 (`UnicodeProperties.Get*`, `JSStringSpecialCasing`) and `Broiler.Unicode.Emoji.StringProperties`
0.1.0-preview.3 (`EmojiStringProperties.GetSequences`), restored from nuget.org. This repository's own projects
of the same names are not referenced. The host: `string.Normalize`, `string.IsNormalized`, `CultureInfo`,
`StringInfo`, `Rune` and `char.GetUnicodeCategory`.

## What each arm does not implement

Each is also in `summary.json` (`notImplemented`) with the same reason:

| Arm | Operation | Why |
|---|---|---|
| J | `fold`, `orbit`, `canonicalize`, `canonicalize-orbit` | no member of arm J's list implements Canonicalize (Broiler.Regex's CaseFolding and the translator's .NET casing, rows F1 and F2, are not copied) |
| J | `lexical` `Soft_Dotted` | `JSStringSpecialCasing`'s Soft_Dotted hand set is private to the package; it is compared through Lithuanian casing (row C5) |
| J | `upper`, `lower`, `normalize` under `;max=` (Broiler.VM's ceiling and the U7 limit variants) | Broiler.JS has no limit (rows C12, N6) |
| V | `upper`, `lower`, `normalize` without `;max=` | Broiler.VM always has its ceiling: it runs under `;max=16777216` and the U7 limits |
| J | `charge` | Broiler.JS has no meter (rows C13, N7) |
| J | `lookup` with `ResolvePropertyOperand` under `u` | it resolves a v-mode class set operand |
| J | `normalize` with the collator in NFC, NFKC, NFKD | the collator normalizes to NFD only |
| J | `ToLocale*Case(undefined)` in any parameter but `default` | with no locale the culture is the host's current one (`@tr-TR`, `@lt-LT` in the second runs) |
| V | `upper`, `lower` with the cells' locales (`tr,en`, `az`, `TR`, `tr-u-co-search`) | a locale's resolution is Broiler.VM's adapter's; arm V runs the language it resolves to, `turkic` |
| J, V | `word-status`: the status's value | both answer isWordLike, a boolean; compared as Y/N per segment of S |
| J, V | the charge of segmentation | the rule compares it with no arm (row G5) |

## The operations and their population

U1, every code point from U+0000 to U+10FFFF, is implicit (generated in the harness). The files' formats are the
generator's (`scripts/reference-arms/README.md`, "The population").

| Operation | Classes | Parameter | S | V | J |
|---|---|---|---|---|---|
| `binary` | U1 | each of ECMA-262's 53 binary properties (canonical names, read from the pinned table) | `TryResolve(name).Contains` | `TryResolveLone(name).Contains` | `TranslateUnicodeProperty(name)` |
| `gc`, `sc`, `scx` | U1 | `-` | `GeneralCategoryOf`; the Script and Script_Extensions sets `TryResolve` answers | the value sets `TryResolve` answers | the value sets the translator answers |
| `lexical` | U1 | `IdentifierStartChar`, `IdentifierPartChar`, `RegExpIdentifierStart`, `RegExpIdentifierPart`, `WhiteSpace`, `Soft_Dotted` | `EcmaScriptLexical`; `UnicodeCaseMapping.IsSoftDotted` | `JsUnicodeLexical` (its tokenizer and group names); `JsIntlTables.IsSoftDotted` | `CharExtensions`; `JSRegExp.IsRegExpIdentifier*`; the scanner's white-space test |
| `fold`, `orbit`, `canonicalize`, `canonicalize-orbit` | U1 | `-` | `SimpleFold`, `Orbit`, `SimpleUpper`, `UpperOrbit` | `JsUnicodeCaseFolding`'s same four | - |
| `lookup` | U2 | `u`, `v` | `UnicodeProperties.TryResolve` (lone or pair); under `v`, `UnicodeStringProperties.TryResolve` first | the same order as its matcher (`JsRegExpMatcher.cs:2776-2810`) | `TranslateUnicodeProperty` with the fall-through; `ResolvePropertyOperand` under `v` |
| `upper`, `lower` | U3, U7 (`casing:` lines) | `default`, `turkic`, `lithuanian` (arm J), each also with `;max=16777216` and `;max=` its result's length less one, itself and plus one (arm V); for the lines tagged `cell:C7` and `cell:C9` also the locales `tr,en`, `az`, `TR` and `tr-u-co-search` (arm J alone; S in Turkic) | `ToUpperCase`/`ToLowerCase` in the language | `StringChangeCase`; `TransformCase` with `tr` or `lt` | `toUpperCase`/`toLowerCase`; `toLocale*Case("tr")`, `("lt")` and the cells' locales; `toLocale*Case(undefined)` under `default` |
| `normalize` | U4, U7 (`normalization:` lines) | `NFC`, `NFD`, `NFKC`, `NFKD` (arm J), each also with `;max=` as above (arm V) | `Transform` | `NormalizeText` through the built-in's call | `string.Normalize`; the collator's NFD |
| `string-property` | U1, U5 | each of the seven properties of strings | a code point by `ContainsCodePoint`, a sequence by the property's sequences (RGI_Emoji's from the other six) | `Collect`'s ranges and strings | the translator's `GetSequences` |
| `grapheme`, `word`, `sentence` | U6 | `-` | `Boundaries` | `JsSegmentation.Boundaries` | `JSIntlSegments.Compute` |
| `word-status` | U6 | `-` | `GetWordStatus != None` per word segment of S | `IsWordLike` on the same segments | `IsWordLike` on the same segments |

A parameter without `;max=` is arm J's: Broiler.JS has no limit, and S runs at `int.MaxValue`. Every case of arm V
carries a `;max=`: `;max=16777216` is Broiler.VM's ceiling as its adapter passes it, and the U7 limits follow
(a limit equal to the ceiling is written once). `;max=` is never negative: an empty result has no "one less".
Under `@tr-TR` and `@lt-LT` (the second runs) only arm J's variants of `upper`, `lower` and `normalize` run.

## Output

### `differences.tsv.gz`

UTF-8, LF, no header, one difference per line:

```
<operation>\t<class>\t<input>\t<parameter>\t<arm>\t<S's result>\t<the arm's result>
```

- **operation**: as in the table above, or `charge`.
- **class**: `U1` to `U7`, the rule's class (the population line's tag is not written).
- **input**: a code point in 4-to-6-digit uppercase hexadecimal (`0041`, `10FFFF`); a name as
  `<kind>/<property>/<value>` (`lone/Lu/`, `pair/sc/Grek`); a text as its UTF-16 code units in 4-digit uppercase
  hexadecimal separated by single spaces (the empty text is the empty field), or `repeat:<piece>:<count>:<suffix>`
  for a `repeat` line of limits.tsv.
- **parameter**: as in the table above; `;max=<n>` follows a limit variant's parameter, and `@<culture>` every
  parameter of a second run under that culture. A `charge` line's parameter is the operation, a space, and the
  answer line's parameter (`upper turkic;max=5`).
- **arm**: `V:` or `J:` and the member (`J:JSRegExp.TranslateUnicodeProperty`); the negative controls' are
  `V-control:` and `J-control:`.
- **results**:
  - `true` / `false`;
  - a code point in hexadecimal (`fold`, `canonicalize`); code points separated by single spaces (`orbit`,
    `canonicalize-orbit`); short value names separated by single spaces, in ordinal order, `-` for none (`gc`,
    `sc`, `scx`);
  - a text as its code units, as an input is written; for a `repeat` input,
    `len:<code units>:sha256:<SHA-256 of its UTF-16LE bytes>`;
  - boundary positions in UTF-16 code units, 0 and the length included, separated by single spaces; `word-status`
    is `Y` or `N` per word segment of S, concatenated;
  - a lookup's resolution, named as the arbiter names it: a binary property's canonical name (`Alphabetic`),
    `<Property>=<Value>` with both long names (`General_Category=Uppercase_Letter`, `Script=Greek`), or a property
    of strings' name (`RGI_Emoji`). A set is named by the canonical name the same arm resolves to the same
    members, the family the lookup's property names first and, among equal sets, the one an alias of which is
    the name given; a set no canonical name of the arm has is `set:` and its ranges in UCD notation
    (`0041..005A 00B5`, `-` for none); arm J's fall-through that .NET admits is `dotnet:` and the code units
    .NET's `\p` matches, in UCD notation;
  - `charge` lines: the total charge, an integer;
  - refusals: `refused` (a lookup's: S's false, arm V's false, arm J's SyntaxError); `over` (a result longer than
    the maximum: S's LimitExceeded, arm V's RangeError "Invalid string length"); `exception:<kind>` (any other:
    an engine error's constructor name, `exception:RangeError`, or a .NET exception's type,
    `exception:ArgumentException`).

An answer and its charge are compared apart: an answer line when the answers differ, a `charge` line when the
totals differ. The lines follow the operations' order (as in the table), then their classes, the inputs in
their files' order (U1 in code point order), each input's parameters, and the variants, an answer's line before
its charge's: the same population gives the same bytes. The work runs in parallel per chunk of inputs and is
written in that order.

### `summary.json`

`rule`, `mode` (`comparison`, `culture` or `negative-control`), `void` and the filters, `started`, `seconds`,
`environment` (runtime, OS, processors, memory, JIT, culture, globalization mode, ICU and its Unicode version
with how each was read, the runtime's character data, each assembly's informational version, the pins, the
arms' commits), `population` (directory, `manifestSha256`, the manifest itself, each file's SHA-256 and line
count), `names` (the names U1's operations run over, and the canonical names an arm refused), `notImplemented`,
`operations` (per operation and class: inputs, inputs not taken, cases, S's runs, seconds, and per variant the
cases ran, differing, not called, and the charges compared and differing), `differences` (the total, per
operation and arm, and the listing's line count and the SHA-256 of its uncompressed text) and `negativeControl`
(each planted change and whether the listing holds it).
