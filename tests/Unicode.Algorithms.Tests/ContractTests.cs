using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Broiler.Unicode.Segmentation;
using Broiler.Unicode.Tables;
using Xunit;

namespace Broiler.Unicode.Algorithms.Tests;

/// <summary>
/// Bundle sjl-7-001's part C (exit clause 2): the contract of every package the engines consume. C-6, no
/// host data, is the architecture tests' SL4.
/// </summary>
public sealed class ContractTests
{
    private static readonly UnicodeNormalization Normalization = new(UnicodeTables.Instance);

    private static readonly UnicodeCaseMapping Casing = new(UnicodeTables.Instance);

    private static readonly UnicodeSegmenter Segmenter = new(UnicodeTables.Instance);

    private static readonly UnicodeProperties Properties = new(UnicodeTables.Instance);

    /// <summary>The packages the engines consume.</summary>
    private static readonly Assembly[] Consumed = [typeof(UnicodeProperties).Assembly, typeof(UnicodeSegmenter).Assembly];

    // ---------------------------------------------------------------------------------------------
    // C-1: exact names. A5 decides the lookup the engines call; this checks the loose one's warning.

    [Fact]
    public void C1_the_loose_lookup_carries_its_documented_warning()
    {
        var source = System.IO.File.ReadAllText(Archive.Full("src/UnicodeProperties/UnicodeProperties.cs"));

        Assert.Contains("loose", source, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Broiler.Unicode.Algorithms", source, StringComparison.Ordinal);
    }

    // ---------------------------------------------------------------------------------------------
    // C-2: immutable results.

    [Fact]
    public void C2_no_public_member_hands_out_a_mutable_reference()
    {
        var failures = new List<string>();

        foreach (var type in Consumed.SelectMany(static assembly => assembly.GetExportedTypes()).Where(static type => !type.IsEnum))
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (!field.IsLiteral && !field.IsInitOnly)
                {
                    failures.Add($"{type.Name}.{field.Name} is a writable public field");
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                // An init-only setter of a value type sets the caller's own copy, never shared data.
                if (property.SetMethod is { IsPublic: true } setter &&
                    !setter.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)))
                {
                    failures.Add($"{type.Name}.{property.Name} has a public setter");
                }

                if (Mutable(property.PropertyType))
                {
                    failures.Add($"{type.Name}.{property.Name} is a {property.PropertyType.Name}");
                }
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                // Boundaries answers a new array per call, the caller's own (below).
                if (Mutable(method.ReturnType) && !(type == typeof(UnicodeSegmenter) && method.Name == nameof(UnicodeSegmenter.Boundaries)))
                {
                    failures.Add($"{type.Name}.{method.Name} returns a {method.ReturnType.Name}");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join("\n", failures));
    }

    [Fact]
    public void C2_boundaries_are_a_new_array_per_call()
    {
        var first = Segmenter.Boundaries("a b", SegmentGranularity.Word, null);
        first[1] = 99;

        var second = Segmenter.Boundaries("a b", SegmentGranularity.Word, null);

        Assert.Equal([0, 1, 2, 3], second);
    }

    // ---------------------------------------------------------------------------------------------
    // C-3: a public version.

    [Fact]
    public void C3_every_package_states_the_pin_s_version()
    {
        Assert.Equal(Generation.UnicodePin.PinnedVersion, UnicodeAlgorithms.UnicodeVersion);
        Assert.Equal(UnicodeAlgorithms.UnicodeVersion, UnicodeSegmentation.UnicodeVersion);
        Assert.Equal(UnicodeAlgorithms.UnicodeVersion, UnicodeTables.Version);
        Assert.Equal(Archive.Pin.Version, UnicodeTables.Instance.UnicodeVersion);
    }

    [Fact]
    public void C3_tables_of_another_version_are_refused()
    {
        var other = new OtherVersion();

        Assert.Throws<ArgumentException>(() => new UnicodeProperties(other));
        Assert.Throws<ArgumentException>(() => new UnicodeNormalization(other));
        Assert.Throws<ArgumentException>(() => new UnicodeCaseMapping(other));
        Assert.Throws<ArgumentException>(() => new UnicodeCaseFolding(other));
        Assert.Throws<ArgumentException>(() => new UnicodeStringProperties(other));
        Assert.Throws<ArgumentException>(() => new UnicodeSegmenter(other));
    }

    // ---------------------------------------------------------------------------------------------
    // C-4: metering and limits.

