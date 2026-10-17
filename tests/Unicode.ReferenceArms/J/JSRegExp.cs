// REFERENCE ARM J: a test-only copy of Broiler.JS's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.JS at 4d70821e (the engine sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at Broiler.JS bdb1a3ca):
//   Broiler.JS/Broiler.JavaScript.BuiltIns/RegExp/JSRegExp.cs
//     blob 4859660c7bf76521d1872040af7a9bb93a9476c8: IsRegExpIdentifierStart and
//     IsRegExpIdentifierPart (:2028-2052); the translator's property resolution: GeneralCategoryNames
//     (:3418-3424), EmojiStringPropertyNames (:3426-3446), EmojiAlternationCache (:3448-3453),
//     BuildGeneralCategoryNames (:3455-3517),
//     TranslateUnicodeProperty with its NormalizeKey and its fall-through (:3691-3781), TranslateGeneralCategory
//     (:3783-3804), NewUnsupportedPropertyError (:3806-3808), ExpandEmojiStringProperty (:3810-3850),
//     ExpandCodePointProperty (:3852-3924) and ComplementCodePointRanges (:3926-3945); and, as
//     NetFallThrough, the lines a fall-through reaches: the options u and v leave (:679, :722-732), new Regex
//     (:1004) and the ArgumentException that becomes a SyntaxError (:1028-1031)
//   Broiler.JS/Broiler.JavaScript.BuiltIns/RegExp/JSRegExp.UnicodeSets.cs
//     blob a8e31b7efe74d9a85c8b0b4835902bd09b085096: ClassSetValue (:34-62), NormalizeKey
//     (:193-203), ResolvePropertyOperand (:419-457), Normalize (:561-581), SubtractRanges (:614-634),
//     ComplementCodePoints (:639-645) and NewUnsupportedSetError (:694-696)
//   packages Broiler.Unicode.Properties 0.1.0-preview.3 (UnicodeProperties.GetGeneralCategory,
//     GetBinaryProperty, GetScript, GetScriptExtensions) and Broiler.Unicode.Emoji.StringProperties
//     0.1.0-preview.3 (EmojiStringProperties.GetSequences, EmojiSequenceProperties), the versions Broiler.JS
//     resolved at 4d70821e through Broiler.Regex 0.1.0-preview.4 (Directory.Packages.props:16)
//
// Changes from the source, each marked "ARM CHANGE" where it is made; none changes which name resolves, to
// what set, or which is refused:
//   - line endings normalised to LF (the first source has CRLF); the namespace is the source's;
//   - the members sit in a static partial class JSRegExp of their own; the ones the comparison calls are
//     internal where the source's are private; the class's other members are not copied;
//   - a resolution answers a PropertyResolution, the ranges or the sequences the source expands into .NET
//     pattern text, where the source answers that text: ExpandCodePointProperty answers the ranges outside
//     a class (the source's BuildPositiveCodePointMatcher, never null) and their complement under \P (the
//     source's matcher over ComplementCodePointRanges, or "(?!)" for none), and ExpandEmojiStringProperty
//     answers GetSequences(property), which the source sorts into an alternation; the in-class expansion
//     (:3862-3895) is not copied, and the arm never resolves an escape inside a class;
//   - TranslateGeneralCategory's in-class answer, .NET's own \p{Lu}, is a PropertyResolution carrying that
//     pattern text, unchanged;
//   - NewUnsupportedPropertyError and NewUnsupportedSetError build the arm's recorded exception through the
//     arm's JSEngine (JSEngine.cs beside this file), whose kind is the SyntaxError the source's builds;
//   - ResolvePropertyOperand and its ClassSetValue, copied whole, are internal where the source's are private;
//   - one nullable annotation, on ResolvePropertyOperand's local ranges, as this project builds with nullable
//     warnings as errors and Broiler.JS's does not;
//   - NetFallThrough is new: it holds the source's lines a resolution's null reaches (:1004, :1028-1031), with
//     the options a u- or v-mode pattern has (:679, :722-732), applied to the escape alone, and answers the
//     code units .NET's \p matches (it matches one UTF-16 code unit at a time); it is how the fall-through
//     (:3742-3743, :3778-3780) is compared.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Broiler.JavaScript.Engine.Core;
using Broiler.JavaScript.Runtime;
using Broiler.Unicode.Properties;
using UnicodeEmoji.StringProperties;

namespace Broiler.JavaScript.BuiltIns.RegExp;

