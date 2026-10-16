using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Segmentation;
using Broiler.Unicode.Tables;
using JI = Broiler.JavaScript.BuiltIns.Intl;
using JP = Broiler.JavaScript.Parser;
using JR = Broiler.JavaScript.BuiltIns.RegExp;
using JS = Broiler.JavaScript.BuiltIns.String;
using JX = Broiler.JavaScript.Runtime;
using V = Broiler.VM.Profile.JavaScript;
using VF = Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.Unicode.ReferenceArms;

/// <summary>
/// One answer, compared exactly: its kind, its value, and for casing and normalization against arm V the
/// total charge (-1 for none). <see cref="Render"/> writes it in the listing's notation (README.md).
/// </summary>
internal readonly record struct Result(string Tag, string Value, long Charge = -1)
{
    public static Result Text(string text) => new("t", text);

    public static Result Refused(string kind) => new("refused", kind);

    public static Result Failed(Exception exception) => new("exception", exception.GetType().Name);

    public static Result Bool(bool value) => new("bool", value ? "true" : "false");

    public static Result CodePoint(int codePoint) => new("cp", Arms.Hex(codePoint));

    public static Result CodePoints(ReadOnlySpan<int> codePoints)
    {
        var built = new StringBuilder();
        foreach (var codePoint in codePoints)
            built.Append(built.Length == 0 ? string.Empty : ",").Append(Arms.Hex(codePoint));
        return new("cps", built.ToString());
    }

    public static Result Values(IEnumerable<string> names) => new("values", string.Join(',', names));

    public static Result Set(IEnumerable<(int Lo, int Hi)> ranges) => new("set", Arms.RangesText(ranges));

    public static Result Strings(string digest) => new("strings", digest);

    public static Result Boundaries(IEnumerable<int> positions) =>
        new("b", string.Join(',', positions.Select(static p => p.ToString(CultureInfo.InvariantCulture))));

    public Result WithCharge(ulong charge) => this with { Charge = (long)charge };

    /// <summary>The listing's notation: see README.md, "Output".</summary>
    public string Render()
    {
        var text = Tag switch
        {
            "t" => "t:" + Population.Shown(Value),
            "bool" => Value,
            _ => Tag + ":" + Value,
        };

        return Charge >= 0 ? text + ";charge=" + Charge.ToString(CultureInfo.InvariantCulture) : text;
    }
}

/// <summary>A case: one input of an operation with one value of its parameters.</summary>
/// <param name="Item">The input.</param>
/// <param name="Parameter">How the listing writes the parameters.</param>
/// <param name="A">The operation's first parameter: a property's index, a language, a form, a mode, or a segment's start.</param>
/// <param name="B">The second: the maximum result length (-1 for none), or a segment's end.</param>
internal readonly record struct Case(Item Item, string Parameter, int A = 0, int B = -1);

/// <summary>
/// One arm's implementation of one operation, with S called as that arm's engine will call it.
/// <see cref="Run"/> answers null for a case the arm does not implement, which is counted as not called.
/// </summary>
internal sealed record Variant(string Arm, string Name, Func<Case, Result?> Run, Func<Case, Result> Shared, string SharedKey)
{
    /// <summary>How the listing names the variant: the arm and the member, <c>V:...</c> or <c>J:...</c>.</summary>
    public string Label => Arm + ":" + Name;
}

/// <summary>An operation of the rule: the classes it runs over, how an input becomes its cases, and its variants.</summary>
internal sealed record Operation(string Name, string[] Classes, Func<Item, IEnumerable<Case>> Expand, Variant[] Variants);

/// <summary>
/// The operations of bundle sjl-7-001's reference-arm rule, and for each S and the variants of the arms that
/// implement it. An arm is called only for what it implements; <see cref="NotImplemented"/> names the rest.
/// </summary>
/// <remarks>
/// <para>
/// <b>S under each engine's policy.</b> Arm V is compared with S as Broiler.VM's adapter will call it: with
/// its meter, whose total charge is part of the answer, and with Broiler.VM's string ceiling, 2^24 code
/// units, as the maximum result length, or the rule's U7 limit for a limit variant; for casing in a language
/// other than the default, the adapter's own charge for the language pass is added before the call
/// (<see cref="LanguagePassCharge"/>). Arm J is compared with S as Broiler.JS's adapter will call it: with no
/// meter and no limit the population reaches (int.MaxValue).
/// </para>
/// <para>
/// <b>A refusal is compared by its kind.</b> S's false from a lookup is the adapter's SyntaxError, and its
/// LimitExceeded the adapter's RangeError, which is what each engine raises (adapter guide).
/// </para>
/// </remarks>
internal static class Arms
{
    // ---- arm S: the owner's implementation, every table read through its provider ----------------

    private static readonly UnicodeTables Tables = UnicodeTables.Instance;
    internal static readonly UnicodeProperties SProperties = new(Tables);
    private static readonly EcmaScriptLexical SLexical = new(SProperties);
    private static readonly UnicodeCaseFolding SFolding = new(Tables);
    private static readonly UnicodeCaseMapping SCasing = new(Tables);
    private static readonly UnicodeNormalization SNormalization = new(Tables);
    private static readonly UnicodeStringProperties SStrings = new(Tables);
    private static readonly UnicodeSegmenter SSegmenter = new(Tables);

    /// <summary>Broiler.VM's string ceiling, the copy's constant (JsRealm.String.cs:60).</summary>
    public const int VmCeiling = V.JsRealm.StringLengthCeiling;

    /// <summary>The negative control: one arm V charge for one U4 input, and one arm J result for one U1 input.</summary>
    public static bool NegativeControl { get; set; }

    /// <summary>The planted arm V change: the first U4 line, NFC, no limit, one unit more on its first charge.</summary>
    public const string PlantedVParameter = "NFC/none";

