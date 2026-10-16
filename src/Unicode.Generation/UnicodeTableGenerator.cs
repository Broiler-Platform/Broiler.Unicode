using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Broiler.Unicode.Algorithms;

namespace Broiler.Unicode.Generation;

/// <summary>
/// The generator of every table <c>Broiler.Unicode.Algorithms</c> and <c>Broiler.Unicode.Segmentation</c>
/// read, from a verified Unicode archive: one function, pure and deterministic.
/// </summary>
/// <remarks>
/// <para>
/// <b>PORTED from Broiler.VM</b> (decision SJL-D-6 (c), the owner holds the generator): the tables are
/// the bytes Broiler.VM's <c>UnicodeTableGenerator</c> writes into its <c>JsUnicodeProperties</c>,
/// <c>JsUnicodeCaseFolding</c>, <c>JsUnicodeStringProperties</c>, <c>JsUnicodeNormalization</c> and
/// <c>JsUnicodeCasing</c>, and its <c>CldrTableGenerator</c> writes into <c>SegmentBreaks</c>, at
/// Broiler.VM <c>7b44dc7</c>, in the same layouts, so an engine that embeds them keeps its size budget.
/// Added to them: the five layout tables, which carry the constants Broiler.VM wrote as C# constants, and
/// <see cref="UnicodeTable.SoftDottedRanges"/>, which Broiler.VM wrote as text into its Intl data.
/// </para>
/// <para>
/// <b>Its inputs are the pin's files and nothing else</b>, after <see cref="UnicodePin.Verified"/>
/// compared their lengths, hashes and version headers with the pin. <b>Its output is deterministic</b>:
/// every table is sorted by code point or by ordinal name, and nothing records a time, a machine or a
/// path. A misread file is a failure, not a quietly wrong table: the database cross-checks every
/// property it reads from two places.
/// </para>
/// </remarks>
public static class UnicodeTableGenerator
{
    /// <summary>The first sequence byte that opens a two-byte dictionary index.</summary>
    private const int ShortLimit = 0xF0;

    /// <summary>
    /// Every table, from the archive <paramref name="pin"/> describes. Throws when the archive is not the
    /// pinned one or the data disagrees with itself; nothing is generated from input that failed a check.
    /// </summary>
    /// <param name="pin">The pin.</param>
    /// <param name="archive">Each pinned file's bytes, by the name the pin writes it under.</param>
    /// <returns>Each table's bytes, in <see cref="UnicodeTable"/> order.</returns>
    /// <exception cref="InvalidDataException">The archive is not the pinned one, or a cross-check fails.</exception>
    public static IReadOnlyList<KeyValuePair<UnicodeTable, byte[]>> Generate(UnicodePin pin, IReadOnlyDictionary<string, byte[]> archive)
    {
        ArgumentNullException.ThrowIfNull(pin);

        var verified = pin.Verified(archive);
        var database = UnicodeDatabase.Read(pin, verified);
        var tables = new SortedDictionary<UnicodeTable, byte[]>();

        Properties(database, tables);
        CaseFolding(database, tables);
        StringProperties(database, tables);
        Normalization(database, tables);
        Casing(database, tables);
        tables[UnicodeTable.SegmentBreaks] = SegmentBreaks(verified);

        return tables.ToList();
    }

