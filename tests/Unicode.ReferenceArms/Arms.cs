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
/// One answer, compared exactly: its kind and value, and for casing and normalization against arm V the
/// total charge (-1 for none), which is compared on a line of its own. <see cref="Render"/> writes it in the
/// notation scripts/reference-arms/README.md fixes ("differences.tsv"), which the classifier reads.
/// </summary>
internal readonly record struct Result(string Tag, string Value, long Charge = -1)
{
    /// <summary>A text: its code units, or for a <c>repeat</c> input its length and digest.</summary>
    public static Result Text(string text) => new("t", text);

    /// <summary>A result longer than the maximum: S's LimitExceeded, arm V's ceiling RangeError.</summary>
    public static readonly Result Over = new("over", string.Empty);

    /// <summary>A lookup's refusal: S's false, arm V's false and arm J's SyntaxError, the one kind a lookup refuses with.</summary>
    public static readonly Result Refused = new("refused", string.Empty);

    /// <summary>Any other refusal or failure, by its kind: an engine error's constructor name or an exception's type.</summary>
    public static Result Exception(string kind) => new("exception", kind);

    /// <summary>An answer written as it is: a boolean, a code point, a list, a resolution, boundaries.</summary>
    public static Result Plain(string value) => new("plain", value);

    public static Result Bool(bool value) => Plain(value ? "true" : "false");

    public static Result CodePoint(int codePoint) => Plain(Arms.Hex(codePoint));

    public static Result CodePoints(ReadOnlySpan<int> codePoints)
    {
        var built = new StringBuilder();
        foreach (var codePoint in codePoints)
            built.Append(built.Length == 0 ? string.Empty : " ").Append(Arms.Hex(codePoint));
        return Plain(built.ToString());
    }

    public static Result Boundaries(IEnumerable<int> positions) =>
        Plain(string.Join(' ', positions.Select(static p => p.ToString(CultureInfo.InvariantCulture))));

    public Result WithCharge(ulong charge) => this with { Charge = (long)charge };

    /// <summary>The answer without its charge, which is compared apart.</summary>
    public Result Answer => this with { Charge = -1 };

    public string Render(Item item) => Tag switch
    {
        "t" => item.IsRepeat ? Population.LengthAndDigest(Value) : Population.Hex(Value),
        "over" => "over",
        "refused" => "refused",
        "exception" => "exception:" + Value,
        _ => Value,
    };
}

/// <summary>A case: one input of an operation with one value of its parameters.</summary>
/// <param name="Item">The input.</param>
/// <param name="Parameter">How the listing writes the parameters.</param>
/// <param name="A">The operation's first parameter: a property's index, a language, a form, a mode or a lexical set.</param>
/// <param name="B">The second: the maximum result length, or -1 for none.</param>
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

/// <summary>An operation: the classes it runs over, the inputs of a class it takes, how an input becomes its cases, and its variants.</summary>
internal sealed record Operation(string Name, string[] Classes, Func<Item, IEnumerable<Case>> Expand, Variant[] Variants, Func<Item, bool>? Takes = null);

/// <summary>
/// The operations of bundle sjl-7-001's reference-arm rule, and for each S and the variants of the arms that
/// implement it. An arm is called only for what it implements; <see cref="NotImplemented"/> names the rest.
/// </summary>
/// <remarks>
/// <para>
/// <b>S under each engine's policy.</b> Arm V is compared with S as Broiler.VM's adapter will call it: with
/// its meter and with Broiler.VM's string ceiling, 2^24 code units, as the maximum result length
/// (<c>;max=16777216</c>), or the U7 limit of a limit variant; for casing in a language other than the default, the adapter's own charge for
/// the language pass is added before the call (<see cref="LanguagePassCharge"/>). Arm J is compared with S
/// as Broiler.JS's adapter will call it: with no meter and no limit the population reaches (int.MaxValue).
/// </para>
/// <para>
/// <b>A refusal is compared by its kind.</b> A lookup's refusal is <c>refused</c> in S and every arm (S's
/// false is its adapter's SyntaxError, which is arm V's and arm J's kind too); a result over the maximum is
/// <c>over</c> in S (LimitExceeded) and in arm V (its RangeError "Invalid string length"); any other is
/// <c>exception:</c> and its kind.
/// </para>
/// </remarks>
internal static class Arms
{
    // ---- arm S: the owner's implementation, every table read through its provider ----------------

    private static readonly UnicodeTables Tables = UnicodeTables.Instance;
    private static readonly UnicodeProperties SProperties = new(Tables);
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

    /// <summary>The host culture of arm J's second run, written after <c>@</c> on every parameter; empty otherwise.</summary>
    public static string CultureSuffix { get; set; } = string.Empty;

    /// <summary>The planted arm V change: the first U4 line, NFC at Broiler.VM's ceiling, one unit more on its first charge.</summary>
    public const string PlantedVParameter = "NFC;max=16777216";

    /// <summary>The planted arm J change: CharExtensions.IsIdentifierStart's answer for U+0041 negated.</summary>
    public const int PlantedJCodePoint = 0x41;

    public const string ControlV = "V-control";

    public const string ControlJ = "J-control";

    /// <summary>
    /// The casing parameters: the three languages; then, for the run cells of rows C7 and C9 alone (tags
    /// <c>cell:C7</c> and <c>cell:C9</c>), the locales those rows pass, which arm J alone takes as its argument:
    /// the list ['tr','en'] as Broiler.JS's ToString makes it, <c>az</c>, <c>TR</c> and <c>tr-u-co-search</c>.
    /// S takes each as ECMA-402's TransformCase resolves it: Turkic.
    /// </summary>
    public static readonly string[] Languages = ["default", "turkic", "lithuanian", "tr,en", "az", "TR", "tr-u-co-search"];

