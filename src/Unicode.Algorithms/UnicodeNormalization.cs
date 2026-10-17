// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   43
// Annotated:        43/43
// Exempt:           32
// Human-reviewed:   0/43
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       43
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

    // Broiler-AI:           Origin=AI; Spec=UAX #15 section 9 quick check; IP=Low; Security=Low; Resources=0; Fingerprint=79C10D
    // Broiler-Human:        PENDING
    private const int StackLimit = 256;

    // Broiler-AI:           Origin=AI; Spec=UAX #15 section 9 quick check; IP=Low; Security=Low; Resources=0; Fingerprint=26E94D
    // Broiler-Human:        PENDING
    private const int CodePointMask = 0xFFFFFF;

    private readonly IUnicodeTables tables;

    private readonly int firstCombining;

    private readonly int firstCanonical;

    private readonly int firstCompatibility;

    private readonly int firstNfcQuickCheck;

    private readonly int firstNfkcQuickCheck;

    private readonly int firstSecond;

    /// <summary>Normalization over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.NormalizationLayout"/> to <see cref="UnicodeTable.QuickCheckIndex"/>.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    /// <remarks>
    /// The lowest code point each table names is read once here, so a code point below it is answered
    /// without a search; a convention 10 value derived from the provider at construction.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=84042C
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

        var compositions = tables.Table(UnicodeTable.CompositionData);

        firstCombining = TableReader.Int24(tables.Table(UnicodeTable.CombiningClassData), 0);
        firstCanonical = TableReader.Int24(tables.Table(UnicodeTable.CanonicalIndex), 0);
        firstCompatibility = Math.Min(firstCanonical, TableReader.Int24(tables.Table(UnicodeTable.CompatibilityIndex), 0));
        firstNfcQuickCheck = Math.Min(FirstOfQuickCheckSet(0), FirstOfQuickCheckSet(1));
        firstNfkcQuickCheck = Math.Min(FirstOfQuickCheckSet(2), FirstOfQuickCheckSet(3));
        firstSecond = int.MaxValue;

        for (var at = 3; at < compositions.Length; at += 9)
        {
            firstSecond = Math.Min(firstSecond, TableReader.Int24(compositions, at));
        }
    }

    /// <summary>The longest full decomposition, in code points; a Hangul syllable's is at most three.</summary>
    public int MaxDecompositionLength { get; }

    /// <summary>The Canonical_Combining_Class of <paramref name="codePoint"/> (<c>UnicodeData.txt</c>); 0 for a starter.</summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=F05199
    // Broiler-Human:        PENDING
    public int CombiningClass(int codePoint) =>
        codePoint < firstCombining ? 0 : ClassOf(tables.Table(UnicodeTable.CombiningClassData), codePoint);

    /// <summary>
    /// <c>NFC_QC</c> of <paramref name="codePoint"/>, or with <paramref name="compatibility"/>
    /// <c>NFKC_QC</c> (<c>DerivedNormalizationProps.txt</c>).
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    /// <param name="compatibility">Whether to answer <c>NFKC_QC</c>.</param>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=075509
    // Broiler-Human:        PENDING
    public NormalizationQuickCheck QuickCheck(int codePoint, bool compatibility) =>
        new FormTables(this, compatibility).QuickCheck(codePoint);

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
    /// <remarks>
    /// The work starts at the first code point the quick check does not pass: the text before the
    /// segment holding it is in the form already and is copied as it is.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=High; Resources=5; Fingerprint=E4472E
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

        Charge(charge, text.Length);

        // ASCII is in every form, and the quick check starts after it.
        var firstNonAscii = text.AsSpan().IndexOfAnyExceptInRange('\0', '\u007F');
        var lookup = firstNonAscii < 0 ? default : new FormTables(this, form is NormalForm.Nfkc or NormalForm.Nfkd);
        var outOfForm = firstNonAscii < 0 ? -1 : FirstOutOfForm(text, firstNonAscii, compose, lookup);

        if (outOfForm < 0)
        {
            result = text.Length > maxLength ? null : text;
            return result is null ? UnicodeStatus.LimitExceeded : UnicodeStatus.Success;
        }

        var start = SegmentStart(text, outOfForm, compose, lookup);
        long points = 0;
        long units = 0;

        // Before the segment start nothing decomposes in the decomposed forms; in the composed forms
        // the code points there are counted by their decompositions and not looked up again.
        for (var at = 0; at < start;)
        {
            var codePoint = Utf16Reader.Next(text, at, out var width);
            at += width;
            Count(compose && codePoint >= 0x80 ? lookup.DecompositionHandle(codePoint) : 0, codePoint, width, lookup, ref points, ref units);
        }

        // From it on, each code point's decomposition is kept for the work when the text is short, so
        // it is looked up once; a long text's is looked up again, so no buffer outgrows the limit.
        Span<int> handles = text.Length - start <= StackLimit ? stackalloc int[StackLimit] : default;
        var count = 0;

        for (var at = start; at < text.Length; count++)
        {
            var codePoint = Utf16Reader.Next(text, at, out var width);
            at += width;
            var handle = codePoint >= 0x80 ? lookup.DecompositionHandle(codePoint) : 0;
            Count(handle, codePoint, width, lookup, ref points, ref units);

            if (!handles.IsEmpty)
            {
                handles[count] = handle;
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

        return Produce(
            text, start, handles.IsEmpty ? handles : handles[..count], compose, lookup,
            (int)Math.Min(units, (long)maxLength + 1), maxLength, out result);
    }

    /// <summary>
    /// The full canonical decomposition of <paramref name="codePoint"/>, or with
    /// <paramref name="compatibility"/> its full compatibility decomposition, when it has one; a Hangul
    /// syllable's is computed. The mapping is applied recursively and is not reordered.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=FE7C0D
    // Broiler-Human:        PENDING
    internal bool TryGetDecomposition(int codePoint, bool compatibility, out CodePoints mapping) =>
        new FormTables(this, compatibility).TryGetDecomposition(codePoint, out mapping);

    /// <summary>
    /// The primary composite of <paramref name="first"/> followed by <paramref name="second"/>, when there
    /// is one: a canonical pair not excluded by Full_Composition_Exclusion, or a Hangul leading consonant
    /// and vowel, or a two-jamo syllable and a trailing consonant. Blocking is the caller's.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=3D3F77
    // Broiler-Human:        PENDING
    internal bool TryCompose(int first, int second, out int composite) =>
        new FormTables(this, compatibility: false).TryCompose(first, second, out composite);

    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=100C46
    // Broiler-Human:        PENDING
    private static void Charge(UnicodeWorkCharge? charge, long units) =>
        charge?.Invoke(units <= 0 ? 1UL : (ulong)units);

    /// <summary>Adds one code point's full decomposition, by its handle, to the code points and code units counted.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=6D1E94
    // Broiler-Human:        PENDING
    private static void Count(int handle, int codePoint, int width, in FormTables lookup, ref long points, ref long units)
    {
        if (handle == 0)
        {
            points++;
            units += width;
            return;
        }

        var mapping = lookup.Decomposition(codePoint, handle);
        points += mapping.Length;

        for (var index = 0; index < mapping.Length; index++)
        {
            units += Utf16Reader.Width(mapping[index]);
        }
    }

    /// <summary>The Canonical_Combining_Class of <paramref name="codePoint"/> in <see cref="UnicodeTable.CombiningClassData"/>: a binary search over its runs.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=C99210
    // Broiler-Human:        PENDING
    private static int ClassOf(ReadOnlySpan<byte> data, int codePoint)
    {
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
    /// The quick check from <paramref name="from"/>, where a code point outside ASCII follows ASCII or
    /// the start: the position of the first code point that keeps <paramref name="text"/> from being
    /// certainly in the form, or -1 when it is, so it can be returned unchanged. A code point
    /// breaks it when it is out of canonical order, when it decomposes (the decomposed forms), or when
    /// its quick-check value is not Yes (the composed forms).
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=413C8C
    // Broiler-Human:        PENDING
    private static int FirstOutOfForm(string text, int from, bool compose, in FormTables lookup)
    {
        var lastClass = 0;

        for (var at = from; at < text.Length;)
        {
            if (text[at] < 0x80)
            {
                lastClass = 0;
                at++;
                continue;
            }

            var codePoint = Utf16Reader.Next(text, at, out var width);
            var combiningClass = lookup.CombiningClass(codePoint);

            if ((combiningClass != 0 && lastClass > combiningClass) ||
                (compose ? !lookup.IsQuickCheckYes(codePoint) : lookup.TryGetDecomposition(codePoint, out _)))
            {
                return at;
            }

            lastClass = combiningClass;
            at += width;
        }

        return -1;
    }

    /// <summary>
    /// Where the segment holding the code point at <paramref name="outOfForm"/> starts: the last code
    /// point at or before it that <see cref="StartsSegment"/> admits, or the start of the text. The
    /// segments before it are in the form already, so they are their own normalization. ASCII starts a
    /// segment, so the walk back is as long as the run of non-ASCII code points before the position.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UAX #15 section 9 quick check; IP=Low; Security=Medium; Resources=1; Fingerprint=DC68AB
    // Broiler-Human:        PENDING
    private static int SegmentStart(string text, int outOfForm, bool compose, in FormTables lookup)
    {
        for (var at = outOfForm; ;)
        {
            var codePoint = Utf16Reader.Next(text, at, out _);
            var first = lookup.TryGetDecomposition(codePoint, out var mapping) ? mapping[0] : codePoint;

            if (at == 0 || StartsSegment(codePoint, first, lookup.CombiningClass(first), compose, lookup))
            {
                return at;
            }

            Utf16Reader.Previous(text, at, out var width);
            at -= width;
        }
    }

    /// <summary>
    /// Whether a segment may begin at <paramref name="codePoint"/>, whose decomposition begins with
    /// <paramref name="first"/>, of class <paramref name="firstClass"/>: a starter, so no reordering
    /// crosses it, and in the composed forms a code point nothing before it can combine with (its quick
    /// check, and its decomposition's first code point's, is Yes). A boundary placed only where both
    /// hold is safe; one missed makes a segment longer.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=BD37C6
    // Broiler-Human:        PENDING
    private static bool StartsSegment(int codePoint, int first, int firstClass, bool compose, in FormTables lookup) =>
        codePoint < 0x80 ||
        (firstClass == 0 && (!compose ||
            (lookup.IsQuickCheckYes(codePoint) && (first == codePoint || lookup.IsQuickCheckYes(first)))));

    /// <summary>
    /// The normalization of <paramref name="text"/> from <paramref name="start"/>, a segment boundary,
    /// after the text before it copied as it is. Each code point is looked up once: its decomposition,
    /// which <paramref name="handles"/> holds for each code point from the start when the sizing pass
    /// kept them, and the combining class of every code point of it, which the buffer carries above the
    /// code point's 24 bits through the ordering and the composition.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=High; Resources=5; Fingerprint=BA0C85
    // Broiler-Falsified-If: a buffer grows past the longest segment times the longest decomposition, or a refused result is returned
    // Broiler-Human:        PENDING
    private static UnicodeStatus Produce(
        string text, int start, ReadOnlySpan<int> handles, bool compose, in FormTables lookup, int capacity, int maxLength, out string? result)
    {
        if (start > maxLength)
        {
            result = null;
            return UnicodeStatus.LimitExceeded;
        }

        Span<char> output = capacity <= StackLimit ? stackalloc char[StackLimit] : new char[capacity];
        Span<int> buffer = stackalloc int[32];
        var written = start;
        var length = 0;

        text.AsSpan(0, start).CopyTo(output);

        for (int at = start, index = 0; ; index++)
        {
            var end = at == text.Length;
            var codePoint = 0;
            var width = 0;
            var firstClass = 0;
            var decomposes = false;
            var boundary = true;
            CodePoints mapping = default;

            if (!end)
            {
                codePoint = Utf16Reader.Next(text, at, out width);

                if (codePoint >= 0x80)
                {
                    var handle = handles.IsEmpty ? lookup.DecompositionHandle(codePoint) : handles[index];
                    decomposes = handle != 0;
                    mapping = decomposes ? lookup.Decomposition(codePoint, handle) : default;
                    var first = decomposes ? mapping[0] : codePoint;
                    firstClass = lookup.CombiningClass(first);
                    boundary = StartsSegment(codePoint, first, firstClass, compose, lookup);
                }
            }

            if (length > 0 && (end || boundary))
            {
                OrderCanonically(buffer[..length]);

                if (compose)
                {
                    length = ComposeCanonically(buffer[..length], lookup);
                }

                for (var position = 0; position < length; position++)
                {
                    var point = buffer[position] & CodePointMask;

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
            var needed = length + (decomposes ? mapping.Length : 1);

            if (needed > buffer.Length)
            {
                var larger = new int[Math.Max(needed, buffer.Length * 2)];
                buffer[..length].CopyTo(larger);
                buffer = larger;
            }

            if (decomposes)
            {
                buffer[length++] = mapping[0] | (firstClass << 24);

                for (var position = 1; position < mapping.Length; position++)
                {
                    var point = mapping[position];
                    buffer[length++] = point | (lookup.CombiningClass(point) << 24);
                }
            }
            else
            {
                buffer[length++] = codePoint | (firstClass << 24);
            }
        }

        result = new string(output[..written]);
        return UnicodeStatus.Success;
    }

    /// <summary>
    /// The canonical ordering algorithm over code points that carry their combining class in their top
    /// byte: each run of non-starters is sorted by combining class, stably.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=D9013D
    // Broiler-Human:        PENDING
    private static void OrderCanonically(Span<int> buffer)
    {
        for (var start = 0; start < buffer.Length;)
        {
            var previous = buffer[start] >>> 24;

            if (previous == 0)
            {
                start++;
                continue;
            }

            var end = start + 1;
            var ordered = true;

            while (end < buffer.Length)
            {
                var next = buffer[end] >>> 24;

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
                SortRun(buffer[start..end]);
            }

            start = end;
        }
    }

    /// <summary>
    /// One run of non-starters sorted by combining class, stably, by a counting sort, so a long run is
    /// linear work rather than an exchange sort's quadratic.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=E89628
    // Broiler-Human:        PENDING
    private static void SortRun(Span<int> run)
    {
        Span<int> starts = stackalloc int[257];
        Span<int> sorted = run.Length <= StackLimit ? stackalloc int[StackLimit] : new int[run.Length];

        starts.Clear();

        foreach (var entry in run)
        {
            starts[(entry >>> 24) + 1]++;
        }

        for (var value = 1; value < starts.Length; value++)
        {
            starts[value] += starts[value - 1];
        }

        foreach (var entry in run)
        {
            sorted[starts[entry >>> 24]++] = entry;
        }

        sorted[..run.Length].CopyTo(run);
    }

    /// <summary>
    /// The canonical composition algorithm over a canonically ordered decomposition whose code points
    /// carry their combining class, in place; answers the composed length. A code point is blocked from
    /// the last starter when something kept between them is a starter or has a class at least its own.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=2E91DD
    // Broiler-Human:        PENDING
    private static int ComposeCanonically(Span<int> buffer, in FormTables lookup)
    {
        var starter = -1;
        var keptSinceStarter = false;
        var lastClass = 0;
        var written = 0;

        for (var at = 0; at < buffer.Length; at++)
        {
            var entry = buffer[at];
            var combiningClass = entry >>> 24;

            if (starter >= 0 &&
                !(keptSinceStarter && (lastClass == 0 || lastClass >= combiningClass)) &&
                lookup.TryCompose(buffer[starter] & CodePointMask, entry & CodePointMask, out var composite))
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
            buffer[written++] = entry;
        }

        return written;
    }

    /// <summary>The lowest code point of one of the four quick-check sets, or <see cref="int.MaxValue"/> for an empty set.</summary>
    // Broiler-AI:           Origin=AI; Spec=UAX #15 section 9 quick check; IP=Low; Security=Medium; Resources=1; Fingerprint=9E5219
    // Broiler-Human:        PENDING
    private int FirstOfQuickCheckSet(int set)
    {
        var index = tables.Table(UnicodeTable.QuickCheckIndex);

        return TableReader.UInt16(index, (set * 4) + 2) == 0
            ? int.MaxValue
            : TableReader.Int24(tables.Table(UnicodeTable.QuickCheckData), TableReader.UInt16(index, set * 4) * 6);
    }

    /// <summary>
    /// The tables one transform reads, fetched from the provider once per call, with the quick-check
    /// sets of its form already sliced out, and the lowest code point each one names.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=1; Fingerprint=13C69A
    // Broiler-Human:        PENDING
    private readonly ref struct FormTables
    {
        private readonly ReadOnlySpan<byte> classes;

        private readonly ReadOnlySpan<byte> canonical;

        private readonly ReadOnlySpan<byte> compatibility;

        private readonly ReadOnlySpan<byte> pool;

        private readonly ReadOnlySpan<byte> compositions;

        private readonly ReadOnlySpan<byte> no;

        private readonly ReadOnlySpan<byte> maybe;

        private readonly int firstClass;

        private readonly int firstDecomposition;

        private readonly int firstQuickCheck;

        private readonly int firstSecond;

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=9B4C9C
        // Broiler-Human:        PENDING
        internal FormTables(UnicodeNormalization owner, bool compatibility)
        {
            var tables = owner.tables;
            var quickCheck = tables.Table(UnicodeTable.QuickCheckData);
            var index = tables.Table(UnicodeTable.QuickCheckIndex);
            var set = compatibility ? 8 : 0;

            classes = tables.Table(UnicodeTable.CombiningClassData);
            canonical = tables.Table(UnicodeTable.CanonicalIndex);
            this.compatibility = compatibility ? tables.Table(UnicodeTable.CompatibilityIndex) : default;
            pool = tables.Table(UnicodeTable.DecompositionPool);
            compositions = tables.Table(UnicodeTable.CompositionData);
            no = quickCheck.Slice(TableReader.UInt16(index, set) * 6, TableReader.UInt16(index, set + 2) * 6);
            maybe = quickCheck.Slice(TableReader.UInt16(index, set + 4) * 6, TableReader.UInt16(index, set + 6) * 6);
            firstClass = owner.firstCombining;
            firstDecomposition = compatibility ? owner.firstCompatibility : owner.firstCanonical;
            firstQuickCheck = compatibility ? owner.firstNfkcQuickCheck : owner.firstNfcQuickCheck;
            firstSecond = owner.firstSecond;
        }

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=1826F8
        // Broiler-Human:        PENDING
        internal int CombiningClass(int codePoint) => codePoint < firstClass ? 0 : ClassOf(classes, codePoint);

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=F31C4D
        // Broiler-Human:        PENDING
        internal bool IsQuickCheckYes(int codePoint) =>
            codePoint < firstQuickCheck || (!TableReader.InRanges(no, codePoint) && !TableReader.InRanges(maybe, codePoint));

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=A05382
        // Broiler-Human:        PENDING
        internal NormalizationQuickCheck QuickCheck(int codePoint)
        {
            if (codePoint < firstQuickCheck)
            {
                return NormalizationQuickCheck.Yes;
            }

            if (TableReader.InRanges(no, codePoint))
            {
                return NormalizationQuickCheck.No;
            }

            return TableReader.InRanges(maybe, codePoint) ? NormalizationQuickCheck.Maybe : NormalizationQuickCheck.Yes;
        }

        // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=0D0169
        // Broiler-Human:        PENDING
        internal bool TryGetDecomposition(int codePoint, out CodePoints mapping)
        {
            var handle = DecompositionHandle(codePoint);

            mapping = handle == 0 ? default : Decomposition(codePoint, handle);
            return handle != 0;
        }

        /// <summary>
        /// A handle on the full decomposition of <paramref name="codePoint"/> in this form: 0 when it has
        /// none, -1 for a Hangul syllable, and otherwise its offset in the pool above its length's eight bits.
        /// </summary>
        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=C2DEEA
        // Broiler-Human:        PENDING
        internal int DecompositionHandle(int codePoint)
        {
            if ((uint)(codePoint - HangulSyllableBase) < HangulSyllableCount)
            {
                return -1;
            }

            if (codePoint < firstDecomposition)
            {
                return 0;
            }

            var handle = compatibility.IsEmpty ? 0 : Lookup(compatibility, codePoint);

            return handle != 0 ? handle : Lookup(canonical, codePoint);
        }

        /// <summary>The decomposition of <paramref name="codePoint"/> its non-zero <paramref name="handle"/> names.</summary>
        // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=BE1117
        // Broiler-Human:        PENDING
        internal CodePoints Decomposition(int codePoint, int handle)
        {
            if (handle > 0)
            {
                return new CodePoints(pool.Slice((handle >> 8) * 3, (handle & 0xFF) * 3));
            }

            var syllable = codePoint - HangulSyllableBase;
            var trailing = syllable % HangulTrailingCount;

            return new CodePoints(
                HangulLeadingBase + (syllable / (HangulVowelCount * HangulTrailingCount)),
                HangulVowelBase + (syllable % (HangulVowelCount * HangulTrailingCount) / HangulTrailingCount),
                HangulTrailingBase + trailing,
                trailing == 0 ? 2 : 3);
        }

        // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=9C1453
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

            composite = 0;

            if (second < firstSecond)
            {
                return false;
            }

            var low = 0;
            var high = (compositions.Length / 9) - 1;

            while (low <= high)
            {
                var middle = (low + high) >>> 1;
                var order = TableReader.Int24(compositions, middle * 9).CompareTo(first);

                if (order == 0)
                {
                    order = TableReader.Int24(compositions, (middle * 9) + 3).CompareTo(second);
                }

                if (order == 0)
                {
                    composite = TableReader.Int24(compositions, (middle * 9) + 6);
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

            return false;
        }

        // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=C48172
        // Broiler-Human:        PENDING
        private static int Lookup(ReadOnlySpan<byte> index, int codePoint)
        {
            var low = 0;
            var high = (index.Length / 6) - 1;

            while (low <= high)
            {
                var middle = (low + high) >>> 1;
                var key = TableReader.Int24(index, middle * 6);

                if (key == codePoint)
                {
                    return (TableReader.UInt16(index, (middle * 6) + 3) << 8) | index[(middle * 6) + 5];
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

            return 0;
        }
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
