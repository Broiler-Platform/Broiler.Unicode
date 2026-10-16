using System;
using System.Collections.Generic;
using Broiler.Unicode.Algorithms;

namespace Broiler.Unicode.Segmentation;

/// <summary>The data version this package's segmentation is written for.</summary>
public static class UnicodeSegmentation
{
    /// <summary>
    /// The Unicode version of the break table <see cref="UnicodeSegmenter"/> reads: <c>17.0.0</c>, the
    /// version of <see cref="UnicodeAlgorithms.UnicodeVersion"/>. A table of another version is refused.
    /// </summary>
    public const string UnicodeVersion = UnicodeAlgorithms.UnicodeVersion;
}

/// <summary>The three segmentations of UAX #29 that ECMA-402's <c>Intl.Segmenter</c> offers (ECMA-402 section 18).</summary>
public enum SegmentGranularity
{
    /// <summary>Extended grapheme clusters (UAX #29 section 3).</summary>
    Grapheme = 0,

    /// <summary>Words (UAX #29 section 4).</summary>
    Word = 1,

    /// <summary>Sentences (UAX #29 section 5).</summary>
    Sentence = 2,
}

/// <summary>
/// A word segment's status, numbered as ICU's word break rule statuses are (<c>UBRK_WORD_*</c>), which
/// ECMA-402's <c>isWordLike</c> reads: a segment is word-like when its status is not <see cref="None"/>.
/// </summary>
public enum WordStatus
{
    /// <summary>Spaces, punctuation, symbols or connectors alone.</summary>
    None = 0,

    /// <summary>A number: Word_Break=Numeric, and no letter, kana or ideograph.</summary>
    Number = 100,

    /// <summary>A letter: Word_Break=ALetter or Hebrew_Letter.</summary>
    Letter = 200,

    /// <summary>Katakana: Word_Break=Katakana, and no letter.</summary>
    Kana = 300,

    /// <summary>
    /// An ideograph or Hiragana, and no letter or Katakana. The break table records Ideographic and the
    /// Hiragana script as one flag, so Hiragana is reported with the ideographs.
    /// </summary>
    Ideographic = 400,
}

/// <summary>
/// UAX #29's default boundaries for Unicode 17.0.0 over a UTF-16 text: extended grapheme clusters, words
/// and sentences, with no locale tailoring and no dictionary, and each word segment's status.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every boundary of the text is found at once</b>, in one pass of the granularity's rules over its
/// code points; a lone surrogate is a code point of its own. Boundaries are code unit indices: the text's
/// start, each boundary inside it, and its end. A rule that looks back or ahead past the two code points
/// around a position scans only the run it needs.
/// </para>
/// <para>
/// <b>The charge</b> is one unit per code point the pass classifies, charged before the pass, and one
/// unit per code point any rule's look-behind or look-ahead reads beyond the two around the position:
/// GB9c's, GB11's and GB12's walks, WB4's skipped code points and WB15's regional indicators, and SB5's,
/// SB7's, SB8's and SB8a to SB11's walks over Close and Sp. <see cref="GetWordStatus"/> charges one unit
/// per code point it reads. These are the units of Broiler.VM's JSD-0074, decision 4, which charges
/// every code point scanned (correspondence row G5).
/// </para>
/// <para>
/// <b>Optional.</b> Only an engine that offers <c>Intl.Segmenter</c> references this package. An
/// instance decodes the break table once, holds nothing else, and is safe to share between threads.
/// </para>
/// </remarks>
public sealed class UnicodeSegmenter
{
    private readonly int[] firsts;

    private readonly byte[] runCombinations;

    private readonly BreakProperties[] combinations;

