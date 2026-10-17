using System;
using System.Collections.Generic;
using System.Linq;
using Broiler.Unicode.Tables;
using Xunit;

namespace Broiler.Unicode.Algorithms.Tests;

/// <summary>
/// Broiler.Unicode's roadmap, item U3, and bundle sjl-7-001's part A, case A6: every set the lookups answer,
/// compared range for range with the set the pinned UCD files state, read here by a parser of their own.
/// The code points of every General_Category, binary property, Script and Script_Extensions value
/// ECMA-262 admits, and the per-code-point classes the engines read: ID_Start, ID_Continue,
/// Space_Separator, White_Space, Soft_Dotted and the lexical grammar's three.
/// </summary>
public sealed class UcdPropertyTests
{
    private const int CodeSpace = 0x110000;

    /// <summary>The two-letter General_Category values in <see cref="GeneralCategory"/>'s order.</summary>
    private const string CategoryOrder = "CcCfCnCoCsLlLmLoLtLuMcMeMnNdNlNoPcPdPePfPiPoPsScSkSmSoZlZpZs";

    private static readonly UnicodeProperties Properties = new(UnicodeTables.Instance);

    private static readonly EcmaScriptLexical Lexical = new(Properties);

    /// <summary>The binary-property files of the pin, each line <c>range ; Property</c>.</summary>
    private static readonly string[] BinaryFiles =
    [
        "ucd-17.0.0/PropList.txt",
        "ucd-17.0.0/DerivedCoreProperties.txt",
        "ucd-17.0.0/emoji/emoji-data.txt",
        "ucd-17.0.0/extracted/DerivedBinaryProperties.txt",
        "ucd-17.0.0/DerivedNormalizationProps.txt",
    ];

