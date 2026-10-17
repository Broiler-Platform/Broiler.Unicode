// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs
//     blob 65b2fd7dbadd9e98eabfc13297eca6fe21058112: of the partial class JsRealm,
//     StringLengthCeiling (:48-60), the normalize built-in's call (:196-220, as NormalizeBuiltIn), NormalizeText
//     (:659-759), IsNormalizedAlready (:761-806), OrderCanonically (:808-881), ComposeCanonically (:883-929),
//     WriteCodePoints (:931-962), StringCharge (:970-974), StringChangeCase (:1084-1136), TransformCase
//     (:1138-1157), LanguageCasing (:1159-1238), AfterCapitalI (:1240-1259), AfterSoftDotted (:1261-1280),
//     BeforeDot (:1282-1301), MoreAbove (:1303-1324) and AppendCodePoint (:1326-1338)
//   package Broiler.ECMAScript.Algorithms 0.1.0-preview.4, the version Broiler.VM's Directory.Packages.props
//     pins at 24632b1: Utf16.CodePointAt, which NormalizeText and IsNormalizedAlready call
//
// Changes from the source, each marked "ARM CHANGE" where it is made; none changes a result or a charge at
// the ceiling the source has:
//   - the ceiling: StringLengthCeiling stays the constant 1 << 24, and StringChangeCase, TransformCase and
//     NormalizeText take it as a parameter, ceiling, where the source reads the constant (:721, :1129). The
//     comparison passes StringLengthCeiling for every input, and the limit of rule class U7 for its limit
//     variants, so that Broiler.VM's own enforcement (rows C12 and N6) runs at each limit; at
//     StringLengthCeiling the copy is the source;
//   - TransformCase takes the language as a parameter: the realm's Intl check (:1147-1150) and the locale's
//     resolution (:1152-1154) are the adapter's (rows C9 and C10) and are not copied; its last line is the
//     source's (:1156);
//   - the normalize built-in (:204-220) is NormalizeBuiltIn, its last line (:219) verbatim: the receiver's
//     coercion, the form's ToString and its RangeError are the adapter's, and the comparison passes a form;
//   - the toUpperCase and toLowerCase built-ins (:430-440) are StringChangeCase called on the receiver's
//     String, as their one line calls it; the toLocale*Case built-ins (:442-446) are TransformCase;
//   - StringChangeCase, TransformCase and StringLengthCeiling are internal where the source's are private,
//     so the comparison can call them and pass the ceiling;
//   - ArgOfString (:964-968), which sits between WriteCodePoints and StringCharge and reads a JsValue, is not
//     copied;
//   - JsEngine is the arm's (JsEngine.cs beside this file): Charge(ulong) records every charge, Error(name,
//     message) is the recorded exception JsThrow, and Intl is arm V's JsIntlTables over the copied tables;
//   - the members sit in a partial class JsRealm of their own, in the source's namespace.

using Broiler.ECMAScript;
using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Text;

namespace Broiler.VM.Profile.JavaScript;

internal sealed partial class JsRealm
{
    /// <summary>
    /// The longest string <c>repeat</c>, <c>padStart</c> and <c>padEnd</c> will materialise.
    /// </summary>
    /// <remarks>
    /// The specification's ceiling is 2^53-1, which no implementation can honour and which this one
    /// would meet by exhausting the host's memory rather than the guest's allowance. Sixteen million
    /// characters is thirty-two megabytes of UTF-16 and is a declared deviation: a program that asks
    /// for more gets the same <c>RangeError</c> the specification gives for a length it cannot
    /// represent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3C0D2E
    // Broiler-Human:        PENDING
    internal const int StringLengthCeiling = 1 << 24; // ARM CHANGE: internal, so the comparison passes it as the ceiling

