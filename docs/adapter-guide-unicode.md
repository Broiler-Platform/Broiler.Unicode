# Adapter guide: `Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation` - Unicode

An engine's adapter is written against this guide, not against the library's source. If an adapter needs
something this guide does not say, the guide is incomplete and is corrected first. Where the obligation
comes from, and what governs it:

- **The obligation** is the shared JavaScript libraries programme's: `governance.md` section 10 and
  `architecture.md` section 5, under `docs/javascript-shared-libraries/` in the Broiler.VM repository. Its
  template is Broiler.JS's `Broiler.ECMAScript/docs/adapter-guide-template.md`.
- **The conventions** every operation follows are in [`conventions.md`](conventions.md).
- **The area's design** is that programme's `areas/unicode.md`.
- **Its verdict** is Broiler.VM's JSD-0074, with its six sub-areas: properties and lexical classes, case
  folding, case mapping, normalization, properties of strings, and segmentation.

Filed 2026-10-16, at work package SJL-7.2.

- **Libraries and version:** `Broiler.Unicode.Algorithms` and the optional `Broiler.Unicode.Segmentation`,
  from this repository's first preview that carries them. API baselines: each project's `PublicAPI.txt`.
  Namespaces `Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation` (JSD-0074, decision 1).
- **Specification pins:**
  - ECMA-262 at `0248456c`, whose three property tables are archived in
    [`data/unicode/pins/ecma262/`](../data/unicode/pins/ecma262);
  - ECMA-402's TransformCase and `Intl.Segmenter` at the same edition;
  - the Unicode Standard 17.0.0 and its annexes UAX #15, #29, #31 and #44, and UTS #51.
- **Data versions:**
  - `UnicodeAlgorithms.UnicodeVersion` and `UnicodeSegmentation.UnicodeVersion`, both `"17.0.0"`.
  - A provider whose `UnicodeVersion` is another is refused at construction.
- **Tables:** none in either package (decision SJL-D-6 (c)).
  - The engine embeds the tables `Broiler.Unicode.Generation` writes from the engine's own pin, and passes
    them as an `IUnicodeTables`: a version and one `ReadOnlySpan<byte>` per `UnicodeTable` member.
  - Its pin equals this repository's (Broiler.VM's rule I4).
  - Its embedded bytes equal the generator's output (rule SL7's shape in the engine).
  - `Broiler.Unicode.Tables` is this repository's own embedding, for tests and probes. Neither engine
    references it.
- **Consumers:** recorded here at the cut-overs. Broiler.VM's is SJL-7.4 and Broiler.JS's is SJL-7.5. The
  correspondence `docs/evidence/sjl-7-001/correspondence.md` in Broiler.VM lists the members each
  replaces.
- **Vectors:** Unicode's own test files at the pin, run by this repository's tests and its Native AOT
  probe, and the shared conformance corpus's Unicode vectors from SJL-7.3.

## What no operation here does

**By convention 2, no operation here does any of the following:**

- `ToString` of a receiver or an argument, or `RequireObjectCoercible`;
- reading of a `RegExp`'s flags;
- locale canonicalisation or negotiation;
- a `RangeError` or a `SyntaxError`;
- iteration objects for `Intl.Segmenter`.

The adapter does those, in the specification's order, and only then calls the library.

**What stays in the engines** (JSD-0074, decision 8):

- receiver and argument conversion;
- each engine's locale canonicalisation, and the mapping of a locale to `CaseLanguage`;
- the engines' ceilings and their `RangeError`s;
- the charging adapters, and the language pass's charge among them;
- `Intl.Segmenter`'s objects and iterators;
- Broiler.VM's realm-without-Intl policy for `toLocale*Case` (correspondence row C10);
- the regular-expression engines and Broiler.JS's translator, which are SJL-11's.

**Failure is data** (X3):

- an unknown property name is a `false` from `TryResolve`;
- an over-long result is `UnicodeStatus.LimitExceeded`;
- an invalid form or granularity is the adapter's to refuse before the call; the library throws on one, as
  a broken precondition.

