# Human Review: Broiler.Unicode shared libraries

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance generate --root Broiler.Unicode/src`, which rewrites this file,
`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 247 relevant units. `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Unicode/src --release`
> fails while any relevant unit is without a decision bound to its current fingerprint.

## 1. How To Use This File

Read it; do not edit it. A decision about a code unit is the `// Broiler-Human:` line on that
unit's declaration, and every table below is read out of those lines. There is nothing here
to fill in and nothing here to leave blank.

## 2. How A Review Is Recorded

In one place: the `// Broiler-Human:` line of the assurance annotation that sits on the
declaration being read. Nothing in this file is edited by hand, no second document carries a
per-item checklist, and no list of permitted aliases exists to be added to.

```csharp
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4A3BFD
// Broiler-Falsified-If: a negative value reaches the running total
// Broiler-Human:        PENDING
```

The last line has four shapes. A human writes three of them; the generator writes the fourth
and may never invent an alias, which the check asserts in both directions.

| Line | Meaning |
|---|---|
| `PENDING` | Nobody has recorded a decision for this unit. The generator leaves it exactly as it stands. |
| `<alias>` | A human states their own alias and leaves the machine field to the generator, which fills it with the declaration's fingerprint at the next run. |
| `<alias>; Fingerprint=<six hex>` | A decision bound to one exact version of one declaration. |
| `STALE; Previous=<alias>@<fingerprint>` | Written by the generator when the code moved after a decision. Only a human clears it, by stating their alias again. |

A human may state their own `IP=`, `Security=`, `Resources=`, `CS=` and `CL=` assessment beside their alias,
which is how a reader disagrees with the machine assessment on the line above: an assessment is
a comment and moves no fingerprint, so there is nowhere else to say it.

**No branch, commit or tag is recorded in this file.** Each decision names the fingerprint of
the declaration it was made against, and the state machine compares that value with the
declaration as it now stands. A commit says a tree moved; a fingerprint says whether this unit
did, which is the narrower and the more useful of the two.

## 3. Summary

| Metric | Value |
|---|---:|
| Files scanned | 14 |
| Code units | 461 |
| Relevant | 247 |
| Exempt | 214 |
| Assessed | 247 of 247 (100%) |
| Human reviewed | 0 of 247 (0%) |
| Unverified | 247 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 247 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 214 |

## 5. Aliases In The Tree

No alias appears on a human line anywhere in the product tree. Nobody has recorded a
decision about any unit of this component.

## 6. Coverage By File

One row per covered file, carrying that file's generated header. `Unverified` counts the
relevant units in a state that blocks a release. The files the configuration leaves out
are listed in `CODE-ASSURANCE.md`.

| File | Units | Relevant | Exempt | Unverified | IP risk | Security risk | Criteria |
|---|---:|---:|---:|---:|---|---|---:|
| `Unicode.Algorithms/IUnicodeTables.cs` | 42 | 3 | 39 | 3 | Low | Medium | 0/0 |
| `Unicode.Algorithms/UnicodeAlgorithms.cs` | 21 | 19 | 2 | 19 | Low | High | 4/4 |
| `Unicode.Algorithms/UnicodeCaseFolding.cs` | 13 | 10 | 3 | 10 | Low | High | 2/2 |
| `Unicode.Algorithms/UnicodeCaseMapping.cs` | 29 | 23 | 6 | 23 | Low | High | 4/4 |
| `Unicode.Algorithms/UnicodeNormalization.cs` | 44 | 29 | 15 | 29 | Low | High | 2/2 |
| `Unicode.Algorithms/UnicodeProperties.cs` | 74 | 27 | 47 | 27 | Low | Medium | 0/0 |
| `Unicode.Algorithms/UnicodeStringProperties.cs` | 35 | 22 | 13 | 22 | Low | High | 1/1 |
| `Unicode.Generation/UnicodeDatabase.cs` | 64 | 47 | 17 | 47 | Low | Medium | 0/0 |
| `Unicode.Generation/UnicodePin.cs` | 18 | 15 | 3 | 15 | Low | High | 3/3 |
| `Unicode.Generation/UnicodeTableGenerator.cs` | 20 | 18 | 2 | 18 | Low | High | 1/1 |
| `Unicode.Generation/UnicodeTableSource.cs` | 2 | 2 | 0 | 2 | Low | Medium | 0/0 |
| `Unicode.Segmentation/BreakProperties.cs` | 57 | 5 | 52 | 5 | Low | Low | 0/0 |
| `Unicode.Segmentation/UnicodeSegmenter.cs` | 36 | 23 | 13 | 23 | Low | High | 1/1 |
| `Unicode.Tables/UnicodeTables.cs` | 6 | 4 | 2 | 4 | Low | Medium | 0/0 |

## 7. Decisions Recorded

No unit in this component carries a decision on its human line. Every one of them reads
`PENDING`.

## 8. Decisions The Code Has Outrun

No unit carries a decision that the code has since moved past.

## 9. Where A Decision Is Required First

The units at the top of the security vocabulary, with the observation that would show each
one wrong and the human line it carries. The set is read from the assessments rather than
written out, so a unit that becomes `High` joins it at the next generation.

- `Broiler.Unicode.Algorithms.TableReader.Require(IUnicodeTables, UnicodeTable, int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, Spec=Unicode 17.0.0 table encoding via Broiler.VM JsUnicodeProperties, `B99053`, PENDING
  - Falsified if: a provider lacking a table, or holding one that is not a whole number of records, is accepted
- `Broiler.Unicode.Algorithms.TableReader.Checked(IUnicodeTables)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, Spec=Unicode 17.0.0 table encoding via Broiler.VM JsUnicodeProperties, `BBE7EE`, PENDING
  - Falsified if: a provider whose version is not 17.0.0 is accepted