    private static void Properties(UnicodeDatabase database, SortedDictionary<UnicodeTable, byte[]> tables)
    {
        var sets = new List<List<UnicodeRange>>();

        sets.AddRange(database.GeneralCategories.Select(static value => value.Ranges));

        var binaryBase = sets.Count;

        sets.AddRange(database.BinaryProperties.Select(static value => value.Ranges));

        var scriptBase = sets.Count;

        sets.AddRange(database.Scripts.Select(static value => value.Ranges));

        var extensionsBase = sets.Count;

        sets.AddRange(database.ScriptExtensions);

        var ranges = new ByteWriter();
        var index = new ByteWriter();
        var rangeCount = 0;

        foreach (var set in sets)
        {
            index.Int24(rangeCount).Int24(set.Count);

            foreach (var range in set)
            {
                ranges.Int24(range.First).Int24(range.Last);
                rangeCount++;
            }
        }

        var lone = database.GeneralCategories.SelectMany(static (value, id) => value.Names.Select(name => (name, id)))
            .Concat(database.BinaryProperties.SelectMany((value, id) => value.Names.Select(name => (name, id: binaryBase + id))));
        var (loneNames, loneIndex) = NameTable(lone);
        var (propertyNames, propertyIndex) = NameTable(database.NonBinaryNames.Select(static name => (name.Name, name.Kind)));
        var (scriptNames, scriptIndex) = NameTable(database.Scripts.SelectMany(static (value, id) => value.Names.Select(name => (name, id))));

        var layout = new ByteWriter()
            .Int24(database.GeneralCategories.Count)
            .Int24(database.GeneralCategoryValueCount)
            .Int24(binaryBase)
            .Int24(scriptBase)
            .Int24(extensionsBase)
            .Int24(database.Scripts.Count)
            .Int24(BinarySet(database, binaryBase, "ID_Start"))
            .Int24(BinarySet(database, binaryBase, "ID_Continue"))
            .Int24(database.GeneralCategories.FindIndex(static value => value.ShortName == "Zs"))
            .Int24(BinarySet(database, binaryBase, "White_Space"))
            .Int24(BinarySet(database, binaryBase, "Soft_Dotted"));

        tables[UnicodeTable.PropertyLayout] = layout.ToArray();
        tables[UnicodeTable.PropertyRanges] = ranges.ToArray();
        tables[UnicodeTable.PropertySets] = index.ToArray();
        tables[UnicodeTable.PropertyLoneNames] = loneNames.ToArray();
        tables[UnicodeTable.PropertyLoneNameIndex] = loneIndex.ToArray();
        tables[UnicodeTable.PropertyNames] = propertyNames.ToArray();
        tables[UnicodeTable.PropertyNameIndex] = propertyIndex.ToArray();
        tables[UnicodeTable.ScriptNames] = scriptNames.ToArray();
        tables[UnicodeTable.ScriptNameIndex] = scriptIndex.ToArray();
    }

    /// <summary>
    /// The seven properties of strings the <c>v</c> flag admits: each one's single code points as ranges
    /// and its sequences, spelt through a dictionary of code points, most frequent first, so that almost
    /// every code point of a sequence is one byte.
    /// </summary>
    private static void StringProperties(UnicodeDatabase database, SortedDictionary<UnicodeTable, byte[]> tables)
    {
        var ranges = new ByteWriter();
        var sequences = new ByteWriter();
        var index = new ByteWriter();
        var dictionary = new ByteWriter();
        var rangeCount = 0;
        var sequenceCount = 0;

        var frequency = database.StringProperties
            .SelectMany(static property => property.Sequences)
            .SelectMany(static sequence => sequence)
            .GroupBy(static codePoint => codePoint)
            .OrderByDescending(static group => group.Count())
            .ThenBy(static group => group.Key)
            .Select(static group => group.Key)
            .ToArray();
        var position = frequency.Select(static (codePoint, at) => (codePoint, at)).ToDictionary(static pair => pair.codePoint, static pair => pair.at);

        if (frequency.Length > (0x100 - ShortLimit) * 0x100)
        {
            throw new InvalidDataException($"{frequency.Length} distinct code points do not fit the sequence dictionary's two-byte indices");
        }

        foreach (var codePoint in frequency)
        {
            dictionary.Int24(codePoint);
        }

        foreach (var (_, codePoints, list) in database.StringProperties)
        {
            index.Int24(rangeCount).Int24(codePoints.Count).Int24(sequences.Count).Int24(list.Count);

            foreach (var range in codePoints)
            {
                ranges.Int24(range.First).Int24(range.Last);
                rangeCount++;
            }

            foreach (var sequence in list)
            {
                sequences.Byte(sequence.Length);

                foreach (var codePoint in sequence)
                {
                    var at = position[codePoint];

                    if (at < ShortLimit)
                    {
                        sequences.Byte(at);
                    }
                    else
                    {
                        sequences.Byte(ShortLimit + ((at - ShortLimit) >> 8)).Byte((at - ShortLimit) & 0xFF);
                    }
                }

                sequenceCount++;
            }
        }

        var (names, nameIndex) = NameTable(database.StringProperties.Select(static (property, id) => (property.Name, id)));

        tables[UnicodeTable.StringPropertyLayout] = new ByteWriter()
            .Int24(database.StringProperties.Count)
            .Int24(database.StringProperties.Count - 1)
            .Int24(sequenceCount)
            .Int24(ShortLimit)
            .ToArray();
        tables[UnicodeTable.StringPropertyRanges] = ranges.ToArray();
        tables[UnicodeTable.SequenceDictionary] = dictionary.ToArray();
        tables[UnicodeTable.SequenceData] = sequences.ToArray();
        tables[UnicodeTable.StringPropertyData] = index.ToArray();
        tables[UnicodeTable.StringPropertyNames] = names.ToArray();
        tables[UnicodeTable.StringPropertyNameIndex] = nameIndex.ToArray();
    }

