// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   47
// Annotated:        47/47
// Exempt:           17
// Human-reviewed:   0/47
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       47
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Broiler.Unicode.Generation;

// PORTED from Broiler.VM's src/tests/Broiler.VM.Architecture.Tests/UnicodeDatabase.cs and the
// UnicodeSpecTables half of its UnicodePin.cs, at 7b44dc7, under decision SJL-D-6 (c): the owner holds
// the generator. The parsing and every cross-check are Broiler.VM's; what changed is that the database
// reads the ECMAScript tables by file name through the pin, so that one generator reads Broiler.VM's pin
// and this repository's alike, and that Soft_Dotted, the segmentation break properties and the
// language-sensitive casing it now serves are read here too (UnicodeTableGenerator).

/// <summary>An inclusive range of code points.</summary>
// Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=635743
// Broiler-Human:        PENDING
internal readonly record struct UnicodeRange(int First, int Last);

/// <summary>Reading the UCD's semicolon-separated files.</summary>
// Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=A3FDC9
// Broiler-Human:        PENDING
internal static class UnicodeDataFile
{
    /// <summary>The highest code point.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=79D97A
    // Broiler-Human:        PENDING
    internal const int MaxCodePoint = 0x10FFFF;

    /// <summary>
    /// Every data line of a UCD file as trimmed fields, with the <c>#</c> comment removed and
    /// blank lines skipped.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=FD0D7D
    // Broiler-Human:        PENDING
    internal static IEnumerable<string[]> Records(string text)
    {
        foreach (var raw in text.Split('\n'))
        {
            var hash = raw.IndexOf('#', StringComparison.Ordinal);
            var line = (hash >= 0 ? raw[..hash] : raw).Trim();

            if (line.Length > 0)
            {
                yield return line.Split(';').Select(static field => field.Trim()).ToArray();
            }
        }
    }

    /// <summary>The comment of a data line, or the empty string.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=E5685A
    // Broiler-Human:        PENDING
    internal static IEnumerable<(string[] Fields, string Comment)> RecordsWithComments(string text)
    {
        foreach (var raw in text.Split('\n'))
        {
            var hash = raw.IndexOf('#', StringComparison.Ordinal);
            var line = (hash >= 0 ? raw[..hash] : raw).Trim();

            if (line.Length > 0)
            {
                yield return (
                    line.Split(';').Select(static field => field.Trim()).ToArray(),
                    hash >= 0 ? raw[(hash + 1)..].Trim() : string.Empty);
            }
        }
    }

    /// <summary><c>0041</c> or <c>0041..005A</c>.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=57D8AF
    // Broiler-Human:        PENDING
    internal static UnicodeRange Range(string field)
    {
        var dots = field.IndexOf("..", StringComparison.Ordinal);

        return dots < 0
            ? new UnicodeRange(Hex(field), Hex(field))
            : new UnicodeRange(Hex(field[..dots]), Hex(field[(dots + 2)..]));
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=4C62DE
    // Broiler-Human:        PENDING
    internal static int Hex(string text)
    {
        var value = int.Parse(text, NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo);

        if (value is < 0 or > MaxCodePoint)
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"{text} is not a code point"));
        }

        return value;
    }

    /// <summary>Sorted, merged: ascending, disjoint and never adjacent.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=2125F4
    // Broiler-Human:        PENDING
    internal static List<UnicodeRange> Normalize(IEnumerable<UnicodeRange> ranges)
    {
        var merged = new List<UnicodeRange>();

        foreach (var range in ranges.OrderBy(static range => range.First))
        {
            if (merged.Count > 0 && range.First <= merged[^1].Last + 1)
            {
                merged[^1] = merged[^1] with { Last = Math.Max(merged[^1].Last, range.Last) };
            }
            else
            {
                merged.Add(range);
            }
        }

        return merged;
    }

    /// <summary>Every code point not in the (normalized) ranges.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=C6CC1E
    // Broiler-Human:        PENDING
    internal static List<UnicodeRange> Complement(IReadOnlyList<UnicodeRange> ranges)
    {
        var complement = new List<UnicodeRange>();
        var next = 0;

        foreach (var range in ranges)
        {
            if (range.First > next)
            {
                complement.Add(new UnicodeRange(next, range.First - 1));
            }

            next = range.Last + 1;
        }

        if (next <= MaxCodePoint)
        {
            complement.Add(new UnicodeRange(next, MaxCodePoint));
        }

        return complement;
    }

    /// <summary>The maximal runs of one value in a per-code-point array, for each value.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=A0B2A6
    // Broiler-Human:        PENDING
    internal static Dictionary<int, List<UnicodeRange>> Runs(IReadOnlyList<int> valueOf)
    {
        var runs = new Dictionary<int, List<UnicodeRange>>();
        var start = 0;

        for (var codePoint = 1; codePoint <= valueOf.Count; codePoint++)
        {
            if (codePoint == valueOf.Count || valueOf[codePoint] != valueOf[start])
            {
                var value = valueOf[start];

                if (!runs.TryGetValue(value, out var list))
                {
                    runs[value] = list = [];
                }

                list.Add(new UnicodeRange(start, codePoint - 1));
                start = codePoint;
            }
        }

        return runs;
    }
}

/// <summary>A property value: the set it resolves to, and every name the UCD gives it.</summary>
// Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=36FBD4
// Broiler-Human:        PENDING
internal sealed record UnicodeValue(string ShortName, string LongName, IReadOnlyList<string> Names, List<UnicodeRange> Ranges);

