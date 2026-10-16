// REFERENCE ARM J: a test-only copy of Broiler.JS's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.JS at 4d70821e (the engine sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at Broiler.JS bdb1a3ca):
//   Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs
//     blob 83bd14139774a2b0ac6c0a12e445d7721d19a31b: of JSIntlSegments, Compute (:3229-3234),
//     ComputeGraphemes (:3236-3244), ComputeSentences (:3246-3255), ComputeWords (:3257-3302), WordCat
//     (:3304), WordCategory (:3306-3320), IsNumericInfix (:3322-3335), Joinable (:3337-3343) and IsWordLike
//     (:3345-3354); the host's StringInfo, Rune.IsLetter, Rune.IsDigit and Rune.IsWhiteSpace, which they call
//
// Changes from the source, each marked "ARM CHANGE" where it is made; none changes a result:
//   - the members sit in a static class JSIntlSegments of their own, in the source's namespace, where the
//     source's is a JSObject holding one string's segments; the ones the comparison calls are internal where
//     the source's are private. The objects, the iterators and containing (:3116-3227) stay in the adapter.

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Broiler.JavaScript.BuiltIns.Intl;

internal static class JSIntlSegments // ARM CHANGE: a static class of its own; the source's is public sealed class JSIntlSegments : JSObject
{
    internal static List<(int start, int length)> Compute(string input, string granularity) => granularity switch // ARM CHANGE: internal
    {
        "word" => ComputeWords(input),
        "sentence" => ComputeSentences(input),
        _ => ComputeGraphemes(input),
    };

    private static List<(int start, int length)> ComputeGraphemes(string input)
    {
        var list = new List<(int, int)>();
        var e = StringInfo.GetTextElementEnumerator(input);
        while (e.MoveNext())
            list.Add((e.ElementIndex, ((string)e.Current).Length));

        return list;
    }

    private static List<(int start, int length)> ComputeSentences(string input)
    {
        // Approximation: the whole string is a single sentence. Sufficient for
        // the spec'd shape; full UAX #29 sentence breaking is not implemented.
        var list = new List<(int, int)>();
        if (input.Length > 0)
            list.Add((0, input.Length));

        return list;
    }

    private static List<(int start, int length)> ComputeWords(string input)
    {
        var graphemes = ComputeGraphemes(input);
        var list = new List<(int, int)>();

        var i = 0;
        while (i < graphemes.Count)
        {
            var (start, length) = graphemes[i];
            var category = WordCategory(input, start, length);

            var j = i + 1;
            while (j < graphemes.Count)
            {
                var (nextStart, nextLength) = graphemes[j];
                if (Joinable(category, WordCategory(input, nextStart, nextLength)))
                {
                    length = nextStart + nextLength - start;
                    j++;
                    continue;
                }

                // UAX #29 WB11/WB12: a numeric run is not broken by a single MidNum/MidNumLet
                // separator (e.g. "." or ",") that sits between two numbers — "1.23" and "3,000"
                // are each one word. Consume the separator and the following numeric grapheme.
                if (category == WordCat.Number
                    && IsNumericInfix(input, nextStart, nextLength)
                    && j + 1 < graphemes.Count
                    && WordCategory(input, graphemes[j + 1].start, graphemes[j + 1].length) == WordCat.Number)
                {
                    var (afterStart, afterLength) = graphemes[j + 1];
                    length = afterStart + afterLength - start;
                    j += 2;
                    continue;
                }

                break;
            }

            list.Add((start, length));
            i = j;
        }

        return list;
    }

    private enum WordCat { Letter, Number, Space, Other }

    private static WordCat WordCategory(string input, int start, int length)
    {
        foreach (var rune in input.Substring(start, length).EnumerateRunes())
        {
            if (Rune.IsLetter(rune))
                return WordCat.Letter;
            if (Rune.IsDigit(rune))
                return WordCat.Number;
            if (Rune.IsWhiteSpace(rune))
                return WordCat.Space;
            return WordCat.Other;
        }

        return WordCat.Other;
    }

    // The UAX #29 MidNum / MidNumLet separators that can sit between two numbers without
    // breaking the numeric word (decimal points and digit-group separators). Only the common
    // ASCII/Arabic separators that test262 exercises are recognised.
    private static bool IsNumericInfix(string input, int start, int length)
    {
        if (length != 1)
            return false;
        return input[start] switch
        {
            '.' or ',' or ';' or '٫' or '٬' or '’' or '，' or '．' => true,
            _ => false,
        };
    }

    private static bool Joinable(WordCat a, WordCat b)
    {
        if (a == WordCat.Space && b == WordCat.Space)
            return true;

        var aWord = a is WordCat.Letter or WordCat.Number;
        var bWord = b is WordCat.Letter or WordCat.Number;
        return aWord && bWord;
    }

    internal static bool IsWordLike(string segment) // ARM CHANGE: internal
    {
        foreach (var rune in segment.EnumerateRunes())
        {
            if (Rune.IsLetter(rune) || Rune.IsDigit(rune))
                return true;
        }

        return false;
    }
}
