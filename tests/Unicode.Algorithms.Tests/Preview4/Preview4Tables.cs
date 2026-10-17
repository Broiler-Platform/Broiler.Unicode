// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The owner's normalization and case mapping as Broiler.Unicode.Algorithms 0.1.0-preview.4 published them,
// copied from src/Unicode.Algorithms at 6e14f93 with only their names and visibility changed. The
// differential tests hold the current implementation to this one: the same result, status and charges.

using System;
using System.Globalization;
using Broiler.Unicode.Algorithms;

namespace Broiler.Unicode.Algorithms.Tests.Preview4;

internal static class Preview4TableReader
{
    /// <summary>A little-endian three-byte value.</summary>
    internal static int Int24(ReadOnlySpan<byte> data, int offset) =>
        data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);

    /// <summary>A little-endian two-byte value.</summary>
    internal static int UInt16(ReadOnlySpan<byte> data, int offset) =>
        data[offset] | (data[offset + 1] << 8);

    /// <summary>Whether <paramref name="codePoint"/> lies in one of the ranges of a range list.</summary>
    internal static bool InRanges(ReadOnlySpan<byte> ranges, int codePoint) =>
        InRanges(ranges, 0, ranges.Length / 6, codePoint);

    /// <summary>
    /// Whether <paramref name="codePoint"/> lies in one of <paramref name="count"/> ranges of a range
    /// list from range <paramref name="first"/>: a binary search, no allocation.
    /// </summary>
    internal static bool InRanges(ReadOnlySpan<byte> ranges, int first, int count, int codePoint)
    {
        var low = first;
        var high = first + count - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < Int24(ranges, middle * 6))
            {
                high = middle - 1;
            }
            else if (codePoint > Int24(ranges, (middle * 6) + 3))
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

    /// <summary>
    /// The value a name table gives <paramref name="name"/>, or -1: a binary search in ordinal order
    /// over entries of a length byte, ASCII characters and a two-byte value. The comparison is ordinal,
    /// so a name containing a character outside ASCII matches nothing.
    /// </summary>
    internal static int Find(ReadOnlySpan<byte> names, ReadOnlySpan<byte> index, ReadOnlySpan<char> name)
    {
        var low = 0;
        var high = (index.Length / 2) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var offset = UInt16(index, middle * 2);
            var entry = names.Slice(offset + 1, names[offset]);
            var order = Compare(entry, name);

            if (order == 0)
            {
                return UInt16(names, offset + 1 + entry.Length);
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

        return -1;
    }

    /// <summary>The <paramref name="position"/>th three-byte value of a layout table.</summary>
    internal static int Layout(ReadOnlySpan<byte> layout, int position) => Int24(layout, position * 3);

    /// <summary>A required table, refused when the provider does not carry it.</summary>
    internal static void Require(IUnicodeTables tables, UnicodeTable table, int recordSize)
    {
        var data = tables.Table(table);

        if (data.IsEmpty)
        {
            throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"The tables do not carry {table}, which this algorithm reads."), nameof(tables));
        }

        if (data.Length % recordSize != 0)
        {
            throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"{table} is {data.Length} bytes, not a whole number of {recordSize}-byte records."), nameof(tables));
        }
    }

    /// <summary>The provider, refused when its version is not the one the algorithms are written for.</summary>
    internal static IUnicodeTables Checked(IUnicodeTables tables)
    {
        ArgumentNullException.ThrowIfNull(tables);

        if (!string.Equals(tables.UnicodeVersion, UnicodeAlgorithms.UnicodeVersion, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                string.Create(NumberFormatInfo.InvariantInfo, $"The tables are Unicode {tables.UnicodeVersion}, and these algorithms read {UnicodeAlgorithms.UnicodeVersion}."),
                nameof(tables));
        }

        return tables;
    }

    /// <summary>Ordinal comparison of an ASCII table entry with a name's characters.</summary>
    private static int Compare(ReadOnlySpan<byte> entry, ReadOnlySpan<char> name)
    {
        var shared = Math.Min(entry.Length, name.Length);

        for (var index = 0; index < shared; index++)
        {
            if (entry[index] != name[index])
            {
                return entry[index] < name[index] ? -1 : 1;
            }
        }

        return entry.Length.CompareTo(name.Length);
    }
}

/// <summary>Reading UTF-16 as ECMA-262's <c>CodePointAt</c> does: a lone surrogate is a code point of its own.</summary>
internal static class Preview4Utf16Reader
{
    /// <summary>The code point starting at <paramref name="start"/>, and its width in code units.</summary>
    internal static int Next(ReadOnlySpan<char> text, int start, out int width)
    {
        var first = text[start];

        if (char.IsHighSurrogate(first) && start + 1 < text.Length && char.IsLowSurrogate(text[start + 1]))
        {
            width = 2;
            return char.ConvertToUtf32(first, text[start + 1]);
        }

        width = 1;
        return first;
    }

    /// <summary>The code point ending just before <paramref name="end"/>, and its width in code units.</summary>
    internal static int Previous(ReadOnlySpan<char> text, int end, out int width)
    {
        var last = text[end - 1];

        if (char.IsLowSurrogate(last) && end >= 2 && char.IsHighSurrogate(text[end - 2]))
        {
            width = 2;
            return char.ConvertToUtf32(text[end - 2], last);
        }

        width = 1;
        return last;
    }

    /// <summary>The number of UTF-16 code units <paramref name="codePoint"/> takes.</summary>
    internal static int Width(int codePoint) => codePoint > 0xFFFF ? 2 : 1;
}