    /// <summary>The set id of a binary property by canonical name; a name the table lacks is a stop.</summary>
    private static int BinarySet(UnicodeDatabase database, int binaryBase, string name)
    {
        var index = database.BinaryProperties.FindIndex(value => value.ShortName == name);

        return index < 0 ? throw new InvalidDataException($"the binary property table has no `{name}`") : binaryBase + index;
    }

    /// <summary>
    /// The non-<c>u</c> Canonicalize of every code unit it moves: the full upper-case mapping, kept only
    /// where it is one code unit and does not move a code unit outside ASCII into it.
    /// </summary>
    private static SortedDictionary<int, int> NonUnicodeCanonical(UnicodeDatabase database)
    {
        var canonical = new SortedDictionary<int, int>();

        foreach (var (source, mapping) in database.FullUppercase)
        {
            if (mapping.Length != 1)
            {
                continue;
            }

            var upper = mapping[0];

            if (source <= 0xFFFF && source is < 0xD800 or > 0xDFFF && upper <= 0xFFFF && !(source >= 128 && upper < 128))
            {
                canonical.Add(source, upper);
            }
        }

        // An orbit is closed by the reverse relation, which assumes one step reaches the canonical form.
        foreach (var target in canonical.Values.Where(canonical.ContainsKey))
        {
            throw new InvalidDataException($"U+{target:X4} is a non-u canonical form and is moved again");
        }

        return canonical;
    }

    private static void CaseFolding(UnicodeDatabase database, SortedDictionary<UnicodeTable, byte[]> tables)
    {
        var folds = database.SimpleFolding.ToArray();
        var data = new ByteWriter();

        foreach (var (source, target) in folds)
        {
            data.Int24(source).Int24(target);
        }

        var orbit = new ByteWriter();

        foreach (var entry in folds.Select(static (fold, index) => (fold.Value, fold.Key, index)).Order())
        {
            orbit.UInt16(entry.index);
        }

        var maxOrbit = folds.GroupBy(static fold => fold.Value).Max(static group => group.Count() + 1);

        var uppers = NonUnicodeCanonical(database).ToArray();
        var upperData = new ByteWriter();

        foreach (var (source, target) in uppers)
        {
            upperData.UInt16(source).UInt16(target);
        }

        var upperOrbit = new ByteWriter();

        foreach (var entry in uppers.Select(static (upper, index) => (upper.Value, upper.Key, index)).Order())
        {
            upperOrbit.UInt16(entry.index);
        }

        var maxUpperOrbit = uppers.GroupBy(static upper => upper.Value).Max(static group => group.Count() + 1);

        tables[UnicodeTable.FoldingLayout] = new ByteWriter().Int24(maxOrbit).Int24(maxUpperOrbit).ToArray();
        tables[UnicodeTable.FoldData] = data.ToArray();
        tables[UnicodeTable.OrbitIndex] = orbit.ToArray();
        tables[UnicodeTable.UpperData] = upperData.ToArray();
        tables[UnicodeTable.UpperOrbitIndex] = upperOrbit.ToArray();
    }

