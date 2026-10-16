// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           2
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.Unicode.Algorithms;

namespace Broiler.Unicode.Tables;

/// <summary>
/// The Unicode tables Broiler.Unicode's generator writes from the pinned Unicode Character Database
/// (<c>data/unicode/pins/unicode.pin</c>), as an <see cref="IUnicodeTables"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Generated, and held byte for byte.</b> <c>UnicodeTableData.g.cs</c> beside this file is written by
/// <c>Broiler.Unicode.Generation</c>, and the test project regenerates it from the pinned archive and
/// compares it byte for byte (the shared JavaScript libraries programme's rule SL7). A change belongs in the
/// generator or the pin, never in the file.
/// </para>
/// <para>
/// <b>Not for the engines.</b> Under decision SJL-D-6 (c) each engine embeds the tables the same generator
/// writes from its own pin, in an assembly of its own, and references no package that carries tables. This
/// package is for this repository's tests and probe, and for any other consumer.
/// </para>
/// <para>
/// The table data is derived from Unicode data files and is subject to the Unicode License v3 (SPDX
/// <c>Unicode-3.0</c>), whose text <c>THIRD_PARTY_NOTICES.md</c> carries.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Low; Resources=1; Fingerprint=7BAF35
// Broiler-Human:        PENDING
public sealed class UnicodeTables : IUnicodeTables
{
    private UnicodeTables()
    {
    }

    /// <summary>The Unicode version of the tables, <c>17.0.0</c>: the pin's.</summary>
    // Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Low; Resources=0; Fingerprint=4D7A09
    // Broiler-Human:        PENDING
    public const string Version = UnicodeAlgorithms.UnicodeVersion;

    /// <summary>The one instance; it holds nothing, and every table it answers is constant data.</summary>
    public static UnicodeTables Instance { get; } = new();

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Medium; Resources=1; Fingerprint=7DF785
    // Broiler-Human:        PENDING
    public string UnicodeVersion => Version;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=JSD-0074 SJL-D-6 (c) table provider; IP=Low; Security=Medium; Resources=1; Fingerprint=C09689
    // Broiler-Human:        PENDING
    public ReadOnlySpan<byte> Table(UnicodeTable table) => UnicodeTableData.Get(table);
}