/// <summary>
/// The Unicode Character Database, parsed from the verified archive into exactly what the tables
/// need, with the internal consistency checks that make a misread file a failure rather than a
/// quietly wrong table.
/// </summary>
/// <remarks>
/// Every derived property this reads from two places is compared across them: the General_Category
/// of <c>DerivedGeneralCategory.txt</c> against <c>UnicodeData.txt</c>, and the NFD and NFKD quick
/// checks of <c>DerivedNormalizationProps.txt</c> against the decompositions this computes. A parser
/// that dropped a line would pass neither.
/// </remarks>
// Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=DFF330
// Broiler-Human:        PENDING
internal sealed class UnicodeDatabase
{
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=EF39E2
    // Broiler-Human:        PENDING
    internal const int HangulSBase = 0xAC00;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=EBB1C6
    // Broiler-Human:        PENDING
    internal const int HangulLBase = 0x1100;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=5BE10C
    // Broiler-Human:        PENDING
    internal const int HangulVBase = 0x1161;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=8D8D7F
    // Broiler-Human:        PENDING
    internal const int HangulTBase = 0x11A7;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=4EB942
    // Broiler-Human:        PENDING
    internal const int HangulVCount = 21;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=C33BAF
    // Broiler-Human:        PENDING
    internal const int HangulTCount = 28;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=652812
    // Broiler-Human:        PENDING
    internal const int HangulNCount = HangulVCount * HangulTCount;
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=866976
    // Broiler-Human:        PENDING
    internal const int HangulSCount = 19 * HangulNCount;

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=35425A
    // Broiler-Human:        PENDING
    private const string Ucd = "ucd-17.0.0/";

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=6C6262
    // Broiler-Human:        PENDING
    private const string Emoji = "emoji-17.0.0/";

    /// <summary>General_Category values (the 30) and groups (the 8), in set-id order.</summary>
    internal List<UnicodeValue> GeneralCategories { get; } = [];

    /// <summary>The number of the above that are single values rather than groups.</summary>
    internal int GeneralCategoryValueCount { get; private set; }

    /// <summary>The binary properties the language admits, in set-id order: canonical name, aliases, ranges.</summary>
    internal List<UnicodeValue> BinaryProperties { get; } = [];

    /// <summary>
    /// The properties of strings the language admits under <c>v</c>, in the ECMAScript table's row
    /// order: each one's single code points and its sequences of two or more. <c>RGI_Emoji</c>, the
    /// table's last row, is the union of the others and has no data of its own (phase F2).
    /// </summary>
    internal List<(string Name, List<UnicodeRange> CodePoints, List<int[]> Sequences)> StringProperties { get; } = [];

    /// <summary>Script values in id order: Script ranges.</summary>
    internal List<UnicodeValue> Scripts { get; } = [];

    /// <summary>Script_Extensions ranges, by the same ids as <see cref="Scripts"/>.</summary>
    internal List<List<UnicodeRange>> ScriptExtensions { get; } = [];

    /// <summary>The non-binary property names: General_Category (0), Script (1), Script_Extensions (2).</summary>
    internal List<(string Name, int Kind)> NonBinaryNames { get; } = [];

    /// <summary>Canonical_Combining_Class per code point.</summary>
    internal int[] CombiningClass { get; } = new int[UnicodeDataFile.MaxCodePoint + 1];

    /// <summary>Each code point's full canonical decomposition, where it has one (Hangul excluded).</summary>
    internal SortedDictionary<int, int[]> CanonicalDecomposition { get; } = [];

    /// <summary>Each code point whose full compatibility decomposition differs from its full canonical one.</summary>
    internal SortedDictionary<int, int[]> CompatibilityDecomposition { get; } = [];

    /// <summary>Primary composites: (first, second) to composite, Full_Composition_Exclusion honoured, Hangul excluded.</summary>
    internal SortedDictionary<(int First, int Second), int> Compositions { get; } = [];

    /// <summary>NFC_QC=No, NFC_QC=Maybe, NFKC_QC=No, NFKC_QC=Maybe, in that order.</summary>
    internal List<List<UnicodeRange>> QuickChecks { get; } = [];

    /// <summary>CaseFolding.txt status C and S: source to target.</summary>
    internal SortedDictionary<int, int> SimpleFolding { get; } = [];

    /// <summary>UnicodeData.txt's Simple_Uppercase_Mapping (field 12), where a code point has one.</summary>
    internal SortedDictionary<int, int> SimpleUppercase { get; } = [];

    /// <summary>UnicodeData.txt's Simple_Lowercase_Mapping (field 13), where a code point has one.</summary>
    internal SortedDictionary<int, int> SimpleLowercase { get; } = [];

    /// <summary>
    /// The full upper-case mapping of every code point upper-casing changes: SpecialCasing.txt's
    /// unconditional mapping where it gives one, and the simple mapping otherwise. This is the
    /// Unicode Default Case Conversion's toUppercase, which ECMA-262 uses with no language.
    /// </summary>
    internal SortedDictionary<int, int[]> FullUppercase { get; } = [];

    /// <summary>
    /// The full lower-case mapping of every code point lower-casing changes, as
    /// <see cref="FullUppercase"/>. GREEK CAPITAL LETTER SIGMA maps here to its ordinary small form;
    /// the one language-independent condition, Final_Sigma, is applied at run time.
    /// </summary>
    internal SortedDictionary<int, int[]> FullLowercase { get; } = [];

    /// <summary>Parses and cross-checks the verified archive.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=4E070F
    // Broiler-Human:        PENDING
    internal static UnicodeDatabase Read(UnicodePin pin, IReadOnlyDictionary<string, byte[]> archive)
    {
        var database = new UnicodeDatabase();
        string Text(string name) => UnicodePin.Decode(archive[name]);

        // A spec table's name in the pin is a path in its repository; it is found by its file name.
        var nonBinary = UnicodeSpecTables.Rows(Text(pin.SpecTable(UnicodeSpecTables.NonBinary).Name));
        var binary = UnicodeSpecTables.Rows(Text(pin.SpecTable(UnicodeSpecTables.Binary).Name));
        var strings = UnicodeSpecTables.Rows(Text(pin.SpecTable(UnicodeSpecTables.Strings).Name));
        var disagreements = UnicodeSpecTables.Disagreements(nonBinary, binary, strings, Text(Ucd + "PropertyAliases.txt")).ToArray();

        if (disagreements.Length > 0)
        {
            throw new InvalidDataException(
                "The ECMAScript property tables and PropertyAliases.txt disagree:\n  " + string.Join("\n  ", disagreements));
        }

        var unicodeData = Text(Ucd + "UnicodeData.txt");
        database.ReadGeneralCategories(Text(Ucd + "PropertyValueAliases.txt"), Text(Ucd + "extracted/DerivedGeneralCategory.txt"), unicodeData);

        database.ReadNonBinaryNames(nonBinary);
        database.ReadBinaryProperties(binary, archive);
        database.ReadScripts(Text(Ucd + "PropertyValueAliases.txt"), Text(Ucd + "Scripts.txt"), Text(Ucd + "ScriptExtensions.txt"));
        database.ReadNormalization(unicodeData, Text(Ucd + "DerivedNormalizationProps.txt"));
        database.ReadCaseFolding(Text(Ucd + "CaseFolding.txt"));
        database.ReadSimpleUppercase(unicodeData);
        database.ReadFullCaseMappings(unicodeData, Text(Ucd + "SpecialCasing.txt"));
        database.ReadStringProperties(strings, Text(Emoji + "emoji-sequences.txt"), Text(Emoji + "emoji-zwj-sequences.txt"));

        return database;
    }

