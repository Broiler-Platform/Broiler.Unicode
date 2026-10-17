// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           46
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       31
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Globalization;

namespace Broiler.Unicode.Algorithms;

/// <summary>
/// An immutable set of code points from the tables: a view of one set's ranges, which ascend, neither
/// overlap nor touch, and have inclusive bounds.
/// </summary>
/// <remarks>
/// It holds the tables it reads, where its ranges start and how many there are, and allocates nothing:
/// a membership test is a binary search over the generated range list (ECMA-262 CharSet, read as a
/// list of ranges). The default value is the empty set.
/// </remarks>
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Low; Resources=1; Fingerprint=A9252C
// Broiler-Human:        PENDING
public readonly struct CodePointSet
{
    private readonly UnicodeProperties? owner;

    private readonly int first;

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=650BF2
    // Broiler-Human:        PENDING
    internal CodePointSet(UnicodeProperties owner, int first, int count)
    {
        this.owner = owner;
        this.first = first;
        RangeCount = count;
    }

    /// <summary>How many ranges the set has; zero for the empty set.</summary>
    public int RangeCount { get; }

    /// <summary>Whether the set has no code point.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=6C4DE1
    // Broiler-Human:        PENDING
    public bool IsEmpty => RangeCount == 0;

    /// <summary>The first code point of range <paramref name="index"/>.</summary>
    /// <param name="index">A range of the set, from zero to <see cref="RangeCount"/> less one.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=F424FA
    // Broiler-Human:        PENDING
    public int First(int index) => TableReader.Int24(Ranges, Checked(index) * 6);

    /// <summary>The last code point of range <paramref name="index"/>, inclusive.</summary>
    /// <param name="index">A range of the set, from zero to <see cref="RangeCount"/> less one.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=7DC5A1
    // Broiler-Human:        PENDING
    public int Last(int index) => TableReader.Int24(Ranges, (Checked(index) * 6) + 3);

    /// <summary>Whether <paramref name="codePoint"/> is in the set: a binary search, no allocation.</summary>
    /// <param name="codePoint">A code point; a value outside U+0000 to U+10FFFF is in no set.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=017C70
    // Broiler-Human:        PENDING
    public bool Contains(int codePoint) =>
        RangeCount != 0 && TableReader.InRanges(Ranges, first, RangeCount, codePoint);

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=511034
    // Broiler-Human:        PENDING
    private ReadOnlySpan<byte> Ranges => owner is null ? default : owner.Ranges;

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=ED18DF
    // Broiler-Human:        PENDING
    private int Checked(int index) =>
        (uint)index < (uint)RangeCount ? first + index : throw new ArgumentOutOfRangeException(nameof(index));
}

/// <summary>The thirty single General_Category values, in the ordinal order of their short names.</summary>
/// <remarks>The order is the tables' set-id order (Unicode Standard section 4.5, <c>PropertyValueAliases.txt</c>).</remarks>
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Low; Resources=0; Fingerprint=885BE9
// Broiler-Human:        PENDING
public enum GeneralCategory
{
    /// <summary>Cc, Control.</summary>
    Control = 0,

    /// <summary>Cf, Format.</summary>
    Format,

    /// <summary>Cn, Unassigned.</summary>
    Unassigned,

    /// <summary>Co, Private_Use.</summary>
    PrivateUse,

    /// <summary>Cs, Surrogate.</summary>
    Surrogate,

    /// <summary>Ll, Lowercase_Letter.</summary>
    LowercaseLetter,

    /// <summary>Lm, Modifier_Letter.</summary>
    ModifierLetter,

    /// <summary>Lo, Other_Letter.</summary>
    OtherLetter,

    /// <summary>Lt, Titlecase_Letter.</summary>
    TitlecaseLetter,

    /// <summary>Lu, Uppercase_Letter.</summary>
    UppercaseLetter,

    /// <summary>Mc, Spacing_Mark.</summary>
    SpacingMark,

