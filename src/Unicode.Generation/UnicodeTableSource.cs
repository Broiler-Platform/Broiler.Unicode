using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Broiler.Unicode.Algorithms;

namespace Broiler.Unicode.Generation;

/// <summary>
/// Writes generated tables as C#: one <c>ReadOnlySpan&lt;byte&gt;</c> property over constant data per table,
/// and a switch from <see cref="UnicodeTable"/> to it, which an <see cref="IUnicodeTables"/> answers from.
/// </summary>
/// <remarks>
/// The text is deterministic: tables in <see cref="UnicodeTable"/> order, bytes in decimal at most 100
/// columns wide, LF line endings, and nothing recording a time, a machine or a path. A test that
/// regenerates the tables compares this text with the committed file byte for byte.
/// </remarks>
public static class UnicodeTableSource
{
    /// <summary>The C# text of a static class holding <paramref name="tables"/>.</summary>
    /// <param name="tables">The generated tables, as <see cref="UnicodeTableGenerator.Generate"/> answers them.</param>
    /// <param name="namespaceName">The class's namespace.</param>
    /// <param name="className">The class's name.</param>
    /// <param name="header">Comment lines written after the usings, each without its <c>//</c>.</param>
    public static string Write(
        IReadOnlyList<KeyValuePair<UnicodeTable, byte[]>> tables, string namespaceName, string className, IReadOnlyList<string> header)
    {
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentNullException.ThrowIfNull(namespaceName);
        ArgumentNullException.ThrowIfNull(className);
        ArgumentNullException.ThrowIfNull(header);

        var text = new StringBuilder();

        text.Append("using System;\nusing Broiler.Unicode.Algorithms;\n\n");
        text.Append("namespace ").Append(namespaceName).Append(";\n\n");

        foreach (var line in header)
        {
            text.Append(line.Length == 0 ? "//" : "// " + line).Append('\n');
        }

        text.Append('\n');
        text.Append("/// <summary>The generated Unicode ").Append(UnicodePin.PinnedVersion).Append(" tables, one span over constant data per table.</summary>\n");
        text.Append("internal static class ").Append(className).Append('\n');
        text.Append("{\n");
        text.Append("    /// <summary>The bytes of <paramref name=\"table\"/>, or an empty span for a table not generated.</summary>\n");
        text.Append("    internal static ReadOnlySpan<byte> Get(UnicodeTable table) => table switch\n");
        text.Append("    {\n");

        foreach (var (table, _) in tables)
        {
            text.Append("        UnicodeTable.").Append(table).Append(" => ").Append(table).Append(",\n");
        }

        text.Append("        _ => default,\n");
        text.Append("    };\n");

        foreach (var (table, bytes) in tables)
        {
            text.Append('\n');
            text.Append("    /// <summary>").Append(table).Append(": ").Append(bytes.Length.ToString(CultureInfo.InvariantCulture)).Append(" bytes.</summary>\n");
            text.Append("    private static ReadOnlySpan<byte> ").Append(table).Append(" => new byte[]\n");
            text.Append("    {\n");

            var line = new StringBuilder("       ");

            foreach (var value in bytes)
            {
                var item = value.ToString(CultureInfo.InvariantCulture);

                if (line.Length + item.Length + 2 > 100)
                {
                    text.Append(line).Append('\n');
                    line.Clear().Append("       ");
                }

                line.Append(' ').Append(item).Append(',');
            }

            if (bytes.Length > 0)
            {
                text.Append(line).Append('\n');
            }

            text.Append("    };\n");
        }

        text.Append("}\n");
        return text.ToString();
    }
}