    private static void Normalization(UnicodeDatabase database, SortedDictionary<UnicodeTable, byte[]> tables)
    {
        var combining = new ByteWriter();
        var start = 0;

        for (var codePoint = 1; codePoint <= database.CombiningClass.Length; codePoint++)
        {
            if (codePoint == database.CombiningClass.Length || database.CombiningClass[codePoint] != database.CombiningClass[start])
            {
                if (database.CombiningClass[start] != 0)
                {
                    combining.Int24(start).Int24(codePoint - 1).Byte(database.CombiningClass[start]);
                }

                start = codePoint;
            }
        }

        var pool = new ByteWriter();
        var poolLength = 0;

        ByteWriter Index(SortedDictionary<int, int[]> decompositions)
        {
            var index = new ByteWriter();

            foreach (var (codePoint, mapping) in decompositions)
            {
                index.Int24(codePoint).UInt16(poolLength).Byte(mapping.Length);

                foreach (var part in mapping)
                {
                    pool.Int24(part);
                }

                poolLength += mapping.Length;
            }

            return index;
        }

        var canonical = Index(database.CanonicalDecomposition);
        var compatibility = Index(database.CompatibilityDecomposition);
        var compositions = new ByteWriter();

        foreach (var ((first, second), composite) in database.Compositions)
        {
            compositions.Int24(first).Int24(second).Int24(composite);
        }

        var quick = new ByteWriter();
        var quickIndex = new ByteWriter();
        var quickCount = 0;

        foreach (var set in database.QuickChecks)
        {
            quickIndex.UInt16(quickCount).UInt16(set.Count);

            foreach (var range in set)
            {
                quick.Int24(range.First).Int24(range.Last);
                quickCount++;
            }
        }

        var longest = database.CanonicalDecomposition.Values.Concat(database.CompatibilityDecomposition.Values).Max(static mapping => mapping.Length);

        tables[UnicodeTable.NormalizationLayout] = new ByteWriter().Int24(longest).ToArray();
        tables[UnicodeTable.CombiningClassData] = combining.ToArray();
        tables[UnicodeTable.CanonicalIndex] = canonical.ToArray();
        tables[UnicodeTable.CompatibilityIndex] = compatibility.ToArray();
        tables[UnicodeTable.DecompositionPool] = pool.ToArray();
        tables[UnicodeTable.CompositionData] = compositions.ToArray();
        tables[UnicodeTable.QuickCheckData] = quick.ToArray();
        tables[UnicodeTable.QuickCheckIndex] = quickIndex.ToArray();
    }

    private static void Casing(UnicodeDatabase database, SortedDictionary<UnicodeTable, byte[]> tables)
    {
        var pool = new ByteWriter();
        var poolLength = 0;

        ByteWriter Index(SortedDictionary<int, int[]> mappings)
        {
            var index = new ByteWriter();

            foreach (var (codePoint, mapping) in mappings)
            {
                index.Int24(codePoint).UInt16(poolLength).Byte(mapping.Length);

                foreach (var part in mapping)
                {
                    pool.Int24(part);
                }

                poolLength += mapping.Length;
            }

            return index;
        }

        var upper = Index(database.FullUppercase);
        var lower = Index(database.FullLowercase);

        byte[] Ranges(string property)
        {
            var ranges = new ByteWriter();

            foreach (var range in database.BinaryProperties.Single(value => value.ShortName == property).Ranges)
            {
                ranges.Int24(range.First).Int24(range.Last);
            }

            return ranges.ToArray();
        }

        var longest = database.FullUppercase.Values.Concat(database.FullLowercase.Values).Max(static mapping => mapping.Length);

        tables[UnicodeTable.CasingLayout] = new ByteWriter().Int24(longest).ToArray();
        tables[UnicodeTable.UpperIndex] = upper.ToArray();
        tables[UnicodeTable.LowerIndex] = lower.ToArray();
        tables[UnicodeTable.MappingPool] = pool.ToArray();
        tables[UnicodeTable.CasedRanges] = Ranges("Cased");
        tables[UnicodeTable.CaseIgnorableRanges] = Ranges("Case_Ignorable");
        tables[UnicodeTable.SoftDottedRanges] = Ranges("Soft_Dotted");
    }