/// <summary>
/// ARM CHANGE: what a resolution answers, in place of the .NET pattern text the source builds from it: the
/// ranges of a code point property, the sequences of a property of strings, or .NET's own pattern text.
/// </summary>
internal sealed class PropertyResolution
{
    public (int Lo, int Hi)[]? Ranges { get; init; }

    public IReadOnlyCollection<string>? Sequences { get; init; }

    public string? NetPattern { get; init; }
}

internal static partial class JSRegExp // ARM CHANGE: a static class of its own; the source's is public partial class JSRegExp : JSObject, IJSRegExp
{
    internal static bool IsRegExpIdentifierStart(int cp)
    {
        if (cp < 0 || cp > 0x10FFFF || cp is >= 0xD800 and <= 0xDFFF) return false;
        if (cp is '$' or '_' or 0x1885 or 0x1886 or 0x2118 or 0x212E or 0x309B or 0x309C) return true;
        return char.GetUnicodeCategory(char.ConvertFromUtf32(cp), 0) switch
        {
            UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter or UnicodeCategory.LetterNumber => true,
            _ => false,
        };
    }

    internal static bool IsRegExpIdentifierPart(int cp)
    {
        if (cp < 0 || cp > 0x10FFFF || cp is >= 0xD800 and <= 0xDFFF) return false;
        if (IsRegExpIdentifierStart(cp)) return true;
        if (cp is 0x200C or 0x200D or 0x00B7 or 0x0387 or 0x19DA or 0x30FB or 0xFF65) return true;
        if (cp is >= 0x1369 and <= 0x1371) return true;
        return char.GetUnicodeCategory(char.ConvertFromUtf32(cp), 0) switch
        {
            UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark
            or UnicodeCategory.DecimalDigitNumber or UnicodeCategory.ConnectorPunctuation => true,
            _ => false,
        };
    }

    /// <summary>
    /// General_Category long names / aliases → the .NET short form that
    /// <see cref="Regex"/> accepts (e.g. <c>Uppercase_Letter</c> → <c>Lu</c>).
    /// Keys are normalized (lower-cased, '_'/' ' stripped). Short forms map to
    /// themselves so that the <c>gc=Lu</c> form normalizes too.
    /// </summary>
    private static readonly Dictionary<string, string> GeneralCategoryNames = BuildGeneralCategoryNames();

    /// <summary>
    /// Emoji string-property names (UTS #51 "properties of strings") mapped to the
    /// <see cref="EmojiSequenceProperties"/> bit they select. These match
    /// multi-code-point sequences and are only valid with the <c>v</c> flag; there
    /// they expand to an alternation of the literal sequences (see
    /// <see cref="ExpandEmojiStringProperty"/>), backed by the bundled Unicode emoji data.
    ///
    /// Keys are stored in normalized form (lower-cased, '_'/' ' stripped) because
    /// lookups go through <c>NormalizeKey</c> — e.g. <c>RGI_Emoji</c> normalizes to
    /// <c>rgiemoji</c>.
    /// </summary>
    private static readonly Dictionary<string, EmojiSequenceProperties> EmojiStringPropertyNames = new(StringComparer.Ordinal)
    {
        ["rgiemoji"] = EmojiSequenceProperties.RgiEmoji,
        ["rgiemojiflagsequence"] = EmojiSequenceProperties.RgiEmojiFlagSequence,
        ["rgiemojimodifiersequence"] = EmojiSequenceProperties.RgiEmojiModifierSequence,
        ["rgiemojitagsequence"] = EmojiSequenceProperties.RgiEmojiTagSequence,
        ["rgiemojizwjsequence"] = EmojiSequenceProperties.RgiEmojiZwjSequence,
        ["basicemoji"] = EmojiSequenceProperties.BasicEmoji,
        ["emojikeycapsequence"] = EmojiSequenceProperties.EmojiKeycapSequence,
    };

    /// <summary>
    /// Cache of the expanded .NET alternation for each emoji string property, keyed by
    /// the <see cref="EmojiSequenceProperties"/> mask. Building the alternation walks the
    /// whole emoji trie (thousands of sequences for <c>RGI_Emoji</c>), so it is computed once.
    /// </summary>
    private static readonly Dictionary<EmojiSequenceProperties, PropertyResolution> EmojiAlternationCache = []; // ARM CHANGE: it caches the PropertyResolution, where the source caches the alternation's text