    /// <summary>
    /// ARM CHANGE: the normalize built-in (:204-220) as the comparison calls it, with the receiver's String and
    /// a form; its last line (:219) follows, verbatim but for the ceiling and the JsValue.String around it.
    /// </summary>
    internal static string NormalizeBuiltIn(JsEngine engine, string text, string form, int ceiling)
    {
        // `normalize` IS THE UNICODE NORMALIZATION OF THE STRING'S CODE POINTS, read from the
        // Unicode 17.0.0 tables JSD-0031 pins and never from the platform: the platform's
        // `String.Normalize` returns its input unchanged under globalization-invariant mode and
        // throws on a lone surrogate the language requires to pass through (rule N23).
        //
        // The order is the specification's: the receiver is coerced first, then the form, and an
        // unknown form is a `RangeError` before any text is looked at. The work itself is
        // `NormalizeText`'s, which charges for what it scans and what it builds.
        return NormalizeText(engine, text, compose: form is "NFC" or "NFKC", compatibility: form is "NFKC" or "NFKD", ceiling); // ARM CHANGE: the ceiling
    }

    /// <summary>
    /// The Unicode normalization of <paramref name="text"/>'s code points: NFD, NFKD with
    /// <paramref name="compatibility"/>, and NFC or NFKC with <paramref name="compose"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The algorithm is UAX #15's, over the JSD-0031 tables.</b> Every code point is replaced by
    /// its full decomposition, each run of non-starters is put in canonical order by combining
    /// class, and for the composed forms each code point is combined with the last starter unless
    /// something between them blocks it. A lone surrogate is read as the code point it is: class 0,
    /// no mapping, no composite, so it passes through where the language says it does.
    /// </para>
    /// <para>
    /// <b>The common case costs one scan and allocates nothing.</b> <see cref="IsNormalizedAlready"/>
    /// is the quick check: ASCII answers yes a unit at a time, and a string that is already in the
    /// form is returned as it came. That is the fast path ASCII always had.
    /// </para>
    /// <para>
    /// <b>The expansion is measured before it is built.</b> A compatibility decomposition can be 18
    /// code points long (U+FDFA), so the first pass counts the decomposition, refuses one past
    /// <see cref="StringLengthCeiling"/> UTF-16 units with the <c>RangeError</c> <c>repeat</c> gives,
    /// and charges the storage and the three passes over it before a buffer exists. The canonical
    /// ordering is a counting sort per run, so a long run of combining marks is linear work too.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=DD3536
    // Broiler-Falsified-If: a guest string makes normalize allocate or loop over an expansion it was not charged for, or answer other than the pinned NormalizationTest.txt vectors
    // Broiler-Human:        PENDING
    internal static string NormalizeText(JsEngine engine, string text, bool compose, bool compatibility, int ceiling) // ARM CHANGE: the ceiling, a parameter
    {
        StringCharge(engine, text.Length);

        if (IsNormalizedAlready(text, compose, compatibility))
        {
            return text;
        }

        long points = 0;
        long units = 0;

        for (var at = 0; at < text.Length;)
        {
            var read = Utf16.CodePointAt(text, at);
            at += read.CodeUnitCount;
            var codePoint = read.CodePoint;

            if (JsUnicodeNormalization.TryGetDecomposition(codePoint, compatibility, out var mapping))
            {
                points += mapping.Length;

                for (var index = 0; index < mapping.Length; index++)
                {
                    units += mapping[index] > 0xFFFF ? 2 : 1;
                }
            }
            else
            {
                points++;
                units += codePoint > 0xFFFF ? 2 : 1;
            }
        }

        if (units > ceiling) // ARM CHANGE: the parameter, where the source reads StringLengthCeiling
        {
            throw engine.Error("RangeError", "Invalid string length");
        }

        // The storage, then the decomposition, reordering and composition passes over it.
        StringCharge(engine, text.Length + ((int)points * 3));

        var buffer = new int[(int)points];
        var length = 0;

        for (var at = 0; at < text.Length;)
        {
            var read = Utf16.CodePointAt(text, at);
            at += read.CodeUnitCount;
            var codePoint = read.CodePoint;

            if (JsUnicodeNormalization.TryGetDecomposition(codePoint, compatibility, out var mapping))
            {
                for (var index = 0; index < mapping.Length; index++)
                {
                    buffer[length++] = mapping[index];
                }
            }
            else
            {
                buffer[length++] = codePoint;
            }
        }

        OrderCanonically(buffer, length);

        if (compose)
        {
            length = ComposeCanonically(buffer, length);
        }

        return WriteCodePoints(buffer, length);
    }

