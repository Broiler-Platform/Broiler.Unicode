using System;
using System.Text;

namespace Broiler.Unicode.Algorithms;

/// <summary>The languages whose case mappings <c>SpecialCasing.txt</c> tailors, as <see cref="UnicodeCaseMapping"/> applies them.</summary>
public enum CaseLanguage
{
    /// <summary>No language: the Default Case Conversion, with Final_Sigma.</summary>
    Default = 0,

    /// <summary>Turkish and Azeri (<c>tr</c>, <c>az</c>): dotted and dotless i.</summary>
    Turkic = 1,

    /// <summary>Lithuanian (<c>lt</c>): the dot above a soft-dotted letter.</summary>
    Lithuanian = 2,
}

/// <summary>
/// Full case mapping: the Unicode Default Case Conversion ECMA-262's <c>toUpperCase</c> and
/// <c>toLowerCase</c> perform (section 22.1.3.29, "String.prototype.toLowerCase"; Unicode Standard
/// section 3.13), and the language-sensitive mappings ECMA-402's TransformCase reads for Turkic and
/// Lithuanian (ECMA-402 section 19.1.2.1).
/// </summary>
/// <remarks>
/// <para>
/// <b>The Default Case Conversion</b> applies the full mappings, <c>SpecialCasing.txt</c>'s unconditional
/// lines over <c>UnicodeData.txt</c>'s simple ones, and of the conditions only Final_Sigma: U+03A3 lowers
/// to U+03C2 when a Cased code point precedes it, Case_Ignorable ones aside, and none follows. A lone
/// surrogate is a code point of its own and maps to itself. No platform casing is read: neither the
/// runtime's case tables nor a culture.
/// </para>
/// <para>
/// <b>A language</b> applies <c>SpecialCasing.txt</c>'s conditional lines for it first - After_I,
/// Before_Dot, After_Soft_Dotted and More_Above, each read on the string as it was - and then the
/// Default Case Conversion; each language rule's output is a fixed point of the default mapping in its
/// direction, so the two passes compose. Soft_Dotted is the tables' (<see cref="UnicodeTable.SoftDottedRanges"/>).
/// </para>
/// <para>
/// <b>The charge</b> is Broiler.VM's, in its units (JSD-0074, decision 4; correspondence row C13), on
/// the default pass: the length of its input, then each mapping's length in code points, then the code
/// units each Final_Sigma test reads. Each charge is at least one unit. The language pass charges nothing
/// here: an engine that charges it charges it itself.
/// </para>
/// <para>
/// <b>The limit is on the result</b>: a result longer than the caller's maximum, in UTF-16 code units,
/// is <see cref="UnicodeStatus.LimitExceeded"/>, checked as the result grows (row C12).
/// </para>
/// </remarks>
public sealed class UnicodeCaseMapping
{
    private const int CapitalSigma = 0x03A3;
    private const int SmallSigma = 0x03C3;
    private const int FinalSigma = 0x03C2;
    private const int CombiningDotAbove = 0x0307;

    private readonly IUnicodeTables tables;

    private readonly UnicodeNormalization normalization;

    /// <summary>Case mapping over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.CasingLayout"/> to <see cref="UnicodeTable.SoftDottedRanges"/>, and the
    /// normalization tables, whose combining classes the language conditions read.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    public UnicodeCaseMapping(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);
        normalization = new UnicodeNormalization(tables);

        TableReader.Require(tables, UnicodeTable.CasingLayout, 3);
        TableReader.Require(tables, UnicodeTable.UpperIndex, 6);
        TableReader.Require(tables, UnicodeTable.LowerIndex, 6);
        TableReader.Require(tables, UnicodeTable.MappingPool, 3);
        TableReader.Require(tables, UnicodeTable.CasedRanges, 6);
        TableReader.Require(tables, UnicodeTable.CaseIgnorableRanges, 6);
        TableReader.Require(tables, UnicodeTable.SoftDottedRanges, 6);

        var layout = tables.Table(UnicodeTable.CasingLayout);

        if (layout.Length != 3)
        {
            throw new ArgumentException($"CasingLayout holds {layout.Length / 3} values, not 1.", nameof(tables));
        }