    /// <summary>
    /// The break properties segmentation reads, each with its values in code order: code 0 is the value a
    /// code point the file does not list has. The orders are <c>Broiler.Unicode.Segmentation</c>'s
    /// enumerations', and Broiler.VM's <c>SegmentBreakValues</c>.
    /// </summary>
    private static readonly (string Name, string[] Values)[] SegmentBreakValues =
    [
        ("gcb", ["Other", "CR", "LF", "Control", "Extend", "ZWJ", "Regional_Indicator", "Prepend", "SpacingMark", "L", "V", "T", "LV", "LVT"]),
        ("wb", ["Other", "CR", "LF", "Newline", "Extend", "ZWJ", "Regional_Indicator", "Format", "Katakana", "Hebrew_Letter", "ALetter", "Single_Quote", "Double_Quote", "MidNumLet", "MidLetter", "MidNum", "Numeric", "ExtendNumLet", "WSegSpace"]),
        ("sb", ["Other", "CR", "LF", "Extend", "Sep", "Format", "Sp", "Lower", "Upper", "OLetter", "Numeric", "ATerm", "SContinue", "STerm", "Close"]),
        ("ep", ["No", "Yes"]),
        ("incb", ["None", "Linker", "Consonant", "Extend"]),
        ("ideo", ["No", "Yes"]),
    ];

