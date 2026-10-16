// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   29
// Annotated:        29/29
// Exempt:           15
// Human-reviewed:   0/29
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       29
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Unicode.Algorithms;

/// <summary>The four Unicode normalization forms (UAX #15, section 1.2).</summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=44AD15
// Broiler-Human:        PENDING
public enum NormalForm
{
    /// <summary>NFC: canonical decomposition, then canonical composition.</summary>
    Nfc = 0,

    /// <summary>NFD: canonical decomposition.</summary>
    Nfd = 1,

    /// <summary>NFKC: compatibility decomposition, then canonical composition.</summary>
    Nfkc = 2,

    /// <summary>NFKD: compatibility decomposition.</summary>
    Nfkd = 3,
}

/// <summary>A normalization quick-check value, <c>NFC_QC</c> or <c>NFKC_QC</c> (UAX #15, section 9).</summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=89AED9
// Broiler-Human:        PENDING
public enum NormalizationQuickCheck
{
    /// <summary>The code point can stand in the normalized form wherever it occurs.</summary>
    Yes = 0,

    /// <summary>Whether it can depends on what precedes it.</summary>
    Maybe = 1,

    /// <summary>It never occurs in the normalized form.</summary>
    No = 2,
}

/// <summary>
/// Unicode normalization (UAX #15) over the tables: what ECMA-262's <c>String.prototype.normalize</c>
/// performs on a String's code points (section 22.1.3.15), in the four forms.
/// </summary>
/// <remarks>
/// <para>
/// <b>The algorithm.</b> Every code point is replaced by its full decomposition, each run of
/// non-starters is put in canonical order by combining class, and for the composed forms each code point
/// is combined with the last starter unless something between them blocks it. A lone surrogate is read
/// as the code point it is, of class 0, with no decomposition and no composite, so it passes through.
/// Hangul syllables are decomposed and composed by the algorithm, not by a table. No platform
/// normalizer is read, so the answer is the pinned version's on every host.
/// </para>
/// <para>
/// <b>The charge</b> is Broiler.VM's, in its units (JSD-0074, decision 4; correspondence row N7): the
/// input's length; then, unless the input is already in the form, the input's length plus three units
/// per code point of its full decomposition, charged before that work. Each charge is at least one unit.
/// </para>
/// <para>
/// <b>The limit is on the result.</b> A result longer than the caller's maximum, in UTF-16 code units,
/// is <see cref="UnicodeStatus.LimitExceeded"/>; a result that fits is returned however long the
/// decomposition it was composed from (row N6). The work is done one segment at a time, from one
/// starter that cannot combine with what precedes it to the next, so no buffer outgrows the longest
/// segment and the result is refused as soon as it is known not to fit.
/// </para>
/// <para>
/// <b>Names.</b> No member is named <c>Normalize</c>, <c>IsNormalized</c> or <c>NormalizationForm</c>
/// (convention 14), which Broiler.VM's rule N23 refuses as the platform's.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=1; Fingerprint=8F642B
// Broiler-Human:        PENDING
public sealed class UnicodeNormalization
{
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=DB3C88
    // Broiler-Human:        PENDING
    private const int HangulSyllableBase = 0xAC00;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=AE3AB8
    // Broiler-Human:        PENDING
    private const int HangulLeadingBase = 0x1100;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=AE6447
    // Broiler-Human:        PENDING
    private const int HangulVowelBase = 0x1161;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=9AB0F3
    // Broiler-Human:        PENDING
    private const int HangulTrailingBase = 0x11A7;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=1E175F
    // Broiler-Human:        PENDING
    private const int HangulLeadingCount = 19;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=97F5FE
    // Broiler-Human:        PENDING
    private const int HangulVowelCount = 21;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=481DBD
    // Broiler-Human:        PENDING
    private const int HangulTrailingCount = 28;
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=9C7B2B
    // Broiler-Human:        PENDING
    private const int HangulSyllableCount = HangulLeadingCount * HangulVowelCount * HangulTrailingCount;

    private readonly IUnicodeTables tables;

