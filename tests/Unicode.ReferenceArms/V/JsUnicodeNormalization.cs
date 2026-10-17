// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript/JsUnicodeNormalization.cs
//     blob 077c5e11b27b7835640502b7755c49e0c0fda845: the whole file: JsNormalizationQuickCheck, JsUnicodeCodePoints and JsUnicodeNormalization (CombiningClass, TryGetDecomposition, TryCompose, QuickCheck, ReadInt24 and their helpers; :25-334)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's: the
// namespace, the assurance header and annotations included, so that the generated half of each partial class
// compiles against it unchanged.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           9
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>A normalization quick-check value, <c>NFC_QC</c> or <c>NFKC_QC</c> in Unicode's terms.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C8329C
// Broiler-Human:        PENDING
internal enum JsNormalizationQuickCheck
{
    /// <summary>The code point can stand in the normalized form wherever it occurs.</summary>
    Yes,

    /// <summary>Whether it can depends on what precedes it; only a full normalization answers.</summary>
    Maybe,

    /// <summary>It never occurs in the normalized form.</summary>
    No,
}

/// <summary>
/// The code points of one full decomposition, read from the generated pool or computed for a
/// Hangul syllable. A view: it allocates nothing.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1E94A5
// Broiler-Human:        PENDING
internal readonly ref struct JsUnicodeCodePoints
{
    /// <summary>The pool slice, three bytes per code point, or empty for a computed decomposition.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CD13E0
    // Broiler-Human:        PENDING
    private readonly ReadOnlySpan<byte> pool;

    /// <summary>A computed decomposition's first code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6B9075
    // Broiler-Human:        PENDING
    private readonly int first;

    /// <summary>A computed decomposition's second code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FC87FA
    // Broiler-Human:        PENDING
    private readonly int second;

    /// <summary>A computed decomposition's third code point, when it has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=67C584
    // Broiler-Human:        PENDING
    private readonly int third;

    /// <summary>A decomposition read from the pool.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AD420D
    // Broiler-Human:        PENDING
    internal JsUnicodeCodePoints(ReadOnlySpan<byte> pool)
    {
        this.pool = pool;
        Length = pool.Length / 3;
    }

    /// <summary>A computed decomposition of two or three code points.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4FDACF
    // Broiler-Human:        PENDING
    internal JsUnicodeCodePoints(int first, int second, int third, int length)
    {
        this.first = first;
        this.second = second;
        this.third = third;
        Length = length;
    }

    /// <summary>How many code points the decomposition has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EA42CE
    // Broiler-Human:        PENDING
    internal int Length { get; }

    /// <summary>The code point at <paramref name="index"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=816B19
    // Broiler-Human:        PENDING
    internal int this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (!pool.IsEmpty)
            {
                return JsUnicodeNormalization.ReadInt24(pool, index * 3);
            }

            return index switch
            {
                0 => first,
                1 => second,
                _ => third,
            };
        }
    }
}

/// <summary>
/// Unicode 17.0.0 normalization data for <c>String.prototype.normalize</c>: canonical combining
/// classes, full canonical and compatibility decompositions, primary composition and the
/// composed-form quick checks, with Hangul handled by the algorithm rather than a table.
/// </summary>
/// <remarks>
/// <para>
/// <b>The data is generated; this half is the only part written by hand.</b>
/// <c>JsUnicodeNormalization.g.cs</c> is written by <c>UnicodeTableGenerator</c> in the
/// architecture test project from the UCD files <c>src/tests/unicode/pins/unicode.pin</c> names,
/// and rule N22 holds it byte for byte to what that generator writes (decision JSD-0031). The
/// platform's <c>string.Normalize</c> is not a substitute and rule N23 forbids it here: it depends
/// on the host's globalization mode and throws on a lone surrogate the language requires to pass
/// through.
/// </para>
/// <para>
/// <b>What this is not.</b> It is the data and the per-code-point lookups; it does not normalize a
/// string. Decomposing, reordering by combining class, composing with blocking and charging the
/// work are <c>JsRealm.NormalizeText</c>'s (slice U3, F08), beside <c>String.prototype.normalize</c>,
/// because the charge is the realm's.
/// </para>
/// <para>
/// <b>Invariants.</b> A lone surrogate, an unassigned code point and every code point the data
/// does not mention have combining class 0, no decomposition and compose with nothing. A
/// decomposition is FULL - applied recursively - and is not canonically reordered. The
/// generator checks the canonical and compatibility sets against <c>NFD_QC</c> and
/// <c>NFKD_QC</c> in <c>DerivedNormalizationProps.txt</c>, so <c>NFD_QC=No</c> holds exactly where
/// <see cref="TryGetDecomposition"/> answers true without <c>compatibility</c>, and
/// <c>NFKD_QC=No</c> exactly where it answers true with it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8408B9
// Broiler-Human:        PENDING
internal static partial class JsUnicodeNormalization
{
    /// <summary>The first Hangul syllable, U+AC00.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E66DF2
    // Broiler-Human:        PENDING
    internal const int HangulSyllableBase = 0xAC00;