    /// <summary>
    /// The properties of strings, from the two emoji sequence files, checked against the table of
    /// names the language admits.
    /// </summary>
    /// <remarks>
    /// <b>The file's type field is the property's name</b>, as the file's own header says: "each of
    /// the type fields defines the name of a binary property of strings", and <c>RGI_Emoji</c> is their
    /// union with <c>RGI_Emoji_ZWJ_Sequence</c>. So every type the files use must be a row of the
    /// ECMAScript table and every row but <c>RGI_Emoji</c> must be a type the files use; a range of
    /// single code points is admitted only for <c>Basic_Emoji</c>, and a sequence is two or more code
    /// points listed once.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=6CF2BB
    // Broiler-Human:        PENDING
    private void ReadStringProperties(IReadOnlyList<UnicodeSpecName> table, string sequences, string zwj)
    {
        const string Union = "RGI_Emoji";
        var names = table.Select(static row => row.Name).ToArray();

        if (names.Length == 0 || names[^1] != Union || table.Any(static row => row.Name != row.Canonical))
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the properties-of-strings table does not end with `{Union}`, or gives a property an alias"));
        }

        var byName = new Dictionary<string, (List<UnicodeRange> CodePoints, List<int[]> Sequences)>(StringComparer.Ordinal);

        foreach (var name in names[..^1])
        {
            byName[name] = ([], []);
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var fields in UnicodeDataFile.Records(sequences).Concat(UnicodeDataFile.Records(zwj)))
        {
            if (fields.Length < 2 || !byName.TryGetValue(fields[1], out var property))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"an emoji sequence line names the type `{(fields.Length < 2 ? string.Empty : fields[1])}`, which the table does not admit"));
            }

            var codePoints = fields[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (codePoints.Length == 1)
            {
                if (fields[1] != "Basic_Emoji")
                {
                    throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"`{fields[0]}` is a single code point of `{fields[1]}`, and only Basic_Emoji lists any"));
                }

                property.CodePoints.Add(UnicodeDataFile.Range(codePoints[0]));
                continue;
            }

            var sequence = codePoints.Select(UnicodeDataFile.Hex).ToArray();

            if (sequence.Any(static codePoint => codePoint > UnicodeDataFile.MaxCodePoint) || !seen.Add(fields[1] + ":" + fields[0]))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"`{fields[0]}` of `{fields[1]}` is out of range or listed twice"));
            }

            property.Sequences.Add(sequence);
        }

        foreach (var name in names[..^1])
        {
            var (codePoints, sequenceList) = byName[name];

            if (codePoints.Count == 0 && sequenceList.Count == 0)
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the emoji sequence files list nothing for `{name}`"));
            }

            StringProperties.Add((name, Merge(codePoints), sequenceList.OrderBy(static sequence => sequence, SequenceOrder.Instance).ToList()));
        }

        StringProperties.Add((Union, [], []));

        static List<UnicodeRange> Merge(List<UnicodeRange> ranges)
        {
            var merged = new List<UnicodeRange>();

            foreach (var range in ranges.OrderBy(static range => range.First))
            {
                if (merged.Count > 0 && range.First <= merged[^1].Last + 1)
                {
                    merged[^1] = merged[^1] with { Last = Math.Max(merged[^1].Last, range.Last) };
                }
                else
                {
                    merged.Add(range);
                }
            }

            return merged;
        }
    }

    /// <summary>Code point sequences ordered code point by code point, a prefix first.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=56C266
    // Broiler-Human:        PENDING
    private sealed class SequenceOrder : IComparer<int[]>
    {
        // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=C8074E
        // Broiler-Human:        PENDING
        internal static readonly SequenceOrder Instance = new();

        // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=0B4FD4
        // Broiler-Human:        PENDING
        public int Compare(int[]? left, int[]? right)
        {
            for (var at = 0; at < Math.Min(left!.Length, right!.Length); at++)
            {
                if (left[at] != right[at])
                {
                    return left[at].CompareTo(right[at]);
                }
            }

            return left.Length.CompareTo(right.Length);
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=C50883
    // Broiler-Human:        PENDING
    private void ReadGeneralCategories(string valueAliases, string derived, string unicodeData)
    {
        var values = new List<(string[] Fields, string Comment)>();

        foreach (var (fields, comment) in UnicodeDataFile.RecordsWithComments(valueAliases))
        {
            if (fields[0] == "gc")
            {
                values.Add((fields, comment));
            }
        }

        // The single values are the two-letter short names, in short-name order; the groups are the
        // rest, and PropertyValueAliases.txt states each group's members in its comment.
        var singles = values.Where(static value => value.Fields[1].Length == 2 && value.Comment.Length == 0)
            .OrderBy(static value => value.Fields[1], StringComparer.Ordinal).ToArray();
        var groups = values.Where(static value => value.Comment.Length > 0)
            .OrderBy(static value => value.Fields[1], StringComparer.Ordinal).ToArray();

        if (singles.Length + groups.Length != values.Count || singles.Length != 30 || groups.Length != 8)
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"PropertyValueAliases.txt gives {singles.Length} single General_Category values and {groups.Length} groups, not 30 and 8"));
        }

        var index = singles.Select(static (value, id) => (value.Fields[1], id)).ToDictionary(static pair => pair.Item1, static pair => pair.id, StringComparer.Ordinal);
        var unassigned = index["Cn"];
        var valueOf = new int[UnicodeDataFile.MaxCodePoint + 1];

        Array.Fill(valueOf, unassigned);

        foreach (var fields in UnicodeDataFile.Records(derived))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
            {
                valueOf[codePoint] = index[fields[1]];
            }
        }

        // UnicodeData.txt says the same thing, <..., First>/<..., Last> pairs included, and a
        // code point it does not list is Cn.
        var fromUnicodeData = new int[UnicodeDataFile.MaxCodePoint + 1];

        Array.Fill(fromUnicodeData, unassigned);

        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            for (var codePoint = first; codePoint <= last; codePoint++)
            {
                fromUnicodeData[codePoint] = index[fields[2]];
            }
        }

        var differing = Enumerable.Range(0, valueOf.Length).FirstOrDefault(codePoint => valueOf[codePoint] != fromUnicodeData[codePoint], -1);

        if (differing >= 0)
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"U+{differing:X4} has General_Category {singles[valueOf[differing]].Fields[1]} in DerivedGeneralCategory.txt and {singles[fromUnicodeData[differing]].Fields[1]} in UnicodeData.txt"));
        }

        var runs = UnicodeDataFile.Runs(valueOf);

        foreach (var (fields, _) in singles)
        {
            var id = index[fields[1]];

            GeneralCategories.Add(new UnicodeValue(fields[1], fields[2], fields.Skip(1).ToArray(), runs.TryGetValue(id, out var ranges) ? ranges : []));
        }

        GeneralCategoryValueCount = GeneralCategories.Count;

        foreach (var (fields, comment) in groups)
        {
            var members = comment.Split('|', StringSplitOptions.TrimEntries);

            if (fields[1].Length == 1 && !members.Order(StringComparer.Ordinal).SequenceEqual(
                    index.Keys.Where(key => key[0] == fields[1][0]).Order(StringComparer.Ordinal), StringComparer.Ordinal))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"General_Category group {fields[1]} lists {comment}, which is not every value starting {fields[1]}"));
            }

            GeneralCategories.Add(new UnicodeValue(
                fields[1],
                fields[2],
                fields.Skip(1).ToArray(),
                UnicodeDataFile.Normalize(members.SelectMany(member => GeneralCategories[index[member]].Ranges))));
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=1CFA5D
    // Broiler-Human:        PENDING
    private void ReadNonBinaryNames(IReadOnlyList<UnicodeSpecName> nonBinary)
    {
        string[] canonical = ["General_Category", "Script", "Script_Extensions"];

        foreach (var row in nonBinary)
        {
            var kind = Array.IndexOf(canonical, row.Canonical);

            if (kind < 0)
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the non-binary property table names `{row.Canonical}`, which no table here is generated for"));
            }

            NonBinaryNames.Add((row.Name, kind));
        }

        if (NonBinaryNames.Select(static name => name.Kind).Distinct().Count() != 3)
        {
            throw new InvalidDataException("the non-binary property table does not name all three of General_Category, Script and Script_Extensions");
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=F1EEDB
    // Broiler-Human:        PENDING
    private void ReadBinaryProperties(IReadOnlyList<UnicodeSpecName> binary, IReadOnlyDictionary<string, byte[]> archive)
    {
        string[] sources =
        [
            Ucd + "PropList.txt",
            Ucd + "DerivedCoreProperties.txt",
            Ucd + "extracted/DerivedBinaryProperties.txt",
            Ucd + "emoji/emoji-data.txt",
            Ucd + "DerivedNormalizationProps.txt",
        ];

        // Every two-field line of every source, by property name and by the file that states it.
        var stated = new Dictionary<string, (string File, List<UnicodeRange> Ranges)>(StringComparer.Ordinal);

        foreach (var source in sources)
        {
            foreach (var fields in UnicodeDataFile.Records(UnicodePin.Decode(archive[source])))
            {
                if (fields.Length != 2)
                {
                    continue;
                }

                if (stated.TryGetValue(fields[1], out var found) && found.File != source)
                {
                    throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"`{fields[1]}` is stated by both {found.File} and {source}"));
                }

                if (found.Ranges is null)
                {
                    stated[fields[1]] = found = (source, []);
                }

                found.Ranges.Add(UnicodeDataFile.Range(fields[0]));
            }
        }

        var unassigned = GeneralCategories.Single(static value => value.ShortName == "Cn").Ranges;

        foreach (var group in binary.GroupBy(static row => row.Canonical, StringComparer.Ordinal).OrderBy(static group => group.Key, StringComparer.Ordinal))
        {
            var ranges = group.Key switch
            {
                "Any" => [new UnicodeRange(0, UnicodeDataFile.MaxCodePoint)],
                "ASCII" => [new UnicodeRange(0, 0x7F)],
                "Assigned" => UnicodeDataFile.Complement(unassigned),
                _ => stated.TryGetValue(group.Key, out var found)
                    ? UnicodeDataFile.Normalize(found.Ranges)
                    : throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the language admits `{group.Key}`, and none of the pinned property files states it")),
            };

            var names = group.Select(static row => row.Name).ToArray();

            BinaryProperties.Add(new UnicodeValue(group.Key, group.Key, names, ranges));
        }

        CheckIdentifierProperties(stated);

        if (BinaryProperties.Count != 53)
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the binary property table admits {BinaryProperties.Count} properties, and JSD-0031 section 7 names 53"));
        }
    }

    /// <summary>
    /// Holds DerivedCoreProperties.txt's <c>ID_Start</c> and <c>ID_Continue</c>, which the tokenizer
    /// and the group-name parser read, to their definitions in UAX #31 over the other pinned files.
    /// </summary>
    /// <remarks>
    /// <c>ID_Start</c> is the letters and <c>Nl</c> plus <c>Other_ID_Start</c>, minus
    /// <c>Pattern_Syntax</c> and <c>Pattern_White_Space</c>; <c>ID_Continue</c> adds <c>Mn</c>,
    /// <c>Mc</c>, <c>Nd</c>, <c>Pc</c> and <c>Other_ID_Continue</c> under the same subtraction. A
    /// misread line in any of the three files would make the two computations part.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=69F699
    // Broiler-Human:        PENDING
    private void CheckIdentifierProperties(Dictionary<string, (string File, List<UnicodeRange> Ranges)> stated)
    {
        var category = GeneralCategories.Take(GeneralCategoryValueCount).ToDictionary(static value => value.ShortName, static value => value.Ranges, StringComparer.Ordinal);

        List<UnicodeRange> Stated(string name) =>
            stated.TryGetValue(name, out var found) ? found.Ranges : throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"no pinned file states `{name}`"));

        List<UnicodeRange> Derive(IEnumerable<List<UnicodeRange>> parts)
        {
            var member = new bool[UnicodeDataFile.MaxCodePoint + 1];

            foreach (var range in parts.SelectMany(static part => part))
            {
                Array.Fill(member, true, range.First, range.Last - range.First + 1);
            }

            foreach (var range in Stated("Pattern_Syntax").Concat(Stated("Pattern_White_Space")))
            {
                Array.Fill(member, false, range.First, range.Last - range.First + 1);
            }

            return UnicodeDataFile.Normalize(Enumerable.Range(0, member.Length).Where(codePoint => member[codePoint]).Select(static codePoint => new UnicodeRange(codePoint, codePoint)));
        }

        var start = Derive(new[] { "Lu", "Ll", "Lt", "Lm", "Lo", "Nl" }.Select(name => category[name]).Append(Stated("Other_ID_Start")));
        var part = Derive(new[] { "Lu", "Ll", "Lt", "Lm", "Lo", "Nl", "Mn", "Mc", "Nd", "Pc" }.Select(name => category[name]).Append(Stated("Other_ID_Start")).Append(Stated("Other_ID_Continue")));

        if (!start.SequenceEqual(BinaryProperties.Single(static value => value.ShortName == "ID_Start").Ranges))
        {
            throw new InvalidDataException("DerivedCoreProperties.txt's ID_Start is not UAX #31's derivation over UnicodeData.txt and PropList.txt");
        }

        if (!part.SequenceEqual(BinaryProperties.Single(static value => value.ShortName == "ID_Continue").Ranges))
        {
            throw new InvalidDataException("DerivedCoreProperties.txt's ID_Continue is not UAX #31's derivation over UnicodeData.txt and PropList.txt");
        }
    }

    /// <summary>
    /// Reads UnicodeData.txt's simple upper-case field, which the non-<c>u</c> Canonicalize reads,
    /// and holds it to <c>Changes_When_Uppercased</c>: every code point with a simple upper case
    /// other than itself must be one that upper-casing changes.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=3E5D16
    // Broiler-Human:        PENDING
    private void ReadSimpleUppercase(string unicodeData)
    {
        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            if (fields[12].Length == 0)
            {
                continue;
            }

            if (first != last)
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"UnicodeData.txt gives the range {fields[0]} a simple upper case"));
            }

            var upper = UnicodeDataFile.Hex(fields[12]);

            if (upper == first || !SimpleUppercase.TryAdd(first, upper))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"UnicodeData.txt gives {fields[0]} a simple upper case that is itself, or two of them"));
            }
        }

        var changes = BinaryProperties.Single(static value => value.ShortName == "Changes_When_Uppercased").Ranges;

        foreach (var source in SimpleUppercase.Keys.Where(source => !changes.Any(range => range.First <= source && source <= range.Last)))
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"U+{source:X4} has a simple upper case in UnicodeData.txt and is not Changes_When_Uppercased"));
        }
    }

    /// <summary>
    /// Reads UnicodeData.txt's simple lower-case field and SpecialCasing.txt's unconditional
    /// mappings into the two full mappings, and holds them to the derived properties: a code point
    /// a mapping changes is exactly one <c>Changes_When_Uppercased</c> or
    /// <c>Changes_When_Lowercased</c> names.
    /// </summary>
    /// <remarks>
    /// A SpecialCasing.txt line with a condition is language-sensitive - every one but
    /// <c>Final_Sigma</c> names a language, and ECMA-262's toUppercase and toLowercase use none - so
    /// only <c>Final_Sigma</c> is read among them, and only to check that it is the one line the
    /// run-time rule for GREEK CAPITAL LETTER SIGMA stands for.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=26AD4D
    // Broiler-Human:        PENDING
    private void ReadFullCaseMappings(string unicodeData, string specialCasing)
    {
        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            if (fields[13].Length == 0)
            {
                continue;
            }

            if (first != last)
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"UnicodeData.txt gives the range {fields[0]} a simple lower case"));
            }

            var lower = UnicodeDataFile.Hex(fields[13]);

            if (lower == first || !SimpleLowercase.TryAdd(first, lower))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"UnicodeData.txt gives {fields[0]} a simple lower case that is itself, or two of them"));
            }
        }

        foreach (var (source, upper) in SimpleUppercase)
        {
            FullUppercase.Add(source, [upper]);
        }

        foreach (var (source, lower) in SimpleLowercase)
        {
            FullLowercase.Add(source, [lower]);
        }

        static int[] Mapping(string field) =>
            field.Length == 0 ? [] : field.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(UnicodeDataFile.Hex).ToArray();

        var finalSigma = 0;

        foreach (var fields in UnicodeDataFile.Records(specialCasing))
        {
            // code; lower; title; upper; (condition_list;)
            var source = UnicodeDataFile.Hex(fields[0]);
            var condition = fields.Length > 4 ? fields[4] : string.Empty;

            if (condition.Length > 0)
            {
                if (condition == "Final_Sigma")
                {
                    if (source != 0x03A3 || !Mapping(fields[1]).SequenceEqual([0x03C2]))
                    {
                        throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"Final_Sigma is given to U+{source:X4}, and the run-time rule is written for U+03A3 to U+03C2 alone"));
                    }

                    finalSigma++;
                }
                else if (!condition.Split(' ')[0].All(static c => c is >= 'a' and <= 'z'))
                {
                    throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"U+{source:X4} carries the condition `{condition}`, which names no language and is not Final_Sigma"));
                }

                continue;
            }

            foreach (var (mapping, full) in new[] { (Mapping(fields[1]), FullLowercase), (Mapping(fields[3]), FullUppercase) })
            {
                if (mapping.Length == 1 && mapping[0] == source)
                {
                    full.Remove(source);
                }
                else
                {
                    full[source] = mapping;
                }
            }
        }

        if (finalSigma != 1)
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"SpecialCasing.txt gives Final_Sigma {finalSigma} times, and the run-time rule expects it once"));
        }

        foreach (var (name, full) in new[] { ("Changes_When_Uppercased", FullUppercase), ("Changes_When_Lowercased", FullLowercase) })
        {
            var changes = BinaryProperties.Single(value => value.ShortName == name).Ranges;
            var listed = UnicodeDataFile.Normalize(full.Keys.Select(static codePoint => new UnicodeRange(codePoint, codePoint)));

            if (!listed.SequenceEqual(changes))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the code points the full mapping changes are not exactly {name}"));
            }
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=A47B14
    // Broiler-Human:        PENDING
    private void ReadScripts(string valueAliases, string scripts, string extensions)
    {
        var values = UnicodeDataFile.Records(valueAliases)
            .Where(static fields => fields[0] == "sc")
            .OrderBy(static fields => fields[1], StringComparer.Ordinal)
            .ToArray();

        var byLong = values.Select(static (fields, id) => (fields[2], id)).ToDictionary(static pair => pair.Item1, static pair => pair.id, StringComparer.Ordinal);
        var byShort = values.Select(static (fields, id) => (fields[1], id)).ToDictionary(static pair => pair.Item1, static pair => pair.id, StringComparer.Ordinal);
        var unknown = byShort["Zzzz"];
        var scriptOf = new int[UnicodeDataFile.MaxCodePoint + 1];

        Array.Fill(scriptOf, unknown);

        foreach (var fields in UnicodeDataFile.Records(scripts))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
            {
                scriptOf[codePoint] = byLong[fields[1]];
            }
        }

        var runs = UnicodeDataFile.Runs(scriptOf);

        foreach (var fields in values)
        {
            var id = byShort[fields[1]];

            Scripts.Add(new UnicodeValue(fields[1], fields[2], fields.Skip(1).ToArray(), runs.TryGetValue(id, out var ranges) ? ranges : []));
        }

        // Script_Extensions: a listed code point has exactly the listed scripts, which need not
        // include its Script; an unlisted one has its Script alone.
        var extended = new List<UnicodeRange>[values.Length];
        var listed = new bool[UnicodeDataFile.MaxCodePoint + 1];

        for (var id = 0; id < extended.Length; id++)
        {
            extended[id] = [];
        }

        foreach (var fields in UnicodeDataFile.Records(extensions))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
            {
                if (listed[codePoint])
                {
                    throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"ScriptExtensions.txt lists U+{codePoint:X4} twice"));
                }

                listed[codePoint] = true;
            }

            foreach (var script in fields[1].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                extended[byShort[script]].Add(range);
            }
        }

        for (var id = 0; id < extended.Length; id++)
        {
            foreach (var range in Scripts[id].Ranges)
            {
                var start = -1;

                for (var codePoint = range.First; codePoint <= range.Last + 1; codePoint++)
                {
                    var member = codePoint <= range.Last && !listed[codePoint];

                    if (member && start < 0)
                    {
                        start = codePoint;
                    }
                    else if (!member && start >= 0)
                    {
                        extended[id].Add(new UnicodeRange(start, codePoint - 1));
                        start = -1;
                    }
                }
            }

            ScriptExtensions.Add(UnicodeDataFile.Normalize(extended[id]));
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=C6A95D
    // Broiler-Human:        PENDING
    private void ReadNormalization(string unicodeData, string derived)
    {
        var canonical = new Dictionary<int, int[]>();
        var compatibility = new Dictionary<int, int[]>();

        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            var combining = int.Parse(fields[3], NumberFormatInfo.InvariantInfo);

            for (var codePoint = first; codePoint <= last; codePoint++)
            {
                CombiningClass[codePoint] = combining;
            }

            if (fields[5].Length == 0)
            {
                continue;
            }

            if (first != last)
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"the UnicodeData.txt range at U+{first:X4} carries a decomposition"));
            }

            var parts = fields[5].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var tagged = parts[0][0] == '<';
            var mapping = parts.Skip(tagged ? 1 : 0).Select(UnicodeDataFile.Hex).ToArray();

            (tagged ? compatibility : canonical)[first] = mapping;
        }

        var exclusions = new HashSet<int>();
        var quickChecks = new Dictionary<string, List<UnicodeRange>>(StringComparer.Ordinal);

        foreach (var fields in UnicodeDataFile.Records(derived))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            if (fields.Length == 2 && fields[1] == "Full_Composition_Exclusion")
            {
                for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
                {
                    exclusions.Add(codePoint);
                }
            }
            else if (fields.Length == 3 && fields[1].EndsWith("_QC", StringComparison.Ordinal))
            {
                var key = string.Create(NumberFormatInfo.InvariantInfo, $"{fields[1]}={fields[2]}");

                if (!quickChecks.TryGetValue(key, out var list))
                {
                    quickChecks[key] = list = [];
                }

                list.Add(range);
            }
        }

        int[] Full(int codePoint, bool compat)
        {
            if (codePoint >= HangulSBase && codePoint < HangulSBase + HangulSCount)
            {
                return Hangul(codePoint);
            }

            var mapping = canonical.TryGetValue(codePoint, out var found)
                ? found
                : compat && compatibility.TryGetValue(codePoint, out found) ? found : null;

            return mapping is null
                ? [codePoint]
                : mapping.SelectMany(part => Full(part, compat)).ToArray();
        }

        foreach (var codePoint in canonical.Keys.Concat(compatibility.Keys).Order())
        {
            var nfd = Full(codePoint, compat: false);
            var nfkd = Full(codePoint, compat: true);

            if (canonical.ContainsKey(codePoint))
            {
                CanonicalDecomposition[codePoint] = nfd;
            }

            if (!nfkd.SequenceEqual(nfd))
            {
                CompatibilityDecomposition[codePoint] = nfkd;
            }
        }

        foreach (var (codePoint, mapping) in canonical)
        {
            if (mapping.Length == 2 && !exclusions.Contains(codePoint))
            {
                Compositions.Add((mapping[0], mapping[1]), codePoint);
            }
        }

        // The quick checks the generator does not store are the ones it can compute; comparing
        // them with the file is what shows the decompositions above were read completely.
        var hangul = new UnicodeRange(HangulSBase, HangulSBase + HangulSCount - 1);

        Compare("NFD_QC=N", UnicodeDataFile.Normalize(CanonicalDecomposition.Keys.Select(static cp => new UnicodeRange(cp, cp)).Append(hangul)));
        Compare("NFKD_QC=N", UnicodeDataFile.Normalize(CanonicalDecomposition.Keys.Concat(CompatibilityDecomposition.Keys).Select(static cp => new UnicodeRange(cp, cp)).Append(hangul)));

        foreach (var key in new[] { "NFC_QC=N", "NFC_QC=M", "NFKC_QC=N", "NFKC_QC=M" })
        {
            QuickChecks.Add(UnicodeDataFile.Normalize(quickChecks.TryGetValue(key, out var list) ? list : throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"DerivedNormalizationProps.txt states no {key}"))));
        }

        void Compare(string key, List<UnicodeRange> computed)
        {
            if (!UnicodeDataFile.Normalize(quickChecks[key]).SequenceEqual(computed))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"DerivedNormalizationProps.txt's {key} is not the set of code points the UnicodeData.txt decompositions change"));
            }
        }
    }

    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=A89D82
    // Broiler-Human:        PENDING
    private void ReadCaseFolding(string text)
    {
        foreach (var fields in UnicodeDataFile.Records(text))
        {
            if (fields[1] is not ("C" or "S"))
            {
                continue;
            }

            var target = fields[2].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (target.Length != 1 || !SimpleFolding.TryAdd(UnicodeDataFile.Hex(fields[0]), UnicodeDataFile.Hex(target[0])))
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"CaseFolding.txt gives {fields[0]} a C or S mapping that is not one code point, or two of them"));
            }
        }

        // Folding is idempotent: nothing a code point folds to folds again.
        foreach (var target in SimpleFolding.Values.Where(SimpleFolding.ContainsKey))
        {
            throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"U+{target:X4} is a simple folding target and has a folding of its own"));
        }
    }

    /// <summary>The full Hangul syllable decomposition, LV or LVT.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=3F88E4
    // Broiler-Human:        PENDING
    internal static int[] Hangul(int syllable)
    {
        var index = syllable - HangulSBase;
        var leading = HangulLBase + (index / HangulNCount);
        var vowel = HangulVBase + (index % HangulNCount / HangulTCount);
        var trailing = HangulTBase + (index % HangulTCount);

        return trailing == HangulTBase ? [leading, vowel] : [leading, vowel, trailing];
    }

    /// <summary>
    /// <c>UnicodeData.txt</c>'s records, with each <c>&lt;..., First&gt;</c>/<c>&lt;..., Last&gt;</c>
    /// pair joined into one range.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=14E42D
    // Broiler-Human:        PENDING
    internal static IEnumerable<(int First, int Last, string[] Fields)> UnicodeDataEntries(string text)
    {
        string[]? opened = null;

        foreach (var raw in text.Split('\n'))
        {
            if (raw.Length == 0)
            {
                continue;
            }

            var fields = raw.Split(';');

            if (fields.Length != 15)
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"a UnicodeData.txt line has {fields.Length} fields, not 15: {raw}"));
            }

            if (fields[1].EndsWith(", First>", StringComparison.Ordinal))
            {
                opened = fields;
                continue;
            }

            if (fields[1].EndsWith(", Last>", StringComparison.Ordinal))
            {
                if (opened is null)
                {
                    throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"UnicodeData.txt closes a range it did not open: {raw}"));
                }

                yield return (UnicodeDataFile.Hex(opened[0]), UnicodeDataFile.Hex(fields[0]), opened);
                opened = null;
                continue;
            }

            yield return (UnicodeDataFile.Hex(fields[0]), UnicodeDataFile.Hex(fields[0]), fields);
        }
    }
}