    private static Dictionary<string, string> BuildGeneralCategoryNames()
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        void Add(string shortName, params string[] names)
        {
            map[Normalize(shortName)] = shortName;
            foreach (var n in names)
                map[Normalize(n)] = shortName;
        }

        Add("L", "Letter");
        Add("Lu", "Uppercase_Letter");
        Add("Ll", "Lowercase_Letter");
        Add("Lt", "Titlecase_Letter");
        Add("Lm", "Modifier_Letter");
        Add("Lo", "Other_Letter");
        Add("M", "Mark", "Combining_Mark");
        Add("Mn", "Nonspacing_Mark");
        Add("Mc", "Spacing_Mark");
        Add("Me", "Enclosing_Mark");
        Add("N", "Number");
        Add("Nd", "Decimal_Number", "digit");
        Add("Nl", "Letter_Number");
        Add("No", "Other_Number");
        Add("P", "Punctuation", "punct");
        Add("Pc", "Connector_Punctuation");
        Add("Pd", "Dash_Punctuation");
        Add("Ps", "Open_Punctuation");
        Add("Pe", "Close_Punctuation");
        Add("Pi", "Initial_Punctuation");
        Add("Pf", "Final_Punctuation");
        Add("Po", "Other_Punctuation");
        Add("S", "Symbol");
        Add("Sm", "Math_Symbol");
        Add("Sc", "Currency_Symbol");
        Add("Sk", "Modifier_Symbol");
        Add("So", "Other_Symbol");
        Add("Z", "Separator");
        Add("Zs", "Space_Separator");
        Add("Zl", "Line_Separator");
        Add("Zp", "Paragraph_Separator");
        Add("C", "Other");
        Add("Cc", "Control", "cntrl");
        Add("Cf", "Format");
        Add("Cs", "Surrogate");
        Add("Co", "Private_Use");
        Add("Cn", "Unassigned");

        return map;

        static string Normalize(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (ch == '_' || ch == ' ')
                    continue;
                sb.Append(char.ToLowerInvariant(ch));
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Returns the .NET replacement text for a single <c>\p{inner}</c> /
    /// <c>\P{inner}</c> escape, or <c>null</c> when it should be left untouched.
    /// Throws a SyntaxError for property classes that are recognized but not
    /// yet supported.
    /// </summary>
    // ARM CHANGE: internal, and it answers a PropertyResolution (null for the fall-through) where the source answers .NET pattern text.
    internal static PropertyResolution? TranslateUnicodeProperty(bool negated, string inner, bool inClass, bool unicodeSets, bool ignoreCase)
    {
        var eq = inner.IndexOf('=');
        var prefix = negated ? "\\P" : "\\p";

        static string NormalizeKey(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (ch == '_' || ch == ' ')
                    continue;
                sb.Append(char.ToLowerInvariant(ch));
            }
            return sb.ToString();
        }

        if (eq >= 0)
        {
            // `Name=Value` form. .NET rejects this syntax entirely today, so it is
            // always safe to handle here.
            var name = NormalizeKey(inner.Substring(0, eq));
            var value = NormalizeKey(inner.Substring(eq + 1));

            if (name is "gc" or "generalcategory")
                return TranslateGeneralCategory(value, prefix, negated, inClass, inner, ignoreCase);

            if (name is "sc" or "script")
            {
                var ranges = UnicodeProperties.GetScript(value);
                if (ranges != null && ExpandCodePointProperty(ranges, negated, inClass, ignoreCase) is { } expanded)
                    return expanded;

                throw NewUnsupportedPropertyError(inner);
            }

            if (name is "scx" or "scriptextensions")
            {
                var ranges = UnicodeProperties.GetScriptExtensions(value);
                if (ranges != null && ExpandCodePointProperty(ranges, negated, inClass, ignoreCase) is { } expanded)
                    return expanded;

                throw NewUnsupportedPropertyError(inner);
            }

            // Unknown Name=Value — let .NET surface its own error.
            return null;
        }

        // Lone `\p{Value}` form.
        var lone = NormalizeKey(inner);

        // A lone General_Category value (Lu, L, Letter, …).
        if (UnicodeProperties.GetGeneralCategory(lone) != null
            || (inClass && GeneralCategoryNames.ContainsKey(lone)))
            return TranslateGeneralCategory(lone, prefix, negated, inClass, inner, ignoreCase);

        // Binary Unicode properties (ASCII, Alphabetic, Assigned, Emoji, …) come from
        // the generated UCD 17.0.0 range tables in Broiler.Unicode.Properties.
        var binaryRanges = UnicodeProperties.GetBinaryProperty(lone);
        if (binaryRanges != null)
        {
            var expanded = ExpandCodePointProperty(binaryRanges, negated, inClass, ignoreCase);
            if (expanded != null)
                return expanded;
            throw NewUnsupportedPropertyError(inner);
        }

        if (EmojiStringPropertyNames.TryGetValue(lone, out var emojiProperty))
        {
            // Properties of strings (UTS #51) match multi-code-point sequences, so they are
            // only meaningful with the `v` flag and cannot be negated or nested in a class
            // (a `[...]` set fragment cannot hold multi-character strings, and `\P` of a set
            // containing strings is a SyntaxError). In `v` mode, standalone, expand to an
            // alternation of the literal sequences; everything else stays a SyntaxError.
            if (unicodeSets && !negated && !inClass)
                return ExpandEmojiStringProperty(emojiProperty);

            throw NewUnsupportedPropertyError(inner);
        }

        // Anything else (native short categories like \p{L}, named blocks like
        // \p{IsBasicLatin}, or unknown lone names) is left for .NET to handle.
        return null;
    }