    /// <summary>Me, Enclosing_Mark.</summary>
    EnclosingMark,

    /// <summary>Mn, Nonspacing_Mark.</summary>
    NonspacingMark,

    /// <summary>Nd, Decimal_Number.</summary>
    DecimalNumber,

    /// <summary>Nl, Letter_Number.</summary>
    LetterNumber,

    /// <summary>No, Other_Number.</summary>
    OtherNumber,

    /// <summary>Pc, Connector_Punctuation.</summary>
    ConnectorPunctuation,

    /// <summary>Pd, Dash_Punctuation.</summary>
    DashPunctuation,

    /// <summary>Pe, Close_Punctuation.</summary>
    ClosePunctuation,

    /// <summary>Pf, Final_Punctuation.</summary>
    FinalPunctuation,

    /// <summary>Pi, Initial_Punctuation.</summary>
    InitialPunctuation,

    /// <summary>Po, Other_Punctuation.</summary>
    OtherPunctuation,

    /// <summary>Ps, Open_Punctuation.</summary>
    OpenPunctuation,

    /// <summary>Sc, Currency_Symbol.</summary>
    CurrencySymbol,

    /// <summary>Sk, Modifier_Symbol.</summary>
    ModifierSymbol,

    /// <summary>Sm, Math_Symbol.</summary>
    MathSymbol,

    /// <summary>So, Other_Symbol.</summary>
    OtherSymbol,

    /// <summary>Zl, Line_Separator.</summary>
    LineSeparator,

    /// <summary>Zp, Paragraph_Separator.</summary>
    ParagraphSeparator,

    /// <summary>Zs, Space_Separator.</summary>
    SpaceSeparator,
}

/// <summary>
/// The Unicode properties ECMA-262's property escapes admit, by the exact names its tables admit, and
/// the code-point lookups both engines classify by: General_Category, ID_Start, ID_Continue,
/// White_Space and Soft_Dotted.
/// </summary>
/// <remarks>
/// <para>
/// <b>Names are matched exactly</b>, as ECMA-262's UnicodeMatchProperty and UnicodeMatchPropertyValue
/// require (section 22.2.2.9, "Static Semantics: UnicodeMatchProperty"): no case folding, no ignoring
/// of <c>_</c>, <c>-</c> or spaces, no <c>Is</c> prefix, and no name or alias outside the tables
/// "Non-binary Unicode property aliases and their canonical property names", "Binary Unicode property
/// aliases and their canonical property names" and <c>PropertyValueAliases.txt</c>. The comparison is
/// ordinal over ASCII, so a name with any other character matches nothing. An unknown name is a false
/// answer, never an exception.
/// </para>
/// <para>
/// <b>What resolves.</b> The lone form takes a General_Category value, group or alias, or one of the
/// binary properties or their aliases, <c>Any</c>, <c>ASCII</c> and <c>Assigned</c> among them. The
/// <c>name=value</c> form takes <c>General_Category</c>/<c>gc</c> with a General_Category value, and
/// <c>Script</c>/<c>sc</c> or <c>Script_Extensions</c>/<c>scx</c> with any Script value or alias, which
/// includes <c>Katakana_Or_Hiragana</c> (an empty set). A binary property is not a <c>name</c> of the
/// <c>name=value</c> form. The properties of strings are <see cref="UnicodeStringProperties"/>'.
/// </para>
/// <para>
/// <b>Immutable and allocation-free.</b> Every result is a <see cref="CodePointSet"/> over the tables.
/// An instance holds nothing but the provider and the layout it read when it was made, and is safe to
/// share between threads. It takes no meter: its work is a binary search bounded by the tables, and a
/// name longer than the longest the tables hold is refused before it is compared.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Low; Resources=1; Fingerprint=62B18C
// Broiler-Human:        PENDING
public sealed class UnicodeProperties
{
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Low; Resources=0; Fingerprint=05AEBB
    // Broiler-Human:        PENDING
    private const int LayoutLength = 11;

