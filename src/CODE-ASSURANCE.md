# Broiler.Unicode shared libraries Code Assurance

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance generate --root Broiler.Unicode/src`, which rewrites this file,
`HUMAN_REVIEW.md`, `assurance.manifest.json` and every generated source header from the
product tree.

**No code unit in this component carries a decision on its human line yet.** This report
records that absence precisely. It is not a claim that the code is reviewed, assured or safe,
and the figures below are the measurement of how far from that claim the per-unit record is.

## Summary

| Metric | Value |
|---|---:|
| Files scanned | 14 |
| Files not covered | 1 |
| Files carrying an annotation | 14 |
| Code units | 546 |
| Relevant | 306 |
| Exempt by predicate | 240 |
| Annotated | 306 of 306 (100%) |
| Human reviewed | 0 of 306 (0%) |
| Unverified | 306 |

## Review states

| State | Count |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 306 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 240 |

## IP risk

| Value | Units |
|---|---:|
| None | 0 |
| Low | 308 |
| Medium | 0 |
| High | 0 |
| Unknown | 0 |
| *not annotated* | 0 |

## Security risk

| Value | Units |
|---|---:|
| None | 0 |
| Low | 102 |
| Medium | 182 |
| High | 24 |
| Critical | 0 |
| *not annotated* | 0 |

## Resource impact

| Metric | Value |
|---|---:|
| Maximum | 5 / 10 |
| Average over annotated units | 1.3 / 10 |
| Units scored | 306 |

## High-security review areas

- `Broiler.Unicode.Algorithms.TableReader.RequireTrie(IUnicodeTables, UnicodeTable)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.TableReader.RequireRecords(IUnicodeTables, UnicodeTable, UnicodeTable, int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.TableReader.Require(IUnicodeTables, UnicodeTable, int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.TableReader.Checked(IUnicodeTables)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.Utf16Reader.Next(ReadOnlySpan<char>, int, out int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.Utf16Reader.Previous(ReadOnlySpan<char>, int, out int)` in `Unicode.Algorithms/UnicodeAlgorithms.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseFolding.Orbit(int, Span<int>)` in `Unicode.Algorithms/UnicodeCaseFolding.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseFolding.UpperOrbit(int, Span<int>)` in `Unicode.Algorithms/UnicodeCaseFolding.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.ToUpperCase(string, CaseLanguage, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.ToLowerCase(string, CaseLanguage, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.NeedsLanguagePass(string, bool, CaseLanguage)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.Map(string, bool, CaseLanguage, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.DefaultPass(ReadOnlySpan<char>, string?, bool, bool, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.MapAscii(string, int, bool, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.MapFrom(ReadOnlySpan<char>, int, bool, int[], in CaseTables, bool, bool, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeCaseMapping.Triggers(bool, CaseLanguage)` in `Unicode.Algorithms/UnicodeCaseMapping.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeNormalization.Transform(string, NormalForm, int, UnicodeWorkCharge?, out string?)` in `Unicode.Algorithms/UnicodeNormalization.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.UnicodeNormalization.Produce(string, int, ReadOnlySpan<int>, bool, in FormTables, int, int, out string?)` in `Unicode.Algorithms/UnicodeNormalization.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Algorithms.StringPropertySequences.TryRead(Span<int>, out int)` in `Unicode.Algorithms/UnicodeStringProperties.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Generation.UnicodePin.Parse(string)` in `Unicode.Generation/UnicodePin.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Generation.UnicodePin.Violations(IReadOnlyDictionary<string, byte[]>)` in `Unicode.Generation/UnicodePin.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Generation.UnicodePin.Verified(IReadOnlyDictionary<string, byte[]>)` in `Unicode.Generation/UnicodePin.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Generation.UnicodeTableGenerator.Generate(UnicodePin, IReadOnlyDictionary<string, byte[]>)` in `Unicode.Generation/UnicodeTableGenerator.cs` - Security=High, human line PENDING
- `Broiler.Unicode.Segmentation.UnicodeSegmenter.Boundaries(ReadOnlySpan<char>, SegmentGranularity, UnicodeWorkCharge?)` in `Unicode.Segmentation/UnicodeSegmenter.cs` - Security=High, human line PENDING

## Falsification criteria

| Metric | Value |
|---|---:|
| Units carrying a criterion | 24 |
| Units required to carry one | 24 |
| Required and missing | 0 |

A `Broiler-Falsified-If:` line states, at the declaration, the observation that would make
the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the
criterion is the test. It is required where `Security` is `High` or `Critical` and written
nowhere else: `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Unicode/src` names every unit that owes one and carries none, and
every unit below `High` that carries one.

The line is a comment, so it is outside every fingerprint by construction: rewording a
criterion moves no recorded value here, in a file header or in
`assurance.manifest.json`, and invalidates nothing. That is the intended reading - a
criterion is an instruction to whoever reads the unit, not part of what a review is bound to.

## Exemption

Exemption is decided by one predicate in `CSharpAssuranceScanner`, not per unit, so
that the rule is reviewable in one place rather than in several hundred.

| Case | Units |
|---|---:|
| TrivialPropertyOrAccessor | 38 |
| ParameterAssigningConstructor | 5 |
| TrivialExpressionBodiedMember | 0 |
| CompilerSuppliedRecordOrEnumMember | 0 |
| DelegatingOverrideOrOperator | 0 |
| InsideAssemblyMarker | 0 |
| FieldDeclaringStorage | 48 |
| EnumMemberOfADeclaredVocabulary | 149 |
| DeclaredInSource | 0 |

## Per-unit exemptions

| Metric | Value |
|---|---:|
| Per-unit exemptions | 0 |

A per-unit `EXEMPT=<reason>` line exempts one unit by a reason a human wrote, for what the
predicate cannot see. Nothing mechanical checks that the reason is true, that it describes
the unit it sits on, or that it says anything at all, so every use is counted and named
here.

No unit in this component states a per-unit exemption.

## Files not covered

These files are under a covered project, or compiled into one, and are left out of the
record. They carry no generated header, no unit of theirs is in `assurance.manifest.json`, and
no figure in this report counts them. A path ending in `/` is a directory the tool does not
enter, and none of its files is covered.

| File | Reason |
|---|---|
| `Unicode.Tables/UnicodeTableData.g.cs` | generated data: the regeneration test holds every byte to Broiler.Unicode.Generation's output, and the generator is covered |

## Change detection

`assurance.manifest.json` lists **every** code unit in the 4 covered assemblies -
546 of them, exempt and relevant alike - with the fingerprint of its declaration.
This manifest is a change-detection record, not a review. A unit listed there is watched, not reviewed:
the entry records what the declaration's tokens hashed to when the generator last ran, and
nothing else. What the manifest adds is that a unit the exemption predicate treats as
trivial is no longer invisible: a semantic change to one moves a value in a generated file
the check compares byte for byte. `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Unicode/src` holds the manifest to the tree.

Beside the units it lists **every covered file** - 14 of them - with a
fingerprint over the complete token stream of its compilation unit. A unit entry exists only
for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an
`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.
Nothing in a covered file can change without something moving here, whatever kind of declaration it is. Comments are outside the stream, because a token's
text is its own characters, so the generated header above and the annotation lines below move
no file fingerprint - which is what lets one generation be a fixed point.

## Verification

The generator and the check are one computation: `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Unicode/src` works out what the
generator would write and compares it with the tree byte for byte, so a record edited by
hand, or left behind by code that moved, is reported rather than trusted.

| Mode | Command | Effect |
|---|---|---|
| Generate | `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance generate --root Broiler.Unicode/src` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `HUMAN_REVIEW.md`, `assurance.manifest.json` and this file. |
| Check | `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Unicode/src` | Reports every generated artefact that is not byte-identical to what the generator would produce, every relevant unit with no annotation, every annotation this system cannot read, every fingerprint out of date and every unit at the top of the security vocabulary without a criterion, or below it with one. |
| Release | `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Unicode/src --release` | The check, and additionally every relevant unit left in a state that blocks a release. |

The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token
texts, joined by single spaces. Trivia is excluded because a token's text is its own
characters and never the comments or whitespace around it, so `dotnet format` moves no
fingerprint and an annotation is never part of what it describes. The value answers whether a
unit changed since it was reviewed. It is not a collision-free identifier across units and it
is not a cryptographic commitment.
