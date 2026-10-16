// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript/JsSegmenter.cs
//     blob 3ac6989889b823386a08f7280bc5ebf08c6ee7e2: the whole file: the break-property enumerations, JsBreakProperties, JsBreakData (the decoding, :76-158) and JsSegmentation (:182-612)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's.
// JsEngine, which the source's JsSegmentation charges through, is the arm's (JsEngine.cs beside this file):
// its Charge(ulong) records every charge; the comparison reads none of them, since the rule compares
// segmentation's charge with no arm.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           55
// Human-reviewed:   0/23
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The Grapheme_Cluster_Break values UAX #29 names.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AA51E8
// Broiler-Human:        PENDING
internal enum JsGraphemeBreak : byte
{
    Other, CR, LF, Control, Extend, ZWJ, Regional_Indicator, Prepend, SpacingMark, L, V, T, LV, LVT,
}

/// <summary>The Word_Break values UAX #29 names.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3A1F27
// Broiler-Human:        PENDING
internal enum JsWordBreak : byte
{
    Other, CR, LF, Newline, Extend, ZWJ, Regional_Indicator, Format, Katakana, Hebrew_Letter, ALetter,
    Single_Quote, Double_Quote, MidNumLet, MidLetter, MidNum, Numeric, ExtendNumLet, WSegSpace,
}

/// <summary>The Sentence_Break values UAX #29 names.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B2D3E4
// Broiler-Human:        PENDING
internal enum JsSentenceBreak : byte
{
    Other, CR, LF, Extend, Sep, Format, Sp, Lower, Upper, OLetter, Numeric, ATerm, SContinue, STerm, Close,
}

/// <summary>The Indic_Conjunct_Break values grapheme rule GB9c reads.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE7F7B
// Broiler-Human:        PENDING
internal enum JsIndicConjunctBreak : byte
{
    None, Linker, Consonant, Extend,
}

/// <summary>The break properties of one code point, and whether it is Ideographic or Hiragana.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D7537C
// Broiler-Human:        PENDING
internal readonly record struct JsBreakProperties(
    JsGraphemeBreak Grapheme,
    JsWordBreak Word,
    JsSentenceBreak Sentence,
    bool ExtendedPictographic,
    JsIndicConjunctBreak Conjunct,
    bool IdeographicOrHiragana);

/// <summary>
/// The break properties <c>Intl.Segmenter</c> reads (JSD-0050), decoded from the composition's tables:
/// each code point's Grapheme_Cluster_Break, Word_Break, Sentence_Break, Extended_Pictographic and
/// Indic_Conjunct_Break, and whether it is Ideographic or Hiragana, held as runs of the code space.
/// </summary>
/// <remarks>
/// <b>The table's codes are translated by name.</b> The generator numbers each property's values in
/// the order it lists them beside the table, and this class reads those names into the enumerations
/// above, so the two orders need not agree and a value the enumerations do not know is refused.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7043FE
// Broiler-Human:        PENDING
internal sealed class JsBreakData
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=92F3B2
    // Broiler-Human:        PENDING
    private readonly int[] firsts;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5C6358
    // Broiler-Human:        PENDING
    private readonly byte[] combinations;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=33A7C8
    // Broiler-Human:        PENDING
    private readonly JsBreakProperties[] decoded;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6BAA7F
    // Broiler-Human:        PENDING
    internal JsBreakData(System.Collections.Generic.List<string> values, System.ReadOnlySpan<byte> table)
    {
        var names = new System.Collections.Generic.Dictionary<string, string[]>(System.StringComparer.Ordinal);

        foreach (var line in values)
        {
            var fields = line.Split('|');
            names[fields[0]] = fields[1..];
        }

        static TEnum Value<TEnum>(string[] list, int code)
            where TEnum : struct, System.Enum =>
            System.Enum.Parse<TEnum>(list[code]);

        var count = table[0];
        decoded = new JsBreakProperties[count];

        for (var i = 0; i < count; i++)
        {
            var at = 1 + (i * 6);
            decoded[i] = new JsBreakProperties(
                Value<JsGraphemeBreak>(names["gcb"], table[at]),
                Value<JsWordBreak>(names["wb"], table[at + 1]),
                Value<JsSentenceBreak>(names["sb"], table[at + 2]),
                names["ep"][table[at + 3]] == "Yes",
                Value<JsIndicConjunctBreak>(names["incb"], table[at + 4]),
                names["ideo"][table[at + 5]] == "Yes");
        }

        var runs = 1 + (count * 6);
        var runCount = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(table[runs..]);
        firsts = new int[runCount];
        combinations = new byte[runCount];

        for (var i = 0; i < runCount; i++)
        {
            var at = runs + 4 + (i * 4);
            firsts[i] = table[at] | (table[at + 1] << 8) | (table[at + 2] << 16);
            combinations[i] = table[at + 3];
        }
    }

    /// <summary>The break properties of a code point: a binary search over the runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F89952
    // Broiler-Human:        PENDING
    internal JsBreakProperties Of(int codePoint)
    {
        var low = 0;
        var high = firsts.Length - 1;

        while (low < high)
        {
            var middle = (low + high + 1) >>> 1;

            if (firsts[middle] <= codePoint)
            {
                low = middle;
            }
            else
            {
                high = middle - 1;
            }
        }

        return decoded[combinations[low]];
    }
}