**Charges** (JSD-0074, decision 4):

- Normalization and casing charge Broiler.VM's units (correspondence rows N7 and C13). Segmentation
  charges one unit per code point scanned (row G5).
- The lookups and folding take no meter. Their work is a bounded binary search, and a name longer than
  the longest the tables hold is refused before it is read.
- An engine with no budget passes `null`.
- Every charge is at least one unit, and is made before the work it pays for.

**Limits** (rows C12 and N6): the caller's maximum result length, in UTF-16 code units, checked on the
whole result.

**Construction.** Each type reads its tables once at construction and refuses a provider of another
version or one that lacks a table (`ArgumentException`). An instance is immutable and safe to share
between threads, so an engine builds one per provider and keeps it.

---

## `UnicodeProperties` - ECMA-262 §22.2.2.9, UnicodeMatchProperty and UnicodeMatchPropertyValue

### Signature

```csharp
public sealed class UnicodeProperties
{
    public UnicodeProperties(IUnicodeTables tables);
    public CodePointSet IdStart { get; }
    public CodePointSet IdContinue { get; }
    public CodePointSet SpaceSeparator { get; }
    public CodePointSet WhiteSpace { get; }
    public CodePointSet SoftDotted { get; }
    public bool TryResolve(ReadOnlySpan<char> name, out CodePointSet set);
    public bool TryResolve(ReadOnlySpan<char> property, ReadOnlySpan<char> value, out CodePointSet set);
    public CodePointSet Category(GeneralCategory category);
    public GeneralCategory GeneralCategoryOf(int codePoint);
}

public readonly struct CodePointSet
{
    public int RangeCount { get; }
    public bool IsEmpty { get; }
    public int First(int index);
    public int Last(int index);
    public bool Contains(int codePoint);
}
```

### What the caller must have done first

| Step | Clause | What the adapter does |
|---|---|---|
| 1 | §22.2.2.9 static semantics | Parse `\p{...}` or `\P{...}` and split `name=value` at the `=`; neither part is changed. |
| 2 | §22.2.2.9 | Decide whether the `u` or `v` flag is present. Without it, `\p` is not a property escape, and the library is not called. |
| 3 | §22.2.2.9, `v` only | Try `UnicodeStringProperties.TryResolve` for a lone name first when the `v` flag is present; a property of strings is not a code point property here. |

### Preconditions

None beyond construction. Every input is admitted: a name the specification does not admit, in any case,
spacing or spelling, is a `false` and an empty set.

### Work and limits

- **Meter:** none. A lookup is a binary search over the name table. A name longer than the longest the
  tables hold is refused before it is read.
- **Limits:** none. A set is a view over the tables and allocates nothing.

### What the caller does with each answer

| Answer | Meaning | The specification's error | Broiler.JS | Broiler.VM |
|---|---|---|---|---|
| `true` | The name, or the pair, is admitted; `set` is its code points | - | Builds its character class from the set's ranges | The same, in its compiler |
| `false` | Not admitted, matched exactly as ECMA-262's tables and `PropertyValueAliases.txt` write it | `SyntaxError`, early | Its `SyntaxError` | Its early error |

`Katakana_Or_Hiragana` (`Hrkt`) is admitted as a Script and Script_Extensions value with an empty set
(correspondence row P2). `Script_Extensions` is answered from `ScriptExtensions.txt` and `Scripts.txt`
together, as UAX #24 defines it, not from Script's sets.

---

## `EcmaScriptLexical` - ECMA-262 §12.2, §12.3 and §12.7

### Signature

```csharp
public sealed class EcmaScriptLexical
{
    public EcmaScriptLexical(UnicodeProperties properties);
    public bool IsIdentifierStart(int codePoint);
    public bool IsIdentifierPart(int codePoint);
    public bool IsWhiteSpace(int codePoint);
    public static bool IsLineTerminator(int codePoint);
}
```