/// <summary>One row of an ECMAScript property table: a name the language admits and its canonical name.</summary>
// Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=C02C36
// Broiler-Human:        PENDING
internal sealed record UnicodeSpecName(string Name, string Canonical);

/// <summary>
/// The three ECMAScript property tables, read out of the archived HTML, and the cross-check
/// against <c>PropertyAliases.txt</c> that JSD-0031 section 5 asks for.
/// </summary>
// Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=1; Fingerprint=B604BD
// Broiler-Human:        PENDING
internal static class UnicodeSpecTables
{
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=28AAAC
    // Broiler-Human:        PENDING
    internal const string NonBinary = "table-nonbinary-unicode-properties.html";
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=C5F2D9
    // Broiler-Human:        PENDING
    internal const string Binary = "table-binary-unicode-properties.html";
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Low; Resources=0; Fingerprint=0A9EB6
    // Broiler-Human:        PENDING
    internal const string Strings = "table-binary-unicode-properties-of-strings.html";

    /// <summary>
    /// The three binary names the language takes from UTS #18 rather than from the UCD. The table
    /// links each of them to UTS #18's General_Category section and <c>PropertyAliases.txt</c>
    /// lists none of them, so they are the one place the cross-check has nothing to compare.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=6C5ADB
    // Broiler-Human:        PENDING
    internal static bool IsUts18Name(string name) => name is "ASCII" or "Any" or "Assigned";

