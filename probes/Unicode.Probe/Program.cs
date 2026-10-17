using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Segmentation;
using Broiler.Unicode.Tables;

namespace Broiler.Unicode.Probe;

/// <summary>
/// The probe's entry point: <c>Broiler.Unicode.Probe [archive-directory] [--report path]</c>. It runs
/// Unicode's own test files from the pinned archive - every line of NormalizationTest.txt, the three break
/// tests and CaseFolding.txt, and every code point's default case mappings - through the packages the
/// engines consume, and exits 0 only when every case holds.
/// </summary>
/// <remarks>
/// The report lists each file with its case count, its failure count and the SHA-256 of every answer the
/// packages gave, in order, so that the Native AOT run and the JIT run can be compared byte for byte. It
/// records no time, machine or path.
/// </remarks>
internal static class Program
{
    private static readonly UnicodeNormalization Normalization = new(UnicodeTables.Instance);

    private static readonly UnicodeCaseFolding Folding = new(UnicodeTables.Instance);

    private static readonly UnicodeCaseMapping Casing = new(UnicodeTables.Instance);

    private static readonly UnicodeProperties Properties = new(UnicodeTables.Instance);

    private static readonly UnicodeSegmenter Segmenter = new(UnicodeTables.Instance);

    internal static int Main(string[] args)
    {
        string? root = null;
        string? reportPath = null;

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--report" && i + 1 < args.Length)
            {
                reportPath = args[++i];
            }
            else if (root is null)
            {
                root = args[i];
            }
            else
            {
                Console.Error.WriteLine("usage: Broiler.Unicode.Probe [archive-directory] [--report path]");
                return 2;
            }
        }

        root ??= Path.Combine("data", "unicode", "pins");

        var report = new StringBuilder();
        var failures = 0;

        failures += NormalizationTest(root, report);
        failures += BreakTest(root, "GraphemeBreakTest.txt", SegmentGranularity.Grapheme, report);
        failures += BreakTest(root, "WordBreakTest.txt", SegmentGranularity.Word, report);
        failures += BreakTest(root, "SentenceBreakTest.txt", SegmentGranularity.Sentence, report);
        failures += CaseFolding(root, report);
        failures += CodePoints(report);

        if (reportPath is not null)
        {
            File.WriteAllText(reportPath, report.ToString());
        }