### What the caller must have done first

The scanner decodes the code point, a surrogate pair included, and any `\u` escape. The library classifies
code points, not source text.

### Preconditions

None. A surrogate code point is neither IdentifierStartChar nor IdentifierPartChar.

### Work and limits

- **Meter:** none.
- **Limits:** none.

### What the caller does with each answer

| Answer | Meaning | Broiler.JS | Broiler.VM |
|---|---|---|---|
| `IsWhiteSpace` | U+0009, U+000B, U+000C, U+FEFF or Space_Separator. U+0085 is **not** white space. | Its scanner and `RegExp.escape` read this answer (JSD-0074, decision 6) | Its tokenizer and matcher read it, in place of the deleted `JsUnicodeLexical` (decision 7) |
| `IsIdentifierStart`, `IsIdentifierPart` | ID_Start or ID_Continue from `DerivedCoreProperties.txt`, whose UAX #31 derivation subtracts Pattern_Syntax, plus `$`, `_`, ZWNJ and ZWJ as the clause adds them | Its identifier scan | Its tokenizer |

---

## `UnicodeCaseFolding` - ECMA-262 §22.2.2.7.3, Canonicalize

### Signature

```csharp
public sealed class UnicodeCaseFolding
{
    public UnicodeCaseFolding(IUnicodeTables tables);
    public int MaxOrbit { get; }
    public int MaxUpperOrbit { get; }
    public int SimpleFold(int codePoint);
    public int Orbit(int codePoint, Span<int> destination);
    public int SimpleUpper(int codeUnit);
    public int UpperOrbit(int codeUnit, Span<int> destination);
}
```

### What the caller must have done first

| Step | Clause | What the adapter does |
|---|---|---|
| 1 | §22.2.2.7.3 step 1 | Decide whether `i` is set; without it, Canonicalize is the identity and the library is not called. |
| 2 | §22.2.2.7.3 steps 2 and 3 | Choose `SimpleFold` and `Orbit` under `u` or `v`, and `SimpleUpper` and `UpperOrbit` without them. |

### Preconditions

- `Orbit`'s span holds at least `MaxOrbit` entries, and `UpperOrbit`'s at least `MaxUpperOrbit`. A
  shorter span is an `ArgumentException`.

### Work and limits

- **Meter:** none; each answer is a bounded binary search.
- **Limits:** the orbit spans, sized by the two constants the tables state.

### What the caller does with each answer

Each answer is a code point or a count of code points written into the caller's span, in ascending order.
Folding uses `CaseFolding.txt`'s statuses C and S only, so U+0130 and U+0131 fold to themselves. Without
`u`, a code unit whose upper case is not exactly one code unit, or would move into ASCII from outside it,
maps to itself. The regular-expression engines that call these are SJL-11's; this guide states only the
contract.

---

## `UnicodeCaseMapping` - ECMA-262 §22.1.3.29 and §22.1.3.31; ECMA-402 TransformCase

### Signature

```csharp
public enum CaseLanguage { Default = 0, Turkic = 1, Lithuanian = 2 }

public sealed class UnicodeCaseMapping
{
    public UnicodeCaseMapping(IUnicodeTables tables);
    public int MaxMappingLength { get; }
    public bool IsCased(int codePoint);
    public bool IsCaseIgnorable(int codePoint);
    public bool IsSoftDotted(int codePoint);
    public UnicodeStatus ToUpperCase(string text, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result);
    public UnicodeStatus ToLowerCase(string text, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result);
}
```

### What the caller must have done first

| Step | Clause | What the adapter does |
|---|---|---|
| 1 | §22.1.3.29 steps 1 and 2 | `RequireObjectCoercible(this)` and `ToString(this)`; an abrupt completion propagates before the library is called. |
| 2 | ECMA-402 TransformCase steps 1 to 4, `toLocale*Case` only | Canonicalise the requested locales, take the best available locale, and map its language to `CaseLanguage`: `tr` and `az` to `Turkic`, `lt` to `Lithuanian`, anything else to `Default`. |
| 3 | Broiler.VM only | Charge the language pass's extra input length itself, before the call, when `language` is not `Default` (JSD-0065, decision 2; JSD-0074, decision 4). |

