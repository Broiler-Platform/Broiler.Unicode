// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           48
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Unicode.Algorithms;

/// <summary>
/// The tables the algorithms of this package read, which the caller supplies: one span of bytes per
/// table, generated from the pinned Unicode Character Database by <c>Broiler.Unicode.Generation</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Decision SJL-D-6 (c) of the shared JavaScript libraries programme</b>, as Broiler.VM's JSD-0074
/// applies it: the owner holds the generator and the pins, each engine embeds the tables that generator
/// writes, in assemblies of its own, and hands them to the owner's algorithms through this interface.
/// No package an engine references carries a table. The shape is Broiler.VM's <c>IJsIntlData</c>: a
/// Unicode version, and one byte span per table.
/// </para>
/// <para>
/// <b>An implementation answers the same bytes for the life of the process</b>, and answers an empty
/// span for a table it does not carry. Each algorithm's constructor names the tables it needs and
/// refuses a provider that lacks one, or whose <see cref="UnicodeVersion"/> is not
/// <see cref="UnicodeAlgorithms.UnicodeVersion"/>. The layout of each table is stated on its member of
/// <see cref="UnicodeTable"/>; every multi-byte number in a table is little-endian.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Low; Resources=0; Fingerprint=008E90
// Broiler-Human:        PENDING
public interface IUnicodeTables
{
    /// <summary>The Unicode version the tables were generated from, e.g. <c>17.0.0</c>.</summary>
    string UnicodeVersion { get; }

    /// <summary>One table's bytes; empty for a table this provider does not carry.</summary>
    /// <param name="table">The table.</param>
    // Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Medium; Resources=1; Fingerprint=17A14C
    // Broiler-Human:        PENDING
    ReadOnlySpan<byte> Table(UnicodeTable table);
}

/// <summary>
/// The tables an <see cref="IUnicodeTables"/> carries, in groups: the properties, case folding, the
/// properties of strings, normalization, case mapping and segmentation.
/// </summary>
/// <remarks>
/// A "range" is two three-byte code points, first and last, inclusive; a range list ascends and its
/// ranges neither overlap nor touch. A "layout" table is a list of three-byte numbers whose order its
/// member states. A name table holds entries in ordinal order, each a length byte, that many ASCII
/// characters and a two-byte value; its index table holds each entry's two-byte offset, in the same
/// order. The numbering is part of this package's contract: a member is never renumbered.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Low; Resources=0; Fingerprint=DFC9C5
// Broiler-Human:        PENDING
public enum UnicodeTable
{
    /// <summary>Nothing.</summary>
    None = 0,

    /// <summary>
    /// The properties' layout: the number of General_Category sets (the single values, then the
    /// groups), the number of single values among them, the first binary property set, the first
    /// Script set, the first Script_Extensions set, the number of Script values, and the set ids of
    /// ID_Start, ID_Continue, General_Category=Space_Separator, White_Space and Soft_Dotted.
    /// </summary>
    PropertyLayout = 1,

    /// <summary>Every set's ranges, set after set.</summary>
    PropertyRanges = 2,

    /// <summary>Per set id: the index of its first range in <see cref="PropertyRanges"/> and its range count, three bytes each.</summary>
    PropertySets = 3,

    /// <summary>
    /// The name table of the lone <c>\p{name}</c> form: every General_Category value, group and alias,
    /// and every binary property and alias, ECMA-262's tables admit, with its set id.
    /// </summary>
    PropertyLoneNames = 4,

    /// <summary>The index of <see cref="PropertyLoneNames"/>.</summary>
    PropertyLoneNameIndex = 5,

    /// <summary>The name table of the <c>\p{name=value}</c> form's names: 0 for General_Category, 1 for Script, 2 for Script_Extensions.</summary>
    PropertyNames = 6,

    /// <summary>The index of <see cref="PropertyNames"/>.</summary>
    PropertyNameIndex = 7,

    /// <summary>The name table of every Script value and alias <c>PropertyValueAliases.txt</c> lists, with its script id.</summary>
    ScriptNames = 8,

    /// <summary>The index of <see cref="ScriptNames"/>.</summary>
    ScriptNameIndex = 9,