    /// <summary>
    /// Every row of one table: the first cell is the name, and a row that opens a group also names
    /// the canonical name, which the rows below it inherit through <c>rowspan</c>. The tables are read
    /// by plain ordinal search, with no regular expression (convention 9): a row runs from <c>&lt;tr&gt;</c>
    /// to the next <c>&lt;/tr&gt;</c>, and a cell from the end of a <c>&lt;td</c> tag to the next
    /// <c>&lt;/td&gt;</c>.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=05AED0
    // Broiler-Human:        PENDING
    internal static IReadOnlyList<UnicodeSpecName> Rows(string html)
    {
        var rows = new List<UnicodeSpecName>();
        string? canonical = null;

        foreach (var row in Between(html, "<tr>", "</tr>"))
        {
            var cells = Cells(row).Select(Cell).ToArray();

            if (cells.Length == 0)
            {
                continue;
            }

            if (cells.Length > 1)
            {
                canonical = cells[1];
            }

            rows.Add(new UnicodeSpecName(
                cells[0],
                canonical ?? cells[0]));
        }

        return rows;

        static IEnumerable<string> Between(string text, string open, string close)
        {
            var at = 0;

            while (true)
            {
                var start = text.IndexOf(open, at, StringComparison.Ordinal);
                var end = start < 0 ? -1 : text.IndexOf(close, start + open.Length, StringComparison.Ordinal);

                if (end < 0)
                {
                    yield break;
                }

                yield return text[(start + open.Length)..end];
                at = end + close.Length;
            }
        }

        static IEnumerable<string> Cells(string row)
        {
            var at = 0;

            while (true)
            {
                var tag = row.IndexOf("<td", at, StringComparison.Ordinal);
                var open = tag < 0 ? -1 : row.IndexOf('>', tag);
                var end = open < 0 ? -1 : row.IndexOf("</td>", open + 1, StringComparison.Ordinal);

                if (end < 0)
                {
                    yield break;
                }

                yield return row[(open + 1)..end];
                at = end + "</td>".Length;
            }
        }

        // Every tag removed: a '<', at least one character that is not '>', and the '>' that closes it.
        static string WithoutTags(string html)
        {
            var text = new System.Text.StringBuilder(html.Length);
            var at = 0;

            while (at < html.Length)
            {
                var close = html[at] == '<' ? html.IndexOf('>', at + 1) : -1;

                if (close > at + 1)
                {
                    at = close + 1;
                    continue;
                }

                text.Append(html[at]);
                at++;
            }

            return text.ToString();
        }

        static string Cell(string html)
        {
            var text = WithoutTags(html).Trim();

            if (text.Length < 2 || text[0] != '`' || text[^1] != '`')
            {
                throw new InvalidDataException(string.Create(NumberFormatInfo.InvariantInfo, $"a property table cell reads `{text}`, not a `name`"));
            }

            return text[1..^1];
        }
    }