    /// <summary>
    /// Translates a General_Category value (the <c>gc=…</c> dimension or a lone gc value).
    /// Outside a character class it expands to the bundled UCD 17.0.0 code-point ranges so
    /// supplementary-plane code points match by code point. Inside a class — where the
    /// range expansion (with its surrogate-pair alternatives) cannot be nested — it falls
    /// back to .NET's native category escape (<c>\p{Lu}</c>), which is code-unit based.
    /// </summary>
    private static PropertyResolution TranslateGeneralCategory(string value, string prefix, bool negated, bool inClass, string inner, bool ignoreCase) // ARM CHANGE: the answer's type
    {
        if (inClass)
        {
            if (GeneralCategoryNames.TryGetValue(value, out var shortName))
                return new PropertyResolution { NetPattern = $"{prefix}{{{shortName}}}" }; // ARM CHANGE: the source's text, carried
            throw NewUnsupportedPropertyError(inner);
        }

        var ranges = UnicodeProperties.GetGeneralCategory(value);
        if (ranges != null && ExpandCodePointProperty(ranges, negated, inClass, ignoreCase) is { } expanded)
            return expanded;

        throw NewUnsupportedPropertyError(inner);
    }

    private static JSException NewUnsupportedPropertyError(string inner)
        => JSEngine.NewSyntaxError(
            $"Unicode property escape '\\p{{{inner}}}' is not supported yet (requires Unicode script/sequence data)");

    /// <summary>
    /// Expands an emoji string property (UTS #51 property of strings, e.g.
    /// <c>RGI_Emoji</c> or <c>Emoji_Keycap_Sequence</c>) into a non-capturing alternation
    /// of the literal sequences drawn from the bundled Unicode emoji data. Alternatives are
    /// ordered longest-first so leftmost-longest matching picks the maximal sequence — e.g.
    /// the full ZWJ family rather than a leading single-code-point emoji. The result is cached
    /// per property because the alternation is large (thousands of sequences for RGI_Emoji).
    /// </summary>
    private static PropertyResolution ExpandEmojiStringProperty(EmojiSequenceProperties property) // ARM CHANGE: the answer's type
    {
        lock (EmojiAlternationCache)
        {
            if (EmojiAlternationCache.TryGetValue(property, out var cached))
                return cached;

            var sequences = EmojiStringProperties.GetSequences(property);

            // ARM CHANGE: the sequences themselves, where the source orders them longest first (:3827-3834) and
            // joins them, escaped, into a non-capturing alternation (:3836-3846).
            var expanded = new PropertyResolution { Sequences = sequences };
            EmojiAlternationCache[property] = expanded;
            return expanded;
        }
    }

