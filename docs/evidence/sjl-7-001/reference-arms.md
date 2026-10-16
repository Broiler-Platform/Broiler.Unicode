# Bundle SJL-7-001's reference-arm rule: the owner's Unicode algorithms against both engines' pre-cut-over implementations

**Written:** 2026-10-16, at milestone SJL-7 (work package SJL-7.1) of the shared JavaScript libraries
programme, before the owner's exact lookups and algorithms exist in this repository, and before either
arm, the population's generator or its reference computation exists. **Milestone judged:** SJL-7,
`Broiler.VM/docs/javascript-shared-libraries/milestones/sjl-7-unicode.md`: its component clauses 2 and 3
(their reference-arm half) and work package SJL-7.2. **Owner:** the Broiler.Unicode maintainer, as the
area's owner. **Reviewer:** none. Every role this rule names, both engines' owners whose engines supply the
arms included, is held by one person (the programme records this as Broiler.VM's exception EX-30), and this
rule does not claim that anyone independent has read it.

**What this file is.** The predeclared decision of the library-level reference-arm comparison of the
Unicode area, in template T4 of the programme's validation document,
`Broiler.VM/docs/javascript-shared-libraries/validation.md` section 4, with the procedure of its section 3,
in the shape Broiler.JS's bundles `sjl-4-001` to `sjl-6-001` took for the areas that live there. It
declares the population: every class of input, and the seeds of the random parts. **It holds no figure**:
the counts and seeds below are parameters of a population declared before any run, not results of one.

**The members each arm copies** are the ones the area verdict's correspondence names, by file and line:
Broiler.VM's `docs/evidence/sjl-7-001/correspondence.md`, read at Broiler.VM `16a35b2`, whose product
sources are `24632b1`'s, at Broiler.JS `bdb1a3ca`, whose engine sources are `4d70821e`'s, and at this
repository's `0faa1f9`, with its rows P, I, W, F, C, N, S and G.

```rule
evidence class: conformance at library level (not a measurement)
arms:
  S  the owner's implementation, at the commit under test, called through the public API the engines will call: the exact property lookup and its code-point lookups, simple folding with its orbits and the canonicalisation without u, full case mapping in each language, the four normalization forms, the properties of strings with the ECMAScript name map, and the three segmentations with the word status; every table read through its provider from the tables part B of decision-rule.md holds
  V  a test-only copy, in this repository's test project, of Broiler.VM's implementation at 24632b1, with the generated tables it reads copied unchanged: JsUnicodeProperties' lookup and JsUnicodeLexical; JsUnicodeCaseFolding; JsUnicodeCasing with JsRealm.String's LanguageCasing, AfterCapitalI, AfterSoftDotted, BeforeDot, MoreAbove, AppendCodePoint and the bodies of StringChangeCase and TransformCase but for the locale's resolution, with JsIntlTables.IsSoftDotted; JsUnicodeNormalization with JsRealm.String's IsNormalizedAlready, OrderCanonically, ComposeCanonically, WriteCodePoints and NormalizeText's body; JsUnicodeStringProperties; and JsSegmenter.cs's JsSegmentation with JsBreakData's decoding
  J  a test-only copy, in the same project, of Broiler.JS's implementation at 4d70821e: CharExtensions.IsIdentifierStart and IsIdentifierPart with their tables and hand lists; the scanner's white-space test; JSRegExp's IsRegExpIdentifierStart and IsRegExpIdentifierPart; the translator's property resolution with NormalizeKey and its fall-through, over the package versions Broiler.JS pinned at that commit; JSStringSpecialCasing at those versions, with the CultureInfo resolution of toLocaleUpperCase and toLocaleLowerCase; the host's string.Normalize and IsNormalized as normalize and the collator call them; EmojiStringPropertyNames over the emoji package at that version; and JSIntl's ComputeGraphemes, ComputeWords, ComputeSentences and IsWordLike with their helpers
  each copy records the source path and git blob of every file it copies, and each package version it calls; where a member takes an engine type, the copy takes the CLR value that type carries, and where it reads the current culture or a locale, the copy takes it as a parameter; each change is recorded beside it; arm V's charge delegate is kept, and every charge it makes is recorded; a member that throws is copied to throw a recorded exception instead; an arm whose copy changes a result or a charge is a defect of the arm, fixed in the arm, and the comparison is run again
operations: membership of each code point in each binary property, General_Category value, Script and Script_Extensions value; the lookup of a name, by its result and its property; ID_Start and ID_Continue with Pattern_Syntax subtracted, White_Space, Soft_Dotted; simple folding, orbits and the canonicalisation without u; full upper and lower case mapping in the default, Turkic and Lithuanian languages; NFC, NFD, NFKC and NFKD; membership of a string in each of the seven properties of strings; grapheme, word and sentence boundaries, and each word segment's status. An arm is called only for the operations it implements, and the bundle names each operation an arm does not implement. Argument conversion, receiver checks, locale canonicalisation and error construction stay in the engines' adapters and are not compared, except that a refusal is compared by its kind
population:
  U1  code points: every code point from U+0000 to U+10FFFF, the surrogate code points included, in every code-point operation
  U2  names: every property name, value and alias of ECMA-262's three tables and of PropertyAliases.txt and PropertyValueAliases.txt, alone and as Name=Value for General_Category, Script and Script_Extensions; and the refused set: each of those names with its letter case changed, with an underscore removed, with a space or a hyphen inserted, with an Is prefix, and as the value of the wrong property; the names of UCD properties outside the tables, among them Age, Block, Name, Bidi_Class and Line_Break; and 2^14 strings drawn with seed 0x534A4C372D303031 from the names' alphabet, each of one to forty characters, a draw being a SplitMix64 output read as stated in the generator's README
  U3  casing: every code point alone; and, for every code point c that is Cased, Case_Ignorable or Soft_Dotted, or is I, J, Į, Σ, İ or ı, the contexts "AcΣ", "AΣc", "cΣ", "Σc", "ċ", "ć̇", "cः̇", "Ic", "Jc̀", "cͅ" and "cA", in each language; the correspondence's run cells for rows C1 to C11 and their Unicode 16 and 17 code points; lone high and low surrogates before, after and inside each context; and 2^16 strings drawn with seed 0x534A4C372D303032 of one to twelve code points from a weighted alphabet of those code points, combining marks of classes 0, 220, 230 and others, and ASCII
  U4  normalization: every code point alone in each form; every NormalizationTest.txt column in each form; the correspondence's run cells for rows N1 to N7; and 2^16 strings drawn with seed 0x534A4C372D303033 of one to twelve code points from starters, combining marks of every class, Hangul L, V and T jamo and LV and LVT syllables, composition exclusions, singletons, non-starter decompositions, supplementary-plane mappings and lone surrogates
  U5  properties of strings: every sequence of emoji-sequences.txt and emoji-zwj-sequences.txt in each of the seven properties; each with one code point removed, one added at either end, and one variation selector changed; every single code point of U1 in each property; and the correspondence's run cells for rows S1 to S3
  U6  segmentation: every line of the three break test files at its own granularity and at the other two; the correspondence's run cells for rows G1 to G7; and 2^16 strings drawn with seed 0x534A4C372D303034 of one to sixteen code points, each code point drawn from a uniform choice of the Grapheme_Cluster_Break, Word_Break and Sentence_Break values and then from that value's code points, with spaces, punctuation, digits, ideographs, Katakana and regional indicators weighted as their test files weigh them
  U7  limits and charges: for casing and normalization, each input of U3 and U4 run with no limit and with a maximum result length of the result's length, one less and one more, and a declared set of inputs whose result is longer than their input, shorter, and of 2^24 code units and one more
  every operation runs over every class whose inputs it takes
configuration: this repository's test project, built in Release and run under JIT only, never in the Native AOT probe; the process's current culture set to the invariant culture; arm J's host operations run on the host's ICU, whose version is recorded with the Unicode version it carries, and arm J's culture-dependent members run a second time under tr-TR and lt-LT; the population generated by a generator SJL-7.2 builds, at the commit the bundle names, its SplitMix64 implementation tested against the generator's published reference outputs
comparison: per operation, input and arm, S's result against the arm's result, exactly: a set by its members, a string by its code points, a boundary list by its positions, a status by its value, and a refusal by its kind; and, for casing and normalization, the total charge S makes through its meter, with the language pass's further input length this family's adapter adds (JSD-0074, decision 4), against the total charge arm V makes, exactly. Segmentation's charge is compared with no arm: its units change by that decision, and part C of decision-rule.md holds them
arbiter: which side of a difference is right is decided by an independent reference computation, written in Python from the standards' text - the UCD's data files, UAX #15, UAX #29, the Unicode Standard's section 3.13 and ECMA-262's tables and their algorithms - reading the same pinned inputs and no generated table, never by the agreement of the arms. A charge has no arbiter but arm V: Broiler.VM's units are the specification of the charge (JSD-0074, decision 4)
admitted difference classes:
  (a) correction - an arm is wrong and S is right by the arbiter; listed with the engine it corrects (J, V or both), the input and the clause or the reference value that shows it, and its row of the correspondence: for J, P1 to P4, I1, I3, W1, C1, C4 to C8, N1, G1 and G2; for V, C12 and N6, both limits checked on the whole result
  (a') correction by data decision - arm J answers from the host's data or the runtime's and S from the pinned data, under decision SJL-D-6 (c) as JSD-0074 applies it; listed with the decision and its row, I2, C3, N2, N3 or G3
  (b) implementation-defined - the specification admits both results: a word segment's status (row G4), where S answers its own word status; listed, and moved into a vector at a named corpus revision
every other difference is class (c), a defect of S, which is fixed before either cut-over lands, after which the whole comparison is run again and both runs are retained; a charge of S that differs from arm V's is always class (c); a difference with arm V on any answer but C12's and N6's limits is class (c), because the correspondence found no answer of Broiler.VM's to correct; and a difference on rows F2 and W5, which are Broiler.JS's .NET translator's, is not in this population, which copies no part of the translator but its property resolution
negative control: a copy of arm V with one charge deliberately changed for one input of U4, and a copy of arm J with one result deliberately changed for one input of U1, are each run once; the comparison reports each as a difference and does not classify it; the copies are then removed
MET if and only if every operation ran over its whole population in S and in every arm that implements it, every difference is in class (a), (a') or (b) and is listed, every class (b) difference is in a vector at a named corpus revision, no class (c) difference remains, every charge of S for casing and normalization equals arm V's, and both negative controls were reported unclassified.
NOT MET otherwise.
```

**How it is read.** As Broiler.JS's bundle `sjl-6-001`'s rule is read: the comparison writes
`summary.json` and `differences.tsv.gz`, and **classifies nothing**. The classification is the owner's,
written as a script before the judged run and applied mechanically, into `classification.tsv.gz`,
`classification.json` and `report.md`. **"Listed"**, in the MET line, is read as present in
`classification.tsv.gz` with its class. **A difference the correspondence does not name falsifies
JSD-0074**, whatever class the arbiter puts it in: the bundle names it, and Broiler.VM's record is revised
before either cut-over lands.

**What the bundle retains.** `summary.json`, `classification.json`, `report.md`, the population's
`manifest.json`, the negative controls' runs and their arms' sources, the host's ICU version, and the
commands of every step. The two listings are **not committed**: the bundle records the SHA-256 of each one's
uncompressed text and its line count, and each is regenerated byte for byte from the commit the bundle names
and the population, itself regenerated by the generator and checked against the manifest's digests. A run
that stopped before the end of any operation's population is void and is not judged.

**Revisions.** A rule revised after any run it judges has been seen is a new dated file in this directory,
quoting this one whole and saying it was written afterwards; its decision is then weaker evidence. Before
SJL-7.2's code lands, a later bundle shows this file unchanged. The arms are retired at SJL-16, or kept as a
frozen regression population by decision.

## What it does not decide

- **Unicode's own test files and generation byte for byte.** [`decision-rule.md`](decision-rule.md) decides
  them; this comparison reads the same tables and does not replace them.
- **Either engine's cut-over.** Engine-level vectors, test262, Broiler.VM's fuel parity and each engine's
  measurement are judged by each engine's own rule; this population meets no test262 clause.
- **Which engine is better.** A class (a) difference names the engine it corrects; the arms are compared
  with S and with the reference, never with each other.
- **The drivers.** Argument conversion, locale canonicalisation and the mapping of a locale to a language,
  each engine's ceiling and errors, `Intl.Segmenter`'s objects and this family's realm-without-Intl policy
  for `toLocale*Case` (row C10) stay in the engines and are compared only through test262 and the
  engine-level vectors.
- **The regular-expression engines.** Folding under `/i` without `u` in Broiler.JS's translator (F2) and its
  line terminators (W5) are SJL-11's.
- **Performance.** Nothing here is timed.