    /// <summary>
    /// Every disagreement between the names the ECMAScript tables admit and the names
    /// <c>PropertyAliases.txt</c> gives the same property. Empty when they agree.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The direction checked is the one that can be wrong in a way that matters: every name the
    /// language admits must be a UCD name or alias OF THE PROPERTY THE TABLE SAYS IT IS, and the
    /// table's canonical column must be the UCD's long name. The other direction is not a
    /// disagreement - the UCD lists aliases the language does not admit (<c>WSpace</c> beside
    /// <c>White_Space</c> and <c>space</c>, for one), and the language's answer to those is a
    /// SyntaxError.
    /// </para>
    /// <para>
    /// The string-property table is checked here for one fact: none of its names is admitted as a
    /// code point property, because only the <c>v</c> flag admits them, and only through
    /// <c>JsUnicodeStringProperties</c>. Its rows are read again by <c>UnicodeDatabase</c>, which holds
    /// them to the two emoji sequence files (phase F2).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=4; Fingerprint=E35DB1
    // Broiler-Human:        PENDING
    internal static IEnumerable<string> Disagreements(
        IReadOnlyList<UnicodeSpecName> nonBinary,
        IReadOnlyList<UnicodeSpecName> binary,
        IReadOnlyList<UnicodeSpecName> strings,
        string propertyAliases)
    {
        var aliases = PropertyAliases(propertyAliases);

        foreach (var row in nonBinary.Concat(binary))
        {
            if (IsUts18Name(row.Canonical))
            {
                if (!string.Equals(row.Name, row.Canonical, StringComparison.Ordinal))
                {
                    yield return string.Create(NumberFormatInfo.InvariantInfo, $"the table gives `{row.Canonical}` an alias `{row.Name}`, and UTS #18 names it alone");
                }

                continue;
            }

            if (!aliases.TryGetValue(row.Canonical, out var names))
            {
                yield return string.Create(NumberFormatInfo.InvariantInfo, $"the table's canonical name `{row.Canonical}` is not a long name in PropertyAliases.txt");
                continue;
            }

            if (!names.Contains(row.Name))
            {
                yield return string.Create(NumberFormatInfo.InvariantInfo, $"the table admits `{row.Name}` for `{row.Canonical}`, and PropertyAliases.txt gives that property ") +
                    string.Create(NumberFormatInfo.InvariantInfo, $"only {string.Join(", ", names.Order(StringComparer.Ordinal).Select(static name => $"`{name}`"))}");
            }
        }

        var admitted = nonBinary.Concat(binary).Select(static row => row.Name).ToHashSet(StringComparer.Ordinal);

        foreach (var row in strings.Where(row => admitted.Contains(row.Name)))
        {
            yield return string.Create(NumberFormatInfo.InvariantInfo, $"`{row.Name}` is a property of strings, and the code point tables also admit it");
        }

        foreach (var duplicate in nonBinary.Concat(binary).GroupBy(static row => row.Name, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            yield return string.Create(NumberFormatInfo.InvariantInfo, $"the tables admit `{duplicate.Key}` more than once");
        }
    }

    /// <summary><c>PropertyAliases.txt</c>: each long name, with every name the file gives that property.</summary>
    // Broiler-AI:           Origin=Ported; Spec=UCD 17.0.0 via Broiler.VM UnicodeDatabase; IP=Low; Security=Medium; Resources=2; Fingerprint=F67EE3
    // Broiler-Human:        PENDING
    internal static IReadOnlyDictionary<string, HashSet<string>> PropertyAliases(string text)
    {
        var byLongName = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var fields in UnicodeDataFile.Records(text))
        {
            if (fields.Length < 2)
            {
                continue;
            }

            var names = byLongName.TryGetValue(fields[1], out var found)
                ? found
                : byLongName[fields[1]] = new HashSet<string>(StringComparer.Ordinal);

            foreach (var name in fields)
            {
                names.Add(name);
            }
        }

        return byLongName;
    }
}