    [Fact]
    public void A6_every_code_point_s_General_Category_is_the_UCD_s()
    {
        var expected = new string[CodeSpace];

        foreach (var (fields, _) in UnicodeTestFileTests.Records("ucd-17.0.0/extracted/DerivedGeneralCategory.txt"))
        {
            var (first, last) = Range(fields[0]);

            for (var codePoint = first; codePoint <= last; codePoint++)
            {
                expected[codePoint] = fields[1];
            }
        }

        var failures = new List<string>();

        for (var codePoint = 0; codePoint < CodeSpace; codePoint++)
        {
            var actual = Properties.GeneralCategoryOf(codePoint);
            var name = CategoryOrder.Substring((int)actual * 2, 2);

            if (!string.Equals(name, expected[codePoint] ?? "Cn", StringComparison.Ordinal))
            {
                failures.Add($"U+{codePoint:X4} is {name}, and the UCD says {expected[codePoint] ?? "Cn"}");
            }
        }

        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(20)));

        for (var category = 0; category < 30; category++)
        {
            var name = CategoryOrder.Substring(category * 2, 2);
            AssertSame(Properties.Category((GeneralCategory)category), Members(codePoint => (expected[codePoint] ?? "Cn") == name), "gc=" + name);
        }

        AssertSame(Properties.SpaceSeparator, Members(codePoint => expected[codePoint] == "Zs"), "Space_Separator");
    }

    [Fact]
    public void A6_every_binary_property_ECMA_262_admits_is_the_UCD_s_set()
    {
        var stated = new Dictionary<string, bool[]>(StringComparer.Ordinal);

        foreach (var file in BinaryFiles)
        {
            foreach (var (fields, _) in UnicodeTestFileTests.Records(file))
            {
                if (fields.Length != 2)
                {
                    continue;
                }

                if (!stated.TryGetValue(fields[1], out var members))
                {
                    stated[fields[1]] = members = new bool[CodeSpace];
                }

                var (first, last) = Range(fields[0]);
                Array.Fill(members, true, first, last - first + 1);
            }
        }

        // Changes_When_NFKC_Casefolded is written CWKCF in DerivedNormalizationProps.txt, its short name.
        if (stated.Remove("CWKCF", out var cwkcf))
        {
            stated["Changes_When_NFKC_Casefolded"] = cwkcf;
        }

        var assigned = Assigned();
        var canonicals = EcmaScriptTableTests.Rows("table-binary-unicode-properties.html").Select(static row => row.Canonical).Distinct(StringComparer.Ordinal).ToArray();

        Assert.True(canonicals.Length > 50, $"read {canonicals.Length} properties");

        foreach (var canonical in canonicals)
        {
            var expected = canonical switch
            {
                "Any" => Members(static _ => true),
                "ASCII" => Members(static codePoint => codePoint < 0x80),
                "Assigned" => Members(codePoint => assigned[codePoint]),
                _ => stated.TryGetValue(canonical, out var members)
                    ? Members(codePoint => members[codePoint])
                    : throw new InvalidOperationException($"no pinned file states {canonical}"),
            };

            Assert.True(Properties.TryResolve(canonical, out var set), $"`{canonical}` is not admitted");
            AssertSame(set, expected, canonical);
        }

        AssertSame(Properties.IdStart, Members(codePoint => stated["ID_Start"][codePoint]), "IdStart");
        AssertSame(Properties.IdContinue, Members(codePoint => stated["ID_Continue"][codePoint]), "IdContinue");
        AssertSame(Properties.WhiteSpace, Members(codePoint => stated["White_Space"][codePoint]), "WhiteSpace");
        AssertSame(Properties.SoftDotted, Members(codePoint => stated["Soft_Dotted"][codePoint]), "SoftDotted");
    }

    [Fact]
    public void A6_every_Script_and_Script_Extensions_value_is_the_UCD_s_set()
    {
        var longName = new Dictionary<string, string>(StringComparer.Ordinal);

        // A PropertyValueAliases.txt line is `sc ; short ; long [; other]`; a line whose short and long
        // names agree has one name after its duplicates are dropped.
        foreach (var aliases in EcmaScriptTableTests.ValueAliases("sc"))
        {
            foreach (var alias in aliases)
            {
                longName[alias] = aliases[Math.Min(1, aliases.Length - 1)];
            }
        }

        var script = new string[CodeSpace];
        Array.Fill(script, "Unknown");

        foreach (var (fields, _) in UnicodeTestFileTests.Records("ucd-17.0.0/Scripts.txt"))
        {
            var (first, last) = Range(fields[0]);
            Array.Fill(script, fields[1], first, last - first + 1);
        }

        var extensions = new string[]?[CodeSpace];

        foreach (var (fields, _) in UnicodeTestFileTests.Records("ucd-17.0.0/ScriptExtensions.txt"))
        {
            var (first, last) = Range(fields[0]);
            var names = fields[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(name => longName[name]).ToArray();

            for (var codePoint = first; codePoint <= last; codePoint++)
            {
                extensions[codePoint] = names;
            }
        }

        foreach (var name in longName.Values.Distinct(StringComparer.Ordinal))
        {
            Assert.True(Properties.TryResolve("Script", name, out var sc), $"Script={name} is not admitted");
            AssertSame(sc, Members(codePoint => script[codePoint] == name), "Script=" + name);

            // UAX #24: a code point ScriptExtensions.txt does not list has its Script as its only extension.
            Assert.True(Properties.TryResolve("Script_Extensions", name, out var scx), $"Script_Extensions={name} is not admitted");
            AssertSame(scx, Members(codePoint => extensions[codePoint] is { } listed ? listed.Contains(name) : script[codePoint] == name), "Script_Extensions=" + name);
        }
    }

    [Fact]
    public void A6_the_lexical_classes_are_the_clauses_sets_over_the_UCD_s()
    {
        for (var codePoint = 0; codePoint < CodeSpace; codePoint++)
        {
            var surrogate = codePoint is >= 0xD800 and <= 0xDFFF;
            var start = !surrogate && (Properties.IdStart.Contains(codePoint) || codePoint is '$' or '_');
            var part = !surrogate && (Properties.IdContinue.Contains(codePoint) || codePoint is '$' or 0x200C or 0x200D);
            var white = codePoint is 0x09 or 0x0B or 0x0C or 0xFEFF || Properties.SpaceSeparator.Contains(codePoint);

            if (Lexical.IsIdentifierStart(codePoint) != start || Lexical.IsIdentifierPart(codePoint) != part || Lexical.IsWhiteSpace(codePoint) != white)
            {
                Assert.Fail($"U+{codePoint:X4}: start {Lexical.IsIdentifierStart(codePoint)}, part {Lexical.IsIdentifierPart(codePoint)}, white space {Lexical.IsWhiteSpace(codePoint)}");
            }
        }

        Assert.False(Lexical.IsWhiteSpace(0x85));
        Assert.False(Lexical.IsIdentifierPart(0x2E2F));
        Assert.True(EcmaScriptLexical.IsLineTerminator(0x2028));
        Assert.False(EcmaScriptLexical.IsLineTerminator(0x85));
    }

    private static bool[] Assigned()
    {
        var assigned = new bool[CodeSpace];

        foreach (var (fields, _) in UnicodeTestFileTests.Records("ucd-17.0.0/extracted/DerivedGeneralCategory.txt"))
        {
            if (fields[1] != "Cn")
            {
                var (first, last) = Range(fields[0]);
                Array.Fill(assigned, true, first, last - first + 1);
            }
        }

        return assigned;
    }

    private static (int First, int Last) Range(string field)
    {
        var dots = field.IndexOf("..", StringComparison.Ordinal);

        return dots < 0
            ? (UnicodeTestFileTests.Code(field), UnicodeTestFileTests.Code(field))
            : (UnicodeTestFileTests.Code(field[..dots]), UnicodeTestFileTests.Code(field[(dots + 2)..]));
    }

    /// <summary>The ranges of the code points <paramref name="member"/> admits, in order and merged.</summary>
    private static List<(int First, int Last)> Members(Func<int, bool> member)
    {
        var ranges = new List<(int, int)>();
        var start = -1;

        for (var codePoint = 0; codePoint <= CodeSpace; codePoint++)
        {
            var inside = codePoint < CodeSpace && member(codePoint);

            if (inside && start < 0)
            {
                start = codePoint;
            }
            else if (!inside && start >= 0)
            {
                ranges.Add((start, codePoint - 1));
                start = -1;
            }
        }

        return ranges;
    }

    private static void AssertSame(CodePointSet set, List<(int First, int Last)> expected, string what)
    {
        var actual = Enumerable.Range(0, set.RangeCount).Select(index => (First: set.First(index), Last: set.Last(index))).ToList();

        if (actual.Count == expected.Count && actual.SequenceEqual(expected))
        {
            return;
        }

        var at = Enumerable.Range(0, Math.Min(actual.Count, expected.Count)).FirstOrDefault(index => actual[index] != expected[index], Math.Min(actual.Count, expected.Count));

        Assert.Fail(
            $"{what}: {actual.Count} ranges, and the UCD states {expected.Count}; the first difference is at range {at}: " +
            $"{(at < actual.Count ? $"{actual[at].First:X4}..{actual[at].Last:X4}" : "none")} against " +
            $"{(at < expected.Count ? $"{expected[at].First:X4}..{expected[at].Last:X4}" : "none")}");
    }
}