    /// <summary>The two-letter short names of <see cref="GeneralCategory"/>'s values, in its order.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Low; Resources=0; Fingerprint=37AC41
    // Broiler-Human:        PENDING
    private const string CategoryShortNames = "CcCfCnCoCsLlLmLoLtLuMcMeMnNdNlNoPcPdPePfPiPoPsScSkSmSoZlZpZs";

    private readonly IUnicodeTables tables;

    private readonly int generalCategorySetCount;

    private readonly int scriptSetBase;

    private readonly int scriptExtensionsSetBase;

    private readonly int scriptCount;

    private readonly int longestName;

    /// <summary>The properties over <paramref name="tables"/>.</summary>
    /// <param name="tables">
    /// A provider of Unicode <see cref="UnicodeAlgorithms.UnicodeVersion"/> carrying the property
    /// tables, <see cref="UnicodeTable.PropertyLayout"/> to <see cref="UnicodeTable.ScriptNameIndex"/>.
    /// </param>
    /// <exception cref="ArgumentException">The provider is of another version or lacks a table.</exception>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=56957A
    // Broiler-Human:        PENDING
    public UnicodeProperties(IUnicodeTables tables)
    {
        this.tables = TableReader.Checked(tables);

        TableReader.Require(tables, UnicodeTable.PropertyLayout, 3);
        TableReader.Require(tables, UnicodeTable.PropertyRanges, 6);
        TableReader.Require(tables, UnicodeTable.PropertySets, 6);
        TableReader.Require(tables, UnicodeTable.PropertyLoneNames, 1);
        TableReader.Require(tables, UnicodeTable.PropertyLoneNameIndex, 2);
        TableReader.Require(tables, UnicodeTable.PropertyNames, 1);
        TableReader.Require(tables, UnicodeTable.PropertyNameIndex, 2);
        TableReader.Require(tables, UnicodeTable.ScriptNames, 1);
        TableReader.Require(tables, UnicodeTable.ScriptNameIndex, 2);

        var layout = tables.Table(UnicodeTable.PropertyLayout);

        if (layout.Length != LayoutLength * 3)
        {
            throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"PropertyLayout holds {layout.Length / 3} values, not {LayoutLength}."), nameof(tables));
        }

        generalCategorySetCount = TableReader.Layout(layout, 0);
        scriptSetBase = TableReader.Layout(layout, 3);
        scriptExtensionsSetBase = TableReader.Layout(layout, 4);
        scriptCount = TableReader.Layout(layout, 5);

        if (TableReader.Layout(layout, 1) != 30 || scriptExtensionsSetBase + scriptCount != SetCount)
        {
            throw new ArgumentException("PropertyLayout does not describe PropertySets: 30 General_Category values and a Script_Extensions set per script.", nameof(tables));
        }

        for (var category = 0; category < 30; category++)
        {
            var shortName = CategoryShortNames.AsSpan(category * 2, 2);

            if (TableReader.Find(tables.Table(UnicodeTable.PropertyLoneNames), tables.Table(UnicodeTable.PropertyLoneNameIndex), shortName) != category)
            {
                throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"PropertyLoneNames does not give General_Category {shortName} set {category}."), nameof(tables));
            }
        }

        IdStart = Set(TableReader.Layout(layout, 6));
        IdContinue = Set(TableReader.Layout(layout, 7));
        SpaceSeparator = Set(TableReader.Layout(layout, 8));
        WhiteSpace = Set(TableReader.Layout(layout, 9));
        SoftDotted = Set(TableReader.Layout(layout, 10));
        longestName = Math.Max(Longest(UnicodeTable.PropertyLoneNames, UnicodeTable.PropertyLoneNameIndex),
            Math.Max(Longest(UnicodeTable.PropertyNames, UnicodeTable.PropertyNameIndex), Longest(UnicodeTable.ScriptNames, UnicodeTable.ScriptNameIndex)));
    }

    /// <summary>ID_Start (<c>DerivedCoreProperties.txt</c>), which subtracts Pattern_Syntax and Pattern_White_Space as UAX #31 defines it.</summary>
    public CodePointSet IdStart { get; }

    /// <summary>ID_Continue (<c>DerivedCoreProperties.txt</c>), under the same subtraction.</summary>
    public CodePointSet IdContinue { get; }

    /// <summary>General_Category=Space_Separator, ECMA-262's USP (section 12.2, "White Space").</summary>
    public CodePointSet SpaceSeparator { get; }

    /// <summary>White_Space (<c>PropList.txt</c>), the binary property; ECMA-262's WhiteSpace production is <see cref="EcmaScriptLexical.IsWhiteSpace"/>.</summary>
    public CodePointSet WhiteSpace { get; }

    /// <summary>Soft_Dotted (<c>PropList.txt</c>).</summary>
    public CodePointSet SoftDotted { get; }

    /// <summary>The number of sets the tables hold.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=669363
    // Broiler-Human:        PENDING
    internal int SetCount => tables.Table(UnicodeTable.PropertySets).Length / 6;

    /// <summary>The provider the properties read.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Low; Resources=0; Fingerprint=F71F26
    // Broiler-Human:        PENDING
    internal IUnicodeTables Tables => tables;

    /// <summary>The range list every set reads.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=1DF51F
    // Broiler-Human:        PENDING
    internal ReadOnlySpan<byte> Ranges => tables.Table(UnicodeTable.PropertyRanges);

    /// <summary>
    /// The set a lone <c>\p{name}</c> selects: a General_Category value or group, or a binary property,
    /// by any name or alias ECMA-262 admits for it (section 22.2.2.9, UnicodeMatchPropertyValue and
    /// UnicodeMatchProperty).
    /// </summary>
    /// <param name="name">The name, exactly as written.</param>
    /// <param name="set">The set, or the empty set when the name is not admitted.</param>
    /// <returns>Whether the name is admitted.</returns>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=7105E1
    // Broiler-Human:        PENDING
    public bool TryResolve(ReadOnlySpan<char> name, out CodePointSet set)
    {
        var id = name.Length > longestName
            ? -1
            : TableReader.Find(tables.Table(UnicodeTable.PropertyLoneNames), tables.Table(UnicodeTable.PropertyLoneNameIndex), name);

        set = id < 0 ? default : Set(id);
        return id >= 0;
    }

    /// <summary>
    /// The set <c>\p{property=value}</c> selects, for the three non-binary properties ECMA-262 admits:
    /// General_Category, Script and Script_Extensions (section 22.2.2.9). A binary property is not a
    /// <paramref name="property"/> here: <c>\p{ASCII=Y}</c> is not admitted.
    /// </summary>
    /// <param name="property">The property's name, exactly as written.</param>
    /// <param name="value">The value's name, exactly as written.</param>
    /// <param name="set">The set, or the empty set when the pair is not admitted.</param>
    /// <returns>Whether the pair is admitted.</returns>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=AEA1A5
    // Broiler-Human:        PENDING
    public bool TryResolve(ReadOnlySpan<char> property, ReadOnlySpan<char> value, out CodePointSet set)
    {
        set = default;

        if (property.Length > longestName || value.Length > longestName)
        {
            return false;
        }

        var kind = TableReader.Find(tables.Table(UnicodeTable.PropertyNames), tables.Table(UnicodeTable.PropertyNameIndex), property);
        var id = kind switch
        {
            0 => TableReader.Find(tables.Table(UnicodeTable.PropertyLoneNames), tables.Table(UnicodeTable.PropertyLoneNameIndex), value) is var category
                && category >= 0 && category < generalCategorySetCount ? category : -1,
            1 => TableReader.Find(tables.Table(UnicodeTable.ScriptNames), tables.Table(UnicodeTable.ScriptNameIndex), value) is var script
                && script >= 0 ? scriptSetBase + script : -1,
            2 => TableReader.Find(tables.Table(UnicodeTable.ScriptNames), tables.Table(UnicodeTable.ScriptNameIndex), value) is var script
                && script >= 0 ? scriptExtensionsSetBase + script : -1,
            _ => -1,
        };

        if (id < 0)
        {
            return false;
        }

        set = Set(id);
        return true;
    }

    /// <summary>The set of one single General_Category value.</summary>
    /// <param name="category">The value.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=5EC5B3
    // Broiler-Human:        PENDING
    public CodePointSet Category(GeneralCategory category) =>
        (uint)category < 30 ? Set((int)category) : throw new ArgumentOutOfRangeException(nameof(category));

    /// <summary>
    /// The General_Category of <paramref name="codePoint"/> (<c>DerivedGeneralCategory.txt</c>); a code
    /// point the tables list in no category, and a value outside the code space, is Unassigned.
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=2FF96C
    // Broiler-Human:        PENDING
    public GeneralCategory GeneralCategoryOf(int codePoint)
    {
        for (var category = 0; category < 30; category++)
        {
            if (Set(category).Contains(codePoint))
            {
                return (GeneralCategory)category;
            }
        }

        return GeneralCategory.Unassigned;
    }

    /// <summary>The set with generated id <paramref name="id"/>.</summary>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=F4B126
    // Broiler-Human:        PENDING
    internal CodePointSet Set(int id)
    {
        var sets = tables.Table(UnicodeTable.PropertySets);

        if ((uint)id >= (uint)(sets.Length / 6))
        {
            throw new ArgumentException(string.Create(NumberFormatInfo.InvariantInfo, $"PropertyLayout names set {id}, and PropertySets holds {sets.Length / 6}."));
        }

        return new CodePointSet(this, TableReader.Int24(sets, id * 6), TableReader.Int24(sets, (id * 6) + 3));
    }

    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 22.2.2.9 via Broiler.VM JsUnicodeProperties; IP=Low; Security=Medium; Resources=1; Fingerprint=F34FD9
    // Broiler-Human:        PENDING
    private int Longest(UnicodeTable names, UnicodeTable index)
    {
        var data = tables.Table(names);
        var offsets = tables.Table(index);
        var longest = 0;

        for (var at = 0; at < offsets.Length; at += 2)
        {
            longest = Math.Max(longest, data[TableReader.UInt16(offsets, at)]);
        }

        return longest;
    }
}

