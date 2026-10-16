using System;

namespace Broiler.Unicode.Algorithms;

/// <summary>
/// The binary properties of strings ECMA-262's <c>v</c> flag admits (section 22.2.2.9, table "Binary
/// Unicode properties of strings"): each one's single code points and its sequences of two or more,
/// from <c>emoji-sequences.txt</c> and <c>emoji-zwj-sequences.txt</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Names are matched exactly</b>, as the table writes them; <c>\p{rgiemoji}</c> is not admitted.
/// <c>RGI_Emoji</c> is read as the union it is defined as, the other six together, so it has no data of
/// its own.
/// </para>
/// <para>
/// <b>Nothing is allocated.</b> A property's sequences are read in order by
/// <see cref="StringPropertySequences"/>, which writes each into the caller's span; the code points are
/// <see cref="StringProperty.FirstCodePoint"/> and <see cref="StringProperty.LastCodePoint"/>. An instance is immutable and safe to share between
/// threads.
/// </para>
/// </remarks>
public sealed class UnicodeStringProperties
{
    private readonly IUnicodeTables tables;

    private readonly int unionProperty;

    private readonly int shortLimit;

    private readonly int longestName;

    /// <summary>The properties of strings over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.StringPropertyLayout"/> to <see cref="UnicodeTable.StringPropertyNameIndex"/>.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    public UnicodeStringProperties(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);

        TableReader.Require(tables, UnicodeTable.StringPropertyLayout, 3);
        TableReader.Require(tables, UnicodeTable.StringPropertyRanges, 6);
        TableReader.Require(tables, UnicodeTable.SequenceDictionary, 3);
        TableReader.Require(tables, UnicodeTable.SequenceData, 1);
        TableReader.Require(tables, UnicodeTable.StringPropertyData, 12);
        TableReader.Require(tables, UnicodeTable.StringPropertyNames, 1);
        TableReader.Require(tables, UnicodeTable.StringPropertyNameIndex, 2);

        var layout = tables.Table(UnicodeTable.StringPropertyLayout);

        if (layout.Length != 12)
        {
            throw new ArgumentException($"StringPropertyLayout holds {layout.Length / 3} values, not 4.", nameof(tables));
        }

        PropertyCount = TableReader.Layout(layout, 0);
        unionProperty = TableReader.Layout(layout, 1);
        shortLimit = TableReader.Layout(layout, 3);

        if (tables.Table(UnicodeTable.StringPropertyData).Length != PropertyCount * 12 || unionProperty != PropertyCount - 1)
        {
            throw new ArgumentException("StringPropertyLayout does not describe StringPropertyData.", nameof(tables));
        }

        var names = tables.Table(UnicodeTable.StringPropertyNames);
        var index = tables.Table(UnicodeTable.StringPropertyNameIndex);

        for (var at = 0; at < index.Length; at += 2)
        {
            longestName = Math.Max(longestName, names[TableReader.UInt16(index, at)]);
        }
    }

    /// <summary>How many properties of strings there are, in the table's row order; the last is <c>RGI_Emoji</c>.</summary>
    public int PropertyCount { get; }

    /// <summary>The property a lone <c>\p{name}</c> names under <c>v</c>, when it is a property of strings.</summary>
    /// <param name="name">The name, exactly as written.</param>
    /// <param name="property">The property, or the default when the name is not one.</param>
    /// <returns>Whether the name is a property of strings.</returns>
    public bool TryResolve(ReadOnlySpan<char> name, out StringProperty property)
    {
        var id = name.Length > longestName
            ? -1
            : TableReader.Find(tables.Table(UnicodeTable.StringPropertyNames), tables.Table(UnicodeTable.StringPropertyNameIndex), name);

        property = id < 0 ? default : new StringProperty(this, id);
        return id >= 0;
    }

    /// <summary>The property with id <paramref name="id"/>, in the table's row order.</summary>
    /// <param name="id">From zero to <see cref="PropertyCount"/> less one.</param>
    public StringProperty Property(int id) =>
        (uint)id < (uint)PropertyCount ? new StringProperty(this, id) : throw new ArgumentOutOfRangeException(nameof(id));

    internal bool IsUnion(int id) => id == unionProperty;

    /// <summary>Row <paramref name="field"/> of property <paramref name="id"/>: first range, range count, first byte, sequence count.</summary>
    internal int Row(int id, int field) => TableReader.Int24(tables.Table(UnicodeTable.StringPropertyData), (id * 12) + (field * 3));

    internal ReadOnlySpan<byte> Ranges => tables.Table(UnicodeTable.StringPropertyRanges);

    internal ReadOnlySpan<byte> Sequences => tables.Table(UnicodeTable.SequenceData);

    internal int ShortLimit => shortLimit;

    internal int DictionaryEntry(int index) => TableReader.Int24(tables.Table(UnicodeTable.SequenceDictionary), index * 3);
}

