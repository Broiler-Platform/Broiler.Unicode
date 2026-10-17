// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   46
// Annotated:        46/46
// Exempt:           20
// Human-reviewed:   0/46
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       46
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

    // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=0; Fingerprint=0C8644
    // Broiler-Human:        PENDING
    private const int RecordSize = 11;

    private readonly IUnicodeTables tables;

    /// <summary>Normalization over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.NormalizationLayout"/>, <see cref="UnicodeTable.DecompositionPool"/>,
    /// <see cref="UnicodeTable.NormalizationTrie"/>, <see cref="UnicodeTable.NormalizationRecords"/> and
    /// <see cref="UnicodeTable.CompositionLists"/>.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version, lacks a table, or holds a malformed one.</exception>
    /// <remarks>
    /// Every per-code-point answer is one trie lookup and one record read; the binary-searched tables of
    /// 0.1.0-preview.4 are no longer read.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=D1A19C
    // Broiler-Human:        PENDING
    public UnicodeNormalization(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);

        TableReader.Require(tables, UnicodeTable.NormalizationLayout, 3);
        TableReader.Require(tables, UnicodeTable.DecompositionPool, 3);
        TableReader.Require(tables, UnicodeTable.CompositionLists, 6);
        TableReader.RequireTrie(tables, UnicodeTable.NormalizationTrie);
        TableReader.RequireRecords(tables, UnicodeTable.NormalizationTrie, UnicodeTable.NormalizationRecords, RecordSize);

        var layout = tables.Table(UnicodeTable.NormalizationLayout);

        if (layout.Length != 3)
        {
            throw new ArgumentException("NormalizationLayout does not have its stated length.", nameof(tables));
        }

        MaxDecompositionLength = TableReader.Layout(layout, 0);
    }

    /// <summary>The longest full decomposition, in code points; a Hangul syllable's is at most three.</summary>
    public int MaxDecompositionLength { get; }

    /// <summary>The Canonical_Combining_Class of <paramref name="codePoint"/> (<c>UnicodeData.txt</c>); 0 for a starter.</summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=240149
    // Broiler-Human:        PENDING
    public int CombiningClass(int codePoint) =>
        (uint)codePoint > UnicodeAlgorithms.MaxCodePoint ? 0 : new FormTables(this, compatibility: false).CombiningClass(codePoint);

    /// <summary>
    /// <c>NFC_QC</c> of <paramref name="codePoint"/>, or with <paramref name="compatibility"/>
    /// <c>NFKC_QC</c> (<c>DerivedNormalizationProps.txt</c>).
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    /// <param name="compatibility">Whether to answer <c>NFKC_QC</c>.</param>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=BFB6A4
    // Broiler-Human:        PENDING
    public NormalizationQuickCheck QuickCheck(int codePoint, bool compatibility) =>
        (uint)codePoint > UnicodeAlgorithms.MaxCodePoint ? NormalizationQuickCheck.Yes : new FormTables(this, compatibility).QuickCheck(codePoint);

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
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=High; Resources=5; Fingerprint=B7CECC
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
        Span<int> handles = text.Length - start <= StackLimit ? stackalloc int[text.Length - start] : default;
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

    /// <summary>
    /// The quick check from <paramref name="from"/>, where a code point outside ASCII follows ASCII or
    /// the start: the position of the first code point that keeps <paramref name="text"/> from being
    /// certainly in the form, or -1 when it is, so it can be returned unchanged. A code point
    /// breaks it when it is out of canonical order, when it decomposes (the decomposed forms), or when
    /// its quick-check value is not Yes (the composed forms).
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=8D909E
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
            var record = lookup.Record(codePoint);
            var combiningClass = lookup.ClassAt(record);

            if ((combiningClass != 0 && lastClass > combiningClass) ||
                (compose ? !lookup.IsQuickCheckYesAt(record) : lookup.DecompositionHandle(codePoint, record) != 0))
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
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=High; Resources=5; Fingerprint=61AC48
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

        Span<char> output = capacity <= StackLimit ? stackalloc char[capacity] : new char[capacity];
        Span<int> buffer = stackalloc int[16];
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
                    // One record read for a code point that does not decompose, two for one that does.
                    var record = lookup.Record(codePoint);
                    var handle = handles.IsEmpty ? lookup.DecompositionHandle(codePoint, record) : handles[index];
                    decomposes = handle != 0;
                    mapping = decomposes ? lookup.Decomposition(codePoint, handle) : default;
                    var firstRecord = decomposes ? lookup.Record(mapping[0]) : record;
                    firstClass = lookup.ClassAt(firstRecord);
                    boundary = firstClass == 0 && (!compose || (lookup.IsQuickCheckYesAt(record) && lookup.IsQuickCheckYesAt(firstRecord)));
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
    /// One run of non-starters sorted by combining class, stably: by insertion when it is short, and
    /// otherwise by a counting sort, so a long run is linear work rather than an exchange sort's quadratic.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=0FFBF2
    // Broiler-Human:        PENDING
    private static void SortRun(Span<int> run)
    {
        // A short run, the common case, is sorted in place by insertion, which is stable too.
        if (run.Length <= 8)
        {
            for (var at = 1; at < run.Length; at++)
            {
                var entry = run[at];
                var position = at;

                while (position > 0 && (run[position - 1] >>> 24) > (entry >>> 24))
                {
                    run[position] = run[position - 1];
                    position--;
                }

                run[position] = entry;
            }

            return;
        }

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

    /// <summary>
    /// The tables one transform reads, fetched from the provider once per call. Each code point is one
    /// trie lookup to its record, whose bytes hold its class, quick checks, decompositions and compositions.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Low; Resources=1; Fingerprint=13C69A
    // Broiler-Human:        PENDING
    private readonly ref struct FormTables
    {
        private readonly ReadOnlySpan<ushort> trie;

        private readonly ReadOnlySpan<byte> records;

        private readonly ReadOnlySpan<byte> pool;

        private readonly ReadOnlySpan<byte> compositions;

        private readonly bool compatibility;

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=515EBE
        // Broiler-Human:        PENDING
        internal FormTables(UnicodeNormalization owner, bool compatibility)
        {
            var tables = owner.tables;

            trie = TableReader.Words(tables.Table(UnicodeTable.NormalizationTrie));
            records = tables.Table(UnicodeTable.NormalizationRecords);
            pool = tables.Table(UnicodeTable.DecompositionPool);
            compositions = tables.Table(UnicodeTable.CompositionLists);
            this.compatibility = compatibility;
        }

        /// <summary>The offset of <paramref name="codePoint"/>'s record; 0, the all-zero record, for most.</summary>
        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=410F6F
        // Broiler-Human:        PENDING
        internal int Record(int codePoint) => TableReader.Trie(trie, codePoint) * RecordSize;

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=E48D4D
        // Broiler-Human:        PENDING
        internal int CombiningClass(int codePoint) => records[Record(codePoint)];

        /// <summary>The combining class a record holds.</summary>
        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=02B0FB
        // Broiler-Human:        PENDING
        internal int ClassAt(int record) => records[record];

        /// <summary>Whether a record's quick check in this form is Yes.</summary>
        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=8C09C3
        // Broiler-Human:        PENDING
        internal bool IsQuickCheckYesAt(int record) => QuickCheckBits(record) == 0;

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=0BA0EB
        // Broiler-Human:        PENDING
        internal bool IsQuickCheckYes(int codePoint) => QuickCheckBits(Record(codePoint)) == 0;

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=6A300E
        // Broiler-Human:        PENDING
        internal NormalizationQuickCheck QuickCheck(int codePoint) => (NormalizationQuickCheck)QuickCheckBits(Record(codePoint));

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
        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=DEE8AE
        // Broiler-Human:        PENDING
        internal int DecompositionHandle(int codePoint) => DecompositionHandle(codePoint, Record(codePoint));

        /// <summary><see cref="DecompositionHandle(int)"/> from the code point's record, already read.</summary>
        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=A19237
        // Broiler-Human:        PENDING
        internal int DecompositionHandle(int codePoint, int record)
        {
            if ((uint)(codePoint - HangulSyllableBase) < HangulSyllableCount)
            {
                return -1;
            }

            if (compatibility && records[record + 7] != 0)
            {
                return (TableReader.UInt16(records, record + 5) << 8) | records[record + 7];
            }

            return records[record + 4] == 0 ? 0 : (TableReader.UInt16(records, record + 2) << 8) | records[record + 4];
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

        /// <summary>
        /// The primary composite of <paramref name="first"/> followed by <paramref name="second"/>: the
        /// Hangul arithmetic, or a scan of the first code point's own list of compositions.
        /// </summary>
        // Broiler-AI:           Origin=Ported; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=48F5B6
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

            var record = Record(first);
            var start = TableReader.UInt16(records, record + 8) * 6;
            var end = start + (records[record + 10] * 6);

            for (var at = start; at < end; at += 6)
            {
                if (TableReader.Int24(compositions, at) == second)
                {
                    composite = TableReader.Int24(compositions, at + 3);
                    return true;
                }
            }

            composite = 0;
            return false;
        }

        // Broiler-AI:           Origin=AI; Spec=UAX #15 via Broiler.VM JsRealm.String and JsUnicodeNormalization; IP=Low; Security=Medium; Resources=1; Fingerprint=9F4887
        // Broiler-Human:        PENDING
        private int QuickCheckBits(int record) => (records[record + 1] >> (compatibility ? 2 : 0)) & 3;
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