        Console.Out.Write(report.ToString());
        Console.Out.WriteLine(failures == 0 ? "Broiler.Unicode.Probe: every case holds" : "Broiler.Unicode.Probe: " + Decimal(failures) + " cases fail");
        return failures == 0 ? 0 : 1;
    }

    private static int NormalizationTest(string root, StringBuilder report)
    {
        var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var cases = 0;
        var failures = 0;
        NormalForm[] forms = [NormalForm.Nfc, NormalForm.Nfd, NormalForm.Nfkc, NormalForm.Nfkd];

        foreach (var fields in Records(Path.Combine(root, "ucd-17.0.0", "NormalizationTest.txt")))
        {
            if (fields.Length < 5)
            {
                continue;
            }

            var c = new string[5];

            for (var i = 0; i < 5; i++)
            {
                c[i] = Text(fields[i]);
            }

            for (var source = 0; source < 5; source++)
            {
                foreach (var form in forms)
                {
                    Normalization.Transform(c[source], form, int.MaxValue, null, out var result);
                    Add(digest, result!);
                    cases++;

                    var expected = form switch
                    {
                        NormalForm.Nfc => source < 3 ? c[1] : c[3],
                        NormalForm.Nfd => source < 3 ? c[2] : c[4],
                        NormalForm.Nfkc => c[3],
                        _ => c[4],
                    };

                    failures += string.Equals(result, expected, StringComparison.Ordinal) ? 0 : 1;
                }
            }
        }

        return Line(report, "NormalizationTest.txt", cases, failures, digest);
    }

    private static int BreakTest(string root, string file, SegmentGranularity granularity, StringBuilder report)
    {
        var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var cases = 0;
        var failures = 0;

        foreach (var raw in File.ReadAllText(Path.Combine(root, "ucd-17.0.0", "auxiliary", file)).Split('\n'))
        {
            var line = raw.Split('#')[0].Trim();

            if (line.Length == 0)
            {
                continue;
            }

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
                    Append(text, int.Parse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                }
            }

            var boundaries = Segmenter.Boundaries(text.ToString(), granularity, null);
            var rendered = string.Join(",", Array.ConvertAll(boundaries, static b => Decimal(b)));

            Add(digest, rendered);
            cases++;
            failures += string.Equals(rendered, string.Join(",", expected.ConvertAll(static b => Decimal(b))), StringComparison.Ordinal) ? 0 : 1;
        }

        return Line(report, file, cases, failures, digest);
    }

    private static int CaseFolding(string root, StringBuilder report)
    {
        var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var cases = 0;
        var failures = 0;

        foreach (var fields in Records(Path.Combine(root, "ucd-17.0.0", "CaseFolding.txt")))
        {
            if (fields[1] is not ("C" or "S"))
            {
                continue;
            }

            var source = int.Parse(fields[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var target = int.Parse(fields[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var folded = Folding.SimpleFold(source);

            Add(digest, Decimal(folded));
            cases++;
            failures += folded == target ? 0 : 1;
        }

        return Line(report, "CaseFolding.txt", cases, failures, digest);
    }

    /// <summary>Every code point's default case mappings, folding, orbit and lexical classes, digested; nothing is judged here but that each call completes.</summary>
    private static int CodePoints(StringBuilder report)
    {
        var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var lexical = new EcmaScriptLexical(Properties);
        Span<int> orbit = stackalloc int[Folding.MaxOrbit];
        var cases = 0;
        var failures = 0;

        for (var codePoint = 0; codePoint <= UnicodeAlgorithms.MaxCodePoint; codePoint++)
        {
            var text = new StringBuilder();
            Append(text, codePoint);
            var single = text.ToString();

            failures += Casing.ToUpperCase(single, CaseLanguage.Default, int.MaxValue, null, out var upper) == UnicodeStatus.Success ? 0 : 1;
            failures += Casing.ToLowerCase(single, CaseLanguage.Default, int.MaxValue, null, out var lower) == UnicodeStatus.Success ? 0 : 1;
            var count = Folding.Orbit(codePoint, orbit);

            Add(digest, upper + "|" + lower + "|" + Decimal(count) + "|" +
                (lexical.IsIdentifierStart(codePoint) ? "S" : "-") + (lexical.IsIdentifierPart(codePoint) ? "P" : "-") +
                (lexical.IsWhiteSpace(codePoint) ? "W" : "-") + Decimal((int)Properties.GeneralCategoryOf(codePoint)));
            cases++;
        }

        return Line(report, "code points", cases, failures, digest);
    }

    private static int Line(StringBuilder report, string name, int cases, int failures, IncrementalHash digest)
    {
        report.Append(name).Append('\t').Append(Decimal(cases)).Append('\t').Append(Decimal(failures)).Append('\t')
            .Append(Convert.ToHexStringLower(digest.GetHashAndReset())).Append('\n');
        digest.Dispose();
        return cases == 0 ? 1 : failures;
    }

    private static IEnumerable<string[]> Records(string path)
    {
        foreach (var raw in File.ReadAllText(path).Split('\n'))
        {
            var line = raw.Split('#')[0].Trim();

            if (line.Length > 0 && line[0] != '@')
            {
                var fields = line.Split(';');

                for (var i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].Trim();
                }

                yield return fields;
            }
        }
    }

    private static string Text(string field)
    {
        var text = new StringBuilder();

        foreach (var part in field.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            Append(text, int.Parse(part, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        }

        return text.ToString();
    }

    private static void Append(StringBuilder text, int codePoint)
    {
        if (codePoint > 0xFFFF)
        {
            text.Append((char)(0xD7C0 + (codePoint >> 10))).Append((char)(0xDC00 | (codePoint & 0x3FF)));
        }
        else
        {
            text.Append((char)codePoint);
        }
    }

    private static void Add(IncrementalHash digest, string answer)
    {
        digest.AppendData(Encoding.UTF8.GetBytes(answer));
        digest.AppendData("\n"u8);
    }

    private static string Decimal(int value) => value.ToString(CultureInfo.InvariantCulture);
}