    /// <summary>Normalization over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.NormalizationLayout"/> to <see cref="UnicodeTable.QuickCheckIndex"/>.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=4587DC
    // Broiler-Human:        PENDING
    public UnicodeNormalization(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);

        TableReader.Require(tables, UnicodeTable.NormalizationLayout, 3);
        TableReader.Require(tables, UnicodeTable.CombiningClassData, 7);
        TableReader.Require(tables, UnicodeTable.CanonicalIndex, 6);
        TableReader.Require(tables, UnicodeTable.CompatibilityIndex, 6);
        TableReader.Require(tables, UnicodeTable.DecompositionPool, 3);
        TableReader.Require(tables, UnicodeTable.CompositionData, 9);
        TableReader.Require(tables, UnicodeTable.QuickCheckData, 6);
        TableReader.Require(tables, UnicodeTable.QuickCheckIndex, 4);

        var layout = tables.Table(UnicodeTable.NormalizationLayout);

        if (layout.Length != 3 || tables.Table(UnicodeTable.QuickCheckIndex).Length != 16)
        {
            throw new ArgumentException("NormalizationLayout or QuickCheckIndex does not have its stated length.", nameof(tables));
        }

        MaxDecompositionLength = TableReader.Layout(layout, 0);
    }

    /// <summary>The longest full decomposition, in code points; a Hangul syllable's is at most three.</summary>
    public int MaxDecompositionLength { get; }

    /// <summary>The Canonical_Combining_Class of <paramref name="codePoint"/> (<c>UnicodeData.txt</c>); 0 for a starter.</summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=F14293
    // Broiler-Human:        PENDING
    public int CombiningClass(int codePoint)
    {
        var data = tables.Table(UnicodeTable.CombiningClassData);
        var low = 0;
        var high = (data.Length / 7) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < TableReader.Int24(data, middle * 7))
            {
                high = middle - 1;
            }
            else if (codePoint > TableReader.Int24(data, (middle * 7) + 3))
            {
                low = middle + 1;
            }
            else
            {
                return data[(middle * 7) + 6];
            }
        }

        return 0;
    }

    /// <summary>
    /// <c>NFC_QC</c> of <paramref name="codePoint"/>, or with <paramref name="compatibility"/>
    /// <c>NFKC_QC</c> (<c>DerivedNormalizationProps.txt</c>).
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    /// <param name="compatibility">Whether to answer <c>NFKC_QC</c>.</param>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=7CD4BD
    // Broiler-Human:        PENDING
    public NormalizationQuickCheck QuickCheck(int codePoint, bool compatibility)
    {
        var set = compatibility ? 2 : 0;

        if (InQuickCheckSet(set, codePoint))
        {
            return NormalizationQuickCheck.No;
        }

        return InQuickCheckSet(set + 1, codePoint) ? NormalizationQuickCheck.Maybe : NormalizationQuickCheck.Yes;
    }

    /// <summary>
    /// The normalization of <paramref name="text"/>'s code points in <paramref name="form"/>
    /// (UAX #15; ECMA-262 section 22.1.3.15 step 7). A text already in the form is answered as itself.
    /// </summary>
    /// <param name="text">The text, read as code points; a lone surrogate is one.</param>
    /// <param name="form">The form.</param>
    /// <param name="maxLength">The longest result the caller accepts, in UTF-16 code units.</param>
    /// <param name="charge">The caller's meter, or null; units as this type states.</param>
    /// <param name="result">The normalized text, or null when the status is not <see cref="UnicodeStatus.Success"/>.</param>
    /// <returns><see cref="UnicodeStatus.Success"/>, or <see cref="UnicodeStatus.LimitExceeded"/> when the result is longer than <paramref name="maxLength"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="form"/> is not a form, or <paramref name="maxLength"/> is negative.</exception>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=High; Resources=5; Fingerprint=DBFB2E
    // Broiler-Falsified-If: a result longer than maxLength is answered as Success, or a charge differs from Broiler.VM row N7
    // Broiler-Human:        PENDING
    public UnicodeStatus Transform(string text, NormalForm form, int maxLength, UnicodeWorkCharge? charge, out string? result)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        if ((uint)form > (uint)NormalForm.Nfkd)
        {
            throw new ArgumentOutOfRangeException(nameof(form));
        }

        var compose = form is NormalForm.Nfc or NormalForm.Nfkc;
        var compatibility = form is NormalForm.Nfkc or NormalForm.Nfkd;

        Charge(charge, text.Length);

        if (IsInFormAlready(text, compose, compatibility))
        {
            result = text.Length > maxLength ? null : text;
            return result is null ? UnicodeStatus.LimitExceeded : UnicodeStatus.Success;
        }

        long points = 0;
        long units = 0;

        for (var at = 0; at < text.Length;)
        {
            var codePoint = Utf16Reader.Next(text, at, out var width);
            at += width;

            if (TryGetDecomposition(codePoint, compatibility, out var mapping))
            {
                points += mapping.Length;

                for (var index = 0; index < mapping.Length; index++)
                {
                    units += Utf16Reader.Width(mapping[index]);
                }
            }
            else
            {
                points++;
                units += width;
            }
        }

        // A decomposed form is its decomposition, so its length is known before any work. A composed
        // form is never longer than its decomposition, and its decomposition is at most
        // MaxDecompositionLength code points per code point of the result, so a decomposition longer
        // than that many times the limit cannot compose to a result that fits.
        if ((!compose && units > maxLength) || (compose && points > (long)MaxDecompositionLength * maxLength))
        {
            result = null;
            return UnicodeStatus.LimitExceeded;
        }

        Charge(charge, text.Length + (points * 3));

        return Produce(text, compose, compatibility, (int)Math.Min(units, (long)maxLength + 1), maxLength, out result);
    }

    /// <summary>
    /// The full canonical decomposition of <paramref name="codePoint"/>, or with
    /// <paramref name="compatibility"/> its full compatibility decomposition, when it has one; a Hangul
    /// syllable's is computed. The mapping is applied recursively and is not reordered.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=8D8E9C
    // Broiler-Human:        PENDING
    internal bool TryGetDecomposition(int codePoint, bool compatibility, out CodePoints mapping)
    {
        var syllable = codePoint - HangulSyllableBase;

        if ((uint)syllable < HangulSyllableCount)
        {
            var trailing = syllable % HangulTrailingCount;

            mapping = new CodePoints(
                HangulLeadingBase + (syllable / (HangulVowelCount * HangulTrailingCount)),
                HangulVowelBase + (syllable % (HangulVowelCount * HangulTrailingCount) / HangulTrailingCount),
                HangulTrailingBase + trailing,
                trailing == 0 ? 2 : 3);

            return true;
        }

        if ((compatibility && Lookup(tables.Table(UnicodeTable.CompatibilityIndex), codePoint, out mapping)) ||
            Lookup(tables.Table(UnicodeTable.CanonicalIndex), codePoint, out mapping))
        {
            return true;
        }

        mapping = default;
        return false;
    }

    /// <summary>
    /// The primary composite of <paramref name="first"/> followed by <paramref name="second"/>, when there
    /// is one: a canonical pair not excluded by Full_Composition_Exclusion, or a Hangul leading consonant
    /// and vowel, or a two-jamo syllable and a trailing consonant. Blocking is the caller's.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=8645E4
    // Broiler-Human:        PENDING
    internal bool TryCompose(int first, int second, out int composite)
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

        var data = tables.Table(UnicodeTable.CompositionData);
        var low = 0;
        var high = (data.Length / 9) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var order = TableReader.Int24(data, middle * 9).CompareTo(first);

            if (order == 0)
            {
                order = TableReader.Int24(data, (middle * 9) + 3).CompareTo(second);
            }

            if (order == 0)
            {
                composite = TableReader.Int24(data, (middle * 9) + 6);
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

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=100C46
    // Broiler-Human:        PENDING
    private static void Charge(UnicodeWorkCharge? charge, long units) =>
        charge?.Invoke(units <= 0 ? 1UL : (ulong)units);

    /// <summary>
    /// The quick check: true when <paramref name="text"/> is certainly in the form, so it can be returned
    /// unchanged. A code point breaks it when it is out of canonical order, when it decomposes (the
    /// decomposed forms), or when its quick-check value is not Yes (the composed forms).
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=1FCE60
    // Broiler-Human:        PENDING
    private bool IsInFormAlready(string text, bool compose, bool compatibility)
    {
        var lastClass = 0;

        for (var at = 0; at < text.Length;)
        {
            if (text[at] < 0x80)
            {
                lastClass = 0;
                at++;
                continue;
            }

            var codePoint = Utf16Reader.Next(text, at, out var width);
            at += width;
            var combiningClass = CombiningClass(codePoint);

            if (combiningClass != 0 && lastClass > combiningClass)
            {
                return false;
            }

            if (compose
                ? QuickCheck(codePoint, compatibility) != NormalizationQuickCheck.Yes
                : TryGetDecomposition(codePoint, compatibility, out _))
            {
                return false;
            }

            lastClass = combiningClass;
        }

        return true;
    }

    /// <summary>
    /// Whether a segment may begin at <paramref name="codePoint"/>, whose decomposition begins with
    /// <paramref name="first"/>: a starter, so no reordering crosses it, and in the composed forms a code
    /// point nothing before it can combine with (its quick check, and its decomposition's first code
    /// point's, is Yes). A boundary placed only where both hold is safe; one missed makes a segment longer.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=450CA3
    // Broiler-Human:        PENDING
    private bool StartsSegment(int codePoint, int first, bool compose, bool compatibility) =>
        codePoint < 0x80 ||
        (CombiningClass(first) == 0 && (!compose ||
            (QuickCheck(codePoint, compatibility) == NormalizationQuickCheck.Yes && QuickCheck(first, compatibility) == NormalizationQuickCheck.Yes)));

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=High; Resources=5; Fingerprint=F5D030
    // Broiler-Falsified-If: a buffer grows past the longest segment times the longest decomposition, or a refused result is returned
    // Broiler-Human:        PENDING
    private UnicodeStatus Produce(string text, bool compose, bool compatibility, int capacity, int maxLength, out string? result)
    {
        var output = new char[capacity];
        var written = 0;
        var buffer = new int[16];
        var length = 0;

        for (var at = 0; ;)
        {
            var end = at == text.Length;
            var codePoint = 0;
            var width = 0;
            var decomposes = false;
            CodePoints mapping = default;

            if (!end)
            {
                codePoint = Utf16Reader.Next(text, at, out width);
                decomposes = codePoint >= 0x80 && TryGetDecomposition(codePoint, compatibility, out mapping);
            }

            if (length > 0 && (end || StartsSegment(codePoint, decomposes ? mapping[0] : codePoint, compose, compatibility)))
            {
                OrderCanonically(buffer, length);

                if (compose)
                {
                    length = ComposeCanonically(buffer, length);
                }

                for (var index = 0; index < length; index++)
                {
                    var point = buffer[index];

                    if (written + Utf16Reader.Width(point) > maxLength)
                    {
                        result = null;
                        return UnicodeStatus.LimitExceeded;
                    }

                    if (point > 0xFFFF)
                    {
                        output[written++] = (char)(0xD7C0 + (point >> 10));
                        output[written++] = (char)(0xDC00 | (point & 0x3FF));
                    }
                    else
                    {
                        output[written++] = (char)point;
                    }
                }

                length = 0;
            }

            if (end)
            {
                break;
            }

            at += width;

            if (decomposes)
            {
                Reserve(ref buffer, length + mapping.Length);

                for (var index = 0; index < mapping.Length; index++)
                {
                    buffer[length++] = mapping[index];
                }
            }
            else
            {
                Reserve(ref buffer, length + 1);
                buffer[length++] = codePoint;
            }
        }

        result = new string(output, 0, written);
        return UnicodeStatus.Success;
    }

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=9E9FF3
    // Broiler-Human:        PENDING
    private static void Reserve(ref int[] buffer, int needed)
    {
        if (needed > buffer.Length)
        {
            Array.Resize(ref buffer, Math.Max(needed, buffer.Length * 2));
        }
    }

    /// <summary>
    /// The canonical ordering algorithm: each run of non-starters is sorted by combining class, stably,
    /// by a counting sort, so a long run is linear work rather than an exchange sort's quadratic.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=1103C1
    // Broiler-Human:        PENDING
    private void OrderCanonically(int[] buffer, int length)
    {
        int[]? scratch = null;
        Span<int> starts = stackalloc int[257];

        for (var start = 0; start < length;)
        {
            var previous = CombiningClass(buffer[start]);

            if (previous == 0)
            {
                start++;
                continue;
            }

            var end = start + 1;
            var ordered = true;

            while (end < length)
            {
                var next = CombiningClass(buffer[end]);

                if (next == 0)
                {
                    break;
                }

                ordered &= previous <= next;
                previous = next;
                end++;
            }

            if (!ordered)
            {
                var count = end - start;

                if (scratch is null || scratch.Length < count)
                {
                    scratch = new int[count];
                }

                starts.Clear();

                for (var at = start; at < end; at++)
                {
                    starts[CombiningClass(buffer[at]) + 1]++;
                }

                for (var value = 1; value < starts.Length; value++)
                {
                    starts[value] += starts[value - 1];
                }

                for (var at = start; at < end; at++)
                {
                    scratch[starts[CombiningClass(buffer[at])]++] = buffer[at];
                }

                Array.Copy(scratch, 0, buffer, start, count);
            }

            start = end;
        }
    }

    /// <summary>
    /// The canonical composition algorithm over a canonically ordered decomposition, in place; answers
    /// the composed length. A code point is blocked from the last starter when something kept between
    /// them is a starter or has a class at least its own.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=A876D5
    // Broiler-Human:        PENDING
    private int ComposeCanonically(int[] buffer, int length)
    {
        var starter = -1;
        var keptSinceStarter = false;
        var lastClass = 0;
        var written = 0;

        for (var at = 0; at < length; at++)
        {
            var codePoint = buffer[at];
            var combiningClass = CombiningClass(codePoint);

            if (starter >= 0 &&
                !(keptSinceStarter && (lastClass == 0 || lastClass >= combiningClass)) &&
                TryCompose(buffer[starter], codePoint, out var composite))
            {
                buffer[starter] = composite;
                continue;
            }

            if (combiningClass == 0)
            {
                starter = written;
                keptSinceStarter = false;
            }
            else
            {
                keptSinceStarter = true;
            }

            lastClass = combiningClass;
            buffer[written++] = codePoint;
        }

        return written;
    }

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=682A15
    // Broiler-Human:        PENDING
    private bool InQuickCheckSet(int set, int codePoint)
    {
        var index = tables.Table(UnicodeTable.QuickCheckIndex);
        var first = TableReader.UInt16(index, set * 4);
        var count = TableReader.UInt16(index, (set * 4) + 2);

        return TableReader.InRanges(tables.Table(UnicodeTable.QuickCheckData), first, count, codePoint);
    }

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=45A4E0
    // Broiler-Human:        PENDING
    private bool Lookup(ReadOnlySpan<byte> index, int codePoint, out CodePoints mapping)
    {
        var low = 0;
        var high = (index.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var key = TableReader.Int24(index, middle * 6);

            if (key == codePoint)
            {
                var offset = TableReader.UInt16(index, (middle * 6) + 3);

                mapping = new CodePoints(tables.Table(UnicodeTable.DecompositionPool).Slice(offset * 3, index[(middle * 6) + 5] * 3));
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

/// <summary>
/// The code points of one mapping, read from a pool of three-byte code points or computed for a Hangul
/// syllable. A view: it allocates nothing.
/// </summary>
// Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=1; Fingerprint=B9D679
// Broiler-Human:        PENDING
internal readonly ref struct CodePoints
{
    private readonly ReadOnlySpan<byte> pool;

    private readonly int first;

    private readonly int second;

    private readonly int third;

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=3FB556
    // Broiler-Human:        PENDING
    internal CodePoints(ReadOnlySpan<byte> pool)
    {
        this.pool = pool;
        Length = pool.Length / 3;
    }

    internal CodePoints(int first, int second, int third, int length)
    {
        this.first = first;
        this.second = second;
        this.third = third;
        Length = length;
    }

    internal int Length { get; }

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=6E3970
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
                return TableReader.Int24(pool, index * 3);
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