- `Broiler.Unicode.Algorithms.Utf16Reader.Next(ReadOnlySpan<char>, int, out int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, Spec=ECMA-262 6.1.4 code points via Broiler.VM JsRealm.String, `F879D7`, PENDING
  - Falsified if: a lone surrogate is read as part of a pair, or the width returned passes the end of the text
- `Broiler.Unicode.Algorithms.Utf16Reader.Previous(ReadOnlySpan<char>, int, out int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, Spec=ECMA-262 6.1.4 code points via Broiler.VM JsRealm.String, `60CAE2`, PENDING
  - Falsified if: a lone surrogate is read as part of a pair, or the width returned passes the start of the text
- `Broiler.Unicode.Algorithms.UnicodeCaseFolding.Orbit(int, Span<int>)` in `Unicode.Algorithms/UnicodeCaseFolding.cs` - Security=High, Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding, `49FFB8`, PENDING
  - Falsified if: more code points are written than the destination holds, or a code point of another orbit is written
- `Broiler.Unicode.Algorithms.UnicodeCaseFolding.UpperOrbit(int, Span<int>)` in `Unicode.Algorithms/UnicodeCaseFolding.cs` - Security=High, Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding, `EF9D2E`, PENDING
  - Falsified if: more code units are written than the destination holds, or a code unit of another orbit is written
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.ToUpperCase(string, CaseLanguage, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing, `1ADC04`, PENDING
  - Falsified if: a result longer than maxLength is answered as Success
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.ToLowerCase(string, CaseLanguage, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing, `503D46`, PENDING
  - Falsified if: a result longer than maxLength is answered as Success
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.Map(string, bool, CaseLanguage, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing, `9F0A22`, PENDING
  - Falsified if: a result longer than maxLength is answered as Success, or a language pass is applied for the default language
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.DefaultPass(string, bool, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing, `B8867F`, PENDING
  - Falsified if: work is done before the meter is charged for it, or a charge differs from Broiler.VM row C13
- `Broiler.Unicode.Algorithms.UnicodeNormalization.Transform(string, NormalForm, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeNormalization.cs` - Security=High, Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization, `DBFB2E`, PENDING
  - Falsified if: a result longer than maxLength is answered as Success, or a charge differs from Broiler.VM row N7
- `Broiler.Unicode.Algorithms.UnicodeNormalization.Produce(string, bool, bool, int, int, out string?)` in `Unicode.Algorithms/UnicodeNormalization.cs` - Security=High, Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization, `F5D030`, PENDING
  - Falsified if: a buffer grows past the longest segment times the longest decomposition, or a refused result is returned
- `Broiler.Unicode.Algorithms.StringPropertySequences.TryRead(Span<int>, out int)` in `Unicode.Algorithms/UnicodeStringProperties.cs` - Security=High, Spec=ECMA-262 22.2.2.9 properties of strings via Broiler.VM JsUnicodeStringProperties, `995B67`, PENDING
  - Falsified if: a sequence is written into a destination shorter than it, or the cursor advances after refusing
- `Broiler.Unicode.Generation.UnicodePin.Parse(string)` in `Unicode.Generation/UnicodePin.cs` - Security=High, Spec=JSD-0031 s5 pin checks via Broiler.VM UnicodeTableGenerator, `80ECFA`, PENDING
  - Falsified if: a malformed or repeated line is accepted
- `Broiler.Unicode.Generation.UnicodePin.Violations(IReadOnlyDictionary<string, byte[]>)` in `Unicode.Generation/UnicodePin.cs` - Security=High, Spec=JSD-0031 s5 pin checks via Broiler.VM UnicodeTableGenerator, `719779`, PENDING
  - Falsified if: a file whose length, hash or version header differs from the pin yields no violation
- `Broiler.Unicode.Generation.UnicodePin.Verified(IReadOnlyDictionary<string, byte[]>)` in `Unicode.Generation/UnicodePin.cs` - Security=High, Spec=JSD-0031 s5 pin checks via Broiler.VM UnicodeTableGenerator, `94612F`, PENDING
  - Falsified if: an archive with a violation is returned instead of refused
- `Broiler.Unicode.Generation.UnicodeTableGenerator.Generate(UnicodePin, IReadOnlyDictionary<string, byte[]>)` in `Unicode.Generation/UnicodeTableGenerator.cs` - Security=High, Spec=JSD-0074 SJL-D-6 (c) tables via Broiler.VM UnicodeTableGenerator, `D8215B`, PENDING
  - Falsified if: a table is written from a file the pin does not name or that failed verification
- `Broiler.Unicode.Segmentation.UnicodeSegmenter.Boundaries(ReadOnlySpan<char>, SegmentGranularity, UnicodeWorkCharge?)` in `Unicode.Segmentation/UnicodeSegmenter.cs` - Security=High, Spec=UAX #29 via Broiler.VM JsSegmentation, `22E7CF`, PENDING
  - Falsified if: a boundary outside 0 to the text length is answered, or a code point is scanned before its unit is charged

## 10. What This Record Does Not Say

It is not an approval of the component, and a full table above would not be one either. It
records which declarations somebody stated a decision about, and against which version of
each. It does not record what they read, how long they spent, or whether they were right.

A fingerprint is six hex characters of SHA-256 over a declaration's token texts. It answers
whether a unit changed since a decision was recorded against it. It is not a collision-free
identifier across units and it is not a cryptographic commitment, so it detects a change and
does not resist a forger with commit access.

An assessment is a comment, so changing one moves no fingerprint anywhere, and nothing
mechanical checks that it is right; the check holds its values to their vocabularies and no
further.

15 of the 247 assessed units declare `Origin=AI`. Reading a declaration is the only thing
that makes it read.
