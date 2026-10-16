using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Broiler.Unicode.Tables;
using Xunit;

namespace Broiler.Unicode.Algorithms.Tests;

/// <summary>
/// Bundle sjl-7-001's part A, case A5: every name ECMA-262's three property tables and
/// <c>PropertyValueAliases.txt</c> admit resolves, to the property it names, and every name of the refused
/// set is refused (ECMA-262 section 22.2.2.9, UnicodeMatchProperty and UnicodeMatchPropertyValue).
/// </summary>
public sealed class EcmaScriptTableTests
{
    private static readonly UnicodeProperties Properties = new(UnicodeTables.Instance);

    private static readonly UnicodeStringProperties Strings = new(UnicodeTables.Instance);

    [Fact]
    public void A5_every_binary_property_name_and_alias_resolves_to_its_canonical_property()
    {
        var rows = Rows("table-binary-unicode-properties.html");

        Assert.True(rows.Count > 60, $"read {rows.Count} rows");

        foreach (var (name, canonical) in rows)
        {
            Assert.True(Properties.TryResolve(name, out var set), $"`{name}` is not admitted");
            Assert.True(Properties.TryResolve(canonical, out var expected), $"`{canonical}` is not admitted");
            Assert.True(SameSet(set, expected), $"`{name}` does not resolve to `{canonical}`");
            Assert.False(Properties.TryResolve(name, "Y", out _), $"`{name}=Y` is admitted");
        }
    }

    [Fact]
    public void A5_every_General_Category_value_and_alias_resolves_alone_and_as_a_value()
    {
        var properties = Rows("table-nonbinary-unicode-properties.html").Where(static row => row.Canonical == "General_Category").Select(static row => row.Name).ToArray();

        Assert.Equal(["General_Category", "gc"], properties.Order(StringComparer.Ordinal).ToArray());

        foreach (var aliases in ValueAliases("gc"))
        {
            CodePointSet? first = null;

            foreach (var alias in aliases)
            {
                Assert.True(Properties.TryResolve(alias, out var lone), $"gc `{alias}` is not admitted alone");

                foreach (var property in properties)
                {
                    Assert.True(Properties.TryResolve(property, alias, out var pair), $"`{property}={alias}` is not admitted");
                    Assert.True(SameSet(lone, pair), $"`{property}={alias}` and `{alias}` differ");
                }

                first ??= lone;
                Assert.True(SameSet(first.Value, lone), $"the aliases of gc `{aliases[0]}` differ");
            }
        }
    }

    [Fact]
    public void A5_every_Script_value_and_alias_resolves_under_Script_and_Script_Extensions()
    {
        var rows = Rows("table-nonbinary-unicode-properties.html");
        var script = rows.Where(static row => row.Canonical == "Script").Select(static row => row.Name).ToArray();
        var extensions = rows.Where(static row => row.Canonical == "Script_Extensions").Select(static row => row.Name).ToArray();
        var values = ValueAliases("sc").ToArray();

        Assert.True(values.Length > 160, $"read {values.Length} scripts");

        foreach (var aliases in values)
        {
            foreach (var alias in aliases)
            {
                foreach (var property in script.Concat(extensions))
                {
                    Assert.True(Properties.TryResolve(property, alias, out _), $"`{property}={alias}` is not admitted");
                }

            }
        }

        Assert.True(Properties.TryResolve("Script", "Katakana_Or_Hiragana", out var hrkt));
        Assert.True(hrkt.IsEmpty);
        Assert.True(Properties.TryResolve("scx", "Hrkt", out var hrktExtensions));
        Assert.True(hrktExtensions.IsEmpty);
    }

    [Fact]
    public void A5_every_property_of_strings_resolves()
    {
        var rows = Rows("table-binary-unicode-properties-of-strings.html");

        Assert.Equal(7, rows.Count);
        Assert.Equal(7, Strings.PropertyCount);

        foreach (var (name, _) in rows)
        {
            Assert.True(Strings.TryResolve(name, out _), $"`{name}` is not admitted");
            Assert.False(Properties.TryResolve(name, out _), $"`{name}` is admitted as a code point property");
        }
    }