/// <summary>
/// UAX #29's default boundaries over a String (JSD-0050): grapheme clusters, words and sentences,
/// each as the code unit indices that begin a segment, and whether a word segment is word-like.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every boundary of the string is found at once</b>, in one pass of each rule set over its code
/// points, and FindBoundary searches the list. A rule that looks back or ahead past one code point
/// scans only within the run it needs, and every code point scanned is charged.
/// </para>
/// <para>
/// <b>The rules are UAX #29's for Unicode 17.0.0</b>, with no locale tailoring and no dictionary, as
/// JSD-0050 records.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=229CAF
// Broiler-Human:        PENDING
internal static class JsSegmentation
{
    /// <summary>The code points of a String, each with the index of its first code unit; a lone surrogate is a code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CDBB08
    // Broiler-Human:        PENDING
    private static (int[] CodePoints, int[] Starts) CodePoints(JsEngine engine, string text)
    {
        var points = new System.Collections.Generic.List<int>(text.Length);
        var starts = new System.Collections.Generic.List<int>(text.Length);

        for (var at = 0; at < text.Length;)
        {
            starts.Add(at);

            if (char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1]))
            {
                points.Add(char.ConvertToUtf32(text[at], text[at + 1]));
                at += 2;
            }
            else
            {
                points.Add(text[at]);
                at++;
            }
        }

        engine.Charge((ulong)(points.Count / 16) + 1);
        return ([.. points], [.. starts]);
    }

    /// <summary>The code unit indices at which a segment of <paramref name="granularity"/> begins, and the string's length last.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A6F667
    // Broiler-Human:        PENDING
    internal static int[] Boundaries(JsEngine engine, JsBreakData data, string text, string granularity)
    {
        var (points, starts) = CodePoints(engine, text);
        var properties = new JsBreakProperties[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            properties[i] = data.Of(points[i]);
        }

        var boundaries = new System.Collections.Generic.List<int> { 0 };

        for (var i = 1; i < points.Length; i++)
        {
            var broken = granularity switch
            {
                "word" => WordBreak(engine, properties, i),
                "sentence" => SentenceBreak(engine, properties, i),
                _ => GraphemeBreak(engine, properties, i),
            };

            if (broken)
            {
                boundaries.Add(starts[i]);
            }
        }

        if (text.Length > 0)
        {
            boundaries.Add(text.Length);
        }

        return [.. boundaries];
    }

    /// <summary>Whether UAX #29's grapheme cluster rules break between code points <paramref name="i"/> - 1 and <paramref name="i"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CE2026
    // Broiler-Human:        PENDING
    private static bool GraphemeBreak(JsEngine engine, JsBreakProperties[] p, int i)
    {
        var previous = p[i - 1].Grapheme;
        var current = p[i].Grapheme;

        // GB3 TO GB5: line ends and controls.
        if (previous == JsGraphemeBreak.CR && current == JsGraphemeBreak.LF)
        {
            return false;
        }

        if (previous is JsGraphemeBreak.Control or JsGraphemeBreak.CR or JsGraphemeBreak.LF ||
            current is JsGraphemeBreak.Control or JsGraphemeBreak.CR or JsGraphemeBreak.LF)
        {
            return true;
        }

        // GB6 TO GB8: Hangul syllable sequences.
        if ((previous == JsGraphemeBreak.L && current is JsGraphemeBreak.L or JsGraphemeBreak.V or JsGraphemeBreak.LV or JsGraphemeBreak.LVT) ||
            (previous is JsGraphemeBreak.LV or JsGraphemeBreak.V && current is JsGraphemeBreak.V or JsGraphemeBreak.T) ||
            (previous is JsGraphemeBreak.LVT or JsGraphemeBreak.T && current == JsGraphemeBreak.T))
        {
            return false;
        }

        // GB9, GB9a, GB9b: extenders, spacing marks, prepends.
        if (current is JsGraphemeBreak.Extend or JsGraphemeBreak.ZWJ or JsGraphemeBreak.SpacingMark || previous == JsGraphemeBreak.Prepend)
        {
            return false;
        }

        // GB9c: Consonant [Extend Linker]* Linker [Extend Linker]* x Consonant.
        if (p[i].Conjunct == JsIndicConjunctBreak.Consonant)
        {
            var linker = false;
            var j = i - 1;

            while (j >= 0 && p[j].Conjunct is JsIndicConjunctBreak.Extend or JsIndicConjunctBreak.Linker)
            {
                engine.Charge(1);
                linker |= p[j].Conjunct == JsIndicConjunctBreak.Linker;
                j--;
            }

            if (linker && j >= 0 && p[j].Conjunct == JsIndicConjunctBreak.Consonant)
            {
                return false;
            }
        }

        // GB11: ExtPict Extend* ZWJ x ExtPict.
        if (previous == JsGraphemeBreak.ZWJ && p[i].ExtendedPictographic)
        {
            var j = i - 2;

            while (j >= 0 && p[j].Grapheme == JsGraphemeBreak.Extend)
            {
                engine.Charge(1);
                j--;
            }

            if (j >= 0 && p[j].ExtendedPictographic)
            {
                return false;
            }
        }

        // GB12, GB13: regional indicators pair.
        if (previous == JsGraphemeBreak.Regional_Indicator && current == JsGraphemeBreak.Regional_Indicator)
        {
            var count = 0;

            for (var j = i - 1; j >= 0 && p[j].Grapheme == JsGraphemeBreak.Regional_Indicator; j--)
            {
                engine.Charge(1);
                count++;
            }

            return count % 2 == 0;
        }

        // GB999.
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E08553
    // Broiler-Human:        PENDING
    private static bool WordIgnored(JsWordBreak value) => value is JsWordBreak.Extend or JsWordBreak.Format or JsWordBreak.ZWJ;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DE5FB7
    // Broiler-Human:        PENDING
    private static bool AHLetter(JsWordBreak value) => value is JsWordBreak.ALetter or JsWordBreak.Hebrew_Letter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=028954
    // Broiler-Human:        PENDING
    private static bool MidNumLetQ(JsWordBreak value) => value is JsWordBreak.MidNumLet or JsWordBreak.Single_Quote;

    /// <summary>The index of the last code point before <paramref name="i"/> that WB4 does not absorb, or -1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FAF4B9
    // Broiler-Human:        PENDING
    private static int WordBefore(JsEngine engine, JsBreakProperties[] p, int i)
    {
        var j = i - 1;

        while (j >= 0 && WordIgnored(p[j].Word))
        {
            engine.Charge(1);
            j--;
        }

        return j;
    }

    /// <summary>Whether UAX #29's word rules break between code points <paramref name="i"/> - 1 and <paramref name="i"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E8CE7F
    // Broiler-Human:        PENDING
    private static bool WordBreak(JsEngine engine, JsBreakProperties[] p, int i)
    {
        var previous = p[i - 1].Word;
        var current = p[i].Word;

        // WB3 TO WB3d.
        if (previous == JsWordBreak.CR && current == JsWordBreak.LF)
        {
            return false;
        }

        if (previous is JsWordBreak.Newline or JsWordBreak.CR or JsWordBreak.LF ||
            current is JsWordBreak.Newline or JsWordBreak.CR or JsWordBreak.LF)
        {
            return true;
        }

        if ((previous == JsWordBreak.ZWJ && p[i].ExtendedPictographic) ||
            (previous == JsWordBreak.WSegSpace && current == JsWordBreak.WSegSpace))
        {
            return false;
        }

        // WB4: Extend, Format and ZWJ attach to what precedes them.
        if (WordIgnored(current))
        {
            return false;
        }

        // THE CONTEXT WITH WB4'S ABSORBED CODE POINTS SKIPPED: two before, two after.
        var l1 = WordBefore(engine, p, i);
        var left = l1 >= 0 ? p[l1].Word : (JsWordBreak?)null;
        var l2 = l1 > 0 ? WordBefore(engine, p, l1) : -1;
        var farLeft = l2 >= 0 ? p[l2].Word : (JsWordBreak?)null;
        var r2 = i + 1;

        while (r2 < p.Length && WordIgnored(p[r2].Word))
        {
            engine.Charge(1);
            r2++;
        }

        var farRight = r2 < p.Length ? p[r2].Word : (JsWordBreak?)null;

        if (left is not { } l)
        {
            return true;
        }

        // WB5 TO WB7c: letters, and letters around a mid-letter.
        if ((AHLetter(l) && AHLetter(current)) ||
            (AHLetter(l) && (current == JsWordBreak.MidLetter || MidNumLetQ(current)) && farRight is { } r && AHLetter(r)) ||
            (farLeft is { } fl && AHLetter(fl) && (l == JsWordBreak.MidLetter || MidNumLetQ(l)) && AHLetter(current)) ||
            (l == JsWordBreak.Hebrew_Letter && current == JsWordBreak.Single_Quote) ||
            (l == JsWordBreak.Hebrew_Letter && current == JsWordBreak.Double_Quote && farRight == JsWordBreak.Hebrew_Letter) ||
            (farLeft == JsWordBreak.Hebrew_Letter && l == JsWordBreak.Double_Quote && current == JsWordBreak.Hebrew_Letter))
        {
            return false;
        }

        // WB8 TO WB12: numbers, and numbers around a mid-number.
        if ((l == JsWordBreak.Numeric && current == JsWordBreak.Numeric) ||
            (AHLetter(l) && current == JsWordBreak.Numeric) ||
            (l == JsWordBreak.Numeric && AHLetter(current)) ||
            (farLeft == JsWordBreak.Numeric && (l == JsWordBreak.MidNum || MidNumLetQ(l)) && current == JsWordBreak.Numeric) ||
            (l == JsWordBreak.Numeric && (current == JsWordBreak.MidNum || MidNumLetQ(current)) && farRight == JsWordBreak.Numeric))
        {
            return false;
        }

        // WB13 TO WB13b: Katakana, and connectors.
        if ((l == JsWordBreak.Katakana && current == JsWordBreak.Katakana) ||
            ((AHLetter(l) || l is JsWordBreak.Numeric or JsWordBreak.Katakana or JsWordBreak.ExtendNumLet) && current == JsWordBreak.ExtendNumLet) ||
            (l == JsWordBreak.ExtendNumLet && (AHLetter(current) || current is JsWordBreak.Numeric or JsWordBreak.Katakana)))
        {
            return false;
        }

        // WB15, WB16: regional indicators pair, WB4's absorbed code points skipped.
        if (l == JsWordBreak.Regional_Indicator && current == JsWordBreak.Regional_Indicator)
        {
            var count = 0;

            for (var j = l1; j >= 0 && p[j].Word == JsWordBreak.Regional_Indicator; j = WordBefore(engine, p, j))
            {
                count++;
            }

            return count % 2 == 0;
        }

        // WB999.
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=24F1F8
    // Broiler-Human:        PENDING
    private static bool SentenceIgnored(JsSentenceBreak value) => value is JsSentenceBreak.Extend or JsSentenceBreak.Format;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=926DAF
    // Broiler-Human:        PENDING
    private static bool ParaSep(JsSentenceBreak value) => value is JsSentenceBreak.Sep or JsSentenceBreak.CR or JsSentenceBreak.LF;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F6199F
    // Broiler-Human:        PENDING
    private static bool SATerm(JsSentenceBreak value) => value is JsSentenceBreak.STerm or JsSentenceBreak.ATerm;

    /// <summary>The index of the last code point before <paramref name="i"/> that SB5 does not absorb, or -1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C9B0CF
    // Broiler-Human:        PENDING
    private static int SentenceBefore(JsEngine engine, JsBreakProperties[] p, int i)
    {
        var j = i - 1;

        while (j >= 0 && SentenceIgnored(p[j].Sentence))
        {
            engine.Charge(1);
            j--;
        }

        return j;
    }

    /// <summary>Whether UAX #29's sentence rules break between code points <paramref name="i"/> - 1 and <paramref name="i"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=361A5B
    // Broiler-Human:        PENDING
    private static bool SentenceBreak(JsEngine engine, JsBreakProperties[] p, int i)
    {
        var previous = p[i - 1].Sentence;
        var current = p[i].Sentence;

        // SB3, SB4: line ends, and a break after a paragraph separator.
        if (previous == JsSentenceBreak.CR && current == JsSentenceBreak.LF)
        {
            return false;
        }

        if (ParaSep(previous))
        {
            return true;
        }

        // SB5: Extend and Format attach to what precedes them.
        if (SentenceIgnored(current))
        {
            return false;
        }

        var l1 = SentenceBefore(engine, p, i);

        if (l1 < 0)
        {
            return false;
        }

        var left = p[l1].Sentence;

        // SB6, SB7: a full stop inside a number or between capitals.
        if (left == JsSentenceBreak.ATerm && current == JsSentenceBreak.Numeric)
        {
            return false;
        }

        if (left == JsSentenceBreak.ATerm && current == JsSentenceBreak.Upper &&
            SentenceBefore(engine, p, l1) is var l2 && l2 >= 0 && p[l2].Sentence is JsSentenceBreak.Upper or JsSentenceBreak.Lower)
        {
            return false;
        }

        // SB8 TO SB11: after SATerm Close* Sp*.
        var j = l1;
        var spaces = 0;

        while (j >= 0 && p[j].Sentence == JsSentenceBreak.Sp)
        {
            spaces++;
            j = SentenceBefore(engine, p, j);
        }

        while (j >= 0 && p[j].Sentence == JsSentenceBreak.Close)
        {
            j = SentenceBefore(engine, p, j);
        }

        if (j < 0 || !SATerm(p[j].Sentence))
        {
            // SB998.
            return false;
        }

        if (p[j].Sentence == JsSentenceBreak.ATerm)
        {
            // SB8: ... ATerm Close* Sp* x ( not OLetter, Upper, Lower, ParaSep or SATerm )* Lower.
            var k = i;

            while (k < p.Length &&
                p[k].Sentence is not (JsSentenceBreak.OLetter or JsSentenceBreak.Upper or JsSentenceBreak.Lower) &&
                !ParaSep(p[k].Sentence) && !SATerm(p[k].Sentence))
            {
                engine.Charge(1);
                k++;
            }

            if (k < p.Length && p[k].Sentence == JsSentenceBreak.Lower)
            {
                return false;
            }
        }

        // SB8a, SB9, SB10.
        if (current == JsSentenceBreak.SContinue || SATerm(current) ||
            (spaces == 0 && (current == JsSentenceBreak.Close || current == JsSentenceBreak.Sp || ParaSep(current))) ||
            current == JsSentenceBreak.Sp || ParaSep(current))
        {
            return false;
        }

        // SB11.
        return true;
    }

    /// <summary>
    /// Whether a word segment is word-like: it holds a letter, a number, Katakana, Hiragana or an
    /// ideograph, as ICU's word statuses mark one; a segment of spaces, punctuation, symbols or
    /// connectors alone is not (JSD-0050 section 4).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=253640
    // Broiler-Human:        PENDING
    internal static bool IsWordLike(JsBreakData data, string text, int start, int end)
    {
        for (var at = start; at < end;)
        {
            var codePoint = char.IsHighSurrogate(text[at]) && at + 1 < end && char.IsLowSurrogate(text[at + 1])
                ? char.ConvertToUtf32(text[at], text[at + 1])
                : text[at];

            var properties = data.Of(codePoint);

            if (properties.Word is JsWordBreak.ALetter or JsWordBreak.Hebrew_Letter or JsWordBreak.Numeric or JsWordBreak.Katakana ||
                properties.IdeographicOrHiragana)
            {
                return true;
            }

            at += codePoint > 0xFFFF ? 2 : 1;
        }

        return false;
    }
}