    private static readonly CaseLanguage[] SLanguages =
        [CaseLanguage.Default, CaseLanguage.Turkic, CaseLanguage.Lithuanian, CaseLanguage.Turkic, CaseLanguage.Turkic, CaseLanguage.Turkic, CaseLanguage.Turkic];

    public static readonly string[] Forms = ["NFC", "NFD", "NFKC", "NFKD"];

    public static readonly string[] Granularities = ["grapheme", "word", "sentence"];

    /// <summary>The lexical sets, by the names the arbiter gives them (unicoderef.py, sets.tsv).</summary>
    public static readonly string[] LexicalSets =
        ["IdentifierStartChar", "IdentifierPartChar", "RegExpIdentifierStart", "RegExpIdentifierPart", "WhiteSpace", "Soft_Dotted"];

    // ---- the names, read from the pinned tables --------------------------------------------------

    /// <summary>The canonical names of ECMA-262's binary properties, in the table's order.</summary>
    public static IReadOnlyList<string> BinaryNames { get; private set; } = [];

    /// <summary>The thirty single General_Category values' short names, in ordinal order, which is S's enum order.</summary>
    public static IReadOnlyList<string> CategoryNames { get; private set; } = [];

    /// <summary>Every General_Category value and group of PropertyValueAliases.txt: short name, long name, every alias.</summary>
    public static IReadOnlyList<string[]> CategoryRecords { get; private set; } = [];

    /// <summary>Every Script value's short name, in ordinal order.</summary>
    public static IReadOnlyList<string> ScriptNames { get; private set; } = [];

    /// <summary>Every Script value of PropertyValueAliases.txt: short name, long name, every alias.</summary>
    public static IReadOnlyList<string[]> ScriptRecords { get; private set; } = [];

    /// <summary>The seven properties of strings, in ECMA-262's table's order.</summary>
    public static IReadOnlyList<string> StringPropertyNames { get; private set; } = [];

    /// <summary>The canonical names an arm refused, or answered as no set, among those the comparison resolves.</summary>
    public static List<(string Arm, string Operation, string Name, string Answer)> Unresolved { get; } = [];

    /// <summary>Reads the names from the pinned ECMA-262 tables and PropertyValueAliases.txt.</summary>
    public static void ReadNames(string pins)
    {
        var binary = File.ReadAllText(Path.Combine(pins, "ecma262", "table-binary-unicode-properties.html"));
        BinaryNames = Regex.Matches(binary, "<a href=\"[^\"]*\">`([A-Za-z_]+)`</a>").Select(static m => m.Groups[1].Value).ToList();
        var strings = File.ReadAllText(Path.Combine(pins, "ecma262", "table-binary-unicode-properties-of-strings.html"));
        StringPropertyNames = Regex.Matches(strings, "<td>`([A-Za-z_]+)`</td>").Select(static m => m.Groups[1].Value).ToList();

        var categories = new List<string[]>();
        var scripts = new List<string[]>();
        foreach (var line in File.ReadLines(Path.Combine(pins, "ucd-17.0.0", "PropertyValueAliases.txt")))
        {
            if (line.Length == 0 || line[0] == '#')
                continue;
            var fields = line.Split('#')[0].Split(';').Select(static f => f.Trim()).ToArray();
            if (fields[0] == "gc")
                categories.Add(fields[1..]);
            else if (fields[0] == "sc")
                scripts.Add(fields[1..]);
        }

        CategoryRecords = categories;
        ScriptRecords = scripts;
        CategoryNames = categories.Select(static r => r[0]).Where(static s => s.Length == 2 && s != "LC").Order(StringComparer.Ordinal).ToList();
        ScriptNames = scripts.Select(static r => r[0]).Order(StringComparer.Ordinal).ToList();
        if (BinaryNames.Count != 53 || StringPropertyNames.Count != 7 || CategoryNames.Count != 30 || ScriptNames.Count == 0)
        {
            throw new InvalidDataException(
                $"the pinned tables gave {BinaryNames.Count} binary properties, {StringPropertyNames.Count} properties of strings, " +
                $"{CategoryNames.Count} categories and {ScriptNames.Count} scripts; expected 53, 7, 30 and some");
        }
    }

    // ---- notation ----------------------------------------------------------------------------------

    public static string Hex(int codePoint) => codePoint.ToString("X4", CultureInfo.InvariantCulture);

