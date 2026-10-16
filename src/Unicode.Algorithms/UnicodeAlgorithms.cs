using System;

namespace Broiler.Unicode.Algorithms;

/// <summary>The data version this package's algorithms are written for.</summary>
public static class UnicodeAlgorithms
{
    /// <summary>
    /// The Unicode version of the tables every algorithm here reads: <c>17.0.0</c>, the version the
    /// shared JavaScript libraries programme pins (Broiler.VM's JSD-0074, decision SJL-D-6 (c)). An
    /// <see cref="IUnicodeTables"/> of another version is refused.
    /// </summary>
    public const string UnicodeVersion = "17.0.0";

    /// <summary>The highest code point, U+10FFFF.</summary>
    public const int MaxCodePoint = 0x10FFFF;
}

/// <summary>
/// The caller's meter: charges <paramref name="units"/> of work before the work it charges for, and may
/// throw to stop it.
/// </summary>
/// <param name="units">The units charged; each operation documents its units.</param>
/// <remarks>
/// The shape of the shared JavaScript libraries' <c>WorkCharge</c> (convention 4), defined here because
/// the shared libraries may depend on this package and never the reverse. An engine passes the same
/// meter to both. When the meter throws, the operation stops and leaves no shared state changed.
/// </remarks>
public delegate void UnicodeWorkCharge(ulong units);

/// <summary>How an operation that can refuse its input ended (convention 3: failure is data).</summary>
public enum UnicodeStatus
{
    /// <summary>The operation completed and its result is set.</summary>
    Success = 0,

    /// <summary>
    /// The result would be longer than the caller's maximum result length (convention 5). No partial
    /// result is returned.
    /// </summary>
    LimitExceeded = 1,
}

/// <summary>Reading the tables' little-endian numbers and range lists.</summary>
internal static class TableReader
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
            throw new ArgumentException($"The tables do not carry {table}, which this algorithm reads.", nameof(tables));
        }

        if (data.Length % recordSize != 0)
        {
            throw new ArgumentException($"{table} is {data.Length} bytes, not a whole number of {recordSize}-byte records.", nameof(tables));
        }
    }

    /// <summary>The provider, refused when its version is not the one the algorithms are written for.</summary>
    internal static IUnicodeTables Checked(IUnicodeTables tables)
    {
        ArgumentNullException.ThrowIfNull(tables);

        if (!string.Equals(tables.UnicodeVersion, UnicodeAlgorithms.UnicodeVersion, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"The tables are Unicode {tables.UnicodeVersion}, and these algorithms read {UnicodeAlgorithms.UnicodeVersion}.",
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
internal static class Utf16Reader
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
