// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.cs
//     blob 9160add90ccb8eef69e6f612a088baa61d238f72: the whole file: JsUnicodeSet and JsUnicodeProperties' lookup (TryResolveLone, TryResolve, Set, RangeFirst, RangeLast, ReadInt24, Find, Compare; :37-259)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's: the
// namespace, the assurance header and annotations included, so that the generated half of each partial class
// compiles against it unchanged.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           2
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// A set of code points from the generated Unicode tables: an immutable view of one set's ranges,
/// which ascend, neither overlap nor touch, and have inclusive bounds.
/// </summary>
/// <remarks>
/// <para>
/// <b>It allocates nothing and holds nothing but two numbers.</b> The ranges live in
/// <see cref="JsUnicodeProperties"/>'s generated data, which the compiler places in the assembly
/// image; a set is where its ranges start and how many there are. The default value is the empty
/// set.
/// </para>
/// <para>
/// This is the shape decision JSD-0031 section 7 gives the matcher's property escapes: a class
/// built from a property is its range list, a complement is the gaps between those ranges, and a
/// membership test is a binary search over at most a few thousand ranges.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9917C2
// Broiler-Human:        PENDING
internal readonly struct JsUnicodeSet
{
    /// <summary>The index in the range data of the set's first range.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6B9075
    // Broiler-Human:        PENDING
    private readonly int first;

    /// <summary>A set over <paramref name="count"/> ranges of the range data, from <paramref name="first"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DFDD2B
    // Broiler-Human:        PENDING
    internal JsUnicodeSet(int first, int count)
    {
        this.first = first;
        RangeCount = count;
    }

    /// <summary>How many ranges the set has; zero for the empty set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1041DC
    // Broiler-Human:        PENDING
    internal int RangeCount { get; }

    /// <summary>The first code point of range <paramref name="index"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D7CC0D
    // Broiler-Human:        PENDING
    internal int First(int index) => JsUnicodeProperties.RangeFirst(Checked(index));

    /// <summary>The last code point of range <paramref name="index"/>, inclusive.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=51A22B
    // Broiler-Human:        PENDING
    internal int Last(int index) => JsUnicodeProperties.RangeLast(Checked(index));

    /// <summary>Whether <paramref name="codePoint"/> is in the set: a binary search, no allocation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B9F991
    // Broiler-Human:        PENDING
    internal bool Contains(int codePoint)
    {
        var low = 0;
        var high = RangeCount - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < JsUnicodeProperties.RangeFirst(first + middle))
            {
                high = middle - 1;
            }
            else if (codePoint > JsUnicodeProperties.RangeLast(first + middle))
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

    /// <summary>The position in the range data of range <paramref name="index"/>, refusing one outside the set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ED18DF
    // Broiler-Human:        PENDING
    private int Checked(int index) =>
        (uint)index < (uint)RangeCount ? first + index : throw new ArgumentOutOfRangeException(nameof(index));
}

/// <summary>
/// The Unicode 17.0.0 properties the matcher's <c>\p{...}</c> and <c>\P{...}</c> resolve to, by the
/// exact names ECMAScript admits.
/// </summary>
/// <remarks>
/// <para>
/// <b>The data is generated; this half is the only part written by hand.</b>
/// <c>JsUnicodeProperties.g.cs</c> holds the ranges and the name tables, written by
/// <c>UnicodeTableGenerator</c> in the architecture test project from the UCD files and the three
/// ECMAScript property tables that <c>src/tests/unicode/pins/unicode.pin</c> names, and rule N22
/// holds it byte for byte to what that generator writes (decision JSD-0031).
/// </para>
/// <para>
/// <b>Names are matched exactly.</b> ES2026 UnicodeMatchProperty and UnicodeMatchPropertyValue
/// forbid loose matching: no case folding, no ignoring of <c>_</c>, <c>-</c> or spaces, no
/// <c>Is</c> prefix, and no name or alias outside the specification's tables and
/// <c>PropertyValueAliases.txt</c>. The comparison is ordinal over ASCII, so a name containing any
/// other character matches nothing.
/// </para>
/// <para>
/// <b>What resolves.</b> The lone form takes a General_Category value, group or alias (<c>L</c>,
/// <c>Letter</c>, <c>LC</c>, <c>Cn</c>, <c>punct</c>) or one of the 53 binary properties or their
/// aliases, <c>Any</c>, <c>ASCII</c> and <c>Assigned</c> among them. The <c>name=value</c> form
/// takes <c>General_Category</c>/<c>gc</c> with a General_Category value, and <c>Script</c>/<c>sc</c>
/// or <c>Script_Extensions</c>/<c>scx</c> with any Script value or alias from
/// <c>PropertyValueAliases.txt</c> - <c>Katakana_Or_Hiragana</c>, which no code point has, included,
/// because the specification admits every value that file lists. Properties of strings and the
/// <c>v</c> flag are not here.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DD9FA1
// Broiler-Human:        PENDING
internal static partial class JsUnicodeProperties
{
    /// <summary>
    /// The set a lone <c>\p{name}</c> selects: a General_Category value or group, or a binary
    /// property, by any name or alias the language admits for it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FA3D51
    // Broiler-Human:        PENDING
    internal static bool TryResolveLone(ReadOnlySpan<char> name, out JsUnicodeSet set)
    {
        var id = Find(LoneNames, LoneNameIndex, name);

        set = id < 0 ? default : Set(id);
        return id >= 0;
    }