### Preconditions

- `text` is not null; `maxLength` is not negative.
- `language` is one of the three; another value is an `ArgumentOutOfRangeException`.

### Work and limits

- **Meter:** Broiler.VM's units on the default pass (row C13), each charge at least one unit:
  - the length of its input;
  - then each mapping's length, in code points;
  - then the code units each Final_Sigma test reads.

  The language pass charges nothing here.
- **Limits:** `maxLength`, in UTF-16 code units, checked as the result grows (row C12).

### What the caller does with each status

| Status | Meaning | The specification's error | Broiler.JS | Broiler.VM |
|---|---|---|---|---|
| `Success` | `result` is the mapped text; a text casing does not change is answered as itself | - | Returns it | Returns it |
| `LimitExceeded` | The result is longer than `maxLength` | None in the specification: an implementation limit | Its string-length `RangeError` | Its ceiling's `RangeError` |

### Engine-specific behaviour

| Parameter | Broiler.JS passes | Broiler.VM passes | Why they differ |
|---|---|---|---|
| `maxLength` | Its string ceiling | Its string ceiling | Each engine's ceiling is its own policy |
| `charge` | `null` | Its fuel adapter | Broiler.JS has no fuel |
| `language` from `toLocale*Case` in a realm without Intl | - | `Default` (row C10) | Broiler.VM's policy for such realms |

---

## `UnicodeNormalization` - ECMA-262 §22.1.3.15, `String.prototype.normalize`; UAX #15

### Signature

```csharp
public enum NormalForm { Nfc = 0, Nfd = 1, Nfkc = 2, Nfkd = 3 }
public enum NormalizationQuickCheck { Yes = 0, Maybe = 1, No = 2 }

public sealed class UnicodeNormalization
{
    public UnicodeNormalization(IUnicodeTables tables);
    public int MaxDecompositionLength { get; }
    public int CombiningClass(int codePoint);
    public NormalizationQuickCheck QuickCheck(int codePoint, bool compatibility);
    public UnicodeStatus Transform(string text, NormalForm form, int maxLength, UnicodeWorkCharge? charge, out string? result);
}
```

No member is named `Normalize`, `IsNormalized` or `NormalizationForm` (convention 14), so Broiler.VM's
rule N23 admits the adapter that calls them.

### What the caller must have done first

| Step | Clause | What the adapter does |
|---|---|---|
| 1 | §22.1.3.15 steps 1 and 2 | `RequireObjectCoercible(this)` and `ToString(this)`. |
| 2 | §22.1.3.15 steps 3 to 6 | `form` undefined is `"NFC"`; otherwise `ToString(form)`. A value other than `"NFC"`, `"NFD"`, `"NFKC"` or `"NFKD"` is the adapter's `RangeError`, thrown before the call. |

### Preconditions

- `text` is not null; `maxLength` is not negative.
- `form` is one of the four. Another value is an `ArgumentOutOfRangeException`, which an adapter that
  did step 2 never sees.

### Work and limits

- **Meter:** Broiler.VM's units (row N7), each charge at least one unit:
  - the input's length;
  - then, unless the input is already in the form, the input's length plus three units per code point of
    its full decomposition, charged before that work.
- **Limits:** `maxLength`, in UTF-16 code units, checked on the result (row N6). A result that fits is
  returned however long its decomposition was. The work is done segment by segment, and a result that
  cannot fit is refused before the second charge.

### What the caller does with each status

| Status | Meaning | The specification's error | Broiler.JS | Broiler.VM |
|---|---|---|---|---|
| `Success` | `result` is the normalized text; a text already in the form is answered as itself | - | Returns it | Returns it |
| `LimitExceeded` | The result is longer than `maxLength` | None in the specification | Its string-length `RangeError` | Its ceiling's `RangeError` |