    /// <summary>Segmentation over <paramref name="tables"/>.</summary>
    /// <param name="tables">A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying <see cref="UnicodeTable.SegmentBreaks"/>.</param>
    /// <exception cref="ArgumentException">The provider is of another version, lacks the table, or the table does not decode.</exception>
    public UnicodeSegmenter(IUnicodeTables tables)
    {
        ArgumentNullException.ThrowIfNull(tables);

        if (!string.Equals(tables.UnicodeVersion, UnicodeAlgorithms.UnicodeVersion, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"The tables are Unicode {tables.UnicodeVersion}, and this segmenter reads {UnicodeAlgorithms.UnicodeVersion}.", nameof(tables));
        }

        var table = tables.Table(UnicodeTable.SegmentBreaks);

        if (table.IsEmpty)
        {
            throw new ArgumentException("The tables do not carry SegmentBreaks.", nameof(tables));
        }

        var count = table[0];
        var runs = 1 + (count * 6);

        if (table.Length < runs + 4)
        {
            throw new ArgumentException("SegmentBreaks is shorter than its combinations and run count.", nameof(tables));
        }

        combinations = new BreakProperties[count];

        for (var i = 0; i < count; i++)
        {
            var at = 1 + (i * 6);

            if (table[at] > (byte)GraphemeBreak.LVT || table[at + 1] > (byte)WordBreak.WSegSpace ||
                table[at + 2] > (byte)SentenceBreak.Close || table[at + 3] > 1 || table[at + 4] > (byte)ConjunctBreak.Extend ||
                table[at + 5] > 1)
            {
                throw new ArgumentException($"SegmentBreaks' combination {i} holds a code no enumeration here names.", nameof(tables));
            }

            combinations[i] = new BreakProperties(
                (GraphemeBreak)table[at],
                (WordBreak)table[at + 1],
                (SentenceBreak)table[at + 2],
                table[at + 3] == 1,
                (ConjunctBreak)table[at + 4],
                table[at + 5] == 1);
        }

        var runCount = table[runs] | (table[runs + 1] << 8) | (table[runs + 2] << 16) | (table[runs + 3] << 24);

        if (runCount <= 0 || table.Length != runs + 4 + (runCount * 4))
        {
            throw new ArgumentException("SegmentBreaks' run count does not match its length.", nameof(tables));
        }

        firsts = new int[runCount];
        runCombinations = new byte[runCount];

        for (var i = 0; i < runCount; i++)
        {
            var at = runs + 4 + (i * 4);
            firsts[i] = table[at] | (table[at + 1] << 8) | (table[at + 2] << 16);
            runCombinations[i] = table[at + 3];

            if (runCombinations[i] >= count || (i > 0 && firsts[i] <= firsts[i - 1]) || (i == 0 && firsts[i] != 0))
            {
                throw new ArgumentException($"SegmentBreaks' run {i} does not ascend from U+0000 or names no combination.", nameof(tables));
            }
        }
    }

