// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           3
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  2/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Globalization;

namespace Broiler.Unicode.Algorithms;

/// <summary>
/// The two mappings ECMA-262's Canonicalize reads (section 22.2.2.7.3, "Canonicalize ( rer, ch )"):
/// simple case folding under <c>u</c> or <c>v</c> with <c>i</c>, and the upper-casing of one code unit
/// without them, each with its orbit, the reverse relation a case-insensitive character class needs.
/// </summary>
/// <remarks>
/// <para>
/// <b>Simple folding</b> is <c>CaseFolding.txt</c>'s status C and S mappings over the whole code space;
/// status F and T are not read, so U+0130 and U+0131 fold to themselves. It is idempotent: nothing a code
/// point folds to folds again.
/// </para>
/// <para>
/// <b>The non-<c>u</c> mapping</b> upper-cases one code unit by the full Default Case Conversion and
/// keeps the code unit when the result is not exactly one code unit, or when it would map a code unit
/// outside ASCII into ASCII; a surrogate code unit maps to itself. The generator applies those rules, so
/// the 27 Greek letters with a ypogegrammeni, whose full upper case is two code points, map to
/// themselves.
/// </para>
/// <para>
/// <b>An orbit is written into the caller's span</b>, in ascending order, so nothing is shared and
/// nothing is allocated: the arrays Broiler.Regex's folding built lazily under a lock are not needed.
/// An instance is immutable and safe to share between threads, and takes no meter: each answer is a
/// bounded binary search.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Low; Resources=1; Fingerprint=37F812
// Broiler-Human:        PENDING
public sealed class UnicodeCaseFolding
{
    private readonly IUnicodeTables tables;

    /// <summary>Case folding over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.FoldingLayout"/> to <see cref="UnicodeTable.UpperOrbitIndex"/>.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=C578E6
    // Broiler-Human:        PENDING
    public UnicodeCaseFolding(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);

        TableReader.Require(tables, UnicodeTable.FoldingLayout, 3);
        TableReader.Require(tables, UnicodeTable.FoldData, 6);
        TableReader.Require(tables, UnicodeTable.OrbitIndex, 2);
        TableReader.Require(tables, UnicodeTable.UpperData, 4);
        TableReader.Require(tables, UnicodeTable.UpperOrbitIndex, 2);

        var layout = tables.Table(UnicodeTable.FoldingLayout);