    /// <summary>The planted arm J change: CharExtensions.IsIdentifierStart's answer for U+0041 negated.</summary>
    public const int PlantedJCodePoint = 0x41;

    public static readonly string[] Languages = ["Default", "Turkic", "Lithuanian"];

    public static readonly string[] Forms = ["NFC", "NFD", "NFKC", "NFKD"];

    public static readonly string[] Granularities = ["grapheme", "word", "sentence"];

    // ---- the names U1's operations run over, read from the pinned tables ------------------------

    /// <summary>The canonical names of ECMA-262's binary properties, in the table's order.</summary>
    public static IReadOnlyList<string> BinaryNames { get; private set; } = [];

    /// <summary>The thirty single General_Category values' short names, in ordinal order, which is S's enum order.</summary>
    public static IReadOnlyList<string> CategoryNames { get; private set; } = [];

    /// <summary>Every Script value's short name in PropertyValueAliases.txt, in ordinal order.</summary>
    public static IReadOnlyList<string> ScriptNames { get; private set; } = [];

    /// <summary>The seven properties of strings, in ECMA-262's table's order.</summary>
    public static IReadOnlyList<string> StringPropertyNames { get; private set; } = [];

    /// <summary>The names an arm refused among those U1's operations resolve, with the arm.</summary>
    public static List<(string Arm, string Operation, string Name, string Answer)> Unresolved { get; } = [];

    /// <summary>Reads the names from the pinned ECMA-262 tables and PropertyValueAliases.txt.</summary>
    public static void ReadNames(string pins)
    {
        var binary = File.ReadAllText(Path.Combine(pins, "ecma262", "table-binary-unicode-properties.html"));
        BinaryNames = Regex.Matches(binary, "<a href=\"[^\"]*\">`([A-Za-z_]+)`</a>").Select(static m => m.Groups[1].Value).ToList();
        var strings = File.ReadAllText(Path.Combine(pins, "ecma262", "table-binary-unicode-properties-of-strings.html"));
        StringPropertyNames = Regex.Matches(strings, "<td>`([A-Za-z_]+)`</td>").Select(static m => m.Groups[1].Value).ToList();

        var categories = new List<string>();
        var scripts = new List<string>();
        foreach (var line in File.ReadLines(Path.Combine(pins, "ucd-17.0.0", "PropertyValueAliases.txt")))
        {
            if (line.Length == 0 || line[0] == '#')
                continue;
            var fields = line.Split('#')[0].Split(';').Select(static f => f.Trim()).ToArray();
            if (fields[0] == "gc" && fields[1].Length == 2 && fields[1] != "LC")
                categories.Add(fields[1]);
            else if (fields[0] == "sc")
                scripts.Add(fields[1]);
        }

        CategoryNames = categories.Order(StringComparer.Ordinal).ToList();
        ScriptNames = scripts.Order(StringComparer.Ordinal).ToList();
        if (BinaryNames.Count != 53 || StringPropertyNames.Count != 7 || CategoryNames.Count != 30 || ScriptNames.Count == 0)
        {
            throw new InvalidDataException(
                $"the pinned tables gave {BinaryNames.Count} binary properties, {StringPropertyNames.Count} properties of strings, " +
                $"{CategoryNames.Count} categories and {ScriptNames.Count} scripts; expected 53, 7, 30 and some");
        }
    }

    // ---- rendering ---------------------------------------------------------------------------------

    public static string Hex(int codePoint) => codePoint.ToString("X4", CultureInfo.InvariantCulture);

    /// <summary>A set's members as ranges, sorted and merged: <c>0041-005A,0061</c>; the empty set is empty.</summary>
    public static string RangesText(IEnumerable<(int Lo, int Hi)> ranges)
    {
        var built = new StringBuilder();
        foreach (var (lo, hi) in Merge(ranges))
        {
            if (built.Length > 0)
                built.Append(',');
            built.Append(Hex(lo));
            if (hi != lo)
                built.Append('-').Append(Hex(hi));
        }

        return built.ToString();
    }

    /// <summary>Ranges sorted by start and merged where they overlap or touch.</summary>
    public static List<(int Lo, int Hi)> Merge(IEnumerable<(int Lo, int Hi)> ranges)
    {
        var sorted = ranges.Where(static r => r.Hi >= r.Lo).OrderBy(static r => r.Lo).ThenBy(static r => r.Hi).ToList();
        var merged = new List<(int Lo, int Hi)>(sorted.Count);
        foreach (var (lo, hi) in sorted)
        {
            if (merged.Count > 0 && lo <= merged[^1].Hi + 1)
            {
                if (hi > merged[^1].Hi)
                    merged[^1] = (merged[^1].Lo, hi);
            }
            else
            {
                merged.Add((lo, hi));
            }
        }

        return merged;
    }

    /// <summary>
    /// A property of strings by its members: their count and the first 16 hexadecimal digits of the SHA-256 of
    /// their UTF-16LE code units, sorted ordinally and joined by U+000A, each code point a member of its own.
    /// </summary>
    public static string MembersDigest(IEnumerable<string> members)
    {
        var sorted = members.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
        var joined = string.Join('\n', sorted);
        var digest = SHA256.HashData(System.Runtime.InteropServices.MemoryMarshal.AsBytes(joined.AsSpan()));
        return sorted.Count.ToString(CultureInfo.InvariantCulture) + ":" + Convert.ToHexStringLower(digest)[..16];
    }

    private static IEnumerable<string> RangeMembers(IEnumerable<(int Lo, int Hi)> ranges)
    {
        foreach (var (lo, hi) in ranges)
        {
            for (var codePoint = lo; codePoint <= hi; codePoint++)
                yield return Population.CodePointText(codePoint);
        }
    }