/// <summary>
/// The code point classes of ECMA-262's lexical grammar that need the Unicode Character Database:
/// IdentifierStartChar, IdentifierPartChar and WhiteSpace (sections 12.2 and 12.7), which an
/// engine's scanner, its <c>RegExp</c> group names and <c>RegExp.escape</c> read.
/// </summary>
/// <remarks>
/// <para>
/// <b>The language's sets, not a platform's.</b> ID_Start and ID_Continue come from
/// <c>DerivedCoreProperties.txt</c>, whose UAX #31 derivation subtracts Pattern_Syntax, so U+2E2F is not
/// an identifier character. WhiteSpace is TAB, VT, FF, ZWNBSP and General_Category=Space_Separator: it
/// does not include U+0085, which a platform's white-space test does, nor the line terminators.
/// </para>
/// <para>
/// ASCII is answered by comparison; anything else is one lookup in <see cref="UnicodeTable.LexicalTrie"/>.
/// Nothing allocates.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Ported; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Low; Resources=1; Fingerprint=1D1E80
// Broiler-Human:        PENDING
public sealed class EcmaScriptLexical
{
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Low; Resources=0; Fingerprint=F43A1C
    // Broiler-Human:        PENDING
    private const int IdStartBit = 1;

    // Broiler-AI:           Origin=AI; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Low; Resources=0; Fingerprint=DEB5ED
    // Broiler-Human:        PENDING
    private const int IdContinueBit = 2;