    /// <summary>
    /// The quick check: true when <paramref name="text"/> is certainly in the form already, so it
    /// can be returned unchanged.
    /// </summary>
    /// <remarks>
    /// A code point breaks it when it is out of canonical order, when it decomposes (the decomposed
    /// forms), or when its <c>NFC_QC</c> or <c>NFKC_QC</c> is not Yes (the composed forms, where
    /// Maybe needs the full algorithm to answer). An ASCII unit is class 0 and Yes in every form.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=68E403
    // Broiler-Human:        PENDING
    private static bool IsNormalizedAlready(string text, bool compose, bool compatibility)
    {
        var lastClass = 0;

        for (var at = 0; at < text.Length;)
        {
            if (text[at] < 0x80)
            {
                lastClass = 0;
                at++;
                continue;
            }

            var read = Utf16.CodePointAt(text, at);
            at += read.CodeUnitCount;
            var codePoint = read.CodePoint;
            var combiningClass = JsUnicodeNormalization.CombiningClass(codePoint);

            if (combiningClass != 0 && lastClass > combiningClass)
            {
                return false;
            }

            if (compose
                ? JsUnicodeNormalization.QuickCheck(codePoint, compatibility) != JsNormalizationQuickCheck.Yes
                : JsUnicodeNormalization.TryGetDecomposition(codePoint, compatibility, out _))
            {
                return false;
            }

            lastClass = combiningClass;
        }

        return true;
    }

    /// <summary>
    /// The canonical ordering algorithm: each run of non-starters is sorted by combining class,
    /// stably, so marks of one class keep their order.
    /// </summary>
    /// <remarks>
    /// A counting sort over the 256 classes rather than the textbook exchange sort, whose work is
    /// quadratic in a run's length - and a run's length is the guest's choice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6AE9E6
    // Broiler-Human:        PENDING
    private static void OrderCanonically(int[] buffer, int length)
    {
        int[]? scratch = null;
        System.Span<int> starts = stackalloc int[257];

        for (var start = 0; start < length;)
        {
            var previous = JsUnicodeNormalization.CombiningClass(buffer[start]);

            if (previous == 0)
            {
                start++;
                continue;
            }

            var end = start + 1;
            var ordered = true;

            while (end < length)
            {
                var next = JsUnicodeNormalization.CombiningClass(buffer[end]);

                if (next == 0)
                {
                    break;
                }

                ordered &= previous <= next;
                previous = next;
                end++;
            }

            if (!ordered)
            {
                var count = end - start;

                if (scratch is null || scratch.Length < count)
                {
                    scratch = new int[count];
                }

                starts.Clear();

                for (var at = start; at < end; at++)
                {
                    starts[JsUnicodeNormalization.CombiningClass(buffer[at]) + 1]++;
                }

                for (var value = 1; value < starts.Length; value++)
                {
                    starts[value] += starts[value - 1];
                }

                for (var at = start; at < end; at++)
                {
                    scratch[starts[JsUnicodeNormalization.CombiningClass(buffer[at])]++] = buffer[at];
                }

                System.Array.Copy(scratch, 0, buffer, start, count);
            }

            start = end;
        }
    }

    /// <summary>
    /// The canonical composition algorithm over a canonically ordered decomposition, in place;
    /// answers the composed length.
    /// </summary>
    /// <remarks>
    /// A code point is blocked from the last starter when something kept between them is a starter
    /// or has a class at least its own. Because the input is in canonical order, the last code point
    /// kept since the starter is the only one that has to be looked at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AD023A
    // Broiler-Human:        PENDING
    private static int ComposeCanonically(int[] buffer, int length)
    {
        var starter = -1;
        var keptSinceStarter = false;
        var lastClass = 0;
        var written = 0;

        for (var at = 0; at < length; at++)
        {
            var codePoint = buffer[at];
            var combiningClass = JsUnicodeNormalization.CombiningClass(codePoint);

            if (starter >= 0 &&
                !(keptSinceStarter && (lastClass == 0 || lastClass >= combiningClass)) &&
                JsUnicodeNormalization.TryCompose(buffer[starter], codePoint, out var composite))
            {
                buffer[starter] = composite;
                continue;
            }

            if (combiningClass == 0)
            {
                starter = written;
                keptSinceStarter = false;
            }
            else
            {
                keptSinceStarter = true;
            }

            lastClass = combiningClass;
            buffer[written++] = codePoint;
        }

        return written;
    }

