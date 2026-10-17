// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   39
// Annotated:        39/39
// Exempt:           13
// Human-reviewed:   0/39
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  5/10 max
// Unverified:       39
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Globalization;
using System.Text;

namespace Broiler.Unicode.Algorithms;

/// <summary>The languages whose case mappings <c>SpecialCasing.txt</c> tailors, as <see cref="UnicodeCaseMapping"/> applies them.</summary>
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=8F0877
// Broiler-Human:        PENDING
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
/// direction, so the two passes compose. Soft_Dotted is the tables' (<see cref="UnicodeTable.CasingRecords"/>).
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
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=1; Fingerprint=C91117
// Broiler-Human:        PENDING
public sealed class UnicodeCaseMapping
{
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=A44FF1
    // Broiler-Human:        PENDING
    private const int CapitalSigma = 0x03A3;
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=22CC20
    // Broiler-Human:        PENDING
    private const int SmallSigma = 0x03C3;
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=6DC9FA
    // Broiler-Human:        PENDING
    private const int FinalSigma = 0x03C2;
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=7FDE59
    // Broiler-Human:        PENDING
    private const int CombiningDotAbove = 0x0307;

    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=79C10D
    // Broiler-Human:        PENDING
    private const int StackLimit = 256;

    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=508689
    // Broiler-Human:        PENDING
    private const int Longer = -1;

    private readonly IUnicodeTables tables;

    private readonly UnicodeNormalization normalization;

    private readonly int[] upperLatin1;

    private readonly int[] lowerLatin1;

    private readonly bool asciiLetters;

    /// <summary>Case mapping over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying
    /// <see cref="UnicodeTable.CasingLayout"/>, <see cref="UnicodeTable.MappingPool"/>,
    /// <see cref="UnicodeTable.CasingTrie"/> and <see cref="UnicodeTable.CasingRecords"/>, and the
    /// normalization tables, whose combining classes the language conditions read.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    /// <remarks>
    /// The default mappings of U+0000 to U+00FF are read from the tables once here, so the commonest code
    /// points are mapped without a search; a convention 10 value derived from the provider at
    /// construction, 2 KB, which never grows.
    /// </remarks>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=8DAD1F
    // Broiler-Human:        PENDING
    public UnicodeCaseMapping(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);
        normalization = new UnicodeNormalization(tables);

        TableReader.Require(tables, UnicodeTable.CasingLayout, 3);
        TableReader.Require(tables, UnicodeTable.MappingPool, 3);
        TableReader.RequireTrie(tables, UnicodeTable.CasingTrie);
        TableReader.RequireRecords(tables, UnicodeTable.CasingTrie, UnicodeTable.CasingRecords, CaseTables.RecordSize);

        var layout = tables.Table(UnicodeTable.CasingLayout);