/// <summary>One property of strings: its single code points and its sequences.</summary>
public readonly struct StringProperty
{
    private readonly UnicodeStringProperties? owner;

    internal StringProperty(UnicodeStringProperties owner, int id)
    {
        this.owner = owner;
        Id = id;
    }

    /// <summary>The property's id, in the table's row order.</summary>
    public int Id { get; }

    /// <summary>Whether this is <c>RGI_Emoji</c>, the union of the others, which has no data of its own.</summary>
    public bool IsUnion => owner is not null && owner.IsUnion(Id);

    /// <summary>
    /// How many ranges of single code points the property lists; zero for <c>RGI_Emoji</c>, whose
    /// single code points are the other properties'.
    /// </summary>
    public int CodePointRangeCount => owner is null || IsUnion ? 0 : owner.Row(Id, 1);

    /// <summary>How many sequences of two or more code points the property lists; zero for <c>RGI_Emoji</c>.</summary>
    public int SequenceCount => owner is null || IsUnion ? 0 : owner.Row(Id, 3);

    /// <summary>The first code point of single-code-point range <paramref name="index"/>.</summary>
    /// <param name="index">From zero to <see cref="CodePointRangeCount"/> less one.</param>
    public int FirstCodePoint(int index) => TableReader.Int24(owner!.Ranges, Range(index) * 6);

    /// <summary>The last code point of single-code-point range <paramref name="index"/>, inclusive.</summary>
    /// <param name="index">From zero to <see cref="CodePointRangeCount"/> less one.</param>
    public int LastCodePoint(int index) => TableReader.Int24(owner!.Ranges, (Range(index) * 6) + 3);

    /// <summary>Whether <paramref name="codePoint"/> alone is in the property; for <c>RGI_Emoji</c>, in any other.</summary>
    /// <param name="codePoint">A code point.</param>
    public bool ContainsCodePoint(int codePoint)
    {
        if (owner is null)
        {
            return false;
        }

        if (IsUnion)
        {
            for (var id = 0; id < Id; id++)
            {
                if (owner.Property(id).ContainsCodePoint(codePoint))
                {
                    return true;
                }
            }

            return false;
        }

        return TableReader.InRanges(owner.Ranges, owner.Row(Id, 0), owner.Row(Id, 1), codePoint);
    }

    /// <summary>The property's sequences, in code point order, read one at a time into the caller's span.</summary>
    public StringPropertySequences GetSequences() =>
        owner is null || IsUnion ? default : new StringPropertySequences(owner, owner.Row(Id, 2), owner.Row(Id, 3));

    private int Range(int index) =>
        (uint)index < (uint)CodePointRangeCount ? owner!.Row(Id, 0) + index : throw new ArgumentOutOfRangeException(nameof(index));
}

/// <summary>
/// A cursor over one property's sequences: each <see cref="TryRead"/> decodes the next sequence's code
/// points into the caller's span. A value type that allocates nothing; copy it to read again.
/// </summary>
public struct StringPropertySequences
{
    /// <summary>The longest sequence any property lists, in code points; a span this long reads every sequence.</summary>
    public const int MaxSequenceLength = 255;

    private readonly UnicodeStringProperties? owner;

    private int offset;

    private int remaining;

    internal StringPropertySequences(UnicodeStringProperties owner, int offset, int count)
    {
        this.owner = owner;
        this.offset = offset;
        remaining = count;
    }

    /// <summary>How many sequences are still to be read.</summary>
    public readonly int Remaining => remaining;

    /// <summary>
    /// Reads the next sequence's code points into <paramref name="destination"/> and advances, or answers
    /// false when none remains.
    /// </summary>
    /// <param name="destination">A span long enough for the sequence; <see cref="MaxSequenceLength"/> always is.</param>
    /// <param name="length">The sequence's length in code points.</param>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than the sequence; the cursor does not advance.</exception>
    public bool TryRead(Span<int> destination, out int length)
    {
        length = 0;

        if (owner is null || remaining == 0)
        {
            return false;
        }

        var data = owner.Sequences;
        var at = offset;
        var count = data[at++];

        if (destination.Length < count)
        {
            throw new ArgumentException($"The destination holds {destination.Length} code points and the sequence has {count}.", nameof(destination));
        }

        for (var step = 0; step < count; step++)
        {
            int index = data[at++];

            if (index >= owner.ShortLimit)
            {
                index = owner.ShortLimit + (((index - owner.ShortLimit) << 8) | data[at++]);
            }

            destination[step] = owner.DictionaryEntry(index);
        }

        offset = at;
        remaining--;
        length = count;
        return true;
    }
}
