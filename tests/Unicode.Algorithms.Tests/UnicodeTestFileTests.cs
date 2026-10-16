using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Segmentation;
using Broiler.Unicode.Tables;
using Xunit;
using Xunit.Abstractions;

namespace Broiler.Unicode.Algorithms.Tests;

/// <summary>
/// Bundle sjl-7-001's part A (exit clause 3; the programme's layer L4): every case of Unicode's own test
/// files, read from the pinned archive, with no case excluded.
/// </summary>
public sealed class UnicodeTestFileTests(ITestOutputHelper output)
{
    private static readonly UnicodeNormalization Normalization = new(UnicodeTables.Instance);

    private static readonly UnicodeCaseFolding Folding = new(UnicodeTables.Instance);

    private static readonly UnicodeCaseMapping Casing = new(UnicodeTables.Instance);

    private static readonly UnicodeSegmenter Segmenter = new(UnicodeTables.Instance);

    /// <summary>A1: every line of NormalizationTest.txt's parts 0 to 3, with the invariants its header states.</summary>
    [Fact]
    public void A1_NormalizationTest_every_line()
    {
        var failures = new List<string>();
        var lines = 0;

        foreach (var (fields, _) in Records("ucd-17.0.0/NormalizationTest.txt"))
        {
            if (fields.Length < 5)
            {
                continue;
            }

            lines++;
            var c = fields.Take(5).Select(static field => Text(field)).ToArray();

            void Check(NormalForm form, int source, int expected)
            {
                var actual = Apply(form, c[source]);

                if (!string.Equals(actual, c[expected], StringComparison.Ordinal))
                {
                    failures.Add($"{form}(c{source + 1}) of `{string.Join(";", fields.Take(5))}` is {Hex(actual)}, not c{expected + 1}");
                }
            }

            for (var source = 0; source < 3; source++)
            {
                Check(NormalForm.Nfc, source, 1);
                Check(NormalForm.Nfd, source, 2);
            }

            for (var source = 3; source < 5; source++)
            {
                Check(NormalForm.Nfc, source, 3);
                Check(NormalForm.Nfd, source, 4);
            }

            for (var source = 0; source < 5; source++)
            {
                Check(NormalForm.Nfkc, source, 3);
                Check(NormalForm.Nfkd, source, 4);
            }
        }

        output.WriteLine($"A1 NormalizationTest.txt: {lines} lines run, {lines * 20} comparisons, {failures.Count} failed");
        Assert.True(lines > 19000, $"read {lines} lines");
        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(20)));
    }

    /// <summary>A1: part 1's closing claim, that every code point not in its c1 column is unchanged by all four forms, surrogates included.</summary>
    [Fact]
    public void A1_NormalizationTest_part_1_closing_claim()
    {
        var listed = new HashSet<int>();
        var part = string.Empty;

        foreach (var raw in Archive.Text("ucd-17.0.0/NormalizationTest.txt").Split('\n'))
        {
            if (raw.StartsWith("@Part", StringComparison.Ordinal))
            {
                part = raw.Split(' ')[0];
                continue;
            }

            var line = raw.Split('#')[0].Trim();

            if (part == "@Part1" && line.Length > 0)
            {
                listed.Add(int.Parse(line.Split(';')[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
        }

        Assert.True(listed.Count > 10000, $"part 1 lists {listed.Count} code points");

        var failures = new List<string>();

        for (var codePoint = 0; codePoint <= UnicodeAlgorithms.MaxCodePoint; codePoint++)
        {
            if (listed.Contains(codePoint))
            {
                continue;
            }

            var text = codePoint is >= 0xD800 and <= 0xDFFF ? ((char)codePoint).ToString() : char.ConvertFromUtf32(codePoint);

            foreach (var form in new[] { NormalForm.Nfc, NormalForm.Nfd, NormalForm.Nfkc, NormalForm.Nfkd })
            {
                if (!string.Equals(Apply(form, text), text, StringComparison.Ordinal))
                {
                    failures.Add($"U+{codePoint:X4} changes under {form}");
                }
            }
        }

        output.WriteLine($"A1 part 1's closing claim: {UnicodeAlgorithms.MaxCodePoint + 1 - listed.Count} code points run in four forms, {failures.Count} failed");
        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(20)));
    }

    /// <summary>A2: every line of the three break test files.</summary>
    [Theory]
    [InlineData("GraphemeBreakTest.txt", SegmentGranularity.Grapheme)]
    [InlineData("WordBreakTest.txt", SegmentGranularity.Word)]
    [InlineData("SentenceBreakTest.txt", SegmentGranularity.Sentence)]
    public void A2_break_tests_every_line(string file, SegmentGranularity granularity)
    {
        var failures = new List<string>();
        var lines = 0;

        foreach (var raw in Archive.Text("ucd-17.0.0/auxiliary/" + file).Split('\n'))
        {
            var line = raw.Split('#')[0].Trim();

            if (line.Length == 0)
            {
                continue;
            }

            lines++;
            var text = new StringBuilder();
            var expected = new List<int>();

            foreach (var token in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (token == "÷")
                {
                    expected.Add(text.Length);
                }
                else if (token != "×")
                {
                    var codePoint = int.Parse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                    text.Append(codePoint is >= 0xD800 and <= 0xDFFF ? ((char)codePoint).ToString() : char.ConvertFromUtf32(codePoint));
                }
            }

            var actual = Segmenter.Boundaries(text.ToString(), granularity, null);

            if (!actual.SequenceEqual(expected))
            {
                failures.Add($"{line} gave {string.Join(",", actual)}");
            }
        }

        output.WriteLine($"A2 {file}: {lines} lines run, {failures.Count} failed");
        Assert.True(lines > 400, $"read {lines} lines");
        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(20)));
    }

    /// <summary>A3: CaseFolding.txt's status C and S lines, the identity elsewhere, and every code point's orbit.</summary>
    [Fact]
    public void A3_CaseFolding_every_line_and_every_orbit()
    {
        var folding = new Dictionary<int, int>();

        foreach (var (fields, _) in Records("ucd-17.0.0/CaseFolding.txt"))
        {
            if (fields[1] is "C" or "S")
            {
                folding.Add(Code(fields[0]), Code(fields[2]));
            }
        }

        Assert.True(folding.Count > 1400, $"read {folding.Count} foldings");

        var members = new Dictionary<int, List<int>>();

        for (var codePoint = 0; codePoint <= UnicodeAlgorithms.MaxCodePoint; codePoint++)
        {
            var target = folding.TryGetValue(codePoint, out var mapped) ? mapped : codePoint;

            if (!members.TryGetValue(target, out var list))
            {
                if (target == codePoint && !folding.ContainsValue(codePoint))
                {
                    continue;
                }

                members[target] = list = [];
            }

            list.Add(codePoint);
        }

        var failures = new List<string>();
        Span<int> orbit = stackalloc int[Folding.MaxOrbit];

        for (var codePoint = 0; codePoint <= UnicodeAlgorithms.MaxCodePoint; codePoint++)
        {
            var target = folding.TryGetValue(codePoint, out var mapped) ? mapped : codePoint;

            if (Folding.SimpleFold(codePoint) != target)
            {
                failures.Add($"U+{codePoint:X4} folds to U+{Folding.SimpleFold(codePoint):X4}, not U+{target:X4}");
            }

            var expected = members.TryGetValue(target, out var list) ? list.Order().ToArray() : [codePoint];
            var count = Folding.Orbit(codePoint, orbit);

            if (!orbit[..count].SequenceEqual(expected))
            {
                failures.Add($"U+{codePoint:X4}'s orbit is {string.Join(" ", orbit[..count].ToArray().Select(static x => x.ToString("X4", CultureInfo.InvariantCulture)))}");
            }
        }

        output.WriteLine($"A3 CaseFolding.txt: {folding.Count} C and S lines, {UnicodeAlgorithms.MaxCodePoint + 1} code points folded and orbited, {failures.Count} failed");
        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(20)));
    }

    /// <summary>A4: every code point alone in the default language, from SpecialCasing.txt's unconditional lines over UnicodeData.txt.</summary>
    [Fact]
    public void A4_every_code_point_alone_in_the_default_language()
    {
        var upper = new Dictionary<int, string>();
        var lower = new Dictionary<int, string>();

        foreach (var raw in Archive.Text("ucd-17.0.0/UnicodeData.txt").Split('\n'))
        {
            var fields = raw.Split(';');

            if (fields.Length < 15)
            {
                continue;
            }

            if (fields[12].Length > 0)
            {
                upper[Code(fields[0])] = Text(fields[12]);
            }

            if (fields[13].Length > 0)
            {
                lower[Code(fields[0])] = Text(fields[13]);
            }
        }

        foreach (var (fields, _) in Records("ucd-17.0.0/SpecialCasing.txt"))
        {
            if (fields.Length > 4 && fields[4].Length > 0)
            {
                continue;
            }

            upper[Code(fields[0])] = Text(fields[3]);
            lower[Code(fields[0])] = Text(fields[1]);
        }

        var failures = new List<string>();

        for (var codePoint = 0; codePoint <= UnicodeAlgorithms.MaxCodePoint; codePoint++)
        {
            var text = codePoint is >= 0xD800 and <= 0xDFFF ? ((char)codePoint).ToString() : char.ConvertFromUtf32(codePoint);
            var expectedUpper = upper.TryGetValue(codePoint, out var u) ? u : text;
            var expectedLower = lower.TryGetValue(codePoint, out var l) ? l : text;

            Assert.Equal(UnicodeStatus.Success, Casing.ToUpperCase(text, CaseLanguage.Default, int.MaxValue, null, out var actualUpper));
            Assert.Equal(UnicodeStatus.Success, Casing.ToLowerCase(text, CaseLanguage.Default, int.MaxValue, null, out var actualLower));

            if (!string.Equals(actualUpper, expectedUpper, StringComparison.Ordinal))
            {
                failures.Add($"upper U+{codePoint:X4} is {Hex(actualUpper!)}, not {Hex(expectedUpper)}");
            }

            if (!string.Equals(actualLower, expectedLower, StringComparison.Ordinal))
            {
                failures.Add($"lower U+{codePoint:X4} is {Hex(actualLower!)}, not {Hex(expectedLower)}");
            }
        }

        output.WriteLine($"A4 every code point alone: {UnicodeAlgorithms.MaxCodePoint + 1} code points upper- and lower-cased, {failures.Count} failed");
        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(20)));
    }

    /// <summary>
    /// A4: every conditional line of SpecialCasing.txt, in a context where its condition holds and one where
    /// it does not. The cases are keyed by the line's code point and condition list, and every conditional
    /// line of the file must have its cases here.
    /// </summary>
    [Fact]
    public void A4_every_conditional_line_where_it_holds_and_where_it_does_not()
    {
        var cases = new (string Line, CaseLanguage Language, bool Upper, string Input, string Expected)[]
        {
            ("03A3 Final_Sigma", CaseLanguage.Default, false, "AΣ", "aς"),
            ("03A3 Final_Sigma", CaseLanguage.Default, false, "A.Σ", "a.ς"),
            ("03A3 Final_Sigma", CaseLanguage.Default, false, "Σ", "σ"),
            ("03A3 Final_Sigma", CaseLanguage.Default, false, "AΣB", "aσb"),
            ("0307 lt After_Soft_Dotted", CaseLanguage.Lithuanian, true, "i̇", "I"),
            ("0307 lt After_Soft_Dotted", CaseLanguage.Lithuanian, true, "j̣̇", "J̣"),
            ("0307 lt After_Soft_Dotted", CaseLanguage.Lithuanian, true, "ȧ", "Ȧ"),
            ("0307 lt After_Soft_Dotted", CaseLanguage.Lithuanian, true, "í̇", "Í̇"),
            ("0049 lt More_Above", CaseLanguage.Lithuanian, false, "Ì", "i̇̀"),
            ("0049 lt More_Above", CaseLanguage.Lithuanian, false, "Ị̀", "ị̇̀"),
            ("0049 lt More_Above", CaseLanguage.Lithuanian, false, "I", "i"),
            ("0049 lt More_Above", CaseLanguage.Lithuanian, false, "Ià", "ià"),
            ("004A lt More_Above", CaseLanguage.Lithuanian, false, "J̀", "j̇̀"),
            ("004A lt More_Above", CaseLanguage.Lithuanian, false, "J", "j"),
            ("012E lt More_Above", CaseLanguage.Lithuanian, false, "Į̀", "į̇̀"),
            ("012E lt More_Above", CaseLanguage.Lithuanian, false, "Į", "į"),
            ("00CC lt", CaseLanguage.Lithuanian, false, "Ì", "i̇̀"),
            ("00CC lt", CaseLanguage.Default, false, "Ì", "ì"),
            ("00CD lt", CaseLanguage.Lithuanian, false, "Í", "i̇́"),
            ("00CD lt", CaseLanguage.Default, false, "Í", "í"),
            ("0128 lt", CaseLanguage.Lithuanian, false, "Ĩ", "i̇̃"),
            ("0128 lt", CaseLanguage.Default, false, "Ĩ", "ĩ"),
            ("0130 tr", CaseLanguage.Turkic, false, "İ", "i"),
            ("0130 tr", CaseLanguage.Default, false, "İ", "i̇"),
            ("0307 tr After_I", CaseLanguage.Turkic, false, "İ", "i"),
            ("0307 tr After_I", CaseLanguage.Turkic, false, "Ị̇", "ị"),
            ("0307 tr After_I", CaseLanguage.Turkic, false, "ȧ", "ȧ"),
            ("0049 tr Not_Before_Dot", CaseLanguage.Turkic, false, "I", "ı"),
            ("0049 tr Not_Before_Dot", CaseLanguage.Turkic, false, "İ", "i"),
            ("0049 tr Not_Before_Dot", CaseLanguage.Default, false, "I", "i"),
            ("0069 tr", CaseLanguage.Turkic, true, "i", "İ"),
            ("0069 tr", CaseLanguage.Default, true, "i", "I"),
        };

        var lines = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (fields, _) in Records("ucd-17.0.0/SpecialCasing.txt"))
        {
            if (fields.Length > 4 && fields[4].Length > 0)
            {
                var conditions = fields[4].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Azeri's lines are Turkish's, condition for condition: one language of the enumeration.
                lines.Add(fields[0] + " " + string.Join(' ', conditions.Select(static condition => condition == "az" ? "tr" : condition)));
            }
        }

        Assert.Equal(lines.Order(StringComparer.Ordinal), cases.Select(static c => c.Line).Distinct().Order(StringComparer.Ordinal));

        foreach (var (line, language, upper, input, expected) in cases)
        {
            var status = upper
                ? Casing.ToUpperCase(input, language, int.MaxValue, null, out var actual)
                : Casing.ToLowerCase(input, language, int.MaxValue, null, out actual);

            Assert.Equal(UnicodeStatus.Success, status);
            Assert.True(string.Equals(actual, expected, StringComparison.Ordinal), $"{line}: `{Hex(input)}` gave {Hex(actual!)}, not {Hex(expected)}");
        }

        output.WriteLine($"A4 SpecialCasing.txt's conditional lines: {lines.Count} lines, {cases.Length} cases run, 0 failed");
    }

    internal static IEnumerable<(string[] Fields, string Comment)> Records(string file)
    {
        foreach (var raw in Archive.Text(file).Split('\n'))
        {
            var hash = raw.IndexOf('#', StringComparison.Ordinal);
            var line = (hash >= 0 ? raw[..hash] : raw).Trim();

            if (line.Length > 0 && !line.StartsWith('@'))
            {
                yield return (line.Split(';').Select(static field => field.Trim()).ToArray(), hash >= 0 ? raw[(hash + 1)..].Trim() : string.Empty);
            }
        }
    }

    internal static int Code(string field) => int.Parse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

    /// <summary>A UTF-16 string of space-separated hexadecimal code points; a surrogate code point is one code unit.</summary>
    internal static string Text(string field)
    {
        var text = new StringBuilder();

        foreach (var part in field.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var codePoint = Code(part);

            text.Append(codePoint is >= 0xD800 and <= 0xDFFF ? ((char)codePoint).ToString() : char.ConvertFromUtf32(codePoint));
        }

        return text.ToString();
    }

    internal static string Hex(string text)
    {
        var parts = new List<string>();

        for (var at = 0; at < text.Length;)
        {
            var codePoint = char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1])
                ? char.ConvertToUtf32(text[at], text[at + 1])
                : text[at];

            parts.Add(codePoint.ToString("X4", CultureInfo.InvariantCulture));
            at += codePoint > 0xFFFF ? 2 : 1;
        }

        return "[" + string.Join(" ", parts) + "]";
    }

    private static string Apply(NormalForm form, string text)
    {
        Assert.Equal(UnicodeStatus.Success, Normalization.Transform(text, form, int.MaxValue, null, out var result));
        return result!;
    }
}