    /// <summary>The first leading consonant jamo, U+1100.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F2918C
    // Broiler-Human:        PENDING
    internal const int HangulLeadingBase = 0x1100;

    /// <summary>The first vowel jamo, U+1161.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CF2DF3
    // Broiler-Human:        PENDING
    internal const int HangulVowelBase = 0x1161;

    /// <summary>One before the first trailing consonant jamo, U+11A7: a syllable with no trailing consonant has index 0.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EC2FC5
    // Broiler-Human:        PENDING
    internal const int HangulTrailingBase = 0x11A7;

    /// <summary>The number of leading consonants, vowels and trailing positions: 19, 21 and 28.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8A277A
    // Broiler-Human:        PENDING
    internal const int HangulLeadingCount = 19, HangulVowelCount = 21, HangulTrailingCount = 28;

    /// <summary>The number of Hangul syllables, 11,172.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3D5D4B
    // Broiler-Human:        PENDING
    internal const int HangulSyllableCount = HangulLeadingCount * HangulVowelCount * HangulTrailingCount;

    /// <summary>The Canonical_Combining_Class of <paramref name="codePoint"/>; 0 for a starter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=099BA8
    // Broiler-Human:        PENDING
    internal static int CombiningClass(int codePoint)
    {
        var low = 0;
        var high = (CombiningClassData.Length / 7) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < ReadInt24(CombiningClassData, middle * 7))
            {
                high = middle - 1;
            }
            else if (codePoint > ReadInt24(CombiningClassData, (middle * 7) + 3))
            {
                low = middle + 1;
            }
            else
            {
                return CombiningClassData[(middle * 7) + 6];
            }
        }

        return 0;
    }

    /// <summary>
    /// The full canonical decomposition of <paramref name="codePoint"/> - or with
    /// <paramref name="compatibility"/>, its full compatibility decomposition - when it has one.
    /// </summary>
    /// <remarks>
    /// False means the code point decomposes to itself. The mapping is at most
    /// <see cref="MaxDecompositionLength"/> code points long, is already applied recursively, and
    /// still has to be canonically reordered with its neighbours. A Hangul syllable decomposes
    /// algorithmically into two or three jamo in both forms.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=37A5BB
    // Broiler-Human:        PENDING
    internal static bool TryGetDecomposition(int codePoint, bool compatibility, out JsUnicodeCodePoints mapping)
    {
        var syllable = codePoint - HangulSyllableBase;

        if ((uint)syllable < HangulSyllableCount)
        {
            var trailing = syllable % HangulTrailingCount;

            mapping = new JsUnicodeCodePoints(
                HangulLeadingBase + (syllable / (HangulVowelCount * HangulTrailingCount)),
                HangulVowelBase + (syllable % (HangulVowelCount * HangulTrailingCount) / HangulTrailingCount),
                HangulTrailingBase + trailing,
                trailing == 0 ? 2 : 3);

            return true;
        }

        if ((compatibility && Lookup(CompatibilityIndex, codePoint, out mapping)) ||
            Lookup(CanonicalIndex, codePoint, out mapping))
        {
            return true;
        }

        mapping = default;
        return false;
    }

    /// <summary>
    /// The primary composite of <paramref name="first"/> followed by <paramref name="second"/>,
    /// when there is one: a canonical pair not excluded by Full_Composition_Exclusion, or a Hangul
    /// leading consonant and vowel, or a two-jamo syllable and a trailing consonant.
    /// </summary>
    /// <remarks>Blocking is the caller's: this answers for the two code points alone.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AEDD1D
    // Broiler-Human:        PENDING
    internal static bool TryCompose(int first, int second, out int composite)
    {
        var leading = first - HangulLeadingBase;
        var vowel = second - HangulVowelBase;

        if ((uint)leading < HangulLeadingCount && (uint)vowel < HangulVowelCount)
        {
            composite = HangulSyllableBase + (((leading * HangulVowelCount) + vowel) * HangulTrailingCount);
            return true;
        }

        var syllable = first - HangulSyllableBase;
        var trailing = second - HangulTrailingBase;

        if ((uint)syllable < HangulSyllableCount && syllable % HangulTrailingCount == 0 &&
            trailing > 0 && trailing < HangulTrailingCount)
        {
            composite = first + trailing;
            return true;
        }

        var low = 0;
        var high = (CompositionData.Length / 9) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var order = ReadInt24(CompositionData, middle * 9).CompareTo(first);

            if (order == 0)
            {
                order = ReadInt24(CompositionData, (middle * 9) + 3).CompareTo(second);
            }

            if (order == 0)
            {
                composite = ReadInt24(CompositionData, (middle * 9) + 6);
                return true;
            }

            if (order < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        composite = 0;
        return false;
    }

    /// <summary>
    /// <c>NFC_QC</c> of <paramref name="codePoint"/>, or with <paramref name="compatibility"/>
    /// <c>NFKC_QC</c>. For the decomposed forms no table is needed: <c>NFD_QC</c> and
    /// <c>NFKD_QC</c> are No exactly where <see cref="TryGetDecomposition"/> answers true.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C306FD
    // Broiler-Human:        PENDING
    internal static JsNormalizationQuickCheck QuickCheck(int codePoint, bool compatibility)
    {
        var set = compatibility ? 2 : 0;

        if (InQuickCheckSet(set, codePoint))
        {
            return JsNormalizationQuickCheck.No;
        }

        return InQuickCheckSet(set + 1, codePoint) ? JsNormalizationQuickCheck.Maybe : JsNormalizationQuickCheck.Yes;
    }

    /// <summary>A little-endian three-byte value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5C0E6B
    // Broiler-Human:        PENDING
    internal static int ReadInt24(ReadOnlySpan<byte> data, int offset) =>
        data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);

    /// <summary>Membership of one of the four quick-check range sets.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=36C2EE
    // Broiler-Human:        PENDING
    private static bool InQuickCheckSet(int set, int codePoint)
    {
        var low = QuickCheckIndex[set * 4] | (QuickCheckIndex[(set * 4) + 1] << 8);
        var high = low + (QuickCheckIndex[(set * 4) + 2] | (QuickCheckIndex[(set * 4) + 3] << 8)) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < ReadInt24(QuickCheckData, middle * 6))
            {
                high = middle - 1;
            }
            else if (codePoint > ReadInt24(QuickCheckData, (middle * 6) + 3))
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

    /// <summary>A decomposition index's entry for <paramref name="codePoint"/>, as a view of the pool.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=23334B
    // Broiler-Human:        PENDING
    private static bool Lookup(ReadOnlySpan<byte> index, int codePoint, out JsUnicodeCodePoints mapping)
    {
        var low = 0;
        var high = (index.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var key = ReadInt24(index, middle * 6);

            if (key == codePoint)
            {
                var offset = index[(middle * 6) + 3] | (index[(middle * 6) + 4] << 8);

                mapping = new JsUnicodeCodePoints(DecompositionPool.Slice(offset * 3, index[(middle * 6) + 5] * 3));
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
}