    /// <summary>Case folding's layout: the most code points one simple folding is shared by, and the most code units one non-<c>u</c> canonical form is.</summary>
    FoldingLayout = 10,

    /// <summary>Every code point with a status C or S folding in <c>CaseFolding.txt</c>, and its folding, three bytes each, in code point order.</summary>
    FoldData = 11,

    /// <summary>The <see cref="FoldData"/> entries ordered by folding and then by code point, as two-byte entry numbers.</summary>
    OrbitIndex = 12,

    /// <summary>Every code unit ECMA-262's Canonicalize without <c>u</c> or <c>v</c> moves, and its canonical form, two bytes each, in code unit order.</summary>
    UpperData = 13,

    /// <summary>The <see cref="UpperData"/> entries ordered by canonical form and then by code unit, as two-byte entry numbers.</summary>
    UpperOrbitIndex = 14,

    /// <summary>The properties of strings' layout: the property count, the id of <c>RGI_Emoji</c> (their union), the sequence count, and the sequence dictionary's short limit.</summary>
    StringPropertyLayout = 15,

    /// <summary>The single code points of each property of strings, as ranges.</summary>
    StringPropertyRanges = 16,

    /// <summary>Every code point a sequence uses, most frequent first, three bytes each.</summary>
    SequenceDictionary = 17,

    /// <summary>
    /// Each property's sequences, in code point order: a length byte, then that many dictionary
    /// indices, each one byte when below the short limit and otherwise two.
    /// </summary>
    SequenceData = 18,

    /// <summary>Per property of strings: its first range, its range count, its first byte in <see cref="SequenceData"/> and its sequence count, three bytes each.</summary>
    StringPropertyData = 19,

    /// <summary>The name table of the properties of strings, with each property's id.</summary>
    StringPropertyNames = 20,

    /// <summary>The index of <see cref="StringPropertyNames"/>.</summary>
    StringPropertyNameIndex = 21,

    /// <summary>Normalization's layout: the longest full decomposition, in code points.</summary>
    NormalizationLayout = 22,

    /// <summary>Every run of one non-zero Canonical_Combining_Class: a range, then the class in one byte.</summary>
    CombiningClassData = 23,

    /// <summary>Every code point with a canonical decomposition, Hangul excluded: the code point in three bytes, its offset in <see cref="DecompositionPool"/> in two and its length in one.</summary>
    CanonicalIndex = 24,

    /// <summary>Every code point whose full compatibility decomposition differs from its full canonical one, in <see cref="CanonicalIndex"/>'s format.</summary>
    CompatibilityIndex = 25,

    /// <summary>The code points of every full decomposition, three bytes each.</summary>
    DecompositionPool = 26,

    /// <summary>Every primary composite, Full_Composition_Exclusion and Hangul excluded: first, second and composite code point, three bytes each, ordered by first and then second.</summary>
    CompositionData = 27,

    /// <summary>The ranges of NFC_QC=No, NFC_QC=Maybe, NFKC_QC=No and NFKC_QC=Maybe, set after set.</summary>
    QuickCheckData = 28,

    /// <summary>For each of those four sets: its first range and its range count, two bytes each.</summary>
    QuickCheckIndex = 29,

    /// <summary>Case mapping's layout: the longest full case mapping, in code points.</summary>
    CasingLayout = 30,

    /// <summary>Every code point upper-casing changes: the code point in three bytes, its offset in <see cref="MappingPool"/> in two and its length in one.</summary>
    UpperIndex = 31,

    /// <summary>Every code point lower-casing changes, in <see cref="UpperIndex"/>'s format; U+03A3 maps to U+03C3, and Final_Sigma is the algorithm's.</summary>
    LowerIndex = 32,

    /// <summary>The code points of every full case mapping, three bytes each.</summary>
    MappingPool = 33,

    /// <summary>The ranges of Cased.</summary>
    CasedRanges = 34,

    /// <summary>The ranges of Case_Ignorable.</summary>
    CaseIgnorableRanges = 35,

    /// <summary>The ranges of Soft_Dotted, which the Lithuanian mappings read.</summary>
    SoftDottedRanges = 36,