    /// <summary>
    /// Builds the .NET regex text matching a single code point in <paramref name="ranges"/>.
    /// Outside a character class this is a non-capturing group that handles both
    /// BMP code points and supplementary code points (via surrogate pairs); for
    /// <paramref name="negated"/> it matches one code point NOT in the set.
    /// Inside a character class only BMP, non-negated ranges can be expressed as
    /// raw class fragments; anything else returns <c>null</c> (caller errors).
    /// </summary>
    private static PropertyResolution? ExpandCodePointProperty((int Lo, int Hi)[] ranges, bool negated, bool inClass, bool ignoreCase) // ARM CHANGE: the answer's type
    {
        if (inClass)
        {
            // ARM CHANGE: the in-class expansion into class fragments (:3862-3895) is not copied: the arm resolves
            // a standalone escape, never one inside a class.
            throw new NotSupportedException("arm J resolves a standalone escape only");
        }

        if (!negated)
            return new PropertyResolution { Ranges = ranges }; // ARM CHANGE: the ranges BuildPositiveCodePointMatcher(ranges) expands, never null

        // `\P{X}` matches one code point that is not in X, which is one code point in the
        // COMPLEMENT of X — so it is the same matcher over complemented ranges, and no
        // negative lookahead is needed.
        //
        // Under ignoreCase it has to be built this way: a `(?!positive)` lookahead has the
        // active IgnoreCase applied to `positive`, so it rejects every ch that case-folds
        // INTO X — `(?i:\P{Lu})` would reject "a" because "a" folds to "A" ∈ Lu, and reject
        // "A" too, whereas 22.2.2.7.1 asks for the case closure of the complement (both
        // match). .NET applies IgnoreCase to the complement set, which is exactly that.
        //
        // Without ignoreCase the two forms are equivalent, and this one is far cheaper: the
        // lookahead re-ran the whole positive alternation at every position before the
        // engine could conclude the code point was not in X, which is what made the negated
        // half of the test262 property-escape files an order of magnitude slower than the
        // positive half (issue #966).
        var complementRanges = ComplementCodePointRanges(ranges);

        // `\P{Any}`: no code point lies outside the set, so the escape matches nothing. An
        // empty range list would otherwise build the empty group `(?:)`, which matches the
        // empty string everywhere — `/\P{Any}/iu.test("")` returned true, and
        // `"a".replace(/\P{Any}/giu, "X")` returned "XaX".
        // ARM CHANGE: the complement's ranges, where the source answers "(?!)" for none and otherwise
        // BuildPositiveCodePointMatcher(complementRanges); neither is null.
        return new PropertyResolution { Ranges = complementRanges };
    }

    /// <summary>
    /// Returns the code-point ranges complementary to <paramref name="ranges"/> over the
    /// whole Unicode range [U+0000, U+10FFFF]. The input is assumed sorted ascending and
    /// non-overlapping (as the bundled UCD tables are), matching the in-class complement
    /// path above.
    /// </summary>
    private static (int Lo, int Hi)[] ComplementCodePointRanges((int Lo, int Hi)[] ranges)
    {
        var result = new List<(int Lo, int Hi)>();
        int cursor = 0;
        foreach (var (lo, hi) in ranges)
        {
            if (lo > cursor)
                result.Add((cursor, lo - 1));
            cursor = Math.Max(cursor, hi + 1);
        }
        if (cursor <= 0x10FFFF)
            result.Add((cursor, 0x10FFFF));
        return result.ToArray();
    }

    /// <summary>
    /// ARM CHANGE: new. The source's lines a resolution's null reaches: the escape is left in the pattern for
    /// .NET (:3742-3743, :3778-3780); a u- or v-mode pattern clears RegexOptions.ECMAScript from the options it
    /// starts with (:679, :722-732), so they are None here; .NET compiles it (:1004); and the ArgumentException
    /// .NET raises for a name it does not know becomes a SyntaxError (:1028-1031). The answer is the UTF-16
    /// code units .NET's \p matches, one unit at a time, as ranges.
    /// </summary>
    internal static PropertyResolution NetFallThrough(string prefix, string inner)
    {
        var options = RegexOptions.ECMAScript; // :679
        options &= ~RegexOptions.ECMAScript; // :722-732, the u and v flags
        System.Text.RegularExpressions.Regex compiled;
        try
        {
            compiled = new System.Text.RegularExpressions.Regex(prefix + "{" + inner + "}", options); // :1004
        }
        catch (ArgumentException ex)
        {
            throw JSEngine.NewSyntaxError(ex.Message); // :1028-1031
        }

        var ranges = new List<(int Lo, int Hi)>();
        for (var unit = 0; unit <= 0xFFFF; unit++)
        {
            if (!compiled.IsMatch(((char)unit).ToString()))
                continue;
            if (ranges.Count > 0 && ranges[^1].Hi == unit - 1)
                ranges[^1] = (ranges[^1].Lo, unit);
            else
                ranges.Add((unit, unit));
        }

        return new PropertyResolution { Ranges = ranges.ToArray() };
    }

