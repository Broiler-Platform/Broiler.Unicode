// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeLexical.cs
//     blob 9909f968c8a28896de9f52a8075ab9667ef90319: the whole file: JsUnicodeLexical (IsIdentifierStart, IsIdentifierPart, IsWhiteSpace; :47-79)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's: the
// namespace, the assurance header and annotations included, so that the generated half of each partial class
// compiles against it unchanged.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           0
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The three code point classes of the language's lexical grammar that need the Unicode character
/// database: <c>IdentifierStartChar</c>, <c>IdentifierPartChar</c> and <c>WhiteSpace</c>, from the
/// pinned Unicode 17.0.0 tables.
/// </summary>
/// <remarks>
/// <para>
/// <b>Public because the tokenizer is in another assembly.</b> The tables are internal to this
/// one, where the matcher's property escapes read them (decision JSD-0031 section 5); the
/// front end's tokenizer references this assembly and reaches them here, and the matcher's group
/// names read the same three answers, so an identifier and a group name cannot disagree about a
/// character. Nothing here allocates: each answer is a comparison or a binary search over a
/// generated range list.
/// </para>
/// <para>
/// <b>They are the language's sets, not the platform's.</b> <c>ID_Start</c> and
/// <c>ID_Continue</c> come from <c>DerivedCoreProperties.txt</c>, which the generator holds to
/// their UAX #31 derivation, so <c>Other_ID_Start</c>, <c>Other_ID_Continue</c> and the
/// <c>Pattern_Syntax</c> subtraction are in them, and a code point outside the Basic Multilingual
/// Plane is classified as the one code point it is. <c>WhiteSpace</c> is TAB, VT, FF, ZWNBSP and
/// <c>General_Category=Space_Separator</c>; <c>char.IsWhiteSpace</c> also answers yes for U+0085
/// and U+001C to U+001F, which the language does not, and for the line terminators, which it
/// classifies separately.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2C295F
// Broiler-Human:        PENDING
public static class JsUnicodeLexical
{
    /// <summary>
    /// Whether <paramref name="codePoint"/> is an <c>IdentifierStartChar</c>: <c>ID_Start</c>,
    /// <c>$</c> or <c>_</c>. A surrogate code point, paired or not, is not one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=21F4B8
    // Broiler-Human:        PENDING
    public static bool IsIdentifierStart(int codePoint) =>
        codePoint < 0x80
            ? (uint)((codePoint | 0x20) - 'a') <= 'z' - 'a' || codePoint is '$' or '_'
            : JsUnicodeProperties.Set(JsUnicodeProperties.IdStartSet).Contains(codePoint);

    /// <summary>
    /// Whether <paramref name="codePoint"/> is an <c>IdentifierPartChar</c>: <c>ID_Continue</c>,
    /// <c>$</c>, ZWNJ or ZWJ. <c>_</c> and the decimal digits are in <c>ID_Continue</c> itself.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=149FC8
    // Broiler-Human:        PENDING
    public static bool IsIdentifierPart(int codePoint) =>
        codePoint < 0x80
            ? (uint)((codePoint | 0x20) - 'a') <= 'z' - 'a' || (uint)(codePoint - '0') <= 9 || codePoint is '$' or '_'
            : codePoint is 0x200C or 0x200D || JsUnicodeProperties.Set(JsUnicodeProperties.IdContinueSet).Contains(codePoint);

    /// <summary>
    /// Whether <paramref name="codePoint"/> is <c>WhiteSpace</c>: U+0009, U+000B, U+000C, U+FEFF or
    /// a <c>Space_Separator</c> (U+0020 and U+00A0 among them). Line terminators are not.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=70499E
    // Broiler-Human:        PENDING
    public static bool IsWhiteSpace(int codePoint) =>
        codePoint is 0x09 or 0x0B or 0x0C or 0x20 or 0xA0 or 0xFEFF ||
        (codePoint > 0xFF && JsUnicodeProperties.Set(JsUnicodeProperties.SpaceSeparatorSet).Contains(codePoint));
}
