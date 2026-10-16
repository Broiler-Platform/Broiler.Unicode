using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Generation;
using Xunit;

namespace Broiler.Unicode.Algorithms.Tests;

/// <summary>
/// Bundle sjl-7-001's part B, generation byte for byte and the pin (exit clause 4), with its negative
/// controls: the shared JavaScript libraries programme's rule SL7 for this repository's tables.
/// </summary>
public sealed class RegenerationTests
{
    [Fact]
    public void Every_table_regenerates_byte_for_byte_from_the_pin()
    {
        var desired = Archive.TableSource(Archive.Tables);
        var path = Archive.Full(Archive.TableSourcePath);

        if (string.Equals(Environment.GetEnvironmentVariable(Archive.WriteVariable), "1", StringComparison.Ordinal))
        {
            File.WriteAllText(path, desired, new System.Text.UTF8Encoding(false));
        }

        var committed = File.ReadAllText(path);

        Assert.True(
            string.Equals(committed, desired, StringComparison.Ordinal),
            Stale(committed, desired) ?? "the committed tables differ from what the generator writes");
    }

    [Fact]
    public void Two_generations_write_the_same_bytes()
    {
        var again = UnicodeTableGenerator.Generate(Archive.Pin, Archive.Read(Archive.Pin));

        Assert.Equal(Archive.Tables.Select(static pair => pair.Key), again.Select(static pair => pair.Key));

        foreach (var ((table, first), (_, second)) in Archive.Tables.Zip(again))
        {
            Assert.True(first.AsSpan().SequenceEqual(second), $"{table} differs between two generations");
        }
    }

    [Fact]
    public void Every_table_the_algorithms_read_is_generated()
    {
        foreach (var table in Enum.GetValues<UnicodeTable>().Where(static table => table != UnicodeTable.None))
        {
            Assert.Contains(Archive.Tables, pair => pair.Key == table && pair.Value.Length > 0);
        }
    }

    [Fact]
    public void The_pin_equals_Broiler_VM_pin_by_file_name_length_and_hash()
    {
        var broilerVm = UnicodePin.Parse(File.ReadAllText(Archive.Full(Archive.BroilerVmPinPath)));

        Assert.Empty(Archive.Pin.Differences(broilerVm));
        Assert.Equal(26, Archive.Pin.Entries.Count);
    }

    [Fact]
    public void The_generator_package_carries_this_repository_s_pin()
    {
        Assert.Empty(UnicodePin.Owner().Differences(Archive.Pin));
    }

    [Fact]
    public void Control_a_changed_byte_of_a_committed_table_is_reported_with_its_table()
    {
        var desired = Archive.TableSource(Archive.Tables);
        var marker = "private static ReadOnlySpan<byte> CasedRanges => new byte[]";
        var at = desired.IndexOf(marker, StringComparison.Ordinal);
        var digit = desired.IndexOfAny("0123456789".ToCharArray(), desired.IndexOf('{', at) + 1);
        var changed = desired[..digit] + (desired[digit] == '9' ? '8' : (char)(desired[digit] + 1)) + desired[(digit + 1)..];

        Assert.NotEqual(desired, changed);
        Assert.Contains("CasedRanges", Stale(changed, desired), StringComparison.Ordinal);
    }

    [Fact]
    public void Control_a_changed_hash_in_the_pin_is_refused_before_anything_is_written()
    {
        var text = File.ReadAllText(Archive.Full(Archive.PinPath));
        var line = text.Split('\n').First(static line => line.StartsWith("file ucd-17.0.0/CaseFolding.txt ", StringComparison.Ordinal));
        var hash = line.Split(' ')[3];
        var changedHash = (hash[0] == '0' ? '1' : '0') + hash[1..];
        var changed = UnicodePin.Parse(text.Replace(line, line.Replace(hash, changedHash, StringComparison.Ordinal), StringComparison.Ordinal));

        var refusal = Assert.Throws<InvalidDataException>(() => UnicodeTableGenerator.Generate(changed, Archive.Files));

        Assert.Contains("ucd-17.0.0/CaseFolding.txt hashes to", refusal.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Control_a_changed_byte_of_a_pinned_input_is_refused()
    {
        var copy = new Dictionary<string, byte[]>(Archive.Files, StringComparer.Ordinal);
        var bytes = (byte[])copy["ucd-17.0.0/UnicodeData.txt"].Clone();

        bytes[100] ^= 1;
        copy["ucd-17.0.0/UnicodeData.txt"] = bytes;

        var refusal = Assert.Throws<InvalidDataException>(() => UnicodeTableGenerator.Generate(Archive.Pin, copy));

        Assert.Contains("ucd-17.0.0/UnicodeData.txt hashes to", refusal.Message, StringComparison.Ordinal);
    }

    /// <summary>Where the committed text first parts from the desired one, and the table it is in; null when they agree.</summary>
    private static string? Stale(string committed, string desired)
    {
        if (string.Equals(committed, desired, StringComparison.Ordinal))
        {
            return null;
        }

        var current = committed.Split('\n');
        var wanted = desired.Split('\n');
        var line = Enumerable.Range(0, Math.Min(current.Length, wanted.Length))
            .FirstOrDefault(index => !string.Equals(current[index], wanted[index], StringComparison.Ordinal), Math.Min(current.Length, wanted.Length));
        var table = wanted.Take(line + 1).LastOrDefault(static text => text.Contains("private static ReadOnlySpan<byte> ", StringComparison.Ordinal))
            ?.Trim().Split(' ')[3] ?? "the file's head";

        return $"{Archive.TableSourcePath} is not what the generator writes: first difference at line {line + 1}, in {table}. " +
            $"Change the generator or the pin and run with {Archive.WriteVariable}=1; never edit the file.";
    }
}