    // ---- JSRegExp.UnicodeSets.cs ----

    /// <summary>
    /// A class set value: single code points (as normalized ranges) plus
    /// multi-code-point string elements (length ≥ 2 code points, or the empty
    /// string). Single code points always live in <see cref="Ranges"/>.
    /// </summary>
    internal sealed class ClassSetValue // ARM CHANGE: internal
    {
        public readonly List<(int Lo, int Hi)> Ranges = [];
        public readonly HashSet<string> Strings = new(StringComparer.Ordinal);

        public void AddCodePoint(int cp) => Ranges.Add((cp, cp));
        public void AddRange(int lo, int hi) => Ranges.Add((lo, hi));

        public void AddString(string s)
        {
            // A string of a single code point is just a code point.
            if (s.Length == 1)
            {
                AddCodePoint(s[0]);
                return;
            }
            if (s.Length == 2 && char.IsHighSurrogate(s[0]) && char.IsLowSurrogate(s[1]))
            {
                AddCodePoint(char.ConvertToUtf32(s[0], s[1]));
                return;
            }
            Strings.Add(s);
        }
    }

    private static string NormalizeKey(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (ch == '_' || ch == ' ')
                continue;
            sb.Append(char.ToLowerInvariant(ch));
        }
        return sb.ToString();
    }

    internal static ClassSetValue ResolvePropertyOperand(bool negated, string inner) // ARM CHANGE: internal
    {
        var normalized = NormalizeKey(inner);

        if (EmojiStringPropertyNames.TryGetValue(normalized, out var emojiProperty))
        {
            // \P of a property of strings is a SyntaxError.
            if (negated)
                throw NewUnsupportedSetError();
            var value = new ClassSetValue();
            foreach (var seq in EmojiStringProperties.GetSequences(emojiProperty))
                value.AddString(seq);
            return value;
        }

        // Character properties for which we have enumerable ranges.
        var eq = inner.IndexOf('=');
        (int Lo, int Hi)[]? ranges = null; // ARM CHANGE: the nullable annotation, as this project builds with nullable warnings and the source's does not
        if (eq >= 0)
        {
            var name = NormalizeKey(inner.Substring(0, eq));
            var val = NormalizeKey(inner.Substring(eq + 1));
            if (name is "sc" or "script")
                ranges = UnicodeProperties.GetScript(val);
            else if (name is "scx" or "scriptextensions")
                ranges = UnicodeProperties.GetScriptExtensions(val);
        }
        else
        {
            ranges = UnicodeProperties.GetBinaryProperty(normalized);
        }

        if (ranges == null)
            throw NewUnsupportedSetError(); // e.g. General_Category — no enumerable ranges here

        var result = new ClassSetValue();
        result.Ranges.AddRange(ranges);
        return negated ? ComplementCodePoints(result) : result;
    }

    private static List<(int Lo, int Hi)> Normalize(IEnumerable<(int Lo, int Hi)> ranges)
    {
        var list = new List<(int Lo, int Hi)>(ranges);
        list.Sort((a, b) => a.Lo != b.Lo ? a.Lo.CompareTo(b.Lo) : a.Hi.CompareTo(b.Hi));
        var merged = new List<(int Lo, int Hi)>();
        foreach (var (lo, hi) in list)
        {
            if (hi < lo)
                continue;
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

    private static List<(int Lo, int Hi)> SubtractRanges(List<(int Lo, int Hi)> a, List<(int Lo, int Hi)> b)
    {
        var result = new List<(int Lo, int Hi)>();
        foreach (var (lo, hi) in a)
        {
            int cur = lo;
            foreach (var (blo, bhi) in b)
            {
                if (bhi < cur || blo > hi)
                    continue;
                if (blo > cur)
                    result.Add((cur, blo - 1));
                cur = Math.Max(cur, bhi + 1);
                if (cur > hi)
                    break;
            }
            if (cur <= hi)
                result.Add((cur, hi));
        }
        return result;
    }

    private static ClassSetValue ComplementCodePoints(ClassSetValue v)
    {
        var result = new ClassSetValue();
        foreach (var r in SubtractRanges([(0, 0x10FFFF)], Normalize(v.Ranges)))
            result.AddRange(r.Lo, r.Hi);
        return result;
    }

    private static JSException NewUnsupportedSetError()
        => JSEngine.NewSyntaxError(
            "Unicode v-mode class set expression is not supported yet (requires Unicode set/sequence data)");
}
