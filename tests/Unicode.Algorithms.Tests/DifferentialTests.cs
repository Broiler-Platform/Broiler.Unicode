// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broiler.Unicode.Algorithms.Tests.Preview4;
using Broiler.Unicode.Tables;
using Xunit;

namespace Broiler.Unicode.Algorithms.Tests;

/// <summary>
/// The current normalization and case mapping against the implementation 0.1.0-preview.4 published
/// (<c>Preview4/</c>): every result, every status and every charge, in order, the same. The faster
/// lookups and passes are not allowed to change anything a caller can observe, a charge to the meter
/// or the point a limit is found exceeded included.
/// </summary>
public sealed class DifferentialTests
{
    private static readonly UnicodeNormalization Normalization = new(UnicodeTables.Instance);

    private static readonly UnicodeCaseMapping Casing = new(UnicodeTables.Instance);

    private static readonly Preview4Normalization ReferenceNormalization = new(UnicodeTables.Instance);

    private static readonly Preview4CaseMapping ReferenceCasing = new(UnicodeTables.Instance);

    private static readonly NormalForm[] Forms = [NormalForm.Nfc, NormalForm.Nfd, NormalForm.Nfkc, NormalForm.Nfkd];

    private static readonly CaseLanguage[] Languages = [CaseLanguage.Default, CaseLanguage.Turkic, CaseLanguage.Lithuanian];

    /// <summary>
    /// Code points the algorithms treat specially: the ASCII letters the languages tailor, Latin-1,
    /// combining marks of several classes, final sigma's context, Hangul syllables and jamo,
    /// compatibility characters, composition exclusions, the Turkic and Lithuanian letters, lone
    /// surrogates and supplementary-plane mappings.
    /// </summary>
    private static readonly int[] Interesting =
    [
        'a', 'b', 'e', 'i', 'j', 'q', 's', 'z', 'A', 'E', 'I', 'J', 'S', 'Z', '0', ' ', '.', '\'', '-',
        0x00AA, 0x00B5, 0x00BD, 0x00C0, 0x00C4, 0x00C5, 0x00CC, 0x00CD, 0x00DF, 0x00E4, 0x00E9, 0x00FF,
        0x0128, 0x012E, 0x0130, 0x0131, 0x0149, 0x017F, 0x01C4, 0x01C5, 0x01C6, 0x01F0,
        0x0300, 0x0301, 0x0307, 0x0308, 0x030A, 0x0315, 0x031B, 0x0323, 0x0327, 0x0334, 0x0338, 0x0340,
        0x0344, 0x0345, 0x034F, 0x035C, 0x0390, 0x0391, 0x0399, 0x03A3, 0x03AC, 0x03B9, 0x03C2, 0x03C3,
        0x0587, 0x05B0, 0x0653, 0x0670, 0x093C, 0x0B3E, 0x0B47, 0x0CD5, 0x0F73, 0x0F75, 0x0F81,
        0x1100, 0x1102, 0x1112, 0x1161, 0x1175, 0x11A7, 0x11A8, 0x11C2, 0x1E9B, 0x1E9E, 0x1F80, 0x1F88,
        0x1FB3, 0x1FBC, 0x2126, 0x212B, 0x2460, 0x3099, 0x309A, 0x304B, 0x3300, 0xAC00, 0xAC01, 0xB098,
        0xD7A3, 0xFB00, 0xFB01, 0xFB02, 0xFDFA, 0xFF76, 0xFF8A, 0xFF9E, 0xFF9F, 0xD800, 0xDBFF, 0xDC00,
        0xDFFF, 0x10400, 0x10428, 0x10D50, 0x1D15E, 0x1D15F, 0x1D165, 0x1D16E, 0x1E900, 0x2F800,
    ];

    [Fact]
    public void D1_every_code_point_alone_is_answered_as_preview_4_answered_it()
    {
        var text = new StringBuilder();

        for (var codePoint = 0; codePoint <= UnicodeAlgorithms.MaxCodePoint; codePoint++)
        {
            text.Clear();
            Append(text, codePoint);
            Compare(text.ToString(), int.MaxValue);
        }
    }