        if (layout.Length != 3)
        {
            throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"CasingLayout holds {layout.Length / 3} values, not 1."), nameof(tables));
        }

        MaxMappingLength = TableReader.Layout(layout, 0);
        upperLatin1 = Latin1(new CaseTables(tables, upper: true));
        lowerLatin1 = Latin1(new CaseTables(tables, upper: false));
        asciiLetters = true;

        for (var unit = 0; unit < 0x80; unit++)
        {
            asciiLetters &= upperLatin1[unit] == (unit is >= 'a' and <= 'z' ? unit - 0x20 : unit) &&
                lowerLatin1[unit] == (unit is >= 'A' and <= 'Z' ? unit + 0x20 : unit);
        }
    }

    /// <summary>The longest full case mapping of one code point, in code points.</summary>
    public int MaxMappingLength { get; }

    /// <summary>Whether <paramref name="codePoint"/> is Cased (<c>DerivedCoreProperties.txt</c>).</summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=A9A20A
    // Broiler-Human:        PENDING
    public bool IsCased(int codePoint) => HasFlag(codePoint, CaseTables.Cased);

    /// <summary>Whether <paramref name="codePoint"/> is Case_Ignorable (<c>DerivedCoreProperties.txt</c>).</summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=EEC7E7
    // Broiler-Human:        PENDING
    public bool IsCaseIgnorable(int codePoint) => HasFlag(codePoint, CaseTables.CaseIgnorable);

    /// <summary>Whether <paramref name="codePoint"/> is Soft_Dotted (<c>PropList.txt</c>).</summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=5ABF71
    // Broiler-Human:        PENDING
    public bool IsSoftDotted(int codePoint) => HasFlag(codePoint, CaseTables.SoftDotted);

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
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=High; Resources=2; Fingerprint=1ADC04
    // Broiler-Falsified-If: a result longer than maxLength is answered as Success
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=High; Resources=2; Fingerprint=503D46
    // Broiler-Falsified-If: a result longer than maxLength is answered as Success
    // Broiler-Human:        PENDING
    public UnicodeStatus ToLowerCase(string text, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result) =>
        Map(text, upper: false, language, maxLength, charge, out result);

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=15F86B
    // Broiler-Human:        PENDING
    private static void Charge(UnicodeWorkCharge? charge, int units) =>
        charge?.Invoke(units <= 0 ? 1UL : (ulong)units);

    /// <summary>
    /// The default mapping of each of U+0000 to U+00FF: the code point it maps to, itself when casing
    /// leaves it, or <see cref="Longer"/> when its mapping is longer than one code point.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=728DC2
    // Broiler-Human:        PENDING
    private static int[] Latin1(CaseTables lookup)
    {
        var mapped = new int[0x100];

        for (var codePoint = 0; codePoint < mapped.Length; codePoint++)
        {
            mapped[codePoint] = !lookup.TryGetMapping(codePoint, out var mapping) ? codePoint
                : mapping.Length == 1 ? mapping[0]
                : Longer;
        }

        return mapped;
    }

    /// <summary>
    /// Appends <paramref name="codePoint"/> to <paramref name="output"/> at <paramref name="written"/>,
    /// moving the output to a larger buffer, of at most <paramref name="bound"/> code units, when it is full.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=15C8B9
    // Broiler-Human:        PENDING
    private static void AppendCodePoint(ref Span<char> output, ref int written, int codePoint, long bound)
    {
        if (codePoint <= 0xFFFF && written < output.Length)
        {
            output[written++] = (char)codePoint;
            return;
        }

        AppendSlowly(ref output, ref written, codePoint, bound);
    }

    /// <summary><see cref="AppendCodePoint"/> for a supplementary code point or a full output, kept apart so the common case inlines.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=3783DD
    // Broiler-Human:        PENDING
    private static void AppendSlowly(ref Span<char> output, ref int written, int codePoint, long bound)
    {
        if (written + 2 > output.Length)
        {
            var larger = new char[Math.Min(Math.Max(written + 2L, output.Length * 2L), Math.Max(bound, written + 2L))];
            output[..written].CopyTo(larger);
            output = larger;
        }

        if (codePoint <= 0xFFFF)
        {
            output[written++] = (char)codePoint;
            return;
        }

        output[written++] = (char)(0xD7C0 + (codePoint >> 10));
        output[written++] = (char)(0xDC00 | (codePoint & 0x3FF));
    }

    /// <summary>Whether <paramref name="codePoint"/>'s casing record carries <paramref name="flag"/>; no code point outside the code space does.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=C9E9B7
    // Broiler-Human:        PENDING
    private bool HasFlag(int codePoint, int flag) =>
        (uint)codePoint <= UnicodeAlgorithms.MaxCodePoint && (new CaseTables(tables, upper: false).Flags(codePoint) & flag) != 0;

    /// <summary>
    /// Whether <paramref name="text"/> holds a code point one of <paramref name="language"/>'s conditional
    /// lines in this direction can change. Without one the language pass would copy the text as it is.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=High; Resources=1; Fingerprint=86262E
    // Broiler-Falsified-If: a text holding a code point a conditional line of the language changes is answered false
    // Broiler-Human:        PENDING
    private static bool NeedsLanguagePass(string text, bool upper, CaseLanguage language) =>
        language == CaseLanguage.Turkic
            ? upper ? text.Contains('i', StringComparison.Ordinal) : text.AsSpan().IndexOfAny('I', 'İ') >= 0
            : upper ? text.Contains('̇', StringComparison.Ordinal) : text.AsSpan().IndexOfAny("IJĮÌÍĨ") >= 0;

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=High; Resources=5; Fingerprint=5ED48B
    // Broiler-Falsified-If: a result longer than maxLength is answered as Success, or a language pass is applied for the default language
    // Broiler-Human:        PENDING
    private UnicodeStatus Map(string text, bool upper, CaseLanguage language, int maxLength, UnicodeWorkCharge? charge, out string? result)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        if ((uint)language > (uint)CaseLanguage.Lithuanian)
        {
            throw new ArgumentOutOfRangeException(nameof(language));
        }

        if (language == CaseLanguage.Default || !NeedsLanguagePass(text, upper, language))
        {
            return DefaultPass(text, text, upper, maxLength, charge, out result);
        }

        // A Lithuanian line writes at most three code units for one; a Turkic line one for one.
        var capacity = language == CaseLanguage.Lithuanian ? text.Length * 3L : text.Length;
        Span<char> staged = capacity <= StackLimit ? stackalloc char[StackLimit] : new char[capacity];
        var length = LanguagePass(text, upper, language, staged);

        return DefaultPass(staged[..length], null, upper, maxLength, charge, out result);
    }

    /// <summary>
    /// The Default Case Conversion over <paramref name="source"/>, which is <paramref name="text"/> when
    /// no language pass ran. Nothing is written before the first code point casing changes, so an
    /// unchanged text costs one pass, allocates nothing and is answered as itself.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=High; Resources=5; Fingerprint=D54FA1
    // Broiler-Falsified-If: work is done before the meter is charged for it, or a charge differs from Broiler.VM row C13
    // Broiler-Human:        PENDING
    private UnicodeStatus DefaultPass(ReadOnlySpan<char> source, string? text, bool upper, int maxLength, UnicodeWorkCharge? charge, out string? result)
    {
        Charge(charge, source.Length);

        var latin1 = upper ? upperLatin1 : lowerLatin1;
        var lookup = new CaseTables(tables, upper);
        var first = 0;

        // When the tables map ASCII as its 26 letters, the ASCII prefix is searched for a letter at once.
        if (asciiLetters)
        {
            var ascii = source.IndexOfAnyExceptInRange('\0', '\u007F');
            var prefix = ascii < 0 ? source : source[..ascii];
            var letter = upper ? prefix.IndexOfAnyInRange('a', 'z') : prefix.IndexOfAnyInRange('A', 'Z');

            first = letter >= 0 ? letter : prefix.Length;
        }

        while (first < source.Length)
        {
            var unit = source[first];

            if (unit < 0x100)
            {
                if (latin1[unit] != unit)
                {
                    break;
                }

                first++;
                continue;
            }

            var codePoint = Utf16Reader.Next(source, first, out var width);

            if ((!upper && codePoint == CapitalSigma) || lookup.Mapping(codePoint, out _) != 0)
            {
                break;
            }

            first += width;
        }

        if (first == source.Length)
        {
            result = source.Length > maxLength ? null : text ?? new string(source);
            return result is null ? UnicodeStatus.LimitExceeded : UnicodeStatus.Success;
        }

        return MapFrom(source, first, upper, latin1, lookup, asciiLetters && charge is null, maxLength, charge, out result);
    }

    /// <summary>
    /// The Default Case Conversion from <paramref name="first"/>, the first code point casing changes,
    /// after the text before it copied as it is. U+0000 to U+00FF are mapped through the values read at
    /// construction; the result is refused as soon as it is longer than <paramref name="maxLength"/>.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=High; Resources=5; Fingerprint=C5FF31
    // Broiler-Falsified-If: work is done before the meter is charged for it, a charge differs from Broiler.VM row C13, or a result longer than maxLength is answered as Success
    // Broiler-Human:        PENDING
    private UnicodeStatus MapFrom(
        ReadOnlySpan<char> source, int first, bool upper, int[] latin1, in CaseTables lookup, bool asciiRuns,
        int maxLength, UnicodeWorkCharge? charge, out string? result)
    {
        // The result is refused once it passes the limit, and one mapping writes at most six code
        // units, so past the unchanged text it copies first the output never needs more than the limit
        // and eight.
        var bound = Math.Max(maxLength, first) + 8L;
        var capacity = Math.Min(source.Length + 8L, bound);
        Span<char> output = capacity <= StackLimit ? stackalloc char[(int)capacity] : new char[capacity];
        var written = first;

        source[..first].CopyTo(output);

        for (var at = first; at < source.Length;)
        {
            var start = at;
            var unit = source[at];

            // Without a meter there is no charge to make per letter: a run of ASCII is mapped at once, and
            // refused before it is written when it would pass the limit, as the run letter by letter would.
            if (asciiRuns && unit < 0x80)
            {
                var run = source[at..].IndexOfAnyExceptInRange('\0', '\u007F');
                var length = run < 0 ? source.Length - at : run;

                if (written + (long)length > maxLength)
                {
                    result = null;
                    return UnicodeStatus.LimitExceeded;
                }

                if (written + length > output.Length)
                {
                    var larger = new char[Math.Max(written + length + 8L, Math.Min(output.Length * 2L, bound))];
                    output[..written].CopyTo(larger);
                    output = larger;
                }

                if (upper)
                {
                    Ascii.ToUpper(source.Slice(at, length), output[written..], out _);
                }
                else
                {
                    Ascii.ToLower(source.Slice(at, length), output[written..], out _);
                }

                at += length;
                written += length;
                continue;
            }

            var mapped = unit < 0x100 ? latin1[unit] : Longer;

            if (mapped == unit)
            {
                at++;
                AppendCodePoint(ref output, ref written, unit, bound);
            }
            else if (mapped != Longer)
            {
                at++;
                Charge(charge, 1);
                AppendCodePoint(ref output, ref written, mapped, bound);
            }
            else
            {
                // A code unit outside the surrogates is its own code point; only a surrogate is decoded.
                var codePoint = char.IsSurrogate(unit) ? Utf16Reader.Next(source, at, out _) : unit;
                var width = Utf16Reader.Width(codePoint);
                at += width;

                if (!upper && codePoint == CapitalSigma)
                {
                    var final = IsFinalSigma(source, start, width, out var scanned);
                    Charge(charge, scanned);
                    AppendCodePoint(ref output, ref written, final ? FinalSigma : SmallSigma, bound);
                }
                else if (lookup.Mapping(codePoint, out var value) is var length && length == 1)
                {
                    Charge(charge, 1);
                    AppendCodePoint(ref output, ref written, value, bound);
                }
                else if (length > 1)
                {
                    Charge(charge, length);
                    var mapping = lookup.Pooled(value, length);

                    for (var position = 0; position < length; position++)
                    {
                        AppendCodePoint(ref output, ref written, mapping[position], bound);
                    }
                }
                else
                {
                    AppendCodePoint(ref output, ref written, codePoint, bound);
                }
            }

            if (written > maxLength)
            {
                result = null;
                return UnicodeStatus.LimitExceeded;
            }
        }

        result = new string(output[..written]);
        return UnicodeStatus.Success;
    }

    /// <summary>
    /// <c>SpecialCasing.txt</c>'s conditional lines for one language, applied before the Default Case
    /// Conversion, written into <paramref name="output"/>; answers the length written. Each condition
    /// reads the string as it was, and each scan stops at the first code point of combining class 0 or
    /// 230, so the passes over the string are linear in all.
    /// </summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=D46111
    // Broiler-Human:        PENDING
    private int LanguagePass(string text, bool upper, CaseLanguage language, Span<char> output)
    {
        var written = 0;

        for (var at = 0; at < text.Length;)
        {
            var start = at;
            var point = Utf16Reader.Next(text, at, out var width);
            at += width;

            if (language == CaseLanguage.Turkic)
            {
                if (!upper && point == 0x0130)
                {
                    output[written++] = 'i';
                    continue;
                }

                if (!upper && point == CombiningDotAbove && AfterCapitalI(text, start))
                {
                    continue;
                }

                if (!upper && point == 'I')
                {
                    output[written++] = BeforeDot(text, at) ? 'i' : 'ı';
                    continue;
                }

                if (upper && point == 'i')
                {
                    output[written++] = 'İ';
                    continue;
                }
            }
            else
            {
                if (!upper && point is 'I' or 'J' or 0x012E && MoreAbove(text, at))
                {
                    output[written++] = point == 0x012E ? 'į' : (char)(point + 0x20);
                    output[written++] = '̇';
                    continue;
                }

                if (!upper && point is 0x00CC or 0x00CD or 0x0128)
                {
                    output[written++] = 'i';
                    output[written++] = '̇';
                    output[written++] = point switch { 0x00CC => '̀', 0x00CD => '́', _ => '̃' };
                    continue;
                }

                if (upper && point == CombiningDotAbove && AfterSoftDotted(text, start))
                {
                    continue;
                }
            }

            text.AsSpan(start, width).CopyTo(output[written..]);
            written += width;
        }

        return written;
    }

    /// <summary>After_I: an upper-case I before <paramref name="end"/>, with no combining class 0 or 230 between.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=85D79D
    // Broiler-Human:        PENDING
    private bool AfterCapitalI(string text, int end)
    {
        for (var at = end; at > 0;)
        {
            var point = Utf16Reader.Previous(text, at, out var width);
            at -= width;

            if (point == 'I')
            {
                return true;
            }

            if (normalization.CombiningClass(point) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>After_Soft_Dotted: a Soft_Dotted code point before <paramref name="end"/>, with no combining class 0 or 230 between.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=A2D110
    // Broiler-Human:        PENDING
    private bool AfterSoftDotted(string text, int end)
    {
        for (var at = end; at > 0;)
        {
            var point = Utf16Reader.Previous(text, at, out var width);
            at -= width;

            if (IsSoftDotted(point))
            {
                return true;
            }

            if (normalization.CombiningClass(point) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>Before_Dot: U+0307 from <paramref name="start"/> on, with only combining classes other than 0 and 230 between.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=EFADCB
    // Broiler-Human:        PENDING
    private bool BeforeDot(string text, int start)
    {
        for (var at = start; at < text.Length;)
        {
            var point = Utf16Reader.Next(text, at, out var width);
            at += width;

            if (point == CombiningDotAbove)
            {
                return true;
            }

            if (normalization.CombiningClass(point) is 0 or 230)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>More_Above: a combining class 230 from <paramref name="start"/> on, with no combining class 0 between.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=898E58
    // Broiler-Human:        PENDING
    private bool MoreAbove(string text, int start)
    {
        for (var at = start; at < text.Length;)
        {
            var combining = normalization.CombiningClass(Utf16Reader.Next(text, at, out var width));
            at += width;

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
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=5F6704
    // Broiler-Human:        PENDING
    private bool IsFinalSigma(ReadOnlySpan<char> text, int at, int width, out int scanned)
    {
        var lookup = new CaseTables(tables, upper: false);
        scanned = 0;
        var before = false;

        for (var position = at; position > 0;)
        {
            var codePoint = Utf16Reader.Previous(text, position, out var size);
            position -= size;
            scanned += size;

            var flags = lookup.Flags(codePoint);

            if ((flags & CaseTables.CaseIgnorable) != 0)
            {
                continue;
            }

            before = (flags & CaseTables.Cased) != 0;
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

            var flags = lookup.Flags(codePoint);

            if ((flags & CaseTables.CaseIgnorable) != 0)
            {
                continue;
            }

            return (flags & CaseTables.Cased) == 0;
        }

        return true;
    }

    /// <summary>
    /// The casing tables one call reads, fetched once: each code point is one trie lookup to its record,
    /// which holds its flags and both of its mappings.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=1; Fingerprint=5C1A90
    // Broiler-Human:        PENDING
    private readonly ref struct CaseTables
    {
        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=BC67F4
        // Broiler-Human:        PENDING
        internal const int RecordSize = 9;

        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=D51C0D
        // Broiler-Human:        PENDING
        internal const int Cased = 1;

        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=EE7A55
        // Broiler-Human:        PENDING
        internal const int CaseIgnorable = 2;

        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Low; Resources=0; Fingerprint=DAA084
        // Broiler-Human:        PENDING
        internal const int SoftDotted = 4;

        private readonly ReadOnlySpan<ushort> trie;

        private readonly ReadOnlySpan<byte> records;

        private readonly ReadOnlySpan<byte> pool;

        private readonly int column;

        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=231035
        // Broiler-Human:        PENDING
        internal CaseTables(IUnicodeTables tables, bool upper)
        {
            trie = TableReader.Words(tables.Table(UnicodeTable.CasingTrie));
            records = tables.Table(UnicodeTable.CasingRecords);
            pool = tables.Table(UnicodeTable.MappingPool);
            column = upper ? 1 : 5;
        }

        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=6274A2
        // Broiler-Human:        PENDING
        internal int Flags(int codePoint) => records[TableReader.Trie(trie, codePoint) * RecordSize];

        /// <summary>
        /// The length of <paramref name="codePoint"/>'s mapping in this direction, 0 when casing leaves
        /// it, and the code point it maps to when the length is 1 or the mapping's pool offset otherwise.
        /// </summary>
        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=37CEE2
        // Broiler-Human:        PENDING
        internal int Mapping(int codePoint, out int value)
        {
            var record = (TableReader.Trie(trie, codePoint) * RecordSize) + column;

            value = TableReader.Int24(records, record + 1);
            return records[record];
        }

        /// <summary>The code points of a mapping longer than one, by its pool offset and length.</summary>
        // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=EE757B
        // Broiler-Human:        PENDING
        internal CodePoints Pooled(int offset, int length) => new(pool.Slice(offset * 3, length * 3));

        /// <summary>The full mapping of <paramref name="codePoint"/> in this direction, when casing changes it.</summary>
        // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.1.3.29 and Unicode 3.13 via Broiler.VM JsRealm.String and JsUnicodeCasing; IP=Low; Security=Medium; Resources=1; Fingerprint=5E6B39
        // Broiler-Human:        PENDING
        internal bool TryGetMapping(int codePoint, out CodePoints mapping)
        {
            var length = Mapping(codePoint, out var value);

            mapping = length switch
            {
                0 => default,
                1 => new CodePoints(value, 0, 0, 1),
                _ => Pooled(value, length),
            };

            return length != 0;
        }
    }
}