    /// <summary>A set in UCD notation, sorted and merged, separated by single spaces: <c>0041..005A 00B5</c>; the empty set is <c>-</c>.</summary>
    public static string RangesText(IEnumerable<(int Lo, int Hi)> ranges)
    {
        var built = new StringBuilder();
        foreach (var (lo, hi) in Merge(ranges))
        {
            if (built.Length > 0)
                built.Append(' ');
            built.Append(Hex(lo));
            if (hi != lo)
                built.Append("..").Append(Hex(hi));
        }

        return built.Length == 0 ? "-" : built.ToString();
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

    /// <summary>The members of a property of strings, as a key: SHA-256 of their sorted UTF-16LE code units joined by U+000A.</summary>
    private static string MembersKey(IEnumerable<string> members)
    {
        var joined = string.Join('\n', members.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal));
        return Convert.ToHexStringLower(SHA256.HashData(System.Runtime.InteropServices.MemoryMarshal.AsBytes(joined.AsSpan())));
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
            return Result.Exception(error.Kind);
        }
        catch (JX.JSException error)
        {
            return Result.Exception(error.Kind);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return Result.Exception(exception.GetType().Name);
        }
    }

    // ---- what each arm does not implement ------------------------------------------------------

    /// <summary>The operations an arm does not implement, with the reason the bundle gives.</summary>
    public static IReadOnlyList<(string Arm, string Operation, string Reason)> NotImplemented { get; } =
    [
        ("J", "fold, orbit, canonicalize, canonicalize-orbit",
            "no member of arm J's list implements Canonicalize: Broiler.JS folds through Broiler.Regex's CaseFolding and, without u, the translator's .NET casing (rows F1 and F2), which the rule does not copy"),
        ("J", "lexical Soft_Dotted",
            "arm J's Soft_Dotted is JSStringSpecialCasing's private hand set inside the package, which no copied member exposes; it is compared through Lithuanian casing (row C5)"),
        ("J", "upper, lower and normalize under a maximum result length (;max=: Broiler.VM's ceiling and the U7 limit variants)",
            "Broiler.JS has no limit and no meter (rows C12, C13, N6 and N7): arm J is called with no limit only, against S at int.MaxValue"),
        ("V", "upper, lower and normalize with no maximum (a parameter without ;max=)",
            "Broiler.VM always has its ceiling: arm V is called with ;max=16777216, its ceiling as its adapter passes it, and with the U7 limits"),
        ("J", "charge", "Broiler.JS charges nothing (rows C13 and N7)"),
        ("J", "lookup with JSRegExp.ResolvePropertyOperand under u", "ResolvePropertyOperand resolves a v-mode class set operand; under u only TranslateUnicodeProperty is reached"),
        ("J", "normalize with JSIntlCollator.Compare's normalization in NFC, NFKC and NFKD", "the collator normalizes to NFD only"),
        ("J", "upper and lower with JSString.ToLocale*Case(undefined) in any parameter but default",
            "with no locale argument the culture is the host's current culture: the invariant culture in the comparison run, tr-TR and lt-LT in the rule's second runs (@tr-TR, @lt-LT)"),
        ("V", "upper and lower with the locales tr,en, az, TR and tr-u-co-search (the run cells of rows C7 and C9)",
            "a locale's resolution is Broiler.VM's adapter's (TransformCase's CanonicalizeLocaleList and LookupMatchingLocale, not copied); arm V is called with the language it resolves, as turkic"),
        ("J,V", "word-status: the status's value",
            "both arms answer isWordLike, a boolean, where S answers a WordStatus; each is compared, as Y or N per word segment of S, with S's status != None (row G4)"),
        ("J,V", "the charge of segmentation", "the rule compares segmentation's charge with no arm: its units change by JSD-0074 decision 4 (row G5)"),
    ];

    // ---- the operations ------------------------------------------------------------------------

    public static IReadOnlyList<Operation> Operations(bool cultureRun, bool negativeControl)
    {
        Prepare();
        if (negativeControl)
        {
            // The rule's two negative controls: a copy of arm V with one charge changed for one input of U4, and
            // a copy of arm J with one result changed for one input of U1, each run once, alone.
            var normalize = Normalize();
            var lexical = Lexical();
            return
            [
                normalize with
                {
                    Classes = ["U4"],
                    Variants = [normalize.Variants.Single(static v => v.Arm == "V") with { Arm = ControlV }],
                },
                lexical with
                {
                    Expand = static item => [new Case(item, LexicalSets[0], 0)],
                    Variants = [lexical.Variants.Single(static v => v.Name == "CharExtensions.IsIdentifierStart") with { Arm = ControlJ }],
                },
            ];
        }

        if (cultureRun)
        {
            // The rule's second run of arm J's culture-dependent members: casing and the host's normalization,
            // arm J's variants alone, against S as Broiler.JS's adapter calls it.
            return new[] { Casing(upper: true), Casing(upper: false), Normalize() }
                .Select(static o => o with { Variants = o.Variants.Where(static v => v.Arm == "J").ToArray() })
                .ToList();
        }

        var all = new List<Operation>
        {
            Binary(),
            Values("gc", "General_Category", CategoryNames),
            Values("sc", "Script", ScriptNames),
            Values("scx", "Script_Extensions", ScriptNames),
            Lexical(),
        };
        all.AddRange(Folding());
        all.Add(Lookup());
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

    // ---- the sets each arm resolves the canonical names to, once, before any parallel work ----------

    private static CodePointSet?[] sBinary = [];
    private static VF.JsUnicodeSet?[] vBinary = [];
    private static (int Lo, int Hi)[]?[] jBinary = [];
    private static readonly Dictionary<string, (CodePointSet?[] S, VF.JsUnicodeSet?[] V, (int Lo, int Hi)[]?[] J)> valueSets = new(StringComparer.Ordinal);
    private static StringMembers[] sStringMembers = [];
    private static StringMembers[] vStringMembers = [];
    private static StringMembers?[] jStringMembers = [];
    private static Namer sNamer = new();
    private static Namer vNamer = new();
    private static Namer jNamer = new();
    private static bool prepared;

    private static void Prepare()
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

        // Each arm's resolutions of every canonical name, by members, to name what a lookup resolves to.
        sNamer = BuildNamer(
            (p, v) => SProperties.TryResolve(p, v, out var set) ? RangesOf(set).ToList() : null,
            n => SProperties.TryResolve(n, out var set) ? RangesOf(set).ToList() : null,
            sStringMembers);
        vNamer = BuildNamer(
            (p, v) => VF.JsUnicodeProperties.TryResolve(p, v, out var set) ? RangesOf(set).ToList() : null,
            n => VF.JsUnicodeProperties.TryResolveLone(n, out var set) ? RangesOf(set).ToList() : null,
            vStringMembers);
        jNamer = BuildNamer(
            (p, v) => JRanges(null, v, p + "=" + v)?.ToList(),
            n => JRanges(null, n, n)?.ToList(),
            jStringMembers);
    }

    private static T? Unresolve<T>(string arm, string operation, string name)
        where T : struct
    {
        lock (Unresolved)
            Unresolved.Add((arm, operation, name, "refused"));
        return null;
    }

    /// <summary>Arm J's set for a standalone \p{inner}, as its translator and fall-through resolve it, or null.</summary>
    private static (int Lo, int Hi)[]? JRanges(string? operation, string name, string inner)
    {
        string answer;
        try
        {
            var resolution = JR.JSRegExp.TranslateUnicodeProperty(false, inner, false, false, false) ?? JR.JSRegExp.NetFallThrough("\\p", inner);
            if (resolution.Ranges is { } ranges)
                return Merge(ranges).ToArray();
            answer = "no set";
        }
        catch (JX.JSException error)
        {
            answer = "exception:" + error.Kind;
        }

        if (operation is not null)
        {
            lock (Unresolved)
                Unresolved.Add(("J", operation, name, answer));
        }

        return null;
    }

    // ---- naming a lookup's resolution ------------------------------------------------------------

    /// <summary>
    /// Names the set a lookup resolves to, in the arbiter's notation: a binary property's canonical name,
    /// <c>Property=Value</c> with both long names, or a property of strings' name. A set is named by the
    /// canonical name the same arm resolves to the same members: among the family the lookup's property
    /// names (General_Category, Script or Script_Extensions for a pair; General_Category and the binary
    /// properties for a lone name) first, the candidate an alias of which is the name given, loosely
    /// compared, before the others. A set no canonical name of the arm has is written <c>set:</c> and its
    /// ranges in UCD notation.
    /// </summary>
    private sealed class Namer
    {
        public readonly Dictionary<string, List<(string Family, string Name, string[] Aliases)>> BySet = new(StringComparer.Ordinal);

        public readonly Dictionary<string, string> ByStrings = new(StringComparer.Ordinal);

        public void Add(string family, string name, string[] aliases, IEnumerable<(int Lo, int Hi)>? ranges)
        {
            if (ranges is null)
                return;
            var key = RangesText(ranges);
            if (!BySet.TryGetValue(key, out var list))
                BySet[key] = list = [];
            list.Add((family, name, aliases));
        }

        public string Name(IEnumerable<(int Lo, int Hi)> ranges, string[] families, string given)
        {
            var key = RangesText(ranges);
            if (!BySet.TryGetValue(key, out var candidates))
                return "set:" + key;

            var loose = Loose(given);
            foreach (var pool in new[] { candidates.Where(c => families.Contains(c.Family)).ToList(), candidates })
            {
                if (pool.Count == 0)
                    continue;
                var named = pool.FirstOrDefault(c => c.Aliases.Any(a => Loose(a) == loose));
                return named.Name ?? pool[0].Name;
            }

            return "set:" + key;
        }
    }

    private static string Loose(string name) =>
        new(name.Where(static c => c is not ('_' or '-' or ' ')).Select(char.ToLowerInvariant).ToArray());

    private static Namer BuildNamer(
        Func<string, string, List<(int Lo, int Hi)>?> pair, Func<string, List<(int Lo, int Hi)>?> lone, IReadOnlyList<StringMembers?> strings)
    {
        var namer = new Namer();
        foreach (var record in CategoryRecords)
            namer.Add("General_Category", "General_Category=" + record[1], record, pair("General_Category", record[0]));
        foreach (var name in BinaryNames)
            namer.Add("binary", name, [name], lone(name));
        foreach (var record in ScriptRecords)
        {
            namer.Add("Script", "Script=" + record[1], record, pair("Script", record[0]));
            namer.Add("Script_Extensions", "Script_Extensions=" + record[1], record, pair("Script_Extensions", record[0]));
        }

        for (var i = 0; i < StringPropertyNames.Count; i++)
        {
            if (strings[i] is { } members)
                namer.ByStrings[members.Key] = StringPropertyNames[i];
        }

        return namer;
    }

    /// <summary>The families a lookup's property names: by the property, loosely read, for a pair; General_Category and binary alone.</summary>
    private static string[] Families(Item item) => item.Kind == "lone"
        ? ["General_Category", "binary"]
        : Loose(item.Property) switch
        {
            "gc" or "generalcategory" => ["General_Category"],
            "sc" or "script" => ["Script"],
            "scx" or "scriptextensions" => ["Script_Extensions"],
            _ => ["General_Category", "Script", "Script_Extensions", "binary"],
        };

    private static string Given(Item item) => item.Kind == "lone" ? item.Property : item.Value;

    // ---- U1: properties, lexical classes, folding ------------------------------------------------

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
        sBinary[c.A] is { } set ? Result.Bool(set.Contains(c.Item.CodePoint)) : Result.Refused;

    /// <summary>
    /// A non-binary property's value at each code point: the short names of the values whose set holds it,
    /// space-separated in ordinal order (<c>-</c> for none). S answers General_Category with GeneralCategoryOf,
    /// and Script and Script_Extensions from the sets its lookup resolves, as neither has a member of its own.
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

            return Result.Plain(found.Count == 0 ? "-" : string.Join(' ', found));
        }

        Result Shared(Case c) => property == "General_Category"
            ? Result.Plain(CategoryNames[(int)SProperties.GeneralCategoryOf(c.Item.CodePoint)])
            : Collect(valueSets[property].S, set => set.Contains(c.Item.CodePoint));

        return new(name, ["U1"], Once,
        [
            new("V", "JsUnicodeProperties.TryResolve", c => Collect(valueSets[property].V, set => set.Contains(c.Item.CodePoint)), Shared, "S"),
            new("J", "JSRegExp.TranslateUnicodeProperty", c =>
                {
                    var sets = valueSets[property].J;
                    var found = new List<string>(2);
                    for (var i = 0; i < sets.Length; i++)
                    {
                        if (sets[i] is { } ranges && InRanges(ranges, c.Item.CodePoint))
                            found.Add(names[i]);
                    }

                    return Result.Plain(found.Count == 0 ? "-" : string.Join(' ', found));
                }, Shared, "S"),
        ]);
    }

    /// <summary>
    /// The lexical sets at each code point. Arm V's tokenizer and its matcher's group names read
    /// JsUnicodeLexical (correspondence rows I1 to I3); arm J's scanner reads CharExtensions and FastScanner, and
    /// its group names JSRegExp's own tests.
    /// </summary>
    private static Operation Lexical()
    {
        static char Unit(int codePoint) => codePoint <= 0xFFFF ? (char)codePoint : char.ConvertFromUtf32(codePoint)[0];

        static Result Shared(Case c) => Result.Bool(c.A switch
        {
            0 or 2 => SLexical.IsIdentifierStart(c.Item.CodePoint),
            1 or 3 => SLexical.IsIdentifierPart(c.Item.CodePoint),
            4 => SLexical.IsWhiteSpace(c.Item.CodePoint),
            _ => SCasing.IsSoftDotted(c.Item.CodePoint),
        });

        return new("lexical", ["U1"],
            static item => LexicalSets.Select((name, index) => new Case(item, name, index)),
            [
                new("V", "JsUnicodeLexical and JsIntlTables.IsSoftDotted", static c => Result.Bool(c.A switch
                    {
                        0 or 2 => VF.JsUnicodeLexical.IsIdentifierStart(c.Item.CodePoint),
                        1 or 3 => VF.JsUnicodeLexical.IsIdentifierPart(c.Item.CodePoint),
                        4 => VF.JsUnicodeLexical.IsWhiteSpace(c.Item.CodePoint),
                        _ => new V.JsEngine().Intl!.IsSoftDotted(c.Item.CodePoint),
                    }), Shared, "S"),
                new("J", "CharExtensions.IsIdentifierStart", static c =>
                    {
                        if (c.A != 0)
                            return null;
                        var answer = JP.CharExtensions.IsIdentifierStart(c.Item.CodePoint);
                        return Result.Bool(NegativeControl && c.Item.CodePoint == PlantedJCodePoint ? !answer : answer);
                    }, Shared, "S"),
                new("J", "CharExtensions.IsIdentifierPart", static c => c.A == 1 ? Result.Bool(JP.CharExtensions.IsIdentifierPart(c.Item.CodePoint)) : null,
                    Shared, "S"),
                new("J", "JSRegExp.IsRegExpIdentifierStart", static c => c.A == 2 ? Result.Bool(JR.JSRegExp.IsRegExpIdentifierStart(c.Item.CodePoint)) : null,
                    Shared, "S"),
                new("J", "JSRegExp.IsRegExpIdentifierPart", static c => c.A == 3 ? Result.Bool(JR.JSRegExp.IsRegExpIdentifierPart(c.Item.CodePoint)) : null,
                    Shared, "S"),
                // The scanner reads one code unit: a supplementary code point's first unit, its high surrogate. A
                // unit its skip loop consumes is white space unless the loop reads it as a line terminator.
                new("J", "FastScanner.IsWhiteSpace", static c =>
                    {
                        if (c.A != 4)
                            return null;
                        var first = Unit(c.Item.CodePoint);
                        return Result.Bool(JP.FastScanner.IsWhiteSpace(first) && !JP.FastScanner.IsLineTerminatorInSkip(first));
                    }, Shared, "S"),
            ]);
    }

    private static IEnumerable<Operation> Folding()
    {
        yield return new("fold", ["U1"], Once,
        [
            new("V", "JsUnicodeCaseFolding.SimpleFold", static c => Result.CodePoint(VF.JsUnicodeCaseFolding.SimpleFold(c.Item.CodePoint)),
                static c => Result.CodePoint(SFolding.SimpleFold(c.Item.CodePoint)), "S"),
        ]);

        yield return new("orbit", ["U1"], Once,
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

        yield return new("canonicalize", ["U1"], Once,
        [
            new("V", "JsUnicodeCaseFolding.SimpleUpper", static c => Result.CodePoint(VF.JsUnicodeCaseFolding.SimpleUpper(c.Item.CodePoint)),
                static c => Result.CodePoint(SFolding.SimpleUpper(c.Item.CodePoint)), "S"),
        ]);

        yield return new("canonicalize-orbit", ["U1"], Once,
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

    /// <summary>The lookup of a name, lone or Name=Value, under u and under v, by its resolution or its refusal.</summary>
    private static Operation Lookup() => new(
        "lookup", ["U2"],
        static item => [new Case(item, "u", 0), new Case(item, "v", 1)],
        [
            new("V", "JsUnicodeProperties.TryResolve and JsUnicodeStringProperties.TryResolve", VLookup, SLookup, "S"),
            new("J", "JSRegExp.TranslateUnicodeProperty", static c => JLookup(c.Item, unicodeSets: c.A == 1), SLookup, "S"),
            new("J", "JSRegExp.ResolvePropertyOperand", static c => c.A == 1 ? JOperand(c.Item) : null, SLookup, "S"),
        ]);

    private static string Inner(Item item) => item.Kind == "lone" ? item.Property : item.Property + "=" + item.Value;

    private static Result SLookup(Case c)
    {
        var item = c.Item;
        if (item.Kind == "pair")
        {
            return SProperties.TryResolve(item.Property, item.Value, out var pair)
                ? Result.Plain(sNamer.Name(RangesOf(pair), Families(item), Given(item)))
                : Result.Refused;
        }

        // Under v, a lone name is first a property of strings (adapter guide, UnicodeProperties step 3).
        if (c.A == 1 && SStrings.TryResolve(item.Property, out var property))
            return Result.Plain(sNamer.ByStrings.GetValueOrDefault(sStringMembers[property.Id].Key, "strings:" + sStringMembers[property.Id].Key));

        return SProperties.TryResolve(item.Property, out var set) ? Result.Plain(sNamer.Name(RangesOf(set), Families(item), Given(item))) : Result.Refused;
    }

    /// <summary>
    /// Arm V's lookup as its matcher calls it (JsRegExpMatcher.cs:2776-2810): under v, a lone name is first a
    /// property of strings; then the lone or the Name=Value lookup. Its false is the matcher's SyntaxError.
    /// </summary>
    private static Result? VLookup(Case c)
    {
        var item = c.Item;
        if (item.Kind == "pair")
        {
            return VF.JsUnicodeProperties.TryResolve(item.Property, item.Value, out var pair)
                ? Result.Plain(vNamer.Name(RangesOf(pair), Families(item), Given(item)))
                : Result.Refused;
        }

        if (c.A == 1 && VF.JsUnicodeStringProperties.TryResolve(item.Property, out var property))
            return Result.Plain(vNamer.ByStrings.GetValueOrDefault(vStringMembers[property].Key, "strings:" + vStringMembers[property].Key));

        return VF.JsUnicodeProperties.TryResolveLone(item.Property, out var set) ? Result.Plain(vNamer.Name(RangesOf(set), Families(item), Given(item))) : Result.Refused;
    }

    /// <summary>
    /// Arm J's translator on a standalone \p{inner}: TranslateUnicodeProperty, and on its null the fall-through to
    /// .NET's \p, whose answer is written <c>dotnet:</c> and the code units .NET matches in UCD notation. Its
    /// SyntaxError is the lookup's refusal.
    /// </summary>
    private static Result? JLookup(Item item, bool unicodeSets)
    {
        var inner = Inner(item);
        try
        {
            if (JR.JSRegExp.TranslateUnicodeProperty(false, inner, false, unicodeSets, false) is { } resolution)
            {
                return resolution.Sequences is { } sequences
                    ? Result.Plain(jNamer.ByStrings.GetValueOrDefault(MembersKey(sequences), "strings:" + MembersKey(sequences)))
                    : resolution.Ranges is { } ranges
                        ? Result.Plain(jNamer.Name(ranges, Families(item), Given(item)))
                        : Result.Plain("dotnet:" + resolution.NetPattern);
            }

            var fallThrough = JR.JSRegExp.NetFallThrough("\\p", inner);
            return Result.Plain("dotnet:" + RangesText(fallThrough.Ranges!));
        }
        catch (JX.JSException error) when (error.Kind == "SyntaxError")
        {
            return Result.Refused;
        }
    }

    /// <summary>Arm J's v-mode class set operand: a property of strings when it holds strings, else its set; its SyntaxError the refusal.</summary>
    private static Result? JOperand(Item item)
    {
        try
        {
            var value = JR.JSRegExp.ResolvePropertyOperand(false, Inner(item));
            if (value.Strings.Count > 0)
            {
                var key = MembersKey(RangeMembers(value.Ranges).Concat(value.Strings));
                return Result.Plain(jNamer.ByStrings.GetValueOrDefault(key, "strings:" + key));
            }

            return Result.Plain(jNamer.Name(value.Ranges, Families(item), Given(item)));
        }
        catch (JX.JSException error) when (error.Kind == "SyntaxError")
        {
            return Result.Refused;
        }
    }

    // ---- U1, U5: the properties of strings ------------------------------------------------------

    /// <summary>One arm's members of one property of strings, built once: its code points and its sequences.</summary>
    private sealed class StringMembers(IReadOnlyList<(int Lo, int Hi)> ranges, HashSet<string> sequences, Func<int, bool>? codePoint = null)
    {
        /// <summary>The members as a key (<see cref="MembersKey"/>), each code point a member of its own.</summary>
        public string Key { get; } = MembersKey(RangeMembers(ranges).Concat(sequences));

        /// <summary>Whether <paramref name="text"/> is a member: a code point by the arm's own test when it has one, else by its ranges; any text by its sequences.</summary>
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

    /// <summary>Arm J's members of a property of strings, as its translator resolves the name under v, or null when it does not.</summary>
    private static StringMembers? JMembers(string name)
    {
        string answer;
        try
        {
            var resolution = JR.JSRegExp.TranslateUnicodeProperty(false, name, false, true, false);
            if (resolution?.Sequences is { } sequences)
                return new StringMembers([], new HashSet<string>(sequences, StringComparer.Ordinal));
            answer = resolution is null ? "fall-through" : "no sequences";
        }
        catch (JX.JSException error)
        {
            answer = "exception:" + error.Kind;
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

    // ---- U3, U7: casing -------------------------------------------------------------------------

    /// <summary>
    /// The charge Broiler.VM's adapter adds for the language pass, before it calls S, when the language is not
    /// the default (JSD-0074 decision 4; adapter guide, UnicodeCaseMapping step 3). It is what Broiler.VM's
    /// LanguageCasing charges (JsRealm.String.cs:1173): StringCharge(engine, text.Length), the input's length in
    /// UTF-16 code units, and one unit for the empty text, as StringCharge charges at least one (:973-974). The
    /// pass runs only for tr, az and lt (:1168-1171), so the default language adds nothing.
    /// </summary>
    public static ulong LanguagePassCharge(string text, CaseLanguage language) =>
        language == CaseLanguage.Default ? 0UL : text.Length <= 0 ? 1UL : (ulong)text.Length;

    /// <summary>
    /// The cases of one parameter. With no limit, the bare parameter, which arm J alone takes (Broiler.JS has no
    /// limit; S at int.MaxValue). Then, in the comparison run, arm V's: Broiler.VM's ceiling as its adapter passes
    /// it (<c>;max=16777216</c>), and the U7 limits, S's own result's length, one less and one more, each written
    /// <c>;max=n</c>, a limit equal to the ceiling only once.
    /// </summary>
    private static IEnumerable<Case> Limited(Item item, string parameter, int a, Func<int> length)
    {
        yield return new Case(item, parameter + CultureSuffix, a);
        if (CultureSuffix.Length > 0)
            yield break;

        yield return new Case(item, parameter + ";max=" + VmCeiling.ToString(CultureInfo.InvariantCulture), a, VmCeiling);
        var n = length();
        foreach (var max in new[] { n - 1, n, n + 1 })
        {
            // The adapter never passes a negative maximum: an empty result has no "one less".
            if (max >= 0 && max != VmCeiling)
                yield return new Case(item, parameter + ";max=" + max.ToString(CultureInfo.InvariantCulture), a, max);
        }
    }

    private static UnicodeStatus SMap(string text, bool upper, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result) =>
        upper ? SCasing.ToUpperCase(text, language, maxLength, charge, out result) : SCasing.ToLowerCase(text, language, maxLength, charge, out result);

    private static bool IsFamily(Item item, string family) =>
        item.Class != "U7" || item.Tag.Length == 0 || item.Tag.StartsWith(family + ":", StringComparison.Ordinal);

    private static Operation Casing(bool upper)
    {
        IEnumerable<Case> Expand(Item item)
        {
            for (var language = 0; language < 3; language++)
            {
                var captured = language;
                foreach (var c in Limited(item, Languages[language], language, () =>
                         {
                             SMap(item.Text, upper, SLanguages[captured], int.MaxValue, null, out var unlimited);
                             return unlimited!.Length;
                         }))
                {
                    yield return c;
                }
            }

            if (item.Tag is "cell:C7" or "cell:C9")
            {
                for (var locale = 3; locale < Languages.Length; locale++)
                    yield return new Case(item, Languages[locale] + CultureSuffix, locale);
            }
        }

        Result SForV(Case c)
        {
            var meter = new Meter();
            var language = SLanguages[c.A];
            var extra = LanguagePassCharge(c.Item.Text, language);
            if (extra > 0)
                meter.Charge(extra);
            var status = SMap(c.Item.Text, upper, language, c.B < 0 ? VmCeiling : c.B, meter.Charge, out var result);
            return (status == UnicodeStatus.Success ? Result.Text(result!) : Result.Over).WithCharge(meter.Total);
        }

        Result SForJ(Case c)
        {
            SMap(c.Item.Text, upper, SLanguages[c.A], int.MaxValue, null, out var result);
            return Result.Text(result!);
        }

        Result? VCase(Case c)
        {
            // Arm V is called with a maximum only: Broiler.VM's ceiling, or a U7 limit.
            if (c.A >= 3 || c.B < 0)
                return null;

            var engine = new V.JsEngine();
            var ceiling = c.B;
            try
            {
                var result = c.A == 0
                    ? V.JsRealm.StringChangeCase(engine, c.Item.Text, upper, ceiling)
                    : V.JsRealm.TransformCase(engine, c.Item.Text, c.A == 1 ? "tr" : "lt", upper, ceiling);
                return Result.Text(result).WithCharge(engine.Charged);
            }
            catch (V.JsThrow error)
            {
                return VRefusal(error).WithCharge(engine.Charged);
            }
        }

        Result? JCase(Case c)
        {
            if (c.B >= 0)
                return null;

            var text = c.Item.Text;
            var culture = CultureInfo.CurrentCulture;
            return Result.Text(c.A switch
            {
                0 => upper ? JS.JSString.ToUpperCase(text) : JS.JSString.ToLowerCase(text),
                1 => upper ? JS.JSString.ToLocaleUpperCase(text, "tr", culture) : JS.JSString.ToLocaleLowerCase(text, "tr", culture),
                2 => upper ? JS.JSString.ToLocaleUpperCase(text, "lt", culture) : JS.JSString.ToLocaleLowerCase(text, "lt", culture),
                _ => upper ? JS.JSString.ToLocaleUpperCase(text, Languages[c.A], culture) : JS.JSString.ToLocaleLowerCase(text, Languages[c.A], culture),
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

        var member = upper ? "Upper" : "Lower";
        return new(upper ? "upper" : "lower", ["U3", "U7"], Expand,
        [
            new("V", "JsRealm.StringChangeCase and TransformCase", VCase, SForV, "S/V"),
            new("J", $"JSString.To{member}Case and ToLocale{member}Case(locale)", JCase, SForJ, "S/J"),
            new("J", $"JSString.ToLocale{member}Case(undefined)", JNoLocale, SForJ, "S/J"),
        ],
        static item => IsFamily(item, "casing"));
    }

    /// <summary>Arm V's refusal: its RangeError "Invalid string length" is the ceiling's, the kind S's LimitExceeded is.</summary>
    private static Result VRefusal(V.JsThrow error) =>
        error.Kind == "RangeError" && error.Message == "Invalid string length" ? Result.Over : Result.Exception(error.Kind);

    // ---- U4, U7: normalization ------------------------------------------------------------------

    private static Operation Normalize()
    {
        static IEnumerable<Case> Expand(Item item)
        {
            for (var form = 0; form < Forms.Length; form++)
            {
                var captured = form;
                foreach (var c in Limited(item, Forms[form], form, () =>
                         {
                             SNormalization.Transform(item.Text, (NormalForm)captured, int.MaxValue, null, out var unlimited);
                             return unlimited!.Length;
                         }))
                {
                    yield return c;
                }
            }
        }

        static Result SForV(Case c)
        {
            var meter = new Meter();
            var status = SNormalization.Transform(c.Item.Text, (NormalForm)c.A, c.B < 0 ? VmCeiling : c.B, meter.Charge, out var result);
            return (status == UnicodeStatus.Success ? Result.Text(result!) : Result.Over).WithCharge(meter.Total);
        }

        static Result SForJ(Case c)
        {
            SNormalization.Transform(c.Item.Text, (NormalForm)c.A, int.MaxValue, null, out var result);
            return Result.Text(result!);
        }

        static Result? VNormalize(Case c)
        {
            // Arm V is called with a maximum only: Broiler.VM's ceiling, or a U7 limit.
            if (c.B < 0)
                return null;

            var planted = NegativeControl && c.Item.Class == "U4" && c.Item.Line == 1 && c.Parameter == PlantedVParameter;
            var engine = new V.JsEngine(planted);
            try
            {
                var result = V.JsRealm.NormalizeBuiltIn(engine, c.Item.Text, Forms[c.A], c.B);
                return Result.Text(result).WithCharge(engine.Charged);
            }
            catch (V.JsThrow error)
            {
                return VRefusal(error).WithCharge(engine.Charged);
            }
        }

        static Result? JNormalize(Case c) => c.B >= 0 ? null : Result.Text(JS.JSString.Normalize(c.Item.Text, Forms[c.A]));

        static Result? JCollator(Case c) => c.B >= 0 || c.A != 1 ? null : Result.Text(JS.JSString.CollatorNormalize(c.Item.Text));

        return new("normalize", ["U4", "U7"], Expand,
        [
            new("V", "JsRealm.NormalizeText", VNormalize, SForV, "S/V"),
            new("J", "JSString.Normalize", JNormalize, SForJ, "S/J"),
            new("J", "JSIntlCollator.Compare's NFD", JCollator, SForJ, "S/J"),
        ],
        static item => IsFamily(item, "normalization"));
    }

    /// <summary>A meter: the total of every charge S's work makes.</summary>
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

    /// <summary>
    /// The word status, on S's word segments: Y or N per segment, in order, as the arbiter writes it; S's Y is
    /// a status other than None, and each arm's its isWordLike on the same segment.
    /// </summary>
    private static Operation WordStatuses() => new(
        "word-status", ["U6"], Once,
        [
            new("V", "JsSegmentation.IsWordLike", static c =>
                {
                    var breaks = new V.JsEngine().Intl!.Breaks;
                    return Statuses(c.Item.Text, (start, end) => V.JsSegmentation.IsWordLike(breaks, c.Item.Text, start, end));
                },
                SStatuses, "S"),
            new("J", "JSIntlSegments.IsWordLike", static c => Statuses(c.Item.Text, (start, end) => JI.JSIntlSegments.IsWordLike(c.Item.Text.Substring(start, end - start))),
                SStatuses, "S"),
        ]);

    private static Result SStatuses(Case c) =>
        Statuses(c.Item.Text, (start, end) => SSegmenter.GetWordStatus(c.Item.Text.AsSpan(start, end - start), null) != WordStatus.None);

    private static Result Statuses(string text, Func<int, int, bool> wordLike)
    {
        var boundaries = SSegmenter.Boundaries(text, SegmentGranularity.Word, null);
        var built = new StringBuilder(boundaries.Length);
        for (var i = 0; i + 1 < boundaries.Length; i++)
            built.Append(wordLike(boundaries[i], boundaries[i + 1]) ? 'Y' : 'N');
        return Result.Plain(built.ToString());
    }
}
