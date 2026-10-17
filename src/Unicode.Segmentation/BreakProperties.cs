// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           52
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.Unicode.Segmentation;

/// <summary>The Grapheme_Cluster_Break values (UAX #29, table 2), numbered as the break table codes them.</summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #29 via Broiler.VM JsSegmentation; IP=Low; Security=Low; Resources=0; Fingerprint=C45A0B
// Broiler-Human:        PENDING
public enum GraphemeBreak : byte
{
    /// <summary>Any other code point.</summary>
    Other = 0,

    /// <summary>CR.</summary>
    CR,

    /// <summary>LF.</summary>
    LF,

    /// <summary>Control.</summary>
    Control,

    /// <summary>Extend.</summary>
    Extend,

    /// <summary>ZWJ.</summary>
    ZWJ,

    /// <summary>Regional_Indicator.</summary>
    Regional_Indicator,

    /// <summary>Prepend.</summary>
    Prepend,

    /// <summary>SpacingMark.</summary>
    SpacingMark,

    /// <summary>L, a leading Hangul jamo.</summary>
    L,

    /// <summary>V, a vowel Hangul jamo.</summary>
    V,

    /// <summary>T, a trailing Hangul jamo.</summary>
    T,

    /// <summary>LV, a two-jamo Hangul syllable.</summary>
    LV,

    /// <summary>LVT, a three-jamo Hangul syllable.</summary>
    LVT,
}

/// <summary>The Word_Break values (UAX #29, table 3), numbered as the break table codes them.</summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #29 via Broiler.VM JsSegmentation; IP=Low; Security=Low; Resources=0; Fingerprint=35E03C
// Broiler-Human:        PENDING
public enum WordBreak : byte
{
    /// <summary>Any other code point.</summary>
    Other = 0,

    /// <summary>CR.</summary>
    CR,

    /// <summary>LF.</summary>
    LF,

    /// <summary>Newline.</summary>
    Newline,

    /// <summary>Extend.</summary>
    Extend,

    /// <summary>ZWJ.</summary>
    ZWJ,

    /// <summary>Regional_Indicator.</summary>
    Regional_Indicator,

    /// <summary>Format.</summary>
    Format,

    /// <summary>Katakana.</summary>
    Katakana,

    /// <summary>Hebrew_Letter.</summary>
    Hebrew_Letter,

    /// <summary>ALetter.</summary>
    ALetter,

    /// <summary>Single_Quote.</summary>
    Single_Quote,

    /// <summary>Double_Quote.</summary>
    Double_Quote,

    /// <summary>MidNumLet.</summary>
    MidNumLet,

    /// <summary>MidLetter.</summary>
    MidLetter,

    /// <summary>MidNum.</summary>
    MidNum,

    /// <summary>Numeric.</summary>
    Numeric,

    /// <summary>ExtendNumLet.</summary>
    ExtendNumLet,

    /// <summary>WSegSpace.</summary>
    WSegSpace,
}

/// <summary>The Sentence_Break values (UAX #29, table 4), numbered as the break table codes them.</summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #29 via Broiler.VM JsSegmentation; IP=Low; Security=Low; Resources=0; Fingerprint=89DE12
// Broiler-Human:        PENDING
public enum SentenceBreak : byte
{
    /// <summary>Any other code point.</summary>
    Other = 0,

    /// <summary>CR.</summary>
    CR,

    /// <summary>LF.</summary>
    LF,

    /// <summary>Extend.</summary>
    Extend,

    /// <summary>Sep.</summary>
    Sep,

    /// <summary>Format.</summary>
    Format,

    /// <summary>Sp.</summary>
    Sp,

    /// <summary>Lower.</summary>
    Lower,

    /// <summary>Upper.</summary>
    Upper,

    /// <summary>OLetter.</summary>
    OLetter,

    /// <summary>Numeric.</summary>
    Numeric,

    /// <summary>ATerm.</summary>
    ATerm,

    /// <summary>SContinue.</summary>
    SContinue,

    /// <summary>STerm.</summary>
    STerm,

    /// <summary>Close.</summary>
    Close,
}

/// <summary>The Indic_Conjunct_Break values grapheme rule GB9c reads (<c>DerivedCoreProperties.txt</c>), numbered as the break table codes them.</summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #29 via Broiler.VM JsSegmentation; IP=Low; Security=Low; Resources=0; Fingerprint=764288
// Broiler-Human:        PENDING
public enum ConjunctBreak : byte
{
    /// <summary>None.</summary>
    None = 0,

    /// <summary>Linker.</summary>
    Linker,

    /// <summary>Consonant.</summary>
    Consonant,

    /// <summary>Extend.</summary>
    Extend,
}

/// <summary>
/// The break properties of one code point: its Grapheme_Cluster_Break, Word_Break, Sentence_Break,
/// Extended_Pictographic and Indic_Conjunct_Break, and whether it is Ideographic or of the Hiragana
/// script, which a word's status reads.
/// </summary>
/// <param name="Grapheme">Grapheme_Cluster_Break.</param>
/// <param name="Word">Word_Break.</param>
/// <param name="Sentence">Sentence_Break.</param>
/// <param name="ExtendedPictographic">Extended_Pictographic (<c>emoji-data.txt</c>).</param>
/// <param name="Conjunct">Indic_Conjunct_Break.</param>
/// <param name="IdeographicOrHiragana">Ideographic (<c>PropList.txt</c>) or Script=Hiragana.</param>
// Broiler-AI:           Origin=Ported; Spec=UAX #29 via Broiler.VM JsSegmentation; IP=Low; Security=Low; Resources=1; Fingerprint=05FEA2
// Broiler-Human:        PENDING
public readonly record struct BreakProperties(
    GraphemeBreak Grapheme,
    WordBreak Word,
    SentenceBreak Sentence,
    bool ExtendedPictographic,
    ConjunctBreak Conjunct,
    bool IdeographicOrHiragana);