    /// <summary>
    /// The code unit indices at which a segment of <paramref name="granularity"/> begins in
    /// <paramref name="text"/>, with the text's length last: UAX #29's default boundaries (sections 3.1,
    /// 4.1 and 5.1). An empty text has the one boundary 0.
    /// </summary>
    /// <param name="text">The text, read as code points; a lone surrogate is one.</param>
    /// <param name="granularity">The segmentation.</param>
    /// <param name="charge">The caller's meter, or null; units as this type states.</param>
    /// <returns>The boundaries, ascending.</returns>
    public int[] Boundaries(ReadOnlySpan<char> text, SegmentGranularity granularity, UnicodeWorkCharge? charge)
    {
        if ((uint)granularity > (uint)SegmentGranularity.Sentence)
        {
            throw new ArgumentOutOfRangeException(nameof(granularity));
        }

        var points = 0;

        for (var at = 0; at < text.Length; points++)
        {
            at += char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1]) ? 2 : 1;
        }

        charge?.Invoke(points == 0 ? 1UL : (ulong)points);

        var properties = new BreakProperties[points];
        var starts = new int[points];

        for (int at = 0, i = 0; at < text.Length; i++)
        {
            starts[i] = at;

            int codePoint;

            if (char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1]))
            {
                codePoint = char.ConvertToUtf32(text[at], text[at + 1]);
                at += 2;
            }
            else
            {
                codePoint = text[at];
                at++;
            }

            properties[i] = PropertiesOf(codePoint);
        }

        var scan = new Scan(properties, charge);
        var boundaries = new List<int>(points + 1) { 0 };

        for (var i = 1; i < points; i++)
        {
            var broken = granularity switch
            {
                SegmentGranularity.Word => scan.WordBreak(i),
                SegmentGranularity.Sentence => scan.SentenceBreak(i),
                _ => scan.GraphemeBreak(i),
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

        return boundaries.ToArray();
    }

    /// <summary>
    /// The status of a word segment, as ICU's word break rule statuses number them: <see cref="WordStatus.Letter"/>
    /// when it holds a letter, then <see cref="WordStatus.Kana"/>, <see cref="WordStatus.Ideographic"/> and
    /// <see cref="WordStatus.Number"/> in that order, and <see cref="WordStatus.None"/> for a segment of
    /// spaces, punctuation, symbols or connectors alone. ECMA-402's <c>isWordLike</c> is a status other
    /// than <see cref="WordStatus.None"/> (section 18.7.1, CreateSegmentDataObject).
    /// </summary>
    /// <param name="segment">The segment's text.</param>
    /// <param name="charge">The caller's meter, or null: one unit per code point read.</param>
    public WordStatus GetWordStatus(ReadOnlySpan<char> segment, UnicodeWorkCharge? charge)
    {
        var status = WordStatus.None;

        for (var at = 0; at < segment.Length;)
        {
            int codePoint;

            if (char.IsHighSurrogate(segment[at]) && at + 1 < segment.Length && char.IsLowSurrogate(segment[at + 1]))
            {
                codePoint = char.ConvertToUtf32(segment[at], segment[at + 1]);
                at += 2;
            }
            else
            {
                codePoint = segment[at];
                at++;
            }

            charge?.Invoke(1);

            var properties = PropertiesOf(codePoint);
            var found = properties.Word switch
            {
                WordBreak.ALetter or WordBreak.Hebrew_Letter => WordStatus.Letter,
                WordBreak.Katakana => WordStatus.Kana,
                _ when properties.IdeographicOrHiragana => WordStatus.Ideographic,
                WordBreak.Numeric => WordStatus.Number,
                _ => WordStatus.None,
            };

            if (found == WordStatus.Letter)
            {
                return found;
            }

            if (Rank(found) > Rank(status))
            {
                status = found;
            }
        }

        return status;

        static int Rank(WordStatus value) => value switch
        {
            WordStatus.Kana => 3,
            WordStatus.Ideographic => 2,
            WordStatus.Number => 1,
            _ => 0,
        };
    }

    /// <summary>The break properties of <paramref name="codePoint"/> (UAX #29's property files): a binary search over the runs.</summary>
    /// <param name="codePoint">A code point; a value outside the code space reads as the last run's.</param>
    public BreakProperties PropertiesOf(int codePoint)
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

        return combinations[runCombinations[low]];
    }

    /// <summary>One pass of a granularity's rules over a text's break properties, charging what it scans.</summary>
    private readonly ref struct Scan
    {
        private readonly BreakProperties[] p;

        private readonly UnicodeWorkCharge? charge;

        internal Scan(BreakProperties[] properties, UnicodeWorkCharge? charge)
        {
            p = properties;
            this.charge = charge;
        }

        /// <summary>Whether UAX #29's grapheme cluster rules break between code points <paramref name="i"/> - 1 and <paramref name="i"/>.</summary>
        internal bool GraphemeBreak(int i)
        {
            var previous = p[i - 1].Grapheme;
            var current = p[i].Grapheme;

            // GB3 TO GB5: line ends and controls.
            if (previous == Segmentation.GraphemeBreak.CR && current == Segmentation.GraphemeBreak.LF)
            {
                return false;
            }

            if (previous is Segmentation.GraphemeBreak.Control or Segmentation.GraphemeBreak.CR or Segmentation.GraphemeBreak.LF ||
                current is Segmentation.GraphemeBreak.Control or Segmentation.GraphemeBreak.CR or Segmentation.GraphemeBreak.LF)
            {
                return true;
            }

            // GB6 TO GB8: Hangul syllable sequences.
            if ((previous == Segmentation.GraphemeBreak.L && current is Segmentation.GraphemeBreak.L or Segmentation.GraphemeBreak.V or Segmentation.GraphemeBreak.LV or Segmentation.GraphemeBreak.LVT) ||
                (previous is Segmentation.GraphemeBreak.LV or Segmentation.GraphemeBreak.V && current is Segmentation.GraphemeBreak.V or Segmentation.GraphemeBreak.T) ||
                (previous is Segmentation.GraphemeBreak.LVT or Segmentation.GraphemeBreak.T && current == Segmentation.GraphemeBreak.T))
            {
                return false;
            }

            // GB9, GB9a, GB9b: extenders, spacing marks, prepends.
            if (current is Segmentation.GraphemeBreak.Extend or Segmentation.GraphemeBreak.ZWJ or Segmentation.GraphemeBreak.SpacingMark ||
                previous == Segmentation.GraphemeBreak.Prepend)
            {
                return false;
            }

            // GB9c: Consonant [Extend Linker]* Linker [Extend Linker]* x Consonant.
            if (p[i].Conjunct == ConjunctBreak.Consonant)
            {
                var linker = false;
                var j = i - 1;

                while (j >= 0 && p[j].Conjunct is ConjunctBreak.Extend or ConjunctBreak.Linker)
                {
                    Charge();
                    linker |= p[j].Conjunct == ConjunctBreak.Linker;
                    j--;
                }

                if (linker && j >= 0 && p[j].Conjunct == ConjunctBreak.Consonant)
                {
                    return false;
                }
            }

            // GB11: ExtPict Extend* ZWJ x ExtPict.
            if (previous == Segmentation.GraphemeBreak.ZWJ && p[i].ExtendedPictographic)
            {
                var j = i - 2;

                while (j >= 0 && p[j].Grapheme == Segmentation.GraphemeBreak.Extend)
                {
                    Charge();
                    j--;
                }

                if (j >= 0 && p[j].ExtendedPictographic)
                {
                    return false;
                }
            }

            // GB12, GB13: regional indicators pair.
            if (previous == Segmentation.GraphemeBreak.Regional_Indicator && current == Segmentation.GraphemeBreak.Regional_Indicator)
            {
                var count = 0;

                for (var j = i - 1; j >= 0 && p[j].Grapheme == Segmentation.GraphemeBreak.Regional_Indicator; j--)
                {
                    Charge();
                    count++;
                }

                return count % 2 == 0;
            }

            // GB999.
            return true;
        }

        /// <summary>Whether UAX #29's word rules break between code points <paramref name="i"/> - 1 and <paramref name="i"/>.</summary>
        internal bool WordBreak(int i)
        {
            var previous = p[i - 1].Word;
            var current = p[i].Word;

            // WB3 TO WB3d.
            if (previous == Segmentation.WordBreak.CR && current == Segmentation.WordBreak.LF)
            {
                return false;
            }

            if (previous is Segmentation.WordBreak.Newline or Segmentation.WordBreak.CR or Segmentation.WordBreak.LF ||
                current is Segmentation.WordBreak.Newline or Segmentation.WordBreak.CR or Segmentation.WordBreak.LF)
            {
                return true;
            }

            if ((previous == Segmentation.WordBreak.ZWJ && p[i].ExtendedPictographic) ||
                (previous == Segmentation.WordBreak.WSegSpace && current == Segmentation.WordBreak.WSegSpace))
            {
                return false;
            }

            // WB4: Extend, Format and ZWJ attach to what precedes them.
            if (WordIgnored(current))
            {
                return false;
            }

            // THE CONTEXT WITH WB4'S ABSORBED CODE POINTS SKIPPED: two before, two after.
            var l1 = WordBefore(i);
            var left = l1 >= 0 ? p[l1].Word : (WordBreak?)null;
            var l2 = l1 > 0 ? WordBefore(l1) : -1;
            var farLeft = l2 >= 0 ? p[l2].Word : (WordBreak?)null;
            var r2 = i + 1;

            while (r2 < p.Length && WordIgnored(p[r2].Word))
            {
                Charge();
                r2++;
            }

            var farRight = r2 < p.Length ? p[r2].Word : (WordBreak?)null;

            if (left is not { } l)
            {
                return true;
            }

            // WB5 TO WB7c: letters, and letters around a mid-letter.
            if ((AHLetter(l) && AHLetter(current)) ||
                (AHLetter(l) && (current == Segmentation.WordBreak.MidLetter || MidNumLetQ(current)) && farRight is { } r && AHLetter(r)) ||
                (farLeft is { } fl && AHLetter(fl) && (l == Segmentation.WordBreak.MidLetter || MidNumLetQ(l)) && AHLetter(current)) ||
                (l == Segmentation.WordBreak.Hebrew_Letter && current == Segmentation.WordBreak.Single_Quote) ||
                (l == Segmentation.WordBreak.Hebrew_Letter && current == Segmentation.WordBreak.Double_Quote && farRight == Segmentation.WordBreak.Hebrew_Letter) ||
                (farLeft == Segmentation.WordBreak.Hebrew_Letter && l == Segmentation.WordBreak.Double_Quote && current == Segmentation.WordBreak.Hebrew_Letter))
            {
                return false;
            }

            // WB8 TO WB12: numbers, and numbers around a mid-number.
            if ((l == Segmentation.WordBreak.Numeric && current == Segmentation.WordBreak.Numeric) ||
                (AHLetter(l) && current == Segmentation.WordBreak.Numeric) ||
                (l == Segmentation.WordBreak.Numeric && AHLetter(current)) ||
                (farLeft == Segmentation.WordBreak.Numeric && (l == Segmentation.WordBreak.MidNum || MidNumLetQ(l)) && current == Segmentation.WordBreak.Numeric) ||
                (l == Segmentation.WordBreak.Numeric && (current == Segmentation.WordBreak.MidNum || MidNumLetQ(current)) && farRight == Segmentation.WordBreak.Numeric))
            {
                return false;
            }

            // WB13 TO WB13b: Katakana, and connectors.
            if ((l == Segmentation.WordBreak.Katakana && current == Segmentation.WordBreak.Katakana) ||
                ((AHLetter(l) || l is Segmentation.WordBreak.Numeric or Segmentation.WordBreak.Katakana or Segmentation.WordBreak.ExtendNumLet) && current == Segmentation.WordBreak.ExtendNumLet) ||
                (l == Segmentation.WordBreak.ExtendNumLet && (AHLetter(current) || current is Segmentation.WordBreak.Numeric or Segmentation.WordBreak.Katakana)))
            {
                return false;
            }

            // WB15, WB16: regional indicators pair, WB4's absorbed code points skipped.
            if (l == Segmentation.WordBreak.Regional_Indicator && current == Segmentation.WordBreak.Regional_Indicator)
            {
                var count = 0;

                for (var j = l1; j >= 0 && p[j].Word == Segmentation.WordBreak.Regional_Indicator; j = WordBefore(j))
                {
                    Charge();
                    count++;
                }

                return count % 2 == 0;
            }

            // WB999.
            return true;
        }

        /// <summary>Whether UAX #29's sentence rules break between code points <paramref name="i"/> - 1 and <paramref name="i"/>.</summary>
        internal bool SentenceBreak(int i)
        {
            var previous = p[i - 1].Sentence;
            var current = p[i].Sentence;

            // SB3, SB4: line ends, and a break after a paragraph separator.
            if (previous == Segmentation.SentenceBreak.CR && current == Segmentation.SentenceBreak.LF)
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

            var l1 = SentenceBefore(i);

            if (l1 < 0)
            {
                return false;
            }

            var left = p[l1].Sentence;

            // SB6, SB7: a full stop inside a number or between capitals.
            if (left == Segmentation.SentenceBreak.ATerm && current == Segmentation.SentenceBreak.Numeric)
            {
                return false;
            }

            if (left == Segmentation.SentenceBreak.ATerm && current == Segmentation.SentenceBreak.Upper &&
                SentenceBefore(l1) is var l2 && l2 >= 0 && p[l2].Sentence is Segmentation.SentenceBreak.Upper or Segmentation.SentenceBreak.Lower)
            {
                return false;
            }

            // SB8 TO SB11: after SATerm Close* Sp*.
            var j = l1;
            var spaces = 0;

            while (j >= 0 && p[j].Sentence == Segmentation.SentenceBreak.Sp)
            {
                Charge();
                spaces++;
                j = SentenceBefore(j);
            }

            while (j >= 0 && p[j].Sentence == Segmentation.SentenceBreak.Close)
            {
                Charge();
                j = SentenceBefore(j);
            }

            if (j < 0 || !SATerm(p[j].Sentence))
            {
                // SB998.
                return false;
            }

            if (p[j].Sentence == Segmentation.SentenceBreak.ATerm)
            {
                // SB8: ... ATerm Close* Sp* x ( not OLetter, Upper, Lower, ParaSep or SATerm )* Lower.
                var k = i;

                while (k < p.Length &&
                    p[k].Sentence is not (Segmentation.SentenceBreak.OLetter or Segmentation.SentenceBreak.Upper or Segmentation.SentenceBreak.Lower) &&
                    !ParaSep(p[k].Sentence) && !SATerm(p[k].Sentence))
                {
                    Charge();
                    k++;
                }

                if (k < p.Length && p[k].Sentence == Segmentation.SentenceBreak.Lower)
                {
                    return false;
                }
            }

            // SB8a, SB9, SB10.
            if (current == Segmentation.SentenceBreak.SContinue || SATerm(current) ||
                (spaces == 0 && (current == Segmentation.SentenceBreak.Close || current == Segmentation.SentenceBreak.Sp || ParaSep(current))) ||
                current == Segmentation.SentenceBreak.Sp || ParaSep(current))
            {
                return false;
            }

            // SB11.
            return true;
        }

        private static bool WordIgnored(WordBreak value) =>
            value is Segmentation.WordBreak.Extend or Segmentation.WordBreak.Format or Segmentation.WordBreak.ZWJ;

        private static bool AHLetter(WordBreak value) => value is Segmentation.WordBreak.ALetter or Segmentation.WordBreak.Hebrew_Letter;

        private static bool MidNumLetQ(WordBreak value) => value is Segmentation.WordBreak.MidNumLet or Segmentation.WordBreak.Single_Quote;

        private static bool SentenceIgnored(SentenceBreak value) => value is Segmentation.SentenceBreak.Extend or Segmentation.SentenceBreak.Format;

        private static bool ParaSep(SentenceBreak value) =>
            value is Segmentation.SentenceBreak.Sep or Segmentation.SentenceBreak.CR or Segmentation.SentenceBreak.LF;

        private static bool SATerm(SentenceBreak value) => value is Segmentation.SentenceBreak.STerm or Segmentation.SentenceBreak.ATerm;

        private void Charge() => charge?.Invoke(1);

        /// <summary>The index of the last code point before <paramref name="i"/> that WB4 does not absorb, or -1.</summary>
        private int WordBefore(int i)
        {
            var j = i - 1;

            while (j >= 0 && WordIgnored(p[j].Word))
            {
                Charge();
                j--;
            }

            return j;
        }

        /// <summary>The index of the last code point before <paramref name="i"/> that SB5 does not absorb, or -1.</summary>
        private int SentenceBefore(int i)
        {
            var j = i - 1;

            while (j >= 0 && SentenceIgnored(p[j].Sentence))
            {
                Charge();
                j--;
            }

            return j;
        }
    }
}