        if (layout.Length != 6)
        {
            throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"FoldingLayout holds {layout.Length / 3} values, not 2."), nameof(tables));
        }

        MaxOrbit = TableReader.Layout(layout, 0);
        MaxUpperOrbit = TableReader.Layout(layout, 1);
    }

    /// <summary>The most code points that share one simple folding, the folding itself included: the span <see cref="Orbit"/> needs.</summary>
    public int MaxOrbit { get; }

    /// <summary>The most code units that share one non-<c>u</c> canonical form, the form itself included: the span <see cref="UpperOrbit"/> needs.</summary>
    public int MaxUpperOrbit { get; }

    /// <summary>
    /// The simple case folding of <paramref name="codePoint"/> (<c>CaseFolding.txt</c> status C and S),
    /// or the code point itself: Canonicalize under <c>u</c> or <c>v</c> with <c>i</c>.
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=F88260
    // Broiler-Human:        PENDING
    public int SimpleFold(int codePoint)
    {
        var data = tables.Table(UnicodeTable.FoldData);
        var entry = Search(data, codePoint);

        return entry < 0 ? codePoint : TableReader.Int24(data, (entry * 6) + 3);
    }

    /// <summary>
    /// Writes every code point whose simple folding equals that of <paramref name="codePoint"/>, the
    /// folding itself and <paramref name="codePoint"/> included, into <paramref name="destination"/> in
    /// ascending order, and answers how many there are.
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    /// <param name="destination">At least <see cref="MaxOrbit"/> entries.</param>
    /// <returns>The number written; one for a code point no folding touches.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than <see cref="MaxOrbit"/>.</exception>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=High; Resources=2; Fingerprint=49FFB8
    // Broiler-Falsified-If: more code points are written than the destination holds, or a code point of another orbit is written
    // Broiler-Human:        PENDING
    public int Orbit(int codePoint, Span<int> destination)
    {
        if (destination.Length < MaxOrbit)
        {
            throw new ArgumentException("The destination is shorter than MaxOrbit.", nameof(destination));
        }

        var data = tables.Table(UnicodeTable.FoldData);
        var index = tables.Table(UnicodeTable.OrbitIndex);
        var target = SimpleFold(codePoint);
        var count = 0;

        destination[count++] = target;

        // The orbit index is ordered by folding, so the code points sharing this one are contiguous:
        // find the first, then read forward while the folding still matches.
        var low = 0;
        var high = (index.Length / 2) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (OrbitTarget(data, index, middle) < target)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        for (var position = low; position < index.Length / 2 && OrbitTarget(data, index, position) == target; position++)
        {
            destination[count++] = TableReader.Int24(data, TableReader.UInt16(index, position * 2) * 6);
        }

        Sort(destination[..count]);
        return count;
    }

    /// <summary>
    /// ECMA-262's Canonicalize of one code unit without <c>u</c> or <c>v</c>: its upper case by the
    /// Default Case Conversion when that is one code unit and does not move a code unit outside ASCII
    /// into it, or the code unit itself. Anything above U+FFFF answers itself.
    /// </summary>
    /// <param name="codeUnit">A UTF-16 code unit, or any code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=740184
    // Broiler-Human:        PENDING
    public int SimpleUpper(int codeUnit)
    {
        var data = tables.Table(UnicodeTable.UpperData);
        var entry = UpperSearch(data, codeUnit);

        return entry < 0 ? codeUnit : TableReader.UInt16(data, (entry * 4) + 2);
    }

    /// <summary>
    /// Writes every code unit whose non-<c>u</c> canonical form equals that of
    /// <paramref name="codeUnit"/>, the form itself and <paramref name="codeUnit"/> included, into
    /// <paramref name="destination"/> in ascending order, and answers how many there are.
    /// </summary>
    /// <param name="codeUnit">A UTF-16 code unit.</param>
    /// <param name="destination">At least <see cref="MaxUpperOrbit"/> entries.</param>
    /// <returns>The number written.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than <see cref="MaxUpperOrbit"/>.</exception>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=High; Resources=2; Fingerprint=EF9D2E
    // Broiler-Falsified-If: more code units are written than the destination holds, or a code unit of another orbit is written
    // Broiler-Human:        PENDING
    public int UpperOrbit(int codeUnit, Span<int> destination)
    {
        if (destination.Length < MaxUpperOrbit)
        {
            throw new ArgumentException("The destination is shorter than MaxUpperOrbit.", nameof(destination));
        }

        var data = tables.Table(UnicodeTable.UpperData);
        var index = tables.Table(UnicodeTable.UpperOrbitIndex);
        var target = SimpleUpper(codeUnit);
        var count = 0;
        var entries = index.Length / 2;

        destination[count++] = target;

        var low = 0;
        var high = entries - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (TableReader.UInt16(data, (TableReader.UInt16(index, middle * 2) * 4) + 2) < target)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        for (var position = low; position < entries; position++)
        {
            var entry = TableReader.UInt16(index, position * 2);

            if (TableReader.UInt16(data, (entry * 4) + 2) != target)
            {
                break;
            }

            destination[count++] = TableReader.UInt16(data, entry * 4);
        }

        Sort(destination[..count]);
        return count;
    }

    /// <summary>An insertion sort: an orbit holds a handful of entries.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=D3DFBC
    // Broiler-Human:        PENDING
    private static void Sort(Span<int> values)
    {
        for (var sorted = 1; sorted < values.Length; sorted++)
        {
            var value = values[sorted];
            var slot = sorted - 1;

            while (slot >= 0 && values[slot] > value)
            {
                values[slot + 1] = values[slot];
                slot--;
            }

            values[slot + 1] = value;
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=9175A1
    // Broiler-Human:        PENDING
    private static int Search(ReadOnlySpan<byte> data, int codePoint)
    {
        var low = 0;
        var high = (data.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var source = TableReader.Int24(data, middle * 6);

            if (source == codePoint)
            {
                return middle;
            }

            if (source < codePoint)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=02117D
    // Broiler-Human:        PENDING
    private static int UpperSearch(ReadOnlySpan<byte> data, int codeUnit)
    {
        var low = 0;
        var high = (data.Length / 4) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var source = TableReader.UInt16(data, middle * 4);

            if (source == codeUnit)
            {
                return middle;
            }

            if (source < codeUnit)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.7.3 via Broiler.VM JsUnicodeCaseFolding; IP=Low; Security=Medium; Resources=1; Fingerprint=B3D123
    // Broiler-Human:        PENDING
    private static int OrbitTarget(ReadOnlySpan<byte> data, ReadOnlySpan<byte> index, int position) =>
        TableReader.Int24(data, (TableReader.UInt16(index, position * 2) * 6) + 3);
}
