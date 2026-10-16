// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeStringProperties.cs
//     blob 333c2fdf742063899babd96631e943befa0ef6c3: the whole file: JsUnicodeStringProperties (TryResolve, Collect; :37-97)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's: the
// namespace, the assurance header and annotations included, so that the generated half of each partial class
// compiles against it unchanged.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// Reads the generated properties of strings: the names the <c>v</c> flag's <c>\p{...}</c> admits
/// for them, and each one's code points and sequences (phase F2).
/// </summary>
/// <remarks>
/// <b><c>RGI_Emoji</c> is read as the union it is defined as</b>, the six other properties together,
/// so it costs the table no data of its own. A sequence is answered as the UTF-16 string it spells.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8FDC8D
// Broiler-Human:        PENDING
internal static partial class JsUnicodeStringProperties
{
    /// <summary>The property a lone <c>\p{name}</c> names, when it is a property of strings.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=694874
    // Broiler-Human:        PENDING
    internal static bool TryResolve(ReadOnlySpan<char> name, out int property)
    {
        property = JsUnicodeProperties.Find(Names, NameIndex, name);
        return property >= 0;
    }

    /// <summary>
    /// Adds <paramref name="property"/>'s single code points to <paramref name="ranges"/>, as
    /// first-last pairs, and its sequences to <paramref name="strings"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1DA7F3
    // Broiler-Human:        PENDING
    internal static void Collect(
        int property,
        System.Collections.Generic.List<int> ranges,
        System.Collections.Generic.HashSet<string> strings)
    {
        if (property == UnionProperty)
        {
            for (var each = 0; each < UnionProperty; each++)
            {
                Collect(each, ranges, strings);
            }

            return;
        }

        var row = property * 12;
        var firstRange = JsUnicodeProperties.ReadInt24(PropertyData, row);
        var rangeCount = JsUnicodeProperties.ReadInt24(PropertyData, row + 3);
        var offset = JsUnicodeProperties.ReadInt24(PropertyData, row + 6);
        var sequenceCount = JsUnicodeProperties.ReadInt24(PropertyData, row + 9);

        for (var at = firstRange; at < firstRange + rangeCount; at++)
        {
            ranges.Add(JsUnicodeProperties.ReadInt24(RangeData, at * 6));
            ranges.Add(JsUnicodeProperties.ReadInt24(RangeData, (at * 6) + 3));
        }

        var text = new System.Text.StringBuilder();

        for (var sequence = 0; sequence < sequenceCount; sequence++)
        {
            var length = SequenceData[offset++];
            text.Clear();

            for (var step = 0; step < length; step++)
            {
                int index = SequenceData[offset++];

                if (index >= SequenceShortLimit)
                {
                    index = SequenceShortLimit + (((index - SequenceShortLimit) << 8) | SequenceData[offset++]);
                }

                text.Append(char.ConvertFromUtf32(JsUnicodeProperties.ReadInt24(CodePointDictionary, index * 3)));
            }

            strings.Add(text.ToString());
        }
    }
}