    [Fact]
    public void A5_the_refused_set_is_refused()
    {
        var admitted = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (name, _) in Rows("table-binary-unicode-properties.html"))
        {
            admitted.Add(name);
        }

        foreach (var aliases in ValueAliases("gc"))
        {
            admitted.UnionWith(aliases);
        }

        var stringNames = Rows("table-binary-unicode-properties-of-strings.html").Select(static row => row.Name).ToHashSet(StringComparer.Ordinal);
        var refused = Refused(admitted).Concat(["Age", "Block", "Name", "Bidi_Class", "Line_Break", "blk", "isLatin", "IsLatin", "ASCII=Y"]).ToArray();

        foreach (var name in refused.Where(name => !admitted.Contains(name)))
        {
            Assert.False(Properties.TryResolve(name, out _), $"`{name}` is admitted alone");
        }

        foreach (var name in Refused(stringNames).Where(name => !stringNames.Contains(name)))
        {
            Assert.False(Strings.TryResolve(name, out _), $"`{name}` is admitted as a property of strings");
        }

        foreach (var (property, value) in new[] { ("Age", "1.1"), ("Block", "Basic_Latin"), ("ASCII", "Y"), ("gc", "Latin"), ("sc", "Lu"), ("Script", "latin"), ("script", "Latin"), ("SC", "Latn") })
        {
            Assert.False(Properties.TryResolve(property, value, out _), $"`{property}={value}` is admitted");
        }

        Assert.False(Properties.TryResolve(new string('L', 1000), out _));
        Assert.False(Properties.TryResolve("Lı", out _));
    }

    /// <summary>
    /// reference-arms.md's refused set U2 for <paramref name="names"/>: each with its letter case changed,
    /// an underscore removed, a space or a hyphen inserted, and an <c>Is</c> prefix.
    /// </summary>
    internal static IEnumerable<string> Refused(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            yield return name.ToUpperInvariant();
            yield return name.ToLowerInvariant();
            yield return char.IsUpper(name[0]) ? char.ToLowerInvariant(name[0]) + name[1..] : char.ToUpperInvariant(name[0]) + name[1..];

            if (name.Contains('_', StringComparison.Ordinal))
            {
                yield return name.Replace("_", string.Empty, StringComparison.Ordinal);
                yield return name.Replace('_', ' ');
                yield return name.Replace('_', '-');
            }
            else if (name.Length > 1)
            {
                yield return name[..1] + " " + name[1..];
                yield return name[..1] + "-" + name[1..];
            }

            yield return "Is" + name;
        }
    }

    internal static IReadOnlyList<(string Name, string Canonical)> Rows(string fileName)
    {
        var html = Archive.Text(Archive.Pin.SpecTable(fileName).Name);
        var rows = new List<(string, string)>();
        string? canonical = null;

        foreach (Match row in Regex.Matches(html, "<tr>(.*?)</tr>", RegexOptions.Singleline))
        {
            var cells = Regex.Matches(row.Groups[1].Value, "<td[^>]*>(.*?)</td>", RegexOptions.Singleline)
                .Select(static cell => Regex.Replace(cell.Groups[1].Value, "<[^>]+>", string.Empty).Trim().Trim('`'))
                .ToArray();

            if (cells.Length == 0)
            {
                continue;
            }

            if (cells.Length > 1)
            {
                canonical = cells[1];
            }

            rows.Add((cells[0], canonical ?? cells[0]));
        }

        return rows;
    }

    /// <summary>Each value of <paramref name="property"/> in PropertyValueAliases.txt, with every name it gives the value.</summary>
    internal static IEnumerable<string[]> ValueAliases(string property)
    {
        foreach (var (fields, _) in UnicodeTestFileTests.Records("ucd-17.0.0/PropertyValueAliases.txt"))
        {
            if (fields[0] == property)
            {
                yield return fields.Skip(1).Distinct(StringComparer.Ordinal).ToArray();
            }
        }
    }

    private static bool SameSet(CodePointSet left, CodePointSet right)
    {
        if (left.RangeCount != right.RangeCount)
        {
            return false;
        }

        for (var index = 0; index < left.RangeCount; index++)
        {
            if (left.First(index) != right.First(index) || left.Last(index) != right.Last(index))
            {
                return false;
            }
        }

        return true;
    }
}