    [Theory]
    [InlineData("abc", NormalForm.Nfc, new ulong[] { 3 })]
    [InlineData("", NormalForm.Nfd, new ulong[] { 1 })]
    [InlineData("é", NormalForm.Nfc, new ulong[] { 2, 8 })]
    [InlineData("é", NormalForm.Nfd, new ulong[] { 1, 7 })]
    [InlineData("ﷺ", NormalForm.Nfkd, new ulong[] { 1, 55 })]
    [InlineData("가", NormalForm.Nfd, new ulong[] { 1, 7 })]
    public void C4_normalization_charges_Broiler_VM_s_units(string text, NormalForm form, ulong[] expected)
    {
        var charges = new List<ulong>();

        Assert.Equal(UnicodeStatus.Success, Normalization.Transform(text, form, int.MaxValue, charges.Add, out _));
        Assert.Equal(expected, charges);
    }

    [Fact]
    public void C4_normalization_checks_its_limit_on_the_whole_result()
    {
        // A decomposed form is refused before the second charge when it does not fit.
        var charges = new List<ulong>();
        Assert.Equal(UnicodeStatus.LimitExceeded, Normalization.Transform("ﷺ", NormalForm.Nfkd, 17, charges.Add, out var refused));
        Assert.Null(refused);
        Assert.Equal([1UL], charges);
        Assert.Equal(UnicodeStatus.Success, Normalization.Transform("ﷺ", NormalForm.Nfkd, 18, null, out var fits));
        Assert.Equal(18, fits!.Length);

        // A composed result that fits is returned, though its decomposition does not (row N6).
        var decomposed = string.Concat(Enumerable.Repeat("é", 5));
        Assert.Equal(UnicodeStatus.Success, Normalization.Transform(decomposed, NormalForm.Nfc, 5, null, out var composed));
        Assert.Equal(new string('é', 5), composed);
        Assert.Equal(UnicodeStatus.LimitExceeded, Normalization.Transform(decomposed, NormalForm.Nfc, 4, null, out _));

        // A result of exactly the maximum is not refused, and an input already in the form longer than it is.
        Assert.Equal(UnicodeStatus.Success, Normalization.Transform("é", NormalForm.Nfd, 2, null, out _));
        Assert.Equal(UnicodeStatus.LimitExceeded, Normalization.Transform("é", NormalForm.Nfd, 1, null, out _));
        Assert.Equal(UnicodeStatus.LimitExceeded, Normalization.Transform("abc", NormalForm.Nfc, 2, null, out _));
    }

    [Theory]
    [InlineData("abc", true, CaseLanguage.Default, new ulong[] { 3, 1, 1, 1 })]
    [InlineData("123", true, CaseLanguage.Default, new ulong[] { 3 })]
    [InlineData("ß", true, CaseLanguage.Default, new ulong[] { 1, 2 })]
    [InlineData("AΣ", false, CaseLanguage.Default, new ulong[] { 2, 1, 1 })]
    [InlineData("Σ", false, CaseLanguage.Default, new ulong[] { 1, 1 })]
    [InlineData("I", false, CaseLanguage.Turkic, new ulong[] { 1 })]
    public void C4_casing_charges_Broiler_VM_s_units(string text, bool upper, CaseLanguage language, ulong[] expected)
    {
        var charges = new List<ulong>();
        var status = upper
            ? Casing.ToUpperCase(text, language, int.MaxValue, charges.Add, out _)
            : Casing.ToLowerCase(text, language, int.MaxValue, charges.Add, out _);

        Assert.Equal(UnicodeStatus.Success, status);
        Assert.Equal(expected, charges);
    }

    [Fact]
    public void C4_casing_checks_its_limit_on_the_whole_result()
    {
        Assert.Equal(UnicodeStatus.Success, Casing.ToUpperCase("ß", CaseLanguage.Default, 2, null, out var fits));
        Assert.Equal("SS", fits);
        Assert.Equal(UnicodeStatus.LimitExceeded, Casing.ToUpperCase("ß", CaseLanguage.Default, 1, null, out var refused));
        Assert.Null(refused);
        Assert.Equal(UnicodeStatus.LimitExceeded, Casing.ToUpperCase("abc", CaseLanguage.Default, 2, null, out _));
        Assert.Equal(UnicodeStatus.LimitExceeded, Casing.ToLowerCase("Ì", CaseLanguage.Lithuanian, 2, null, out _));
        Assert.Equal(UnicodeStatus.Success, Casing.ToLowerCase("Ì", CaseLanguage.Lithuanian, 3, null, out _));
    }

