// REFERENCE ARM J: a test-only copy of Broiler.JS's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.JS at 4d70821e (the engine sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at Broiler.JS bdb1a3ca):
//   Broiler.JS/Broiler.JavaScript.BuiltIns/String/JSStringPrototype.Transform.cs
//     blob f346d9c9c3ce7d4296ccc3781833c5b05d203376: the bodies of normalize (:16-33),
//     toLocaleLowerCase (:128-144), toLocaleUpperCase (:146-162), toLowerCase (:164-170) and toUpperCase
//     (:172-178)
//   Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs
//     blob 83bd14139774a2b0ac6c0a12e445d7721d19a31b: the collator's normalization of its operands,
//     JSIntlCollator.Compare's lines that call IsNormalized and Normalize (:6345-6348)
//   package Broiler.Unicode.Properties 0.1.0-preview.3: JSStringSpecialCasing.ToUpper, ToLower,
//     ToLocaleUpper and ToLocaleLower, called from the package as Broiler.JS calls them; the host's
//     string.Normalize and string.IsNormalized, which run on the host's ICU
//
// Changes from the source, each marked "ARM CHANGE" where it is made; none changes a result:
//   - each built-in takes the receiver's String where the source runs a.This.AsString(), and returns the
//     String where the source wraps it in a JSString; the [JSPrototypeMethod] and [JSExport] attributes are
//     not copied; the members sit in a static class JSString of their own, in the source's namespace;
//   - normalize takes the form's string, null for undefined, where the source reads a.Get1() and its
//     StringValue; its RangeError is the arm's recorded exception;
//   - toLocaleLowerCase and toLocaleUpperCase take the locale's string, null for null and undefined, where
//     the source reads a.Get1(), and the current culture as a parameter, where the source reads
//     CultureInfo.CurrentCulture: the comparison passes the culture its thread runs under. The CultureInfo
//     resolution is otherwise the source's, CultureNotFoundException and its RangeError included;
//   - the collator's lines are the function CollatorNormalize of one operand, which the source runs on each.

using System.Globalization;
using System.Text;
using Broiler.JavaScript.Engine.Core;
using Broiler.Unicode.Properties;

namespace Broiler.JavaScript.BuiltIns.String;

internal static class JSString // ARM CHANGE: a static class of its own; the source's is public partial class JSString
{
    // ARM CHANGE: the receiver's String and the form (null for undefined) are parameters, and the String is
    // returned; the source's lines follow.
    internal static string Normalize(string @this, string? input)
    {
        string form = input is null ? "NFC" : input; // ARM CHANGE: input.IsUndefined ? "NFC" : input.StringValue

        return form switch
        {
            "NFC" => @this.Normalize(NormalizationForm.FormC), // ARM CHANGE: unwrapped from new JSString(...), here and below
            "NFD" => @this.Normalize(NormalizationForm.FormD),
            "NFKC" => @this.Normalize(NormalizationForm.FormKC),
            "NFKD" => @this.Normalize(NormalizationForm.FormKD),
            _ => throw JSEngine.NewRangeError($"The normalization form should be one of NFC, NFD, NFKC, NFKD."),
        };
    }

    // ARM CHANGE: the receiver's String, the locale (null for null and undefined) and the current culture are
    // parameters, and the String is returned; the source's lines follow.
    internal static string ToLocaleLowerCase(string @this, string? locale, CultureInfo currentCulture)
    {
        try
        {
            CultureInfo culture = locale is null ? currentCulture : CultureInfo.GetCultureInfo(locale); // ARM CHANGE: locale.IsNullOrUndefined, CultureInfo.CurrentCulture and locale.ToString()
            return JSStringSpecialCasing.ToLocaleLower(@this, culture); // ARM CHANGE: unwrapped from new JSString(...)
        }
        catch (CultureNotFoundException)
        {
            throw JSEngine.NewRangeError($"Incorrect locale information provided");
        }
    }

    // ARM CHANGE: as ToLocaleLowerCase.
    internal static string ToLocaleUpperCase(string @this, string? locale, CultureInfo currentCulture)
    {
        try
        {
            CultureInfo culture = locale is null ? currentCulture : CultureInfo.GetCultureInfo(locale); // ARM CHANGE: as above
            return JSStringSpecialCasing.ToLocaleUpper(@this, culture); // ARM CHANGE: unwrapped from new JSString(...)
        }
        catch (CultureNotFoundException)
        {
            throw JSEngine.NewRangeError($"Incorrect locale information provided");
        }
    }

    // ARM CHANGE: the receiver's String is a parameter, and the String is returned.
    internal static string ToLowerCase(string @this)
    {
        return JSStringSpecialCasing.ToLower(@this); // ARM CHANGE: unwrapped from new JSString(...)
    }

    // ARM CHANGE: as ToLowerCase.
    internal static string ToUpperCase(string @this)
    {
        return JSStringSpecialCasing.ToUpper(@this); // ARM CHANGE: unwrapped from new JSString(...)
    }

    // ARM CHANGE: JSIntlCollator.Compare's normalization of one operand (:6345-6346; :6347-6348 are the same
    // lines for the other), as a function of that operand.
    internal static string CollatorNormalize(string left)
    {
        if (!left.IsNormalized(NormalizationForm.FormD))
            left = left.Normalize(NormalizationForm.FormD);
        return left;
    }
}