    /// <summary>The UTF-16 string of <paramref name="length"/> code points from <paramref name="buffer"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B9F894
    // Broiler-Human:        PENDING
    private static string WriteCodePoints(int[] buffer, int length)
    {
        var units = 0;

        for (var at = 0; at < length; at++)
        {
            units += buffer[at] > 0xFFFF ? 2 : 1;
        }

        return string.Create(units, (buffer, length), static (span, state) =>
        {
            var written = 0;

            for (var at = 0; at < state.length; at++)
            {
                var codePoint = state.buffer[at];

                if (codePoint > 0xFFFF)
                {
                    span[written++] = (char)(0xD7C0 + (codePoint >> 10));
                    span[written++] = (char)(0xDC00 | (codePoint & 0x3FF));
                }
                else
                {
                    span[written++] = (char)codePoint;
                }
            }
        });
    }

    // ARM CHANGE: ArgOfString (:964-968), which reads a JsValue argument, is not copied.

    /// <summary>Charges fuel for work proportional to <paramref name="units"/> characters.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3A5162
    // Broiler-Human:        PENDING
    private static void StringCharge(JsEngine engine, int units) =>
        engine.Charge(units <= 0 ? 1UL : (ulong)units);

    /// <summary>
    /// The Unicode Default Case Conversion over the pinned tables: full mappings, and Final_Sigma
    /// for GREEK CAPITAL LETTER SIGMA when lowering. No language is applied.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F34134
    // Broiler-Human:        PENDING
    internal static string StringChangeCase(JsEngine engine, string text, bool upper, int ceiling) // ARM CHANGE: internal, and the ceiling, a parameter
    {
        StringCharge(engine, text.Length);

        // THE PINNED TABLES AND NOT THE PLATFORM'S TextInfo, which maps one code unit to one code
        // unit and reads whatever Unicode version the host carries (JSD-0027 N2). The text is
        // copied only from the first code point casing changes, so an unchanged string costs one
        // pass and no allocation.
        System.Text.StringBuilder? built = null;

        for (var at = 0; at < text.Length;)
        {
            var codePoint = JsUnicodeCasing.NextCodePoint(text, at, out var width);
            var start = at;
            at += width;

            if (!upper && codePoint == JsUnicodeCasing.CapitalSigma)
            {
                var final = JsUnicodeCasing.IsFinalSigma(text, start, width, out var scanned);
                StringCharge(engine, scanned);
                built ??= new System.Text.StringBuilder(text, 0, start, text.Length + 8);
                built.Append((char)(final ? JsUnicodeCasing.FinalSigma : 0x03C3));
                continue;
            }

            if (!JsUnicodeCasing.TryGetMapping(codePoint, upper, out var mapping))
            {
                built?.Append(text, start, width);
                continue;
            }

            StringCharge(engine, mapping.Length);
            built ??= new System.Text.StringBuilder(text, 0, start, text.Length + 8);

            for (var index = 0; index < mapping.Length; index++)
            {
                AppendCodePoint(built, mapping[index]);
            }

            if (built.Length > ceiling) // ARM CHANGE: the parameter, where the source reads StringLengthCeiling
            {
                throw engine.Error("RangeError", "Invalid string length");
            }
        }

        return built?.ToString() ?? text;
    }

    /// <summary>
    /// ECMA-402's <c>TransformCase</c> where the realm has <c>Intl</c>: the first requested locale, and
    /// the language rules of SpecialCasing.txt when it is Turkish, Azeri or Lithuanian; without
    /// <c>Intl</c>, the locale is ignored as JSD-0027 section 1 states.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s20.1.2.1; IP=Low; Security=Medium; Resources=3; Fingerprint=920956
    // Broiler-Human:        PENDING
    // ARM CHANGE: internal; the language (the locale's resolution's result, "und" when it matches none of
    // az, lt and tr) and the ceiling are parameters, where the source takes the arguments.
    internal static string TransformCase(JsEngine engine, string text, string language, bool upper, int ceiling)
    {
        // ARM CHANGE: the realm's Intl check (:1147-1150) and the locale's resolution (:1152-1154) are the
        // adapter's, and are not copied.

        return StringChangeCase(engine, LanguageCasing(engine, text, upper, language), upper, ceiling); // ARM CHANGE: the ceiling
    }