    [Fact]
    public void D2_random_texts_are_answered_as_preview_4_answered_them_at_every_limit()
    {
        var random = new Random(0x5317);
        var text = new StringBuilder();

        for (var round = 0; round < 40_000; round++)
        {
            text.Clear();
            var count = random.Next(0, 24);

            for (var index = 0; index < count; index++)
            {
                Append(text, random.Next(8) switch
                {
                    0 => random.Next(0x20, 0x7F),
                    1 => random.Next(0x80, 0x250),
                    2 => random.Next(0x300, 0x370),
                    3 => random.Next(0, 0x10000),
                    4 => random.Next(0, UnicodeAlgorithms.MaxCodePoint + 1),
                    _ => Interesting[random.Next(Interesting.Length)],
                });
            }

            var value = text.ToString();
            Compare(value, int.MaxValue);
            Compare(value, random.Next(0, (value.Length * 3) + 2));
        }
    }

    [Fact]
    public void D3_texts_longer_than_any_stack_buffer_are_answered_as_preview_4_answered_them()
    {
        var random = new Random(0x1715);
        var text = new StringBuilder();

        for (var round = 0; round < 300; round++)
        {
            text.Clear();
            var count = random.Next(200, 1200);

            for (var index = 0; index < count; index++)
            {
                Append(text, random.Next(3) == 0 ? random.Next(0x20, 0x7F) : Interesting[random.Next(Interesting.Length)]);
            }

            var value = text.ToString();
            Compare(value, int.MaxValue);
            Compare(value, random.Next(0, (value.Length * 3) + 2));
        }
    }

    [Fact]
    public void D4_a_long_unchanged_prefix_before_the_first_change_is_answered_as_preview_4_answered_it()
    {
        foreach (var length in new[] { 0, 1, 9, 10, 11, 255, 256, 257, 300, 1000 })
        {
            foreach (var tail in new[] { "a", "A", "\u00DF", "\u03A3", "\u0130", "a\u0307", "\uD801\uDC28" })
            {
                var text = new string('0', length) + tail;

                foreach (var limit in new[] { 0, 1, 2, 8, 10, 16, length - 1, length, length + 1, length + 3, int.MaxValue })
                {
                    if (limit >= 0)
                    {
                        Compare(text, limit);
                    }
                }
            }
        }
    }

    [Fact]
    public void D5_every_code_point_s_trie_answers_are_the_range_and_index_tables_answers()
    {
        var properties = new UnicodeProperties(UnicodeTables.Instance);
        var lexical = new EcmaScriptLexical(properties);

        for (var codePoint = -2; codePoint <= UnicodeAlgorithms.MaxCodePoint + 2; codePoint++)
        {
            Assert.Equal(ReferenceNormalization.CombiningClass(codePoint), Normalization.CombiningClass(codePoint));
            Assert.Equal(ReferenceNormalization.QuickCheck(codePoint, false), Normalization.QuickCheck(codePoint, false));
            Assert.Equal(ReferenceNormalization.QuickCheck(codePoint, true), Normalization.QuickCheck(codePoint, true));
            Assert.Equal(ReferenceCasing.IsCased(codePoint), Casing.IsCased(codePoint));
            Assert.Equal(ReferenceCasing.IsCaseIgnorable(codePoint), Casing.IsCaseIgnorable(codePoint));
            Assert.Equal(ReferenceCasing.IsSoftDotted(codePoint), Casing.IsSoftDotted(codePoint));

            var inRange = (uint)codePoint <= UnicodeAlgorithms.MaxCodePoint;
            var isAscii = (uint)codePoint < 0x80;

            if (inRange && !isAscii)
            {
                Assert.Equal(properties.IdStart.Contains(codePoint), lexical.IsIdentifierStart(codePoint));
                Assert.Equal(codePoint is 0x200C or 0x200D || properties.IdContinue.Contains(codePoint), lexical.IsIdentifierPart(codePoint));
            }

            Assert.Equal(
                codePoint is 0x09 or 0x0B or 0x0C or 0x20 or 0xA0 or 0xFEFF || (codePoint > 0xFF && properties.SpaceSeparator.Contains(codePoint)),
                lexical.IsWhiteSpace(codePoint));
        }
    }

