// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript/JsIntlTables.cs
//     blob 4316d98b23ff7935b1229a36e8b7bba5665528dc: of JsIntlTables, the fields data (:46-48),
//     softDotted (:70-72) and breaks (:86-88), the constructor's lines that set them (:102-106, :112, :116),
//     IsSoftDotted (:191-219), Breaks (:236-239), Lines (:513-530) and ReadRanges (:548-564)
//   src/Broiler.VM.Profile.JavaScript.Format/JsIntlData.cs
//     blob b85f88d42724a5790eaebc2c9f577ff59e3cccfa: IJsIntlData's Table (:46-49) and the three members of
//     JsIntlTable those lines read, with their values (:83-84, :148-155)
//   src/Broiler.VM.Profile.JavaScript.Intl/JsCldrData.cs
//     blob ea0235dbfa7e0c08fa305705e5f19a49309bc91a: the three arms of JsCldrData.Table that answer them (:56-60, :67,
//     :83-84, :91-92), and JsCldrData.Instance (:46-49)
//
// Changes from the source, none of which changes a result:
//   - only the members above are copied; the other fields of JsIntlTables, the other members of IJsIntlData
//     (CldrVersion) and of JsIntlTable, and the other arms of JsCldrData.Table are left out, so the
//     constructor sets only the three fields it keeps; each kept line is the source's, indented by four
//     spaces because the three files' types sit in one file, each in a block namespace of its source's name;
//   - the type declarations' own lines (interface IJsIntlData, enum JsIntlTable, class JsCldrData, class
//     JsIntlTables and the constructor's signature) are the source's without their documentation and
//     annotation lines, and JsCldrData's private constructor is the implicit one.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Broiler.VM.Profile.JavaScript.Format;
using Broiler.VM.Profile.JavaScript.Intl;

namespace Broiler.VM.Profile.JavaScript.Format
{
    public interface IJsIntlData
    {
        /// <summary>One table's bytes; empty for a table this data does not carry.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E1A035
        // Broiler-Human:        PENDING
        System.ReadOnlySpan<byte> Table(JsIntlTable table);
    }

    public enum JsIntlTable
    {
        /// <summary>The ranges of Soft_Dotted, which Lithuanian's casing reads: <c>first</c>, <c>last</c>, in hexadecimal.</summary>
        SoftDotted = 7,

        /// <summary>The break properties' value names in code order: <c>property</c>, then its values.</summary>
        SegmentBreakValues = 23,

        /// <summary>
        /// The break properties over the code space: the combination count and combinations of five value
        /// codes, then a little-endian run count and each run's three-byte first code point and combination.
        /// </summary>
        SegmentBreaks = 24,
    }
}

namespace Broiler.VM.Profile.JavaScript.Intl
{
    public sealed class JsCldrData : IJsIntlData
    {
        /// <summary>The one instance.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F80706
        // Broiler-Human:        PENDING
        public static JsCldrData Instance { get; } = new();

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9B4B1A
        // Broiler-Human:        PENDING
        public System.ReadOnlySpan<byte> Table(JsIntlTable table) => table switch
        {
            JsIntlTable.SoftDotted => JsCldrTables.SoftDotted,
            JsIntlTable.SegmentBreakValues => JsCldrTables.SegmentBreakValues,
            JsIntlTable.SegmentBreaks => JsCldrTables.SegmentBreaks,
            _ => default,
        };
    }
}

namespace Broiler.VM.Profile.JavaScript
{
    internal sealed class JsIntlTables
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=503B2D
        // Broiler-Human:        PENDING
        private readonly IJsIntlData data;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8CD2C4
        // Broiler-Human:        PENDING
        private readonly Lazy<(int First, int Last)[]> softDotted;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B2406B
        // Broiler-Human:        PENDING
        private readonly Lazy<JsBreakData> breaks;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=46DF78
        // Broiler-Human:        PENDING
        internal JsIntlTables(IJsIntlData data)
        {
            this.data = data;
            softDotted = new(ReadRanges);
            breaks = new(() => new JsBreakData(Lines(JsIntlTable.SegmentBreakValues), data.Table(JsIntlTable.SegmentBreaks)));
        }

        /// <summary>Whether a code point is Soft_Dotted: an <c>i</c> or <c>j</c> whose dot an accent above replaces.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=65A410
        // Broiler-Human:        PENDING
        internal bool IsSoftDotted(int codePoint)
        {
            var ranges = softDotted.Value;
            var low = 0;
            var high = ranges.Length - 1;

            while (low <= high)
            {
                var middle = (low + high) >>> 1;

                if (codePoint < ranges[middle].First)
                {
                    high = middle - 1;
                }
                else if (codePoint > ranges[middle].Last)
                {
                    low = middle + 1;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>The break properties <c>Intl.Segmenter</c> reads, decoded the first time a string is segmented.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=82FD5A
        // Broiler-Human:        PENDING
        internal JsBreakData Breaks => breaks.Value;

        /// <summary>The lines of a text table.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=7154A5
        // Broiler-Human:        PENDING
        private List<string> Lines(JsIntlTable table)
        {
            var text = Encoding.ASCII.GetString(data.Table(table));
            var lines = new List<string>();

            foreach (var line in text.Split('\n'))
            {
                if (line.Length != 0)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=F4CF0D
        // Broiler-Human:        PENDING
        private (int First, int Last)[] ReadRanges()
        {
            var lines = Lines(JsIntlTable.SoftDotted);
            var ranges = new (int First, int Last)[lines.Count];

            for (var at = 0; at < lines.Count; at++)
            {
                var bar = lines[at].IndexOf('|');
                ranges[at] = (
                    int.Parse(lines[at][..bar], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                    int.Parse(lines[at][(bar + 1)..], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }

            return ranges;
        }
    }
}