    /// <summary>
    /// The set <c>\p{property=value}</c> selects, for the three non-binary properties the language
    /// admits. A binary property is not a <paramref name="property"/> here: <c>\p{ASCII=Y}</c> is a
    /// SyntaxError in the language, and this answers false for it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6F6210
    // Broiler-Human:        PENDING
    internal static bool TryResolve(ReadOnlySpan<char> property, ReadOnlySpan<char> value, out JsUnicodeSet set)
    {
        set = default;

        var id = Find(PropertyNames, PropertyNameIndex, property) switch
        {
            0 => Find(LoneNames, LoneNameIndex, value) is var category && category < GeneralCategorySetCount ? category : -1,
            1 => Find(ScriptNames, ScriptNameIndex, value) is var script && script >= 0 ? ScriptSetBase + script : -1,
            2 => Find(ScriptNames, ScriptNameIndex, value) is var script && script >= 0 ? ScriptExtensionsSetBase + script : -1,
            _ => -1,
        };

        if (id < 0)
        {
            return false;
        }

        set = Set(id);
        return true;
    }

    /// <summary>The set with generated id <paramref name="id"/>; the ids are laid out by the generated constants.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=79D1E9
    // Broiler-Human:        PENDING
    internal static JsUnicodeSet Set(int id) =>
        new(ReadInt24(SetData, id * 6), ReadInt24(SetData, (id * 6) + 3));

    /// <summary>The first code point of the range at <paramref name="rangeIndex"/> in the range data.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DDD289
    // Broiler-Human:        PENDING
    internal static int RangeFirst(int rangeIndex) => ReadInt24(RangeData, rangeIndex * 6);

    /// <summary>The last code point of the range at <paramref name="rangeIndex"/>, inclusive.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E42772
    // Broiler-Human:        PENDING
    internal static int RangeLast(int rangeIndex) => ReadInt24(RangeData, (rangeIndex * 6) + 3);

    /// <summary>A little-endian three-byte value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5C0E6B
    // Broiler-Human:        PENDING
    internal static int ReadInt24(ReadOnlySpan<byte> data, int offset) =>
        data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);

    /// <summary>
    /// The value a generated name table gives <paramref name="name"/>, or -1: a binary search in
    /// ordinal order over entries of a length byte, ASCII characters and a two-byte value.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2A21D1
    // Broiler-Human:        PENDING
    internal static int Find(ReadOnlySpan<byte> names, ReadOnlySpan<byte> index, ReadOnlySpan<char> name)
    {
        var low = 0;
        var high = (index.Length / 2) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var offset = index[middle * 2] | (index[(middle * 2) + 1] << 8);
            var entry = names.Slice(offset + 1, names[offset]);
            var order = Compare(entry, name);

            if (order == 0)
            {
                var value = offset + 1 + entry.Length;
                return names[value] | (names[value + 1] << 8);
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

    /// <summary>Ordinal comparison of an ASCII table entry with a pattern's characters.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B247B3
    // Broiler-Human:        PENDING
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