    /// <summary>
    /// SpecialCasing.txt's rules for one language, applied before the default conversion: each one's
    /// output is a fixed point of the default mapping in its direction, so the two passes compose,
    /// and each condition is read on the string as it was.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=Unicode SpecialCasing.txt; IP=Low; Security=Medium; Resources=3; Fingerprint=62B580
    // Broiler-Human:        PENDING
    private static string LanguageCasing(JsEngine engine, string text, bool upper, string language)
    {
        if (language is not ("tr" or "az" or "lt"))
        {
            return text;
        }

        StringCharge(engine, text.Length);

        var points = new System.Collections.Generic.List<int>(text.Length);

        for (var at = 0; at < text.Length;)
        {
            points.Add(JsUnicodeCasing.NextCodePoint(text, at, out var width));
            at += width;
        }

        var built = new System.Text.StringBuilder(text.Length + 4);

        for (var index = 0; index < points.Count; index++)
        {
            var point = points[index];

            if (language is "tr" or "az")
            {
                if (!upper && point == 0x0130)
                {
                    built.Append('i');
                    continue;
                }

                if (!upper && point == 0x0307 && AfterCapitalI(points, index))
                {
                    continue;
                }

                if (!upper && point == 'I')
                {
                    built.Append(BeforeDot(points, index) ? 'i' : '\u0131');
                    continue;
                }

                if (upper && point == 'i')
                {
                    built.Append('\u0130');
                    continue;
                }
            }
            else
            {
                if (!upper && point is 'I' or 'J' or 0x012E && MoreAbove(points, index))
                {
                    built.Append(point == 0x012E ? '\u012F' : (char)(point + 0x20)).Append('\u0307');
                    continue;
                }

                if (!upper && point is 0x00CC or 0x00CD or 0x0128)
                {
                    built.Append("i\u0307").Append(point switch { 0x00CC => '\u0300', 0x00CD => '\u0301', _ => '\u0303' });
                    continue;
                }

                if (upper && point == 0x0307 && AfterSoftDotted(engine.Intl!, points, index))
                {
                    continue;
                }
            }

            AppendCodePoint(built, point);
        }

        return built.ToString();
    }

    /// <summary>After_I: an upper-case I before, with no combining class 0 or 230 between.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DC6A33
    // Broiler-Human:        PENDING
    private static bool AfterCapitalI(System.Collections.Generic.List<int> points, int index)
    {
        for (var at = index - 1; at >= 0; at--)
        {
            if (points[at] == 'I')
            {
                return true;
            }

            if (JsUnicodeNormalization.CombiningClass(points[at]) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>After_Soft_Dotted: a Soft_Dotted character before, with no combining class 0 or 230 between.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0DC57E
    // Broiler-Human:        PENDING
    private static bool AfterSoftDotted(JsIntlTables intl, System.Collections.Generic.List<int> points, int index)
    {
        for (var at = index - 1; at >= 0; at--)
        {
            if (intl.IsSoftDotted(points[at]))
            {
                return true;
            }

            if (JsUnicodeNormalization.CombiningClass(points[at]) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>Before_Dot: COMBINING DOT ABOVE after, with only combining classes other than 0 and 230 between.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1DB648
    // Broiler-Human:        PENDING
    private static bool BeforeDot(System.Collections.Generic.List<int> points, int index)
    {
        for (var at = index + 1; at < points.Count; at++)
        {
            if (points[at] == 0x0307)
            {
                return true;
            }

            if (JsUnicodeNormalization.CombiningClass(points[at]) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>More_Above: a combining class 230 after, with no combining class 0 between.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0640C3
    // Broiler-Human:        PENDING
    private static bool MoreAbove(System.Collections.Generic.List<int> points, int index)
    {
        for (var at = index + 1; at < points.Count; at++)
        {
            var combining = JsUnicodeNormalization.CombiningClass(points[at]);

            if (combining == 230)
            {
                return true;
            }

            if (combining == 0)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>Appends one code point as one or two code units.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2EBD3E
    // Broiler-Human:        PENDING
    private static void AppendCodePoint(System.Text.StringBuilder built, int codePoint)
    {
        if (codePoint <= 0xFFFF)
        {
            built.Append((char)codePoint);
            return;
        }

        built.Append(char.ConvertFromUtf32(codePoint));
    }
}