    private static bool InRanges(IReadOnlyList<(int Lo, int Hi)> ranges, int codePoint)
    {
        int low = 0, high = ranges.Count - 1;
        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            if (codePoint < ranges[middle].Lo)
                high = middle - 1;
            else if (codePoint > ranges[middle].Hi)
                low = middle + 1;
            else
                return true;
        }

        return false;
    }

    private static IEnumerable<(int Lo, int Hi)> RangesOf(CodePointSet set)
    {
        for (var i = 0; i < set.RangeCount; i++)
            yield return (set.First(i), set.Last(i));
    }

    private static IEnumerable<(int Lo, int Hi)> RangesOf(VF.JsUnicodeSet set)
    {
        for (var i = 0; i < set.RangeCount; i++)
            yield return (set.First(i), set.Last(i));
    }

    /// <summary>Runs one implementation, recording an engine's error by its kind and any other exception by its type.</summary>
    public static Result? Safe(Func<Case, Result?> run, Case input)
    {
        try
        {
            return run(input);
        }
        catch (V.JsThrow error)
        {
            return Result.Refused(error.Kind);
        }
        catch (JX.JSException error)
        {
            return Result.Refused(error.Kind);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return Result.Failed(exception);
        }
    }

    // ---- what each arm does not implement ------------------------------------------------------

    /// <summary>The operations an arm does not implement, with the reason the bundle gives.</summary>
    public static IReadOnlyList<(string Arm, string Operation, string Reason)> NotImplemented { get; } =
    [
        ("J", "SimpleFold, Orbit, SimpleUpper, UpperOrbit",
            "no member of arm J's list implements Canonicalize: Broiler.JS folds through Broiler.Regex's CaseFolding and, without u, the translator's .NET casing (rows F1 and F2), which the rule does not copy"),
        ("J", "Soft_Dotted",
            "arm J's Soft_Dotted is JSStringSpecialCasing's private hand set inside the package, which no copied member exposes; it is compared through Lithuanian casing (row C5)"),
        ("J", "toUpperCase, toLowerCase and normalize under a maximum result length (the U7 limit variants len-1, len and len+1)",
            "Broiler.JS has no limit and no meter (rows C12, C13, N6 and N7): arm J is called with no limit only, against S at int.MaxValue"),
        ("J", "the charge of casing and normalization", "Broiler.JS charges nothing (rows C13 and N7)"),
        ("J", "name with JSRegExp.ResolvePropertyOperand in the u mode", "ResolvePropertyOperand is the v-mode set operand's resolution; in u mode only TranslateUnicodeProperty is reached"),
        ("J", "normalize with JSIntlCollator.Compare's normalization in NFC, NFKC and NFKD", "the collator normalizes to NFD only"),
        ("J", "toUpperCase and toLowerCase with JSString.ToLocale*Case(undefined) in the Turkic and Lithuanian languages",
            "with no locale argument the culture is the host's current culture, which the rule's runs set to the invariant culture, tr-TR and lt-LT; the Turkic and Lithuanian languages are reached with the locales tr and lt"),
        ("J,V", "word-status: the status's value",
            "both arms answer isWordLike, a boolean, where S answers a WordStatus; each is compared with S's isWordLike, status != None, on S's word segments (row G4)"),
        ("J,V", "the charge of segmentation", "the rule compares segmentation's charge with no arm: its units change by JSD-0074 decision 4 (row G5)"),
        ("J,V", "segmentation of U1, U2, U3, U4, U5 and U7", "segmentation runs over U6, the class whose inputs it takes"),
    ];

    // ---- the operations ------------------------------------------------------------------------

    public static IReadOnlyList<Operation> Operations(bool cultureRun, bool negativeControl)
    {
        PrepareResolutions();
        var all = new List<Operation>();
        if (negativeControl)
        {
            all.Add(Normalize());
            all.Add(Lexical("IdentifierStart"));
            return all.Select(static o => o.Name == "normalize" ? o with { Classes = ["U4"] } : o with { Classes = ["U1"] }).ToList();
        }

        if (cultureRun)
        {
            // The rule's second run of arm J's culture-dependent members: casing and the host's normalization,
            // arm J's variants alone, against S as Broiler.JS's adapter calls it.
            return new[] { Casing(upper: true), Casing(upper: false), Normalize() }
                .Select(static o => o with { Variants = o.Variants.Where(static v => v.Arm == "J").ToArray() })
                .ToList();
        }

        all.Add(Binary());
        all.Add(Values("General_Category", "General_Category", CategoryNames));
        all.Add(Values("Script", "Script", ScriptNames));
        all.Add(Values("Script_Extensions", "Script_Extensions", ScriptNames));
        all.Add(Lexical("IdentifierStart"));
        all.Add(Lexical("IdentifierPart"));
        all.Add(Lexical("WhiteSpace"));
        all.Add(SoftDotted());
        all.AddRange(Folding());
        all.Add(Names());
        all.Add(Casing(upper: true));
        all.Add(Casing(upper: false));
        all.Add(Normalize());
        all.Add(StringMembership());
        foreach (var granularity in Granularities)
            all.Add(Segmentation(granularity));
        all.Add(WordStatuses());
        return all;
    }

    private static IEnumerable<Case> Once(Item item) => [new Case(item, "-")];

    // ---- U1: properties, lexical classes, Soft_Dotted, folding -------------------------------------

    private static CodePointSet?[] sBinary = [];
    private static VF.JsUnicodeSet?[] vBinary = [];
    private static (int Lo, int Hi)[]?[] jBinary = [];
    private static readonly Dictionary<string, (CodePointSet?[] S, VF.JsUnicodeSet?[] V, (int Lo, int Hi)[]?[] J)> valueSets = new(StringComparer.Ordinal);
    private static StringMembers[] sStringMembers = [];
    private static StringMembers[] vStringMembers = [];
    private static StringMembers?[] jStringMembers = [];
    private static bool prepared;

    /// <summary>Resolves, once and before any parallel work, the names U1's operations and U5's run over, in each arm.</summary>
    private static void PrepareResolutions()
    {
        if (prepared)
            return;
        prepared = true;

        sBinary = BinaryNames.Select(n => SProperties.TryResolve(n, out var set) ? set : Unresolve<CodePointSet>("S", "binary", n)).ToArray();
        vBinary = BinaryNames.Select(n => VF.JsUnicodeProperties.TryResolveLone(n, out var set) ? set : Unresolve<VF.JsUnicodeSet>("V", "binary", n)).ToArray();
        jBinary = BinaryNames.Select(n => JRanges("binary", n, n)).ToArray();

        foreach (var (property, names) in new[] { ("General_Category", CategoryNames), ("Script", ScriptNames), ("Script_Extensions", ScriptNames) })
        {
            valueSets[property] = (
                names.Select(n => SProperties.TryResolve(property, n, out var set) ? set : Unresolve<CodePointSet>("S", property, n)).ToArray(),
                names.Select(n => VF.JsUnicodeProperties.TryResolve(property, n, out var set) ? set : Unresolve<VF.JsUnicodeSet>("V", property, n)).ToArray(),
                names.Select(n => JRanges(property, n, property + "=" + n)).ToArray());
        }

        sStringMembers = StringPropertyNames.Select(SMembers).ToArray();
        vStringMembers = StringPropertyNames.Select(VMembers).ToArray();
        jStringMembers = StringPropertyNames.Select(JMembers).ToArray();
    }

    private static T? Unresolve<T>(string arm, string operation, string name)
        where T : struct
    {
        lock (Unresolved)
            Unresolved.Add((arm, operation, name, "refused:SyntaxError"));
        return null;
    }

    /// <summary>Arm J's set for a name, as JResolve resolves it, or null when it refuses the name or answers no set.</summary>
    private static (int Lo, int Hi)[]? JRanges(string operation, string name, string inner)
    {
        string answer;
        try
        {
            var resolution = JR.JSRegExp.TranslateUnicodeProperty(false, inner, false, false, false) ?? JR.JSRegExp.NetFallThrough("\\p", inner);
            if (resolution.Ranges is { } ranges)
                return Merge(ranges).ToArray();
            answer = Render(resolution).Render();
        }
        catch (JX.JSException error)
        {
            answer = "refused:" + error.Kind;
        }

        lock (Unresolved)
            Unresolved.Add(("J", operation, name, answer));
        return null;
    }

    private static Operation Binary() => new(
        "binary", ["U1"],
        static item => BinaryNames.Select((name, index) => new Case(item, name, index)),
        [
            new("V", "JsUnicodeProperties.TryResolveLone", static c => vBinary[c.A] is { } set ? Result.Bool(set.Contains(c.Item.CodePoint)) : null,
                SBinary, "S"),
            new("J", "JSRegExp.TranslateUnicodeProperty", static c => jBinary[c.A] is { } ranges ? Result.Bool(InRanges(ranges, c.Item.CodePoint)) : null,
                SBinary, "S"),
        ]);

    private static Result SBinary(Case c) =>
        sBinary[c.A] is { } set ? Result.Bool(set.Contains(c.Item.CodePoint)) : Result.Refused("SyntaxError");

    /// <summary>
    /// A non-binary property's value at each code point: the names of the values whose set holds it, in
    /// ordinal order. S answers General_Category with GeneralCategoryOf, and Script and Script_Extensions
    /// from the sets its lookup resolves, as neither has a member of its own.
    /// </summary>
    private static Operation Values(string name, string property, IReadOnlyList<string> names)
    {
        Result Collect<T>(T?[] sets, Func<T, bool> holds)
            where T : struct
        {
            var found = new List<string>(2);
            for (var i = 0; i < sets.Length; i++)
            {
                if (sets[i] is { } set && holds(set))
                    found.Add(names[i]);
            }

            return Result.Values(found);
        }

        Result? JCollect(Case c)
        {
            var sets = valueSets[property].J;
            var found = new List<string>(2);
            for (var i = 0; i < sets.Length; i++)
            {
                if (sets[i] is { } ranges && InRanges(ranges, c.Item.CodePoint))
                    found.Add(names[i]);
            }

            return Result.Values(found);
        }

        Result Shared(Case c) => property == "General_Category"
            ? Result.Values([CategoryNames[(int)SProperties.GeneralCategoryOf(c.Item.CodePoint)]])
            : Collect(valueSets[property].S, set => set.Contains(c.Item.CodePoint));

        return new(name, ["U1"], Once,
        [
            new("V", "JsUnicodeProperties.TryResolve", c => Collect(valueSets[property].V, set => set.Contains(c.Item.CodePoint)), Shared, "S"),
            new("J", "JSRegExp.TranslateUnicodeProperty", JCollect, Shared, "S"),
        ]);
    }

    private static Operation Lexical(string name)
    {
        static char Unit(int codePoint) => codePoint <= 0xFFFF ? (char)codePoint : char.ConvertFromUtf32(codePoint)[0];

        return name switch
        {
            "IdentifierStart" => new(name, ["U1"], Once,
            [
                new("V", "JsUnicodeLexical.IsIdentifierStart", static c => Result.Bool(VF.JsUnicodeLexical.IsIdentifierStart(c.Item.CodePoint)),
                    static c => Result.Bool(SLexical.IsIdentifierStart(c.Item.CodePoint)), "S"),
                new("J", "CharExtensions.IsIdentifierStart", static c =>
                    {
                        var answer = JP.CharExtensions.IsIdentifierStart(c.Item.CodePoint);
                        return Result.Bool(NegativeControl && c.Item.CodePoint == PlantedJCodePoint ? !answer : answer);
                    },
                    static c => Result.Bool(SLexical.IsIdentifierStart(c.Item.CodePoint)), "S"),
                new("J", "JSRegExp.IsRegExpIdentifierStart", static c => Result.Bool(JR.JSRegExp.IsRegExpIdentifierStart(c.Item.CodePoint)),
                    static c => Result.Bool(SLexical.IsIdentifierStart(c.Item.CodePoint)), "S"),
            ]),
            "IdentifierPart" => new(name, ["U1"], Once,
            [
                new("V", "JsUnicodeLexical.IsIdentifierPart", static c => Result.Bool(VF.JsUnicodeLexical.IsIdentifierPart(c.Item.CodePoint)),
                    static c => Result.Bool(SLexical.IsIdentifierPart(c.Item.CodePoint)), "S"),
                new("J", "CharExtensions.IsIdentifierPart", static c => Result.Bool(JP.CharExtensions.IsIdentifierPart(c.Item.CodePoint)),
                    static c => Result.Bool(SLexical.IsIdentifierPart(c.Item.CodePoint)), "S"),
                new("J", "JSRegExp.IsRegExpIdentifierPart", static c => Result.Bool(JR.JSRegExp.IsRegExpIdentifierPart(c.Item.CodePoint)),
                    static c => Result.Bool(SLexical.IsIdentifierPart(c.Item.CodePoint)), "S"),
            ]),
            _ => new(name, ["U1"], Once,
            [
                new("V", "JsUnicodeLexical.IsWhiteSpace", static c => Result.Bool(VF.JsUnicodeLexical.IsWhiteSpace(c.Item.CodePoint)),
                    static c => Result.Bool(SLexical.IsWhiteSpace(c.Item.CodePoint)), "S"),
                // The scanner reads one code unit: a supplementary code point's first unit, its high surrogate.
                // A unit its skip loop consumes is white space unless the loop reads it as a line terminator.
                new("J", "FastScanner.IsWhiteSpace", static c =>
                    {
                        var first = Unit(c.Item.CodePoint);
                        return Result.Bool(JP.FastScanner.IsWhiteSpace(first) && !JP.FastScanner.IsLineTerminatorInSkip(first));
                    },
                    static c => Result.Bool(SLexical.IsWhiteSpace(c.Item.CodePoint)), "S"),
            ]),
        };
    }

    private static Operation SoftDotted() => new(
        "Soft_Dotted", ["U1"], Once,
        [
            new("V", "JsIntlTables.IsSoftDotted", static c => Result.Bool(new V.JsEngine().Intl!.IsSoftDotted(c.Item.CodePoint)),
                static c => Result.Bool(SCasing.IsSoftDotted(c.Item.CodePoint)), "S"),
        ]);

    private static IEnumerable<Operation> Folding()
    {
        yield return new("SimpleFold", ["U1"], Once,
        [
            new("V", "JsUnicodeCaseFolding.SimpleFold", static c => Result.CodePoint(VF.JsUnicodeCaseFolding.SimpleFold(c.Item.CodePoint)),
                static c => Result.CodePoint(SFolding.SimpleFold(c.Item.CodePoint)), "S"),
        ]);

        yield return new("Orbit", ["U1"], Once,
        [
            new("V", "JsUnicodeCaseFolding.Orbit", static c =>
                {
                    Span<int> orbit = stackalloc int[VF.JsUnicodeCaseFolding.MaxOrbit];
                    return Result.CodePoints(orbit[..VF.JsUnicodeCaseFolding.Orbit(c.Item.CodePoint, orbit)]);
                },
                static c =>
                {
                    Span<int> orbit = stackalloc int[SFolding.MaxOrbit];
                    return Result.CodePoints(orbit[..SFolding.Orbit(c.Item.CodePoint, orbit)]);
                }, "S"),
        ]);

        yield return new("SimpleUpper", ["U1"], Once,
        [
            new("V", "JsUnicodeCaseFolding.SimpleUpper", static c => Result.CodePoint(VF.JsUnicodeCaseFolding.SimpleUpper(c.Item.CodePoint)),
                static c => Result.CodePoint(SFolding.SimpleUpper(c.Item.CodePoint)), "S"),
        ]);

        yield return new("UpperOrbit", ["U1"], Once,
        [
            new("V", "JsUnicodeCaseFolding.UpperOrbit", static c =>
                {
                    Span<int> orbit = stackalloc int[VF.JsUnicodeCaseFolding.MaxUpperOrbit];
                    return Result.CodePoints(orbit[..VF.JsUnicodeCaseFolding.UpperOrbit(c.Item.CodePoint, orbit)]);
                },
                static c =>
                {
                    Span<int> orbit = stackalloc int[SFolding.MaxUpperOrbit];
                    return Result.CodePoints(orbit[..SFolding.UpperOrbit(c.Item.CodePoint, orbit)]);
                }, "S"),
        ]);
    }

    // ---- U2: the lookup of a name ---------------------------------------------------------------

    /// <summary>
    /// The lookup of a name, lone or Name=Value, in u mode and in v mode, by its result: a set by its members,
    /// a property of strings by its members, or a refusal by its kind.
    /// </summary>
    private static Operation Names() => new(
        "name", ["U2"],
        static item => [new Case(item, item.Kind + "/u", 0), new Case(item, item.Kind + "/v", 1)],
        [
            new("V", "JsUnicodeProperties.TryResolve and JsUnicodeStringProperties.TryResolve", VName, SName, "S"),
            new("J", "JSRegExp.TranslateUnicodeProperty", static c => JResolve(Inner(c.Item), unicodeSets: c.A == 1), SName, "S"),
            new("J", "JSRegExp.ResolvePropertyOperand", static c => c.A == 1 ? JOperand(Inner(c.Item)) : null, SName, "S"),
        ]);

    private static string Inner(Item item) => item.Kind == "lone" ? item.Property : item.Property + "=" + item.Value;

    private static Result SName(Case c)
    {
        var item = c.Item;
        if (item.Kind == "pair")
            return SProperties.TryResolve(item.Property, item.Value, out var pair) ? Result.Set(RangesOf(pair)) : Result.Refused("SyntaxError");

        // Under v, a lone name is first a property of strings (adapter guide, UnicodeProperties step 3).
        if (c.A == 1 && SStrings.TryResolve(item.Property, out var property))
            return Result.Strings(sStringMembers[property.Id].Digest);

        return SProperties.TryResolve(item.Property, out var set) ? Result.Set(RangesOf(set)) : Result.Refused("SyntaxError");
    }

    /// <summary>
    /// Arm V's lookup as its matcher calls it (JsRegExpMatcher.cs:2776-2810): under v, a lone name is first
    /// a property of strings; then the lone or the Name=Value lookup. False is its SyntaxError.
    /// </summary>
    private static Result? VName(Case c)
    {
        var item = c.Item;
        if (item.Kind == "pair")
            return VF.JsUnicodeProperties.TryResolve(item.Property, item.Value, out var pair) ? Result.Set(RangesOf(pair)) : Result.Refused("SyntaxError");

        if (c.A == 1 && VF.JsUnicodeStringProperties.TryResolve(item.Property, out var property))
            return Result.Strings(vStringMembers[property].Digest);

        return VF.JsUnicodeProperties.TryResolveLone(item.Property, out var set) ? Result.Set(RangesOf(set)) : Result.Refused("SyntaxError");
    }

    /// <summary>
    /// Arm J's translator on a standalone \p{inner}: TranslateUnicodeProperty, and on its null the fall-through
    /// to .NET's \p, as JSRegExp builds the pattern.
    /// </summary>
    private static Result? JResolve(string inner, bool unicodeSets)
    {
        var resolution = JR.JSRegExp.TranslateUnicodeProperty(false, inner, false, unicodeSets, false)
            ?? JR.JSRegExp.NetFallThrough("\\p", inner);
        return Render(resolution);
    }

    private static Result Render(JR.PropertyResolution resolution) =>
        resolution.Sequences is { } sequences ? Result.Strings(MembersDigest(sequences))
        : resolution.Ranges is { } ranges ? Result.Set(ranges)
        : new Result("net", resolution.NetPattern ?? string.Empty);

    /// <summary>Arm J's v-mode set operand: its class set value, a property of strings when it holds strings.</summary>
    private static Result? JOperand(string inner)
    {
        var value = JR.JSRegExp.ResolvePropertyOperand(false, inner);
        return value.Strings.Count > 0
            ? Result.Strings(MembersDigest(RangeMembers(value.Ranges).Concat(value.Strings)))
            : Result.Set(value.Ranges);
    }

    // ---- U1, U5: the properties of strings ------------------------------------------------------

    /// <summary>One arm's members of one property of strings, built once: its code points and its sequences.</summary>
    private sealed class StringMembers(IReadOnlyList<(int Lo, int Hi)> ranges, HashSet<string> sequences, Func<int, bool>? codePoint = null)
    {
        public string Digest { get; } = MembersDigest(RangeMembers(ranges).Concat(sequences));

        /// <summary>Whether <paramref name="text"/> is a member: a code point by the arm's own test when it has one, else by its ranges, and a sequence by its sequences.</summary>
        public bool Contains(string text)
        {
            var single = text.Length == 1 || (text.Length == 2 && char.IsSurrogatePair(text[0], text[1]));
            if (single)
            {
                var point = text.Length == 1 ? text[0] : char.ConvertToUtf32(text[0], text[1]);
                if (codePoint is not null ? codePoint(point) : InRanges(ranges, point))
                    return true;
            }

            return sequences.Contains(text);
        }
    }

    private static StringMembers SMembers(string name)
    {
        if (!SStrings.TryResolve(name, out var property))
            throw new InvalidDataException("S does not resolve the property of strings " + name);

        List<StringProperty> parts = property.IsUnion ? Enumerable.Range(0, property.Id).Select(SStrings.Property).ToList() : [property];
        var ranges = new List<(int Lo, int Hi)>();
        var sequences = new HashSet<string>(StringComparer.Ordinal);
        Span<int> buffer = stackalloc int[StringPropertySequences.MaxSequenceLength];
        foreach (var part in parts)
        {
            for (var i = 0; i < part.CodePointRangeCount; i++)
                ranges.Add((part.FirstCodePoint(i), part.LastCodePoint(i)));

            var reader = part.GetSequences();
            while (reader.TryRead(buffer, out var length))
            {
                var built = new StringBuilder();
                foreach (var point in buffer[..length])
                    built.Append(Population.CodePointText(point));
                sequences.Add(built.ToString());
            }
        }

        var captured = property;
        return new StringMembers(Merge(ranges), sequences, point => captured.ContainsCodePoint(point));
    }

    private static StringMembers VMembers(string name)
    {
        if (!VF.JsUnicodeStringProperties.TryResolve(name, out var property))
            throw new InvalidDataException("arm V does not resolve the property of strings " + name);

        var pairs = new List<int>();
        var strings = new HashSet<string>(StringComparer.Ordinal);
        VF.JsUnicodeStringProperties.Collect(property, pairs, strings);
        var ranges = new List<(int Lo, int Hi)>();
        for (var i = 0; i + 1 < pairs.Count; i += 2)
            ranges.Add((pairs[i], pairs[i + 1]));

        // The matcher merges the collected pairs into one class (JsRegExpMatcher.cs:2793), as Merge does here.
        return new StringMembers(Merge(ranges), strings);
    }

    /// <summary>Arm J's members of a property of strings, as its translator resolves the name under v, or null when it refuses it.</summary>
    private static StringMembers? JMembers(string name)
    {
        string answer;
        try
        {
            var resolution = JR.JSRegExp.TranslateUnicodeProperty(false, name, false, true, false);
            if (resolution?.Sequences is { } sequences)
                return new StringMembers([], new HashSet<string>(sequences, StringComparer.Ordinal));
            answer = resolution is null ? "fall-through" : Render(resolution).Render();
        }
        catch (JX.JSException error)
        {
            answer = "refused:" + error.Kind;
        }

        lock (Unresolved)
            Unresolved.Add(("J", "string-property", name, answer));
        return null;
    }

    private static Operation StringMembership() => new(
        "string-property", ["U1", "U5"],
        static item => StringPropertyNames.Select((name, index) => new Case(item, name, index)),
        [
            new("V", "JsUnicodeStringProperties.Collect", static c => Result.Bool(vStringMembers[c.A].Contains(c.Item.Text)),
                static c => Result.Bool(sStringMembers[c.A].Contains(c.Item.Text)), "S"),
            new("J", "JSRegExp.EmojiStringPropertyNames and GetSequences", static c => jStringMembers[c.A] is { } members ? Result.Bool(members.Contains(c.Item.Text)) : null,
                static c => Result.Bool(sStringMembers[c.A].Contains(c.Item.Text)), "S"),
        ]);

    // ---- U1, U3, U7: casing ---------------------------------------------------------------------

    /// <summary>
    /// The charge Broiler.VM's adapter adds for the language pass, before it calls S, when the language is not
    /// the default (JSD-0074 decision 4; adapter guide, UnicodeCaseMapping step 3). It is what Broiler.VM's
    /// LanguageCasing charges (JsRealm.String.cs:1173): StringCharge(engine, text.Length), the input's length in
    /// UTF-16 code units, and one unit for the empty text, as StringCharge charges at least one (:973-974). The
    /// pass runs only for tr, az and lt (:1168-1171), so the default language adds nothing.
    /// </summary>
    public static ulong LanguagePassCharge(string text, CaseLanguage language) =>
        language == CaseLanguage.Default ? 0UL : text.Length <= 0 ? 1UL : (ulong)text.Length;

    /// <summary>The limit variants of U7: none, and S's own result's length, one less and one more.</summary>
    private static IEnumerable<Case> Limited(Item item, string prefix, int a, int length)
    {
        yield return new Case(item, prefix + "/none", a, -1);
        if (length >= 1)
            yield return new Case(item, prefix + "/len-1=" + (length - 1).ToString(CultureInfo.InvariantCulture), a, length - 1);
        yield return new Case(item, prefix + "/len=" + length.ToString(CultureInfo.InvariantCulture), a, length);
        yield return new Case(item, prefix + "/len+1=" + (length + 1).ToString(CultureInfo.InvariantCulture), a, length + 1);
    }

    private static UnicodeStatus SMap(string text, bool upper, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result) =>
        upper ? SCasing.ToUpperCase(text, language, maxLength, charge, out result) : SCasing.ToLowerCase(text, language, maxLength, charge, out result);

    private static Operation Casing(bool upper)
    {
        IEnumerable<Case> Expand(Item item)
        {
            for (var language = 0; language < Languages.Length; language++)
            {
                SMap(item.Text, upper, (CaseLanguage)language, int.MaxValue, null, out var unlimited);
                foreach (var c in Limited(item, Languages[language], language, unlimited!.Length))
                    yield return c;
            }
        }

        Result SForV(Case c)
        {
            var meter = new Meter();
            var language = (CaseLanguage)c.A;
            var extra = LanguagePassCharge(c.Item.Text, language);
            if (extra > 0)
                meter.Charge(extra);
            var status = SMap(c.Item.Text, upper, language, c.B < 0 ? VmCeiling : c.B, meter.Charge, out var result);
            return (status == UnicodeStatus.Success ? Result.Text(result!) : Result.Refused("RangeError")).WithCharge(meter.Total);
        }

        Result SForJ(Case c)
        {
            SMap(c.Item.Text, upper, (CaseLanguage)c.A, int.MaxValue, null, out var result);
            return Result.Text(result!);
        }

        Result? VCase(Case c)
        {
            var engine = new V.JsEngine();
            var ceiling = c.B < 0 ? VmCeiling : c.B;
            try
            {
                var result = c.A == 0
                    ? V.JsRealm.StringChangeCase(engine, c.Item.Text, upper, ceiling)
                    : V.JsRealm.TransformCase(engine, c.Item.Text, c.A == 1 ? "tr" : "lt", upper, ceiling);
                return Result.Text(result).WithCharge(engine.Charged);
            }
            catch (V.JsThrow error)
            {
                return Result.Refused(error.Kind).WithCharge(engine.Charged);
            }
        }

        Result? JCase(Case c)
        {
            if (c.B >= 0)
                return null;

            var text = c.Item.Text;
            return Result.Text(c.A switch
            {
                0 => upper ? JS.JSString.ToUpperCase(text) : JS.JSString.ToLowerCase(text),
                1 => upper ? JS.JSString.ToLocaleUpperCase(text, "tr", CultureInfo.CurrentCulture) : JS.JSString.ToLocaleLowerCase(text, "tr", CultureInfo.CurrentCulture),
                _ => upper ? JS.JSString.ToLocaleUpperCase(text, "lt", CultureInfo.CurrentCulture) : JS.JSString.ToLocaleLowerCase(text, "lt", CultureInfo.CurrentCulture),
            });
        }

        Result? JNoLocale(Case c)
        {
            if (c.B >= 0 || c.A != 0)
                return null;

            var text = c.Item.Text;
            return Result.Text(upper
                ? JS.JSString.ToLocaleUpperCase(text, null, CultureInfo.CurrentCulture)
                : JS.JSString.ToLocaleLowerCase(text, null, CultureInfo.CurrentCulture));
        }

        var name = upper ? "toUpperCase" : "toLowerCase";
        var member = upper ? "Upper" : "Lower";
        return new(name, ["U1", "U3", "U7"], Expand,
        [
            new("V", "JsRealm.StringChangeCase and TransformCase", VCase, SForV, "S/V"),
            new("J", $"JSString.To{member}Case and ToLocale{member}Case(tr, lt)", JCase, SForJ, "S/J"),
            new("J", $"JSString.ToLocale{member}Case(undefined)", JNoLocale, SForJ, "S/J"),
        ]);
    }

    // ---- U1, U4, U7: normalization --------------------------------------------------------------

    private static Operation Normalize()
    {
        static IEnumerable<Case> Expand(Item item)
        {
            for (var form = 0; form < Forms.Length; form++)
            {
                SNormalization.Transform(item.Text, (NormalForm)form, int.MaxValue, null, out var unlimited);
                foreach (var c in Limited(item, Forms[form], form, unlimited!.Length))
                    yield return c;
            }
        }

        static Result SForV(Case c)
        {
            var meter = new Meter();
            var status = SNormalization.Transform(c.Item.Text, (NormalForm)c.A, c.B < 0 ? VmCeiling : c.B, meter.Charge, out var result);
            return (status == UnicodeStatus.Success ? Result.Text(result!) : Result.Refused("RangeError")).WithCharge(meter.Total);
        }

        static Result SForJ(Case c)
        {
            SNormalization.Transform(c.Item.Text, (NormalForm)c.A, int.MaxValue, null, out var result);
            return Result.Text(result!);
        }

        static Result? VNormalize(Case c)
        {
            var planted = NegativeControl && c.Item.Class.StartsWith("U4", StringComparison.Ordinal) && c.Item.Line == 1 && c.Parameter == PlantedVParameter;
            var engine = new V.JsEngine(planted);
            try
            {
                var result = V.JsRealm.NormalizeBuiltIn(engine, c.Item.Text, Forms[c.A], c.B < 0 ? VmCeiling : c.B);
                return Result.Text(result).WithCharge(engine.Charged);
            }
            catch (V.JsThrow error)
            {
                return Result.Refused(error.Kind).WithCharge(engine.Charged);
            }
        }

        static Result? JNormalize(Case c) => c.B >= 0 ? null : Result.Text(JS.JSString.Normalize(c.Item.Text, Forms[c.A]));

        static Result? JCollator(Case c) => c.B >= 0 || c.A != 1 ? null : Result.Text(JS.JSString.CollatorNormalize(c.Item.Text));

        return new("normalize", ["U1", "U4", "U7"], Expand,
        [
            new("V", "JsRealm.NormalizeText", VNormalize, SForV, "S/V"),
            new("J", "JSString.Normalize", JNormalize, SForJ, "S/J"),
            new("J", "JSIntlCollator.Compare's NFD", JCollator, SForJ, "S/J"),
        ]);
    }

    /// <summary>A meter: every charge S's or arm V's work makes, recorded, and their total.</summary>
    private sealed class Meter
    {
        public ulong Total { get; private set; }

        public void Charge(ulong units) => Total += units;
    }

    // ---- U6: segmentation -----------------------------------------------------------------------

    private static Operation Segmentation(string granularity)
    {
        var s = (SegmentGranularity)Array.IndexOf(Granularities, granularity);
        return new(granularity, ["U6"], Once,
        [
            new("V", "JsSegmentation.Boundaries", c =>
                {
                    var engine = new V.JsEngine();
                    return Result.Boundaries(V.JsSegmentation.Boundaries(engine, engine.Intl!.Breaks, c.Item.Text, granularity));
                },
                c => Result.Boundaries(SSegmenter.Boundaries(c.Item.Text, s, null)), "S"),
            new("J", "JSIntlSegments.Compute", c => Result.Boundaries(JBoundaries(c.Item.Text, granularity)),
                c => Result.Boundaries(SSegmenter.Boundaries(c.Item.Text, s, null)), "S"),
        ]);
    }

    /// <summary>Arm J's segments as boundaries: each segment's start, then the end of the last; no segment is <c>0</c>.</summary>
    private static List<int> JBoundaries(string text, string granularity)
    {
        var segments = JI.JSIntlSegments.Compute(text, granularity);
        var boundaries = segments.Select(static s => s.start).ToList();
        boundaries.Add(segments.Count == 0 ? 0 : segments[^1].start + segments[^1].length);
        return boundaries;
    }

    /// <summary>Each word segment's status, on S's word segments: isWordLike in each arm against S's status != None.</summary>
    private static Operation WordStatuses() => new(
        "word-status", ["U6"],
        static item =>
        {
            var boundaries = SSegmenter.Boundaries(item.Text, SegmentGranularity.Word, null);
            return Enumerable.Range(0, Math.Max(0, boundaries.Length - 1)).Select(i => new Case(
                item,
                boundaries[i].ToString(CultureInfo.InvariantCulture) + "-" + boundaries[i + 1].ToString(CultureInfo.InvariantCulture),
                boundaries[i],
                boundaries[i + 1]));
        },
        [
            new("V", "JsSegmentation.IsWordLike", static c => Result.Bool(V.JsSegmentation.IsWordLike(new V.JsEngine().Intl!.Breaks, c.Item.Text, c.A, c.B)),
                SWordLike, "S"),
            new("J", "JSIntlSegments.IsWordLike", static c => Result.Bool(JI.JSIntlSegments.IsWordLike(c.Item.Text.Substring(c.A, c.B - c.A))),
                SWordLike, "S"),
        ]);

    private static Result SWordLike(Case c) =>
        Result.Bool(SSegmenter.GetWordStatus(c.Item.Text.AsSpan(c.A, c.B - c.A), null) != WordStatus.None);
}