    // Broiler-AI:           Origin=AI; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Low; Resources=0; Fingerprint=A3C764
    // Broiler-Human:        PENDING
    private const int SpaceSeparatorBit = 4;

    private readonly IUnicodeTables tables;

    /// <summary>The lexical classes over <paramref name="properties"/>.</summary>
    /// <param name="properties">The property lookups, over a provider that also carries <see cref="UnicodeTable.LexicalTrie"/>.</param>
    /// <exception cref="ArgumentException">The provider lacks <see cref="UnicodeTable.LexicalTrie"/> or holds a malformed one.</exception>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Medium; Resources=1; Fingerprint=63442F
    // Broiler-Human:        PENDING
    public EcmaScriptLexical(UnicodeProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        tables = properties.Tables;
        TableReader.RequireTrie(tables, UnicodeTable.LexicalTrie);
    }

    /// <summary>
    /// Whether <paramref name="codePoint"/> is an IdentifierStartChar: ID_Start, <c>$</c> or <c>_</c>
    /// (ECMA-262 section 12.7, "Names and Keywords"). A surrogate code point is not one.
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Medium; Resources=1; Fingerprint=DDDD07
    // Broiler-Human:        PENDING
    public bool IsIdentifierStart(int codePoint) =>
        codePoint < 0x80
            ? (uint)((codePoint | 0x20) - 'a') <= 'z' - 'a' || codePoint is '$' or '_'
            : HasBit(codePoint, IdStartBit);

