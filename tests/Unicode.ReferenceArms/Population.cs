using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.Unicode.ReferenceArms;

/// <summary>
/// One input of an operation: a code point of U1, a name of U2, or a text of U3 to U7, with the class it
/// belongs to, its line in that class's file, its tag, and how the listing writes it.
/// </summary>
internal sealed class Item
{
    /// <summary>The rule's class, U1 to U7.</summary>
    public required string Class { get; init; }

    /// <summary>The tag of the population's line (empty for U1 and U2): <c>alone</c>, <c>random</c>, <c>casing:...</c>.</summary>
    public string Tag { get; init; } = string.Empty;

    /// <summary>The 1-based line of the class's file; for U1, the code point.</summary>
    public required long Line { get; init; }

    /// <summary>How the listing writes the input: README.md, "Output".</summary>
    public required string Shown { get; init; }

    /// <summary>Whether the input is a <c>repeat</c> line of limits.tsv, whose results the listing writes by length and digest.</summary>
    public bool IsRepeat { get; init; }

    /// <summary>The text, for U1 the code point alone (a surrogate as its one code unit); empty for a name.</summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>The code point, for U1; -1 otherwise.</summary>
    public int CodePoint { get; init; } = -1;

    /// <summary>For a name of U2: <c>lone</c> or <c>pair</c>.</summary>
    public string Kind { get; init; } = string.Empty;

    /// <summary>For a name of U2: the lone name, or the pair's property.</summary>
    public string Property { get; init; } = string.Empty;

    /// <summary>For a pair of U2: its value.</summary>
    public string Value { get; init; } = string.Empty;
}

/// <summary>
/// The population a run reads: the files scripts/reference-arms/generate_population.py writes into one
/// directory (its README.md, "The population", is the format's specification), and U1, every code point from
/// U+0000 to U+10FFFF, which is implicit and read from no file.
/// </summary>
/// <remarks>
/// <para>
/// <b>The files.</b> <c>names.tsv</c> (U2): <c>kind TAB property TAB value</c>, the kind <c>lone</c> (the value
/// empty) or <c>pair</c>, no tag. <c>casing.tsv</c> (U3), <c>normalization.tsv</c> (U4),
/// <c>string-properties.tsv</c> (U5), <c>segmentation.tsv</c> (U6) and <c>limits.tsv</c> (U7): one text per
/// line, UTF-16 code units in 4-digit uppercase hexadecimal separated by single spaces, then <c>TAB tag</c>;
/// <c>limits.tsv</c> also holds <c>repeat TAB piece TAB count TAB suffix TAB tag</c> lines, the text being the
/// piece repeated count times and then the suffix. <c>manifest.json</c> is the generator's; its SHA-256 is
/// recorded. A file that is absent is an empty class, which the summary says.
/// </para>
/// <para>
/// <b>U1.</b> casing.tsv and normalization.tsv hold every code point alone (tag <c>alone</c>), as the rule's U3
/// and U4 begin; the single code points of U5 are not written, so the properties of strings run over U1 here.
/// </para>
/// </remarks>
internal sealed class Population
{
    public const int MaxCodePoint = 0x10FFFF;

    private readonly Dictionary<string, List<Item>> classes = new(StringComparer.Ordinal);

    private Population(string? directory) => Directory = directory;

    /// <summary>The population's directory, or null for a run of U1 alone.</summary>
    public string? Directory { get; }

    /// <summary>The SHA-256 of manifest.json's bytes, or null when there is none.</summary>
    public string? ManifestDigest { get; private set; }

    /// <summary>manifest.json's text, or null.</summary>
    public string? ManifestText { get; private set; }

    /// <summary>The files read, with their SHA-256 and line counts, or "absent".</summary>
    public Dictionary<string, object> Files { get; } = new(StringComparer.Ordinal);

    /// <summary>The file of each class, as the generator names them.</summary>
    public static readonly (string Class, string File)[] ClassFiles =
    [
        ("U2", "names.tsv"),
        ("U3", "casing.tsv"),
        ("U4", "normalization.tsv"),
        ("U5", "string-properties.tsv"),
        ("U6", "segmentation.tsv"),
        ("U7", "limits.tsv"),
    ];

    public static Population Read(string? directory, long? lineLimit)
    {
        var population = new Population(directory);
        foreach (var (name, _) in ClassFiles)
            population.classes[name] = [];

        if (directory is null)
            return population;

        var manifest = Path.Combine(directory, "manifest.json");
        if (File.Exists(manifest))
        {
            var bytes = File.ReadAllBytes(manifest);
            population.ManifestDigest = Convert.ToHexStringLower(SHA256.HashData(bytes));
            population.ManifestText = Encoding.UTF8.GetString(bytes);
        }

        foreach (var (name, file) in ClassFiles)
        {
            var path = Path.Combine(directory, file);
            if (!File.Exists(path))
            {
                population.Files[file] = "absent";
                continue;
            }

            var bytes = File.ReadAllBytes(path);
            var text = new UTF8Encoding(false, true).GetString(bytes);
            var lines = text.Split('\n');
            var count = lines.Length > 0 && lines[^1].Length == 0 ? lines.Length - 1 : lines.Length;
            population.Files[file] = new Dictionary<string, object>
            {
                ["sha256"] = Convert.ToHexStringLower(SHA256.HashData(bytes)),
                ["lines"] = count,
            };

            var items = population.classes[name];
            for (var at = 0; at < count && (lineLimit is not { } most || at < most); at++)
                items.Add(name == "U2" ? NameItem(lines[at], at + 1) : TextItem(name, file, lines[at], at + 1));
        }

        return population;
    }