    /// <summary>
    /// UAX #29's break properties over the code space: a byte giving the number of distinct
    /// combinations; each combination as six codes - Grapheme_Cluster_Break, Word_Break,
    /// Sentence_Break, Extended_Pictographic, Indic_Conjunct_Break, and Ideographic or Hiragana -
    /// numbered as <c>Broiler.Unicode.Segmentation</c>'s enumerations are; a four-byte run count; and
    /// each run as its first code point in three bytes and its combination's index in one. A run lasts
    /// to the next run's first code point.
    /// </summary>
    SegmentBreaks = 37,

    /// <summary>
    /// Normalization's per-code-point trie (<see cref="UnicodeTable.NormalizationRecords"/> by number):
    /// 17,408 two-byte block numbers, one per 64 code points, then the blocks, each 64 two-byte values.
    /// A code point's value is the two bytes at 34,816 + (block × 64 + the code point's low six bits) × 2.
    /// Every trie table has this layout.
    /// </summary>
    NormalizationTrie = 38,

    /// <summary>
    /// The records <see cref="NormalizationTrie"/> numbers, thirteen bytes each; record 0 is every code
    /// point's default and all zero. Each holds, in order: the Canonical_Combining_Class; a byte of flags,
    /// NFC_QC in its low two bits and NFKC_QC in the next two (0 Yes, 1 Maybe, 2 No), 16 when the first
    /// code point of the canonical decomposition is NFC_QC=Yes and 32 when the first of the decomposition
    /// NFKC uses is NFKC_QC=Yes; the full canonical decomposition's first element in
    /// <see cref="DecompositionElements"/> in two bytes, its length in code points in one and in UTF-16
    /// code units in one (0 and 0 when there is none); the full compatibility decomposition's likewise,
    /// when it differs from the canonical one; and the code point's first entry in
    /// <see cref="CompositionLists"/> in two bytes and its entry count in one. Hangul syllables are the
    /// algorithm's and have no decomposition here.
    /// </summary>
    NormalizationRecords = 39,

    /// <summary>
    /// Every primary composite by its first code point, Full_Composition_Exclusion and Hangul excluded:
    /// the second code point and the composite, three bytes each, grouped by first and ordered by second.
    /// </summary>
    CompositionLists = 40,

    /// <summary>Case mapping's per-code-point trie (<see cref="UnicodeTable.CasingRecords"/> by number), in <see cref="NormalizationTrie"/>'s layout.</summary>
    CasingTrie = 41,

    /// <summary>
    /// The records <see cref="CasingTrie"/> numbers, nine bytes each; record 0 is all zero. Each holds a
    /// byte of flags (1 Cased, 2 Case_Ignorable, 4 Soft_Dotted); the full upper-case mapping's length in
    /// code points in one byte (0 when upper-casing leaves the code point) and, in three, the code point
    /// it maps to when the length is 1 or its offset in <see cref="MappingPool"/> when it is longer; and
    /// the full lower-case mapping likewise, U+03A3 mapping to U+03C3.
    /// </summary>
    CasingRecords = 42,

    /// <summary>
    /// The ECMAScript lexical classes per code point, in <see cref="NormalizationTrie"/>'s layout: each
    /// value's bit 1 is ID_Start, bit 2 ID_Continue and bit 4 General_Category=Space_Separator.
    /// </summary>
    LexicalTrie = 43,

    /// <summary>
    /// The code points of every full decomposition as four-byte elements: the code point in the low three
    /// bytes and its Canonical_Combining_Class in the high one, little-endian.
    /// </summary>
    DecompositionElements = 44,

    /// <summary>
    /// The full upper-case mapping per code point, in <see cref="NormalizationTrie"/>'s layout: 0 when
    /// upper-casing leaves the code point; 0x8000 when the mapping is longer than one code point or its
    /// difference does not fit, so <see cref="CasingRecords"/> holds it; and otherwise the difference of
    /// the code point it maps to, as a fifteen-bit two's-complement number.
    /// </summary>
    UpperDeltaTrie = 45,

    /// <summary>The full lower-case mapping per code point, in <see cref="UpperDeltaTrie"/>'s encoding.</summary>
    LowerDeltaTrie = 46,
}