---

## `UnicodeStringProperties` - ECMA-262 §22.2.2.9, the properties of strings

### Signature

```csharp
public sealed class UnicodeStringProperties
{
    public UnicodeStringProperties(IUnicodeTables tables);
    public int PropertyCount { get; }
    public bool TryResolve(ReadOnlySpan<char> name, out StringProperty property);
    public StringProperty Property(int id);
}

public readonly struct StringProperty
{
    public int Id { get; }
    public bool IsUnion { get; }
    public int CodePointRangeCount { get; }
    public int SequenceCount { get; }
    public int FirstCodePoint(int index);
    public int LastCodePoint(int index);
    public bool ContainsCodePoint(int codePoint);
    public StringPropertySequences GetSequences();
}

public struct StringPropertySequences
{
    public const int MaxSequenceLength = 255;
    public readonly int Remaining { get; }
    public bool TryRead(Span<int> destination, out int length);
}
```

### What the caller must have done first

The adapter has established that the `v` flag is present and that `\p{name}` has no `=`. `\P{...}` of a
property of strings is the adapter's early `SyntaxError`.

### Preconditions

- `TryRead`'s span holds at least the sequence's length; `MaxSequenceLength` always does. A shorter span
  is an `ArgumentException`, and the cursor does not advance.
- `Property`'s id is from zero to `PropertyCount` less one.

### Work and limits

- **Meter:** none. Reading every sequence is bounded by the tables.
- **Limits:** none. Nothing is allocated: each sequence is decoded into the caller's span.

### What the caller does with each answer

`RGI_Emoji` is the union of the other six. It has no data of its own: `IsUnion` is true, its counts are
zero, `ContainsCodePoint` asks the others, and the adapter reads the others' sequences.

---

## `UnicodeSegmenter` - ECMA-402 §18, `Intl.Segmenter`; UAX #29

### Signature

```csharp
public enum SegmentGranularity { Grapheme = 0, Word = 1, Sentence = 2 }
public enum WordStatus { None = 0, Number = 100, Letter = 200, Kana = 300, Ideographic = 400 }

public sealed class UnicodeSegmenter
{
    public UnicodeSegmenter(IUnicodeTables tables);
    public int[] Boundaries(ReadOnlySpan<char> text, SegmentGranularity granularity, UnicodeWorkCharge? charge);
    public WordStatus GetWordStatus(ReadOnlySpan<char> segment, UnicodeWorkCharge? charge);
    public BreakProperties PropertiesOf(int codePoint);
}
```

### What the caller must have done first

| Step | Clause | What the adapter does |
|---|---|---|
| 1 | ECMA-402 `Intl.Segmenter` constructor | Resolve the locale and `granularity` option. An invalid granularity is the adapter's `RangeError`. |
| 2 | `segment(string)` | `ToString(string)`. |

### Preconditions

- `granularity` is one of the three; another value is an `ArgumentOutOfRangeException`.

### Work and limits

- **Meter:** one unit per code point the pass classifies, charged before the pass. Then one unit per code
  point any rule's look-behind or look-ahead reads beyond the two around the position: GB9c, GB11, GB12,
  WB4, WB15 and SB5 to SB11. `GetWordStatus` charges one unit per code point it reads. These units correct
  Broiler.VM's row G5 (JSD-0074, decision 4).
- **Limits:** none. The output is bounded by the input: at most one boundary per code unit, plus one.

### What the caller does with each answer

`Boundaries` answers code unit indices: the text's start, every boundary inside it, and its end. The
adapter's segments and iterators are built from consecutive pairs. `isWordLike` is
`GetWordStatus(segment) != WordStatus.None`. The boundaries are UAX #29's defaults, with no locale tailoring
and no dictionary.

---

*Revisions of this guide are dated in place, quoting the text they replace.*
