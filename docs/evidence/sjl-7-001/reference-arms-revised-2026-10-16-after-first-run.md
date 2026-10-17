# Bundle SJL-7-001's reference-arm rule, revised 2026-10-16 after its first judged run was seen: charges inside row N6, the host's data, and the classifier's defects

**Written:** 2026-10-16, at milestone SJL-7.2, **after the first run of the comparison had been seen**. That run
started 2026-10-16T17:39:41Z, from the harness of this repository's `tests/Unicode.ReferenceArms` before it was
committed, over S at `378c2f0`'s library sources. This revision is written before the run that judges. **Its
decisions are therefore weaker evidence than the rule's**, which was written before any run.

**Owner:** the Broiler.Unicode maintainer. **Reviewer:** none (the programme's exception EX-30).

**What it revises, and what it does not.** The rule, [`reference-arms.md`](reference-arms.md), is quoted whole at
the end of this file, as it asks. It stands except where section 2 replaces its text, which section 2 quotes.
Nothing else changes:
- the arms, the population and the operations;
- the arbiter and the negative controls;
- what "listed" means.

Both runs are retained. The rule's own classifier, `scripts/reference-arms/classify_reference_arms.py` at its
version 1, is revised to version 2 with this file. Its docstring lists each change, and each one applies only
where version 1 put a line in class (c).

## 1. What the first run showed

The first run listed 16,022,956 differences, and version 1 of the classifier classified them:

| Class | Lines |
|---|---|
| (a) | 15,607,018 |
| (a') | 23,029 |
| (b) | 10,217 |
| (c) | 382,692 |

**S's answer was the arbiter's on every line.** Read one by one, the class (c) lines had six causes, and not one
is a defect of S:

| Cause | Lines | What it is |
|---|---|---|
| Charges inside row N6's correction | 377,592 | **Each is a composed form, NFC or NFKC, under a maximum.** The input's full decomposition is longer than the maximum, so arm V refused after its first charge, the input's length. S composed, found the result fitting or not, and charged row N7's units in full: the input's length, then the input's length plus three units per code point of the decomposition. **The rule admits row N6 for arm V and also holds S's charge exactly to arm V's.** On these inputs the two cannot both hold: arm V never builds what S must compose to know whether the result fits. Of the 377,592, 5 are at Broiler.VM's own ceiling of 2^24, and the rest are at the limits class U7 sets. **No charge differed anywhere else, and no casing charge differed at all** |
| Row C2, named but admitted for no class | 2,987 | Arm J reads Cased and Case_Ignorable from the runtime's categories, which do not know code points Unicode 16 and 17 add (U+1ACF, U+A7CE and others). A final sigma beside one of them is decided differently. The correspondence recorded row C2 as an agreement, with Broiler.JS's Case_Ignorable "an approximation", and the rule's class (a') does not list it |
| Rows C3 and C4 together with Final_Sigma | 1,781 | Arm J leaves a code point Unicode 17 adds unmapped (Beria Erfe's letters, U+A7CE in the Turkic path), and its Final_Sigma reads the same code point's Cased from the same data. In one line, Final_Sigma reads Cased as row C1 reads it. Version 1 tried each row alone |
| Row C6 in its other contexts | 259 | Arm J's `IsContextTransparent` passes over every mark not of class 230, and U+0903 is a mark of class 0. Row C6 names More_Above. The same reading ends After_Soft_Dotted in Lithuanian upper case (244 lines) and After_I in Turkic lower case (15) |
| Row N2, missed by version 1 | 68 | Version 1's composition, which reads older data, composed nothing with the first character when it was a starter. It also tried only all the code points normalization reads, or any one of them, as unknown to the host. 20 of the lines need two code points Unicode 16 adds to be unknown |
| Row N1 on U+FFFE | 5 | The runtime's normalizer refuses U+FFFE as it refuses a lone surrogate, in `normalize` and in the collator's decomposition |

## 2. What is revised

**The charges inside row N6.**
- **The rule read:**
  - in its comparison, "a charge of S that differs from arm V's is always class (c)";
  - in its MET line, "every charge of S for casing and normalization equals arm V's".
- **Now:**
  - Every such charge difference is class (a), row N6, when all of these hold:
    - it is a composed form under a maximum;
    - the input's full decomposition, in code units, is longer than that maximum, which the arbiter's
      decomposition decides;
    - arm V's charge is the input's length alone;
    - S's charge is row N7's units in full.
  - Every other charge difference is class (c), as before.
  - The MET line reads "every charge of S for casing and normalization equals arm V's, but for those of class
    (a), row N6".
- **Why S is not changed:**
  - Row N6 is a correction the rule admits: the limit is on the result.
  - Row N7 makes the charge before the work.
  - S therefore cannot charge only the input's length and then compose a decomposition longer than the maximum,
    because that work would go uncharged. Its unit's falsifier, "a charge differs from Broiler.VM row N7", is
    about N7's units, which S charges.

**The host's data.**
- **Rows C2, C3, C4 and N2 explain a difference by data the host does not know.** The classifier now bounds that
  data: a code point the pinned UCD assigns and Python's `unicodedata` leaves unassigned, at the Unicode version
  the run records for the host's ICU (`icuUnicodeVersion`, 15.1.0 on this host).
- **The classifier refuses a Python whose `unicodedata` is of another version.** This decides no answer, only
  which code points may stand for the host's older data.

**The admitted classes.**
- **Class (a') gains row C2**, a data policy under decision SJL-D-6 (c) like row C3. The rule's class (a') read
  "listed with the decision and its row, I2, C3, N2, N3 or G3", and now reads "I2, C2, C3, N2, N3 or G3".
- **Row C3's explanation**, in class (a'), and **row C4's**, in class (a) as the rule lists it, may now read
  Final_Sigma's Cased and Case_Ignorable from the same older data. They may also read Cased as row C1 reads it.
- **Row C6** covers the four contexts the same reading of a mark serves: After_I, Before_Dot, After_Soft_Dotted and
  More_Above.
- **Row N1** covers the runtime's refusal of U+FFFE.

**The record these rows rest on.** The rule says that a difference the correspondence does not name falsifies
JSD-0074, and that Broiler.VM's record is revised before either cut-over lands. Broiler.VM's
`docs/evidence/sjl-7-001/correspondence.md` is revised the same day for rows C2, C3, C6 and N1, each with a dated
note that quotes what it replaced. No row of Broiler.VM's own changes.

**The classifier's two defects in row N2**, with no change of rule: composition now starts as UAX #15's does, and
the candidate sets now include every set of the code points the host's data does not know.

**The classifier's memory**, with no change of rule. Version 1 held the whole listing and was stopped at 11.5 GB
on the first run. Its counts above were taken in line ranges and summed. Version 2 reads and writes one line at a
time.

## 3. What this revision does not change

- **No answer of S, and no charge of S.** The library's sources are `378c2f0`'s before and after this file.
- **No line version 1 classified otherwise than class (c).** Version 2 applied to the first run's whole listing
  leaves all 15,640,264 such lines in their verdict and row. It moves the 382,692 lines of class (c) into
  class (a), 378,341 lines, and class (a'), 4,351 lines.
- **The population, its generator and seeds, the arms and their copies.** Arm J's culture-dependent members still
  run a second time under tr-TR and lt-LT, and the negative controls still run once each.
- **The rule's revisions clause.** This file is the dated revision it asks for, and it quotes the rule whole
  below.

---

## The rule, as written at SJL-7.1 and quoted whole

> # Bundle SJL-7-001's reference-arm rule: the owner's Unicode algorithms against both engines' pre-cut-over implementations
>
> **Written:** 2026-10-16, at milestone SJL-7 (work package SJL-7.1) of the shared JavaScript libraries
> programme, before the owner's exact lookups and algorithms exist in this repository, and before either
> arm, the population's generator or its reference computation exists. **Milestone judged:** SJL-7,
> `Broiler.VM/docs/javascript-shared-libraries/milestones/sjl-7-unicode.md`: its component clauses 2 and 3
> (their reference-arm half) and work package SJL-7.2. **Owner:** the Broiler.Unicode maintainer, as the
> area's owner. **Reviewer:** none. Every role this rule names, both engines' owners whose engines supply the
> arms included, is held by one person (the programme records this as Broiler.VM's exception EX-30), and this
> rule does not claim that anyone independent has read it.
>
> **What this file is.** The predeclared decision of the library-level reference-arm comparison of the
> Unicode area, in template T4 of the programme's validation document,
> `Broiler.VM/docs/javascript-shared-libraries/validation.md` section 4, with the procedure of its section 3,
> in the shape Broiler.JS's bundles `sjl-4-001` to `sjl-6-001` took for the areas that live there. It
> declares the population: every class of input, and the seeds of the random parts. **It holds no figure**:
> the counts and seeds below are parameters of a population declared before any run, not results of one.
>
> **The members each arm copies** are the ones the area verdict's correspondence names, by file and line:
> Broiler.VM's `docs/evidence/sjl-7-001/correspondence.md`, read at Broiler.VM `16a35b2`, whose product
> sources are `24632b1`'s, at Broiler.JS `bdb1a3ca`, whose engine sources are `4d70821e`'s, and at this
> repository's `0faa1f9`, with its rows P, I, W, F, C, N, S and G.
>
> ```rule
> evidence class: conformance at library level (not a measurement)
> arms:
>   S  the owner's implementation, at the commit under test, called through the public API the engines will call: the exact property lookup and its code-point lookups, simple folding with its orbits and the canonicalisation without u, full case mapping in each language, the four normalization forms, the properties of strings with the ECMAScript name map, and the three segmentations with the word status; every table read through its provider from the tables part B of decision-rule.md holds
>   V  a test-only copy, in this repository's test project, of Broiler.VM's implementation at 24632b1, with the generated tables it reads copied unchanged: JsUnicodeProperties' lookup and JsUnicodeLexical; JsUnicodeCaseFolding; JsUnicodeCasing with JsRealm.String's LanguageCasing, AfterCapitalI, AfterSoftDotted, BeforeDot, MoreAbove, AppendCodePoint and the bodies of StringChangeCase and TransformCase but for the locale's resolution, with JsIntlTables.IsSoftDotted; JsUnicodeNormalization with JsRealm.String's IsNormalizedAlready, OrderCanonically, ComposeCanonically, WriteCodePoints and NormalizeText's body; JsUnicodeStringProperties; and JsSegmenter.cs's JsSegmentation with JsBreakData's decoding
>   J  a test-only copy, in the same project, of Broiler.JS's implementation at 4d70821e: CharExtensions.IsIdentifierStart and IsIdentifierPart with their tables and hand lists; the scanner's white-space test; JSRegExp's IsRegExpIdentifierStart and IsRegExpIdentifierPart; the translator's property resolution with NormalizeKey and its fall-through, over the package versions Broiler.JS pinned at that commit; JSStringSpecialCasing at those versions, with the CultureInfo resolution of toLocaleUpperCase and toLocaleLowerCase; the host's string.Normalize and IsNormalized as normalize and the collator call them; EmojiStringPropertyNames over the emoji package at that version; and JSIntl's ComputeGraphemes, ComputeWords, ComputeSentences and IsWordLike with their helpers
>   each copy records the source path and git blob of every file it copies, and each package version it calls; where a member takes an engine type, the copy takes the CLR value that type carries, and where it reads the current culture or a locale, the copy takes it as a parameter; each change is recorded beside it; arm V's charge delegate is kept, and every charge it makes is recorded; a member that throws is copied to throw a recorded exception instead; an arm whose copy changes a result or a charge is a defect of the arm, fixed in the arm, and the comparison is run again
> operations: membership of each code point in each binary property, General_Category value, Script and Script_Extensions value; the lookup of a name, by its result and its property; ID_Start and ID_Continue with Pattern_Syntax subtracted, White_Space, Soft_Dotted; simple folding, orbits and the canonicalisation without u; full upper and lower case mapping in the default, Turkic and Lithuanian languages; NFC, NFD, NFKC and NFKD; membership of a string in each of the seven properties of strings; grapheme, word and sentence boundaries, and each word segment's status. An arm is called only for the operations it implements, and the bundle names each operation an arm does not implement. Argument conversion, receiver checks, locale canonicalisation and error construction stay in the engines' adapters and are not compared, except that a refusal is compared by its kind
> population:
>   U1  code points: every code point from U+0000 to U+10FFFF, the surrogate code points included, in every code-point operation
>   U2  names: every property name, value and alias of ECMA-262's three tables and of PropertyAliases.txt and PropertyValueAliases.txt, alone and as Name=Value for General_Category, Script and Script_Extensions; and the refused set: each of those names with its letter case changed, with an underscore removed, with a space or a hyphen inserted, with an Is prefix, and as the value of the wrong property; the names of UCD properties outside the tables, among them Age, Block, Name, Bidi_Class and Line_Break; and 2^14 strings drawn with seed 0x534A4C372D303031 from the names' alphabet, each of one to forty characters, a draw being a SplitMix64 output read as stated in the generator's README
>   U3  casing: every code point alone; and, for every code point c that is Cased, Case_Ignorable or Soft_Dotted, or is I, J, Į, Σ, İ or ı, the contexts "AcΣ", "AΣc", "cΣ", "Σc", "ċ", "ć̇", "cः̇", "Ic", "Jc̀", "cͅ" and "cA", in each language; the correspondence's run cells for rows C1 to C11 and their Unicode 16 and 17 code points; lone high and low surrogates before, after and inside each context; and 2^16 strings drawn with seed 0x534A4C372D303032 of one to twelve code points from a weighted alphabet of those code points, combining marks of classes 0, 220, 230 and others, and ASCII
>   U4  normalization: every code point alone in each form; every NormalizationTest.txt column in each form; the correspondence's run cells for rows N1 to N7; and 2^16 strings drawn with seed 0x534A4C372D303033 of one to twelve code points from starters, combining marks of every class, Hangul L, V and T jamo and LV and LVT syllables, composition exclusions, singletons, non-starter decompositions, supplementary-plane mappings and lone surrogates
>   U5  properties of strings: every sequence of emoji-sequences.txt and emoji-zwj-sequences.txt in each of the seven properties; each with one code point removed, one added at either end, and one variation selector changed; every single code point of U1 in each property; and the correspondence's run cells for rows S1 to S3
>   U6  segmentation: every line of the three break test files at its own granularity and at the other two; the correspondence's run cells for rows G1 to G7; and 2^16 strings drawn with seed 0x534A4C372D303034 of one to sixteen code points, each code point drawn from a uniform choice of the Grapheme_Cluster_Break, Word_Break and Sentence_Break values and then from that value's code points, with spaces, punctuation, digits, ideographs, Katakana and regional indicators weighted as their test files weigh them
>   U7  limits and charges: for casing and normalization, each input of U3 and U4 run with no limit and with a maximum result length of the result's length, one less and one more, and a declared set of inputs whose result is longer than their input, shorter, and of 2^24 code units and one more
>   every operation runs over every class whose inputs it takes
> configuration: this repository's test project, built in Release and run under JIT only, never in the Native AOT probe; the process's current culture set to the invariant culture; arm J's host operations run on the host's ICU, whose version is recorded with the Unicode version it carries, and arm J's culture-dependent members run a second time under tr-TR and lt-LT; the population generated by a generator SJL-7.2 builds, at the commit the bundle names, its SplitMix64 implementation tested against the generator's published reference outputs
> comparison: per operation, input and arm, S's result against the arm's result, exactly: a set by its members, a string by its code points, a boundary list by its positions, a status by its value, and a refusal by its kind; and, for casing and normalization, the total charge S makes through its meter, with the language pass's further input length this family's adapter adds (JSD-0074, decision 4), against the total charge arm V makes, exactly. Segmentation's charge is compared with no arm: its units change by that decision, and part C of decision-rule.md holds them
> arbiter: which side of a difference is right is decided by an independent reference computation, written in Python from the standards' text - the UCD's data files, UAX #15, UAX #29, the Unicode Standard's section 3.13 and ECMA-262's tables and their algorithms - reading the same pinned inputs and no generated table, never by the agreement of the arms. A charge has no arbiter but arm V: Broiler.VM's units are the specification of the charge (JSD-0074, decision 4)
> admitted difference classes:
>   (a) correction - an arm is wrong and S is right by the arbiter; listed with the engine it corrects (J, V or both), the input and the clause or the reference value that shows it, and its row of the correspondence: for J, P1 to P4, I1, I3, W1, C1, C4 to C8, N1, G1 and G2; for V, C12 and N6, both limits checked on the whole result
>   (a') correction by data decision - arm J answers from the host's data or the runtime's and S from the pinned data, under decision SJL-D-6 (c) as JSD-0074 applies it; listed with the decision and its row, I2, C3, N2, N3 or G3
>   (b) implementation-defined - the specification admits both results: a word segment's status (row G4), where S answers its own word status; listed, and moved into a vector at a named corpus revision
> every other difference is class (c), a defect of S, which is fixed before either cut-over lands, after which the whole comparison is run again and both runs are retained; a charge of S that differs from arm V's is always class (c); a difference with arm V on any answer but C12's and N6's limits is class (c), because the correspondence found no answer of Broiler.VM's to correct; and a difference on rows F2 and W5, which are Broiler.JS's .NET translator's, is not in this population, which copies no part of the translator but its property resolution
> negative control: a copy of arm V with one charge deliberately changed for one input of U4, and a copy of arm J with one result deliberately changed for one input of U1, are each run once; the comparison reports each as a difference and does not classify it; the copies are then removed
> MET if and only if every operation ran over its whole population in S and in every arm that implements it, every difference is in class (a), (a') or (b) and is listed, every class (b) difference is in a vector at a named corpus revision, no class (c) difference remains, every charge of S for casing and normalization equals arm V's, and both negative controls were reported unclassified.
> NOT MET otherwise.
> ```
>
> **How it is read.** As Broiler.JS's bundle `sjl-6-001`'s rule is read: the comparison writes
> `summary.json` and `differences.tsv.gz`, and **classifies nothing**. The classification is the owner's,
> written as a script before the judged run and applied mechanically, into `classification.tsv.gz`,
> `classification.json` and `report.md`. **"Listed"**, in the MET line, is read as present in
> `classification.tsv.gz` with its class. **A difference the correspondence does not name falsifies
> JSD-0074**, whatever class the arbiter puts it in: the bundle names it, and Broiler.VM's record is revised
> before either cut-over lands.
>
> **What the bundle retains.** `summary.json`, `classification.json`, `report.md`, the population's
> `manifest.json`, the negative controls' runs and their arms' sources, the host's ICU version, and the
> commands of every step. The two listings are **not committed**: the bundle records the SHA-256 of each one's
> uncompressed text and its line count, and each is regenerated byte for byte from the commit the bundle names
> and the population, itself regenerated by the generator and checked against the manifest's digests. A run
> that stopped before the end of any operation's population is void and is not judged.
>
> **Revisions.** A rule revised after any run it judges has been seen is a new dated file in this directory,
> quoting this one whole and saying it was written afterwards; its decision is then weaker evidence. Before
> SJL-7.2's code lands, a later bundle shows this file unchanged. The arms are retired at SJL-16, or kept as a
> frozen regression population by decision.
>
> ## What it does not decide
>
> - **Unicode's own test files and generation byte for byte.** [`decision-rule.md`](decision-rule.md) decides
>   them; this comparison reads the same tables and does not replace them.
> - **Either engine's cut-over.** Engine-level vectors, test262, Broiler.VM's fuel parity and each engine's
>   measurement are judged by each engine's own rule; this population meets no test262 clause.
> - **Which engine is better.** A class (a) difference names the engine it corrects; the arms are compared
>   with S and with the reference, never with each other.
> - **The drivers.** Argument conversion, locale canonicalisation and the mapping of a locale to a language,
>   each engine's ceiling and errors, `Intl.Segmenter`'s objects and this family's realm-without-Intl policy
>   for `toLocale*Case` (row C10) stay in the engines and are compared only through test262 and the
>   engine-level vectors.
> - **The regular-expression engines.** Folding under `/i` without `u` in Broiler.JS's translator (F2) and its
>   line terminators (W5) are SJL-11's.
> - **Performance.** Nothing here is timed.