    /// <summary>The inputs of a file-backed class, in line order.</summary>
    public IReadOnlyList<Item> Items(string name) => classes.TryGetValue(name, out var items) ? items : [];

    /// <summary>U1's inputs from <paramref name="first"/>, at most <paramref name="count"/> of them, generated on demand.</summary>
    public static List<Item> CodePoints(int first, int count)
    {
        var items = new List<Item>(count);
        for (var codePoint = first; codePoint < first + count && codePoint <= MaxCodePoint; codePoint++)
        {
            items.Add(new Item
            {
                Class = "U1",
                Line = codePoint,
                Shown = codePoint.ToString("X4", CultureInfo.InvariantCulture),
                Text = CodePointText(codePoint),
                CodePoint = codePoint,
            });
        }

        return items;
    }

    /// <summary>A code point as a string: a surrogate code point is its one code unit.</summary>
    public static string CodePointText(int codePoint) =>
        codePoint is >= 0xD800 and <= 0xDFFF ? ((char)codePoint).ToString() : char.ConvertFromUtf32(codePoint);

    private static Item NameItem(string line, long number)
    {
        var fields = line.Split('\t');
        if (fields.Length < 2 || fields[0] is not ("lone" or "pair"))
            throw new FormatException($"names.tsv:{number}: expected 'lone|pair TAB property TAB value', read '{line}'");

        var kind = fields[0];
        var property = fields[1];
        var value = fields.Length > 2 ? fields[2] : string.Empty;
        return new Item { Class = "U2", Line = number, Shown = kind + "/" + property + "/" + value, Kind = kind, Property = property, Value = value };
    }

    private static Item TextItem(string name, string file, string line, long number)
    {
        var fields = line.Split('\t');
        if (name == "U7" && fields[0] == "repeat")
        {
            if (fields.Length < 3)
                throw new FormatException($"{file}:{number}: expected 'repeat TAB piece TAB count TAB suffix TAB tag', read '{line}'");

            var piece = Units(fields[1], file, number);
            var times = int.Parse(fields[2], NumberStyles.None, CultureInfo.InvariantCulture);
            var suffix = fields.Length > 3 ? Units(fields[3], file, number) : string.Empty;
            var built = new StringBuilder(checked((piece.Length * times) + suffix.Length));
            for (var i = 0; i < times; i++)
                built.Append(piece);
            built.Append(suffix);

            return new Item
            {
                Class = name,
                Tag = fields.Length > 4 ? fields[4] : string.Empty,
                Line = number,
                Shown = "repeat:" + fields[1] + ":" + times.ToString(CultureInfo.InvariantCulture) + ":" + (fields.Length > 3 ? fields[3] : string.Empty),
                IsRepeat = true,
                Text = built.ToString(),
            };
        }

        var text = Units(fields[0], file, number);
        return new Item
        {
            Class = name,
            Tag = fields.Length > 1 ? fields[1] : string.Empty,
            Line = number,
            Shown = Hex(text),
            Text = text,
        };
    }

    private static string Units(string hex, string file, long number)
    {
        if (hex.Length == 0)
            return string.Empty;

        var tokens = hex.Split(' ');
        var units = new char[tokens.Length];
        for (var i = 0; i < tokens.Length; i++)
        {
            if (tokens[i].Length != 4 || !ushort.TryParse(tokens[i], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var unit))
                throw new FormatException($"{file}:{number}: '{tokens[i]}' is not a 4-digit hexadecimal code unit");
            units[i] = (char)unit;
        }

        return new string(units);
    }

    /// <summary>A text's code units in 4-digit uppercase hexadecimal separated by single spaces, the population's own form; empty for the empty text.</summary>
    public static string Hex(string text)
    {
        if (text.Length == 0)
            return string.Empty;

        var built = new StringBuilder((text.Length * 5) - 1);
        for (var i = 0; i < text.Length; i++)
        {
            if (i > 0)
                built.Append(' ');
            built.Append(((int)text[i]).ToString("X4", CultureInfo.InvariantCulture));
        }

        return built.ToString();
    }

    /// <summary>A text too long to list: <c>len:&lt;code units&gt;:sha256:&lt;SHA-256 of its UTF-16LE bytes&gt;</c>.</summary>
    public static string LengthAndDigest(string text) =>
        "len:" + text.Length.ToString(CultureInfo.InvariantCulture) + ":sha256:" +
        Convert.ToHexStringLower(SHA256.HashData(MemoryMarshal.AsBytes(text.AsSpan())));
}
