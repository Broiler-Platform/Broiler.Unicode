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
public sealed class UnicodeTables : IUnicodeTables
{
    private UnicodeTables()
    {
    }

    /// <summary>The one instance; it holds nothing, and every table it answers is constant data.</summary>
    public static UnicodeTables Instance { get; } = new();

    /// <inheritdoc/>
    public string UnicodeVersion => UnicodeAlgorithms.UnicodeVersion;

    /// <inheritdoc/>
    public ReadOnlySpan<byte> Table(UnicodeTable table) => UnicodeTableData.Get(table);
}