    /// <summary>
    /// The five break properties over the whole code space, and whether a code point is Ideographic or of
    /// the Hiragana script, as one table: a byte giving the number of distinct combinations, each
    /// combination as six value codes, a four-byte little-endian run count, and each run as its first code
    /// point (three bytes, little-endian) and the index of its combination.
    /// </summary>
    private static byte[] SegmentBreaks(IReadOnlyDictionary<string, byte[]> ucd)
    {
        var properties = new byte[6][];

        for (var p = 0; p < 6; p++)
        {
            properties[p] = new byte[0x110000];
        }

        void Read(int property, string file, Func<string[], string?> value)
        {
            var codes = SegmentBreakValues[property].Values;

            foreach (var raw in UnicodePin.Decode(ucd[file]).Split('\n'))
            {
                var line = raw.Split('#')[0].Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                var fields = line.Split(';').Select(static field => field.Trim()).ToArray();

                if (value(fields) is not { } name)
                {
                    continue;
                }

                var code = Array.IndexOf(codes, name);

                if (code <= 0)
                {
                    throw new InvalidDataException($"{file} names a value the table does not code: {name}");
                }

                var range = fields[0].Split("..");
                var first = int.Parse(range[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                var last = range.Length == 1 ? first : int.Parse(range[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                for (var c = first; c <= last; c++)
                {
                    properties[property][c] = (byte)code;
                }
            }
        }

        Read(0, "ucd-17.0.0/auxiliary/GraphemeBreakProperty.txt", static fields => fields[1]);
        Read(1, "ucd-17.0.0/auxiliary/WordBreakProperty.txt", static fields => fields[1]);
        Read(2, "ucd-17.0.0/auxiliary/SentenceBreakProperty.txt", static fields => fields[1]);
        Read(3, "ucd-17.0.0/emoji/emoji-data.txt", static fields => fields[1] == "Extended_Pictographic" ? "Yes" : null);
        Read(4, "ucd-17.0.0/DerivedCoreProperties.txt", static fields => fields.Length == 3 && fields[1] == "InCB" ? fields[2] : null);
        Read(5, "ucd-17.0.0/PropList.txt", static fields => fields[1] == "Ideographic" ? "Yes" : null);
        Read(5, "ucd-17.0.0/Scripts.txt", static fields => fields[1] == "Hiragana" ? "Yes" : null);

        var combinations = new List<int>();
        var runs = new List<(int First, int Combination)>();

        for (var c = 0; c < 0x110000; c++)
        {
            var key = 0;

            for (var p = 0; p < 6; p++)
            {
                key = (key << 5) | properties[p][c];
            }

            var index = combinations.IndexOf(key);

            if (index < 0)
            {
                index = combinations.Count;
                combinations.Add(key);
            }

            if (runs.Count == 0 || runs[^1].Combination != index)
            {
                runs.Add((c, index));
            }
        }

        if (combinations.Count > 255)
        {
            throw new InvalidDataException($"{combinations.Count} combinations do not fit in a byte");
        }

        var data = new List<byte> { (byte)combinations.Count };

        foreach (var key in combinations)
        {
            for (var p = 5; p >= 0; p--)
            {
                data.Add((byte)((key >> (5 * p)) & 0x1F));
            }
        }

        data.AddRange(BitConverter.GetBytes(runs.Count));

        foreach (var (first, combination) in runs)
        {
            data.Add((byte)(first & 0xFF));
            data.Add((byte)((first >> 8) & 0xFF));
            data.Add((byte)(first >> 16));
            data.Add((byte)combination);
        }

        return data.ToArray();
    }

    /// <summary>A name table in ordinal order: a length byte, the ASCII name and a two-byte value, plus two-byte offsets.</summary>
    private static (ByteWriter Names, ByteWriter Index) NameTable(IEnumerable<(string Name, int Value)> entries)
    {
        var names = new ByteWriter();
        var index = new ByteWriter();

        // PropertyValueAliases.txt writes a value whose short and long names coincide twice
        // (`Ahom ; Ahom`); the same name for the same value is one entry, a name for two is a stop.
        var ordered = entries.Distinct().OrderBy(static entry => entry.Name, StringComparer.Ordinal).ToArray();

        foreach (var duplicate in ordered.GroupBy(static entry => entry.Name, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            throw new InvalidDataException($"the name `{duplicate.Key}` selects more than one table entry");
        }

        foreach (var (name, value) in ordered)
        {
            if (name.Length is 0 or > 255 || name.Any(static character => character is < '!' or > '~'))
            {
                throw new InvalidDataException($"the name `{name}` is not 1 to 255 printable ASCII characters");
            }

            index.UInt16(names.Count);
            names.Byte(name.Length);

            foreach (var character in name)
            {
                names.Byte(character);
            }

            names.UInt16(value);
        }

        return (names, index);
    }

    /// <summary>Little-endian bytes, bounds-checked as they are written.</summary>
    private sealed class ByteWriter
    {
        private readonly List<byte> bytes = [];

        internal int Count => bytes.Count;

        internal ByteWriter Byte(int value)
        {
            if (value is < 0 or > 0xFF)
            {
                throw new InvalidDataException($"{value} does not fit in a byte");
            }

            bytes.Add((byte)value);
            return this;
        }

        internal ByteWriter UInt16(int value)
        {
            if (value is < 0 or > 0xFFFF)
            {
                throw new InvalidDataException($"{value} does not fit in two bytes");
            }

            bytes.Add((byte)value);
            bytes.Add((byte)(value >> 8));
            return this;
        }

        internal ByteWriter Int24(int value)
        {
            if (value is < 0 or > 0xFFFFFF)
            {
                throw new InvalidDataException($"{value} does not fit in three bytes");
            }

            bytes.Add((byte)value);
            bytes.Add((byte)(value >> 8));
            bytes.Add((byte)(value >> 16));
            return this;
        }

        internal byte[] ToArray() => bytes.ToArray();
    }
}
