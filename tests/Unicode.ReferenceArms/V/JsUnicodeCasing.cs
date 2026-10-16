// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript/JsUnicodeCasing.cs
//     blob 4f8f0739be6aa523fbad849df43ada2932a0b223: the whole file: JsUnicodeCasing (TryGetMapping, IsCased, IsCaseIgnorable, IsFinalSigma, NextCodePoint, PreviousCodePoint, InRanges; :44-192)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's: the
// namespace, the assurance header and annotations included, so that the generated half of each partial class
// compiles against it unchanged.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           0
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The Unicode Default Case Conversion that <c>toUpperCase</c> and <c>toLowerCase</c> perform,
/// over the tables <see cref="JsUnicodeCasing"/>'s generated half holds.
/// </summary>
/// <remarks>
/// <para>
/// <b>ECMA-262 names the algorithm and no language.</b> <c>toLowercase</c> and <c>toUppercase</c>
/// are the Unicode Default Case Conversion over code points: the full mappings, which
/// SpecialCasing.txt's unconditional lines lay over UnicodeData.txt's simple ones, and of its
/// conditions only <c>Final_Sigma</c>, the one that names no language. Until 2026-10-03 this
/// profile called the platform's invariant <c>TextInfo</c>, which maps one code unit to one code
/// unit: <c>'ß'.toUpperCase()</c> answered <c>"ß"</c>, no final sigma was written, and the answer
/// depended on the platform's Unicode version rather than on the pinned one (JSD-0027 slice N2).
/// </para>
/// <para>
/// <b>Final_Sigma is the caller's to test</b>, through <see cref="IsFinalSigma"/>, because it reads
/// the string around the code point: GREEK CAPITAL LETTER SIGMA lowers to the final form when a
/// cased letter precedes it, case-ignorable code points aside, and none follows it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B8462B
// Broiler-Human:        PENDING
internal static partial class JsUnicodeCasing
{
    /// <summary>GREEK CAPITAL LETTER SIGMA, the one code point a condition applies to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E0FEC6
    // Broiler-Human:        PENDING
    internal const int CapitalSigma = 0x03A3;

    /// <summary>GREEK SMALL LETTER FINAL SIGMA, what it lowers to under Final_Sigma.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C7D3FC
    // Broiler-Human:        PENDING
    internal const int FinalSigma = 0x03C2;

    /// <summary>
    /// The full upper-case mapping of <paramref name="codePoint"/>, or with <paramref name="upper"/>
    /// false its full lower-case mapping, when casing changes it.
    /// </summary>
    /// <remarks>False means the code point maps to itself. A mapping may be longer than one code point.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8F917B
    // Broiler-Human:        PENDING
    internal static bool TryGetMapping(int codePoint, bool upper, out JsUnicodeCodePoints mapping)
    {
        var index = upper ? UpperIndex : LowerIndex;
        var low = 0;
        var high = (index.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var key = JsUnicodeNormalization.ReadInt24(index, middle * 6);

            if (key == codePoint)
            {
                var offset = index[(middle * 6) + 3] | (index[(middle * 6) + 4] << 8);

                mapping = new JsUnicodeCodePoints(MappingPool.Slice(offset * 3, index[(middle * 6) + 5] * 3));
                return true;
            }

            if (key < codePoint)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        mapping = default;
        return false;
    }

    /// <summary>Whether <paramref name="codePoint"/> is <c>Cased</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8A031C
    // Broiler-Human:        PENDING
    internal static bool IsCased(int codePoint) => InRanges(CasedRanges, codePoint);

    /// <summary>Whether <paramref name="codePoint"/> is <c>Case_Ignorable</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0E404C
    // Broiler-Human:        PENDING
    internal static bool IsCaseIgnorable(int codePoint) => InRanges(CaseIgnorableRanges, codePoint);

    /// <summary>
    /// Whether the code point at <paramref name="at"/>, <paramref name="width"/> code units wide,
    /// meets Final_Sigma in <paramref name="text"/>, and how many code units the test read.
    /// </summary>
    /// <remarks>
    /// Unicode's definition, section 3.13: a cased code point precedes it, with only case-ignorable
    /// ones between, and no cased code point follows it with only case-ignorable ones between. A
    /// code point that is both cased and case-ignorable is skipped, as the definition's sequences
    /// read it. The count is returned so the caller can charge the scan, which a run of
    /// case-ignorable code points makes as long as the string.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DD41F5
    // Broiler-Human:        PENDING
    internal static bool IsFinalSigma(string text, int at, int width, out int scanned)
    {
        scanned = 0;
        var before = false;

        for (var position = at; position > 0;)
        {
            var codePoint = PreviousCodePoint(text, position, out var size);
            position -= size;
            scanned += size;

            if (IsCaseIgnorable(codePoint))
            {
                continue;
            }

            before = IsCased(codePoint);
            break;
        }

        if (!before)
        {
            return false;
        }

        for (var position = at + width; position < text.Length;)
        {
            var codePoint = NextCodePoint(text, position, out var size);
            position += size;
            scanned += size;

            if (IsCaseIgnorable(codePoint))
            {
                continue;
            }

            return !IsCased(codePoint);
        }

        return true;
    }

    /// <summary>
    /// The code point starting at <paramref name="start"/>, and its width; a lone surrogate is a
    /// code point of its own, as <c>StringToCodePoints</c> reads it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E011E4
    // Broiler-Human:        PENDING
    internal static int NextCodePoint(string text, int start, out int size)
    {
        if (start + 1 < text.Length && char.IsHighSurrogate(text[start]) && char.IsLowSurrogate(text[start + 1]))
        {
            size = 2;
            return char.ConvertToUtf32(text[start], text[start + 1]);
        }

        size = 1;
        return text[start];
    }

    /// <summary>The code point ending just before <paramref name="end"/>, and its width.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=03CA2D
    // Broiler-Human:        PENDING
    private static int PreviousCodePoint(string text, int end, out int size)
    {
        if (end >= 2 && char.IsLowSurrogate(text[end - 1]) && char.IsHighSurrogate(text[end - 2]))
        {
            size = 2;
            return char.ConvertToUtf32(text[end - 2], text[end - 1]);
        }

        size = 1;
        return text[end - 1];
    }

    /// <summary>Whether <paramref name="codePoint"/> lies in one of the sorted three-byte ranges.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E968BC
    // Broiler-Human:        PENDING
    private static bool InRanges(ReadOnlySpan<byte> ranges, int codePoint)
    {
        var low = 0;
        var high = (ranges.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < JsUnicodeNormalization.ReadInt24(ranges, middle * 6))
            {
                high = middle - 1;
            }
            else if (codePoint > JsUnicodeNormalization.ReadInt24(ranges, (middle * 6) + 3))
            {
                low = middle + 1;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
}