    [Fact]
    public void C4_an_unchanged_text_is_answered_as_itself()
    {
        var text = "plain 123";

        Assert.Equal(UnicodeStatus.Success, Casing.ToLowerCase(text, CaseLanguage.Default, int.MaxValue, null, out var lower));
        Assert.Same(text, lower);
        Assert.Equal(UnicodeStatus.Success, Normalization.Transform(text, NormalForm.Nfkc, int.MaxValue, null, out var normal));
        Assert.Same(text, normal);
    }

    [Fact]
    public void C4_segmentation_charges_every_code_point_it_scans()
    {
        var charges = new List<ulong>();

        Segmenter.Boundaries("ab", SegmentGranularity.Grapheme, charges.Add);
        Assert.Equal([2UL], charges);

        // Four regional indicators: the pass, then the GB12 walks of one, two and three code points.
        charges.Clear();
        Assert.Equal([0, 4, 8], Segmenter.Boundaries("\U0001F1E9\U0001F1EA\U0001F1EB\U0001F1F7", SegmentGranularity.Grapheme, charges.Add));
        Assert.Equal(4UL + 1 + 2 + 3, charges.Aggregate(0UL, static (sum, unit) => sum + unit));

        // The word status reads until it finds a letter.
        charges.Clear();
        Assert.Equal(WordStatus.Letter, Segmenter.GetWordStatus("ab", charges.Add));
        Assert.Equal([1UL], charges);
        charges.Clear();
        Assert.Equal(WordStatus.None, Segmenter.GetWordStatus(" .,", charges.Add));
        Assert.Equal(3, charges.Count);
    }

    [Fact]
    public void C4_a_meter_that_throws_stops_the_work()
    {
        static void Stop(ulong units) => throw new InvalidOperationException("out of fuel");

        Assert.Throws<InvalidOperationException>(() => Normalization.Transform("é", NormalForm.Nfc, int.MaxValue, Stop, out _));
        Assert.Throws<InvalidOperationException>(() => Casing.ToUpperCase("abc", CaseLanguage.Default, int.MaxValue, Stop, out _));
        Assert.Throws<InvalidOperationException>(() => Segmenter.Boundaries("abc", SegmentGranularity.Sentence, Stop));
    }

    // ---------------------------------------------------------------------------------------------
    // C-5: failure as data.

    [Fact]
    public void C5_an_unknown_name_is_a_false_and_a_lone_surrogate_passes_through()
    {
        Assert.False(Properties.TryResolve("NoSuchProperty", out var none));
        Assert.True(none.IsEmpty);
        Assert.False(none.Contains('a'));
        Assert.False(Properties.TryResolve(string.Empty, out _));
        Assert.False(Properties.TryResolve("gc", "NoSuchValue", out _));

        foreach (var text in new[] { "\uD800", "\uDC00", "a\uDC00́", "\uD800Σ", "Σ\uDC00" })
        {
            foreach (var form in new[] { NormalForm.Nfc, NormalForm.Nfd, NormalForm.Nfkc, NormalForm.Nfkd })
            {
                Assert.Equal(UnicodeStatus.Success, Normalization.Transform(text, form, int.MaxValue, null, out var result));
                Assert.Contains(text.First(char.IsSurrogate), result!);
            }

            Assert.Equal(UnicodeStatus.Success, Casing.ToUpperCase(text, CaseLanguage.Lithuanian, int.MaxValue, null, out _));
            Assert.Equal(UnicodeStatus.Success, Casing.ToLowerCase(text, CaseLanguage.Turkic, int.MaxValue, null, out _));
            Assert.Equal(text.Length, Segmenter.Boundaries(text, SegmentGranularity.Word, null)[^1]);
        }
    }

    private static bool Mutable(Type type) =>
        type.IsArray ||
        (type.IsGenericType && type.GetGenericTypeDefinition() is var definition &&
            (definition == typeof(List<>) || definition == typeof(Span<>) || definition == typeof(Memory<>) ||
             definition == typeof(IList<>) || definition == typeof(ICollection<>) || definition == typeof(Dictionary<,>)));

    private sealed class OtherVersion : IUnicodeTables
    {
        public string UnicodeVersion => "16.0.0";

        public ReadOnlySpan<byte> Table(UnicodeTable table) => UnicodeTables.Instance.Table(table);
    }
}