        MaxMappingLength = TableReader.Layout(layout, 0);
    }

    /// <summary>The longest full case mapping of one code point, in code points.</summary>
    public int MaxMappingLength { get; }

    /// <summary>Whether <paramref name="codePoint"/> is Cased (<c>DerivedCoreProperties.txt</c>).</summary>
    /// <param name="codePoint">A code point.</param>
    public bool IsCased(int codePoint) => TableReader.InRanges(tables.Table(UnicodeTable.CasedRanges), codePoint);

    /// <summary>Whether <paramref name="codePoint"/> is Case_Ignorable (<c>DerivedCoreProperties.txt</c>).</summary>
    /// <param name="codePoint">A code point.</param>
    public bool IsCaseIgnorable(int codePoint) => TableReader.InRanges(tables.Table(UnicodeTable.CaseIgnorableRanges), codePoint);

    /// <summary>Whether <paramref name="codePoint"/> is Soft_Dotted (<c>PropList.txt</c>).</summary>
    /// <param name="codePoint">A code point.</param>
    public bool IsSoftDotted(int codePoint) => TableReader.InRanges(tables.Table(UnicodeTable.SoftDottedRanges), codePoint);

    /// <summary>
    /// The full upper-case mapping of <paramref name="text"/> in <paramref name="language"/> (ECMA-262
    /// section 22.1.3.31 and ECMA-402 TransformCase). A text casing does not change is answered as itself.
    /// </summary>
    /// <param name="text">The text, read as code points; a lone surrogate is one.</param>
    /// <param name="language">The language whose conditional mappings apply first.</param>
    /// <param name="maxLength">The longest result the caller accepts, in UTF-16 code units.</param>
    /// <param name="charge">The caller's meter, or null; units as this type states.</param>
    /// <param name="result">The mapped text, or null when the status is not <see cref="UnicodeStatus.Success"/>.</param>
    /// <returns><see cref="UnicodeStatus.Success"/>, or <see cref="UnicodeStatus.LimitExceeded"/> when the result is longer than <paramref name="maxLength"/>.</returns>
    public UnicodeStatus ToUpperCase(string text, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result) =>
        Map(text, upper: true, language, maxLength, charge, out result);

    /// <summary>
    /// The full lower-case mapping of <paramref name="text"/> in <paramref name="language"/>, with
    /// Final_Sigma (ECMA-262 section 22.1.3.29 and ECMA-402 TransformCase). A text casing does not change
    /// is answered as itself.
    /// </summary>
    /// <param name="text">The text, read as code points; a lone surrogate is one.</param>
    /// <param name="language">The language whose conditional mappings apply first.</param>
    /// <param name="maxLength">The longest result the caller accepts, in UTF-16 code units.</param>
    /// <param name="charge">The caller's meter, or null; units as this type states.</param>
    /// <param name="result">The mapped text, or null when the status is not <see cref="UnicodeStatus.Success"/>.</param>
    /// <returns><see cref="UnicodeStatus.Success"/>, or <see cref="UnicodeStatus.LimitExceeded"/> when the result is longer than <paramref name="maxLength"/>.</returns>
    public UnicodeStatus ToLowerCase(string text, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result) =>
        Map(text, upper: false, language, maxLength, charge, out result);

    private static void Charge(UnicodeWorkCharge? charge, int units) =>
        charge?.Invoke(units <= 0 ? 1UL : (ulong)units);

    private static void AppendCodePoint(StringBuilder built, int codePoint)
    {
        if (codePoint <= 0xFFFF)
        {
            built.Append((char)codePoint);
            return;
        }

        built.Append((char)(0xD7C0 + (codePoint >> 10))).Append((char)(0xDC00 | (codePoint & 0x3FF)));
    }

    private UnicodeStatus Map(string text, bool upper, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        if ((uint)language > (uint)CaseLanguage.Lithuanian)
        {
            throw new ArgumentOutOfRangeException(nameof(language));
        }

        var source = language == CaseLanguage.Default ? text : LanguagePass(text, upper, language);

        return DefaultPass(source, upper, maxLength, charge, out result);
    }

    /// <summary>
    /// The Default Case Conversion over <paramref name="text"/>. The text is copied only from the first
    /// code point casing changes, so an unchanged text costs one pass and no allocation.
    /// </summary>
    private UnicodeStatus DefaultPass(string text, bool upper, int maxLength, UnicodeWorkCharge? charge, out string? result)
    {
        Charge(charge, text.Length);

        StringBuilder? built = null;

        for (var at = 0; at < text.Length;)
        {
            var codePoint = Utf16Reader.Next(text, at, out var width);
            var start = at;
            at += width;

            if (!upper && codePoint == CapitalSigma)
            {
                var final = IsFinalSigma(text, start, width, out var scanned);
                Charge(charge, scanned);
                built ??= new StringBuilder(text, 0, start, (int)Math.Min(text.Length + 8L, maxLength + 1L));
                built.Append((char)(final ? FinalSigma : SmallSigma));
            }
            else if (TryGetMapping(codePoint, upper, out var mapping))
            {
                Charge(charge, mapping.Length);
                built ??= new StringBuilder(text, 0, start, (int)Math.Min(text.Length + 8L, maxLength + 1L));

                for (var index = 0; index < mapping.Length; index++)
                {
                    AppendCodePoint(built, mapping[index]);
                }
            }
            else
            {
                built?.Append(text, start, width);
            }

            if (built is not null && built.Length > maxLength)
            {
                result = null;
                return UnicodeStatus.LimitExceeded;
            }
        }

        result = built?.ToString() ?? text;

        if (result.Length > maxLength)
        {
            result = null;
            return UnicodeStatus.LimitExceeded;
        }

        return UnicodeStatus.Success;
    }

    /// <summary>
    /// <c>SpecialCasing.txt</c>'s conditional lines for one language, applied before the Default Case
    /// Conversion. Each condition reads the string as it was, and each scan stops at the first code point
    /// of combining class 0 or 230, so the passes over the string are linear in all.
    /// </summary>
    private string LanguagePass(string text, bool upper, CaseLanguage language)
    {
        var points = new int[text.Length];
        var count = 0;

        for (var at = 0; at < text.Length;)
        {
            points[count++] = Utf16Reader.Next(text, at, out var width);
            at += width;
        }

        var built = new StringBuilder(text.Length + 4);

        for (var index = 0; index < count; index++)
        {
            var point = points[index];

            if (language == CaseLanguage.Turkic)
            {
                if (!upper && point == 0x0130)
                {
                    built.Append('i');
                    continue;
                }

                if (!upper && point == CombiningDotAbove && AfterCapitalI(points, index))
                {
                    continue;
                }

                if (!upper && point == 'I')
                {
                    built.Append(BeforeDot(points, count, index) ? 'i' : 'ı');
                    continue;
                }

                if (upper && point == 'i')
                {
                    built.Append('İ');
                    continue;
                }
            }
            else
            {
                if (!upper && point is 'I' or 'J' or 0x012E && MoreAbove(points, count, index))
                {
                    built.Append(point == 0x012E ? 'į' : (char)(point + 0x20)).Append('̇');
                    continue;
                }

                if (!upper && point is 0x00CC or 0x00CD or 0x0128)
                {
                    built.Append("i̇").Append(point switch { 0x00CC => '̀', 0x00CD => '́', _ => '̃' });
                    continue;
                }

                if (upper && point == CombiningDotAbove && AfterSoftDotted(points, index))
                {
                    continue;
                }
            }

            AppendCodePoint(built, point);
        }

        return built.ToString();
    }

    /// <summary>After_I: an upper-case I before, with no combining class 0 or 230 between.</summary>
    private bool AfterCapitalI(int[] points, int index)
    {
        for (var at = index - 1; at >= 0; at--)
        {
            if (points[at] == 'I')
            {
                return true;
            }

            if (normalization.CombiningClass(points[at]) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>After_Soft_Dotted: a Soft_Dotted code point before, with no combining class 0 or 230 between.</summary>
    private bool AfterSoftDotted(int[] points, int index)
    {
        for (var at = index - 1; at >= 0; at--)
        {
            if (IsSoftDotted(points[at]))
            {
                return true;
            }

            if (normalization.CombiningClass(points[at]) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>Before_Dot: U+0307 after, with only combining classes other than 0 and 230 between.</summary>
    private bool BeforeDot(int[] points, int count, int index)
    {
        for (var at = index + 1; at < count; at++)
        {
            if (points[at] == CombiningDotAbove)
            {
                return true;
            }

            if (normalization.CombiningClass(points[at]) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>More_Above: a combining class 230 after, with no combining class 0 between.</summary>
    private bool MoreAbove(int[] points, int count, int index)
    {
        for (var at = index + 1; at < count; at++)
        {
            var combining = normalization.CombiningClass(points[at]);

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

    /// <summary>
    /// Whether the code point at <paramref name="at"/>, <paramref name="width"/> code units wide, meets
    /// Final_Sigma (Unicode Standard section 3.13, table 3-17), and how many code units the test read.
    /// A code point both Cased and Case_Ignorable is skipped, as the definition's sequences read it.
    /// </summary>
    private bool IsFinalSigma(string text, int at, int width, out int scanned)
    {
        scanned = 0;
        var before = false;

        for (var position = at; position > 0;)
        {
            var codePoint = Utf16Reader.Previous(text, position, out var size);
            position -= size;
            scanned += size;

            if (IsCaseIgnorable(codePoint))
            {
                continue;
            }

            before = IsCased(codePoint);
            break;
        }

        if (!before)
        {
            return false;
        }

        for (var position = at + width; position < text.Length;)
        {
            var codePoint = Utf16Reader.Next(text, position, out var size);
            position += size;
            scanned += size;

            if (IsCaseIgnorable(codePoint))
            {
                continue;
            }

            return !IsCased(codePoint);
        }

        return true;
    }

    /// <summary>The full mapping of <paramref name="codePoint"/> in the default language, when casing changes it.</summary>
    private bool TryGetMapping(int codePoint, bool upper, out CodePoints mapping)
    {
        var index = tables.Table(upper ? UnicodeTable.UpperIndex : UnicodeTable.LowerIndex);
        var low = 0;
        var high = (index.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var key = TableReader.Int24(index, middle * 6);

            if (key == codePoint)
            {
                var offset = TableReader.UInt16(index, (middle * 6) + 3);

                mapping = new CodePoints(tables.Table(UnicodeTable.MappingPool).Slice(offset * 3, index[(middle * 6) + 5] * 3));
                return true;
            }

            if (key < codePoint)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        mapping = default;
        return false;
    }
}
