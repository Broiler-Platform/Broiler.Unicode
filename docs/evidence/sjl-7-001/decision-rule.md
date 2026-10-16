# Bundle SJL-7-001's decision rule: the owner's clauses of the Unicode area - its conditions, its contract, Unicode's test files and generation byte for byte

**Written:** 2026-10-16, at milestone SJL-7 (work package SJL-7.1) of the shared JavaScript libraries
programme, with the dated record of this repository's `docs/roadmap.md` that adopts the four owner
conditions, and before any code of the milestone exists in this repository: before the pin file, the
generators, the exact lookups and the algorithms this rule judges. **Milestone judged:** SJL-7,
`Broiler.VM/docs/javascript-shared-libraries/milestones/sjl-7-unicode.md`: its component clauses 1 to 4.
**Owner:** the Broiler.Unicode maintainer. **Reviewer:** none. Every role this rule names, the two engines'
owners included, is held by one person (the programme records this as Broiler.VM's exception EX-30), and
this rule does not claim that anyone independent has read it.

**What this file is.** Four predeclared decisions, one per clause, each judged on its own; none reads
another's runs. Part A is layer L4 of the programme's validation document
(`Broiler.VM/docs/javascript-shared-libraries/validation.md` section 2), Unicode's own test files. Part B is
rule SL7's byte-for-byte regeneration, with the milestone's negative controls (section 7 of the same
document). Parts C and D are the contract and the conditions, each judged by a mechanism this file names.
The library-level reference-arm comparison is [`reference-arms.md`](reference-arms.md) beside this file;
each engine's test262, vectors, fuel parity and measurement are judged by its own rule in its own
repository. **It holds no figure.** Where a count matters, it is a parameter of the protocol, not a result.

**The area's record** is Broiler.VM's JSD-0074
(`src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md`
in that repository). Its input, the correspondence from code with each difference numbered (P, I, W, F, C,
N, S, G), is Broiler.VM's `docs/evidence/sjl-7-001/correspondence.md`.

**The pinned inputs.** Every part reads Unicode's files from this repository's pin file, which SJL-7.2
creates as a copy of Broiler.VM's `src/tests/unicode/pins/unicode.pin`: the UCD 17.0.0 files, the six
auxiliary files of UAX #29, the two emoji sequence files and ECMA-262's three property tables, each by its
length and SHA-256. A file whose bytes do not match its pin line is a configuration failure of the part that
reads it, and that part is not judged.

## Part A - Unicode's test files (clause 3; layer L4)

```rule
evidence class: conformance at library level against the standards' own tests (not a measurement)
population: every case of each file below, read from the pinned inputs, with no case excluded:
  A1 NormalizationTest.txt - every line of its parts 0 to 3, with the invariants its header states: c2 = NFC(c1) = NFC(c2) = NFC(c3), c4 = NFC(c4) = NFC(c5), c3 = NFD(c1) = NFD(c2) = NFD(c3), c5 = NFD(c4) = NFD(c5), c4 = NFKC(c1) to NFKC(c5), c5 = NFKD(c1) to NFKD(c5); and part 1's closing claim, that every code point from U+0000 to U+10FFFF not in its c1 column, the surrogate code points included, is unchanged by all four forms
  A2 GraphemeBreakTest.txt, WordBreakTest.txt and SentenceBreakTest.txt - every line: the boundaries the owner's segmenter reports for the line's code points, at the line's granularity, are exactly the positions the line marks with a division sign
  A3 CaseFolding.txt - every line of status C and S: the owner's simple folding maps the code point to the line's mapping; every code point from U+0000 to U+10FFFF on no such line folds to itself; and every code point's orbit is exactly the set of code points with the same simple folding
  A4 SpecialCasing.txt with UnicodeData.txt - for every code point from U+0000 to U+10FFFF, alone, the owner's full upper and lower mappings in the default language are the file's unconditional mapping where it has one and UnicodeData's simple mapping otherwise, or the code point itself; and for every conditional line, for its language or none, each context the condition names (Final_Sigma, After_Soft_Dotted, More_Above, Before_Dot and After_I) built once where the condition holds and once where it does not, from the contexts reference-arms.md's class U3 declares
  A5 ECMA-262's three property tables - every property name and alias the tables admit, and every value and alias of General_Category, Script and Script_Extensions that PropertyValueAliases.txt lists, Katakana_Or_Hiragana and Hrkt included, is admitted by the owner's exact lookup and resolves to the property the tables name; and every name in reference-arms.md's refused set U2 is refused
configuration: this repository's test project, built in Release, at the commit the bundle names; the owner's code reads its tables through its provider from the tables its generator writes from the pinned inputs, which part B holds byte for byte; the process's current culture set to the invariant culture and its globalization mode recorded, since no owner member may read either
comparison: per case, the owner's answer against the file's, exactly: strings by their code points, boundaries as the list of positions, sets by their members
admitted difference classes: none; a case that fails is a defect of the owner, fixed before either cut-over lands, after which the whole part is run again and both runs are retained
MET if and only if every file was read under its pin, every case of A1 to A5 ran, and every case passed.
NOT MET otherwise.
```

**How it is read.** The bundle retains each file's run log, with the count of cases run and passed and the
first failing cases if any, and the commit the run was taken at. A part run that stopped before the end of
any file is void and is not judged. **There is no exclusion list**: Broiler.VM's segmenter and normalizer
pass these files today in its own tests, and the owner starts from ports of its generators.

## Part B - generation byte for byte, and the pin (clause 4)

```rule
evidence class: artifact identity (not a measurement)
population: every table the owner's generators write from the pinned inputs: the property and lexical tables, the case-folding and case-mapping tables with Soft_Dotted, the normalization tables, the properties of strings with the ECMAScript name map, and the segmentation break tables
configuration: the test of rule SL7's shape in this repository's test project, which runs the generators from the pinned inputs into a temporary directory and compares each output with the committed table; run twice, in two processes, at the commit the bundle names
comparison: per table, the generated bytes against the committed bytes, exactly; the two runs' outputs against each other, exactly; and the pin file against Broiler.VM's src/tests/unicode/pins/unicode.pin at the revision Broiler.VM's rule I4 reads, line by line, by file name, length and SHA-256, a specification table's repository-relative path compared by its file name
negative controls, each run once, each reverted, and each revert checked by the same test passing again:
  1. one byte of one committed table changed: the test fails and names the table;
  2. one hash of the pin file changed: the generator refuses the input the line names, before writing anything;
  3. one byte of one pinned input changed in a scratch copy of the inputs: the generator refuses it
admitted difference classes: none
MET if and only if every table is byte-identical to its committed bytes in both runs, the two runs agree, the pin file agrees with Broiler.VM's line by line, and each negative control failed as stated and its revert passed.
NOT MET otherwise.
```

**Why both pins are compared here as well.** Decision SJL-D-6 (c) lets each engine generate its tables
from its own pin, and Broiler.VM's rule `I4` holds its pin equal to this one in Broiler.VM's repository. The
comparison here is the owner's half of the same fact, so that a difference is found in both repositories;
neither replaces the other, and a Unicode version moves in both pins by one dated decision, never in one.

## Part C - the contract (clause 2)

```rule
evidence class: library contract (not a measurement)
population: every public member of the owner's packages the engines consume, at the commit the bundle names, in the sub-areas JSD-0074 places here: properties and lexical classes, case folding, case mapping, normalization, properties of strings and segmentation
checks, each a test in this repository's test project or architecture-test project, each with a witness that was watched failing once:
  C-1 exact names: the lookup the engines call is the exact one of part A's A5; the loose lookup carries its documented warning and no engine-facing member reaches it
  C-2 immutable results: no public member returns a mutable reference to shared data - an array, a List or a writable span over a cache - shown by the API baseline and by a test that writes through every result type the API exposes and finds the next caller's answer unchanged
  C-3 a public version: every package that carries or reads Unicode data exposes UnicodeVersion, and it reads 17.0.0, the pin's version
  C-4 metering and limits: casing, normalization and segmentation take the caller's meter and, where the output can outgrow the input, a maximum result length; for each, a declared set of inputs charges exactly the units JSD-0074's decision 4 states (for casing and normalization, Broiler.VM's units of correspondence rows C13 and N7; for segmentation, one unit per code point each rule scans); a result longer than the maximum is reported as its own status, with nothing written past it, and a result of exactly the maximum is not; the lookups and folding take no meter, and refuse an input longer than the longest name their tables hold before reading it
  C-5 failure as data: an unknown property name is a false from the lookup, an over-long result its status, and no member throws for an input its contract admits, lone surrogates included
  C-6 no host data: no owner member calls CultureInfo, TextInfo, CharUnicodeInfo, char.Is* classification, Rune classification, string.Normalize, IsNormalized or StringInfo, shown by an architecture test over the packages' IL
MET if and only if every check passes and each witness was watched failing once.
NOT MET otherwise.
```

## Part D - the four owner conditions (clause 1)

```rule
evidence class: process record (not a measurement)
population: the four conditions this repository's roadmap adopts, at the end of SJL-7.2 and again at the commit the bundle names
checks:
  D-1 conventions - this repository's copy of the programme's conventions is filed with the same numbering; its rules of the SL shape are Active in an architecture-test project with a witness each, watched failing once; its consumed packages target net10.0, enable nullable, disable implicit usings and declare IsAotCompatible and IsTrimmable; and its Native AOT probe publishes with no warning and runs in CI
  D-2 the roadmap - docs/roadmap.md exists on the default branch, with this milestone's items and this record
  D-3 the assurance gate - every relevant unit of the consumed packages carries the Broiler Code Assurance annotation; the artefacts are generated by Broiler.Code's command-line tool and committed; CI fails when they drift, watched failing once on an injected drift; and HUMAN_REVIEW.md names the revision it reviews
  D-4 pinned and hashed inputs - part B is MET
MET if and only if every check holds at both readings.
NOT MET otherwise; and a condition that does not hold at the end of SJL-7.2 moves the sub-areas it reaches to the Broiler.JS repository by a dated record, as SJL-D-1 (b) and the roadmap's record say, whatever the later reading shows.
```

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory, quoting this one
whole and saying it was written afterwards; its decision is then weaker evidence. Before any code of
SJL-7.2 lands, a later bundle shows this file unchanged.

## What this file does not decide

- **The comparison with the engines' own implementations.** [`reference-arms.md`](reference-arms.md)
  decides it.
- **Either engine's cut-over, test262, vectors, fuel parity or measurement.** Each engine's own rule, in its
  own repository's bundle `sjl-7-001`, decides them; JSD-0031 §4(b)'s fifth reason is answered there.
- **Broiler.VM's rules `I1` to `I7`, N22 and N28.** Broiler.VM's rule decides them, rule `I4` included.
- **The packages' names, the provider's binding and the tables' format.** SJL-7.2 settles them in a dated
  record here, within JSD-0074's constraints.
- **A Unicode version.** The pin stays 17.0.0 unless a dated decision moves it in both repositories.