    /// <summary>
    /// Whether <paramref name="codePoint"/> is an IdentifierPartChar: ID_Continue, <c>$</c>, ZWNJ or ZWJ
    /// (ECMA-262 section 12.7). <c>_</c> and the decimal digits are in ID_Continue itself.
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Medium; Resources=1; Fingerprint=6F38C8
    // Broiler-Human:        PENDING
    public bool IsIdentifierPart(int codePoint) =>
        codePoint < 0x80
            ? (uint)((codePoint | 0x20) - 'a') <= 'z' - 'a' || (uint)(codePoint - '0') <= 9 || codePoint is '$' or '_'
            : codePoint is 0x200C or 0x200D || HasBit(codePoint, IdContinueBit);

    /// <summary>
    /// Whether <paramref name="codePoint"/> is WhiteSpace: U+0009, U+000B, U+000C, U+FEFF or a
    /// Space_Separator (ECMA-262 section 12.2, "White Space"). Line terminators and U+0085 are not.
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Medium; Resources=1; Fingerprint=67C1A3
    // Broiler-Human:        PENDING
    public bool IsWhiteSpace(int codePoint) =>
        codePoint is 0x09 or 0x0B or 0x0C or 0x20 or 0xA0 or 0xFEFF ||
        (codePoint > 0xFF && HasBit(codePoint, SpaceSeparatorBit));

    /// <summary>
    /// Whether <paramref name="codePoint"/> is a LineTerminator: LF, CR, U+2028 or U+2029 (ECMA-262
    /// section 12.3, "Line Terminators").
    /// </summary>
    /// <param name="codePoint">A code point.</param>
    // Broiler-AI:           Origin=Ported; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Medium; Resources=1; Fingerprint=3081D2
    // Broiler-Human:        PENDING
    public static bool IsLineTerminator(int codePoint) => codePoint is 0x0A or 0x0D or 0x2028 or 0x2029;

    /// <summary>Whether <paramref name="codePoint"/>'s lexical value carries <paramref name="bit"/>; no code point outside the code space does.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 12.2 and 12.7 via Broiler.VM JsUnicodeLexical; IP=Low; Security=Medium; Resources=1; Fingerprint=7490E3
    // Broiler-Human:        PENDING
    private bool HasBit(int codePoint, int bit) =>
        (uint)codePoint <= UnicodeAlgorithms.MaxCodePoint && (TableReader.Trie(TableReader.Words(tables.Table(UnicodeTable.LexicalTrie)), codePoint) & bit) != 0;
}