    [Fact]
    public void D6_every_pair_of_a_composing_first_and_second_composes_as_preview_4_composed_it()
    {
        var data = UnicodeTables.Instance.Table(UnicodeTable.CompositionData);
        var firsts = new SortedSet<int>();
        var seconds = new SortedSet<int>();

        for (var at = 0; at < data.Length; at += 9)
        {
            firsts.Add(data[at] | (data[at + 1] << 8) | (data[at + 2] << 16));
            seconds.Add(data[at + 3] | (data[at + 4] << 8) | (data[at + 5] << 16));
        }

        var text = new StringBuilder();

        foreach (var first in firsts)
        {
            foreach (var second in seconds)
            {
                text.Clear();
                Append(text, first);
                Append(text, second);
                Compare(text.ToString(), int.MaxValue);
            }
        }
    }

    private static void Compare(string text, int maxLength)
    {
        foreach (var form in Forms)
        {
            Check(
                text, form.ToString(), maxLength,
                Run((charge, out result) => ReferenceNormalization.Transform(text, form, maxLength, charge, out result)),
                Run((charge, out result) => Normalization.Transform(text, form, maxLength, charge, out result)));
        }

        foreach (var language in Languages)
        {
            Check(
                text, "upper " + language, maxLength,
                Run((charge, out result) => ReferenceCasing.ToUpperCase(text, language, maxLength, charge, out result)),
                Run((charge, out result) => Casing.ToUpperCase(text, language, maxLength, charge, out result)));
            Check(
                text, "lower " + language, maxLength,
                Run((charge, out result) => ReferenceCasing.ToLowerCase(text, language, maxLength, charge, out result)),
                Run((charge, out result) => Casing.ToLowerCase(text, language, maxLength, charge, out result)));
        }
    }

    private delegate UnicodeStatus Operation(UnicodeWorkCharge? charge, out string? result);

    /// <summary>
    /// The operation with a meter that records every charge, and again without one: the unmetered result
    /// and status must be the metered ones, since some paths are taken only when there is no meter.
    /// </summary>
    private static (UnicodeStatus Status, string? Result, List<ulong> Charges) Run(Operation operation)
    {
        var charges = new List<ulong>();
        var status = operation(charges.Add, out var result);
        var unmetered = operation(null, out var unmeteredResult);

        if (unmetered != status || !string.Equals(unmeteredResult, result, StringComparison.Ordinal))
        {
            Assert.Fail($"without a meter the answer is {unmetered} {Escape(unmeteredResult)}, with one {status} {Escape(result)}");
        }

        return (status, result, charges);
    }

    private static void Check(
        string text, string operation, int maxLength,
        (UnicodeStatus Status, string? Result, List<ulong> Charges) expected,
        (UnicodeStatus Status, string? Result, List<ulong> Charges) actual)
    {
        var expectedResult = expected.Result;
        var actualResult = actual.Result;

        if (expected.Status != actual.Status || !string.Equals(expectedResult, actualResult, StringComparison.Ordinal) ||
            !expected.Charges.SequenceEqual(actual.Charges))
        {
            Assert.Fail(
                $"{operation} of {Escape(text)} with limit {maxLength}: preview.4 answered {expected.Status} {Escape(expectedResult)} " +
                $"charging [{string.Join(", ", expected.Charges)}], the current code {actual.Status} {Escape(actualResult)} " +
                $"charging [{string.Join(", ", actual.Charges)}]");
        }
    }

    private static string Escape(string? text)
    {
        if (text is null)
        {
            return "null";
        }

        var escaped = new StringBuilder("\"");

        foreach (var unit in text)
        {
            escaped.Append(unit is >= ' ' and < (char)0x7F ? unit.ToString() : $"\\u{(int)unit:X4}");
        }

        return escaped.Append('"').ToString();
    }

    private static void Append(StringBuilder text, int codePoint)
    {
        if (codePoint > 0xFFFF)
        {
            text.Append((char)(0xD7C0 + (codePoint >> 10))).Append((char)(0xDC00 | (codePoint & 0x3FF)));
        }
        else
        {
            text.Append((char)codePoint);
        }
    }
}
