using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Broiler.Unicode.ReferenceArms;

/// <summary>What a run is asked to do.</summary>
/// <param name="Mode">comparison, culture or negative-control.</param>
/// <param name="LineLimit">At most this many lines of each file, or null; a limited run is void.</param>
/// <param name="Operations">Only these operations, or null; a filtered run is void.</param>
/// <param name="Classes">Only these classes, or null; a filtered run is void.</param>
internal sealed record RunOptions(string Mode, long? LineLimit, IReadOnlySet<string>? Operations, IReadOnlySet<string>? Classes)
{
    public bool Void => LineLimit is not null || Operations is not null || Classes is not null;
}

/// <summary>
/// The comparison of bundle sjl-7-001's reference-arm rule: every operation over every class it takes, S
/// against every variant of every arm that implements it, exactly. It classifies nothing.
/// </summary>
/// <remarks>
/// <para>
/// <b>Output.</b> <c>differences.tsv.gz</c> holds one line per case and variant whose answer differs from
/// S's, <c>operation TAB class TAB input TAB parameter TAB arm TAB S's result TAB the arm's result</c>, in the
/// order of the operations, then of their classes, then of the inputs in their files (U1 in code point
/// order), then of each input's parameters, then of the variants: the same population gives the same bytes.
/// <c>summary.json</c> holds the counts per operation, class and variant, the run's environment, the
/// population's manifest and its digest, the listing's line count and SHA-256, and what each arm does not
/// implement. README.md documents the notation.
/// </para>
/// <para>
/// <b>Parallel and deterministic.</b> Each class is read in chunks; the inputs of a chunk run in parallel,
/// each into its own rows and counters, and the chunk is written in input order before the next one runs.
/// An input longer than 2^20 code units runs in a chunk of its own, alone, to bound the memory of U7.
/// </para>
/// </remarks>
internal sealed class Comparison(Population population, IReadOnlyList<Operation> operations, RunOptions options, TextWriter log)
{
    private const int CodePointChunk = 8192;

    private const int ItemChunk = 512;

    private const int AloneLength = 1 << 20;

    /// <summary>The planted differences of the negative control, and whether the listing holds each.</summary>
    private readonly Dictionary<string, bool> planted = new(StringComparer.Ordinal);

    public void Run(string outputDirectory, Dictionary<string, object?> environment)
    {
        Directory.CreateDirectory(outputDirectory);
        var started = DateTime.UtcNow;
        var clock = Stopwatch.StartNew();
        if (options.Mode == "negative-control")
        {
            planted["V: normalize, U4 line 1, NFC/none, one unit more on the first charge"] = false;
            planted["J: IdentifierStart, U1 U+0041, CharExtensions.IsIdentifierStart negated"] = false;
        }

        var summary = new List<Dictionary<string, object?>>();
        var totals = new SortedDictionary<string, long>(StringComparer.Ordinal);
        long lines = 0;
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        using (var file = File.Create(Path.Combine(outputDirectory, "differences.tsv.gz")))
        using (var gzip = new GZipStream(file, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzip, new UTF8Encoding(false)) { NewLine = "\n" })
        {
            void Write(string row)
            {
                var line = row + "\n";
                writer.Write(line);
                hash.AppendData(Encoding.UTF8.GetBytes(line));
                lines++;
            }

            foreach (var operation in operations.Where(o => options.Operations is null || options.Operations.Contains(o.Name)))
            {
                foreach (var name in operation.Classes.Where(c => options.Classes is null || options.Classes.Contains(c)))
                {
                    var counters = new Counters(operation.Variants.Length);
                    var operationClock = Stopwatch.StartNew();
                    foreach (var chunk in Chunks(name))
                        RunChunk(operation, chunk, counters, Write);

                    var line = $"{operation.Name} {name}: {counters.Inputs} inputs, {counters.Cases} cases, " +
                               string.Join(", ", operation.Variants.Select((v, i) => $"{v.Label} {counters.Differ[i]} of {counters.Ran[i]} differ ({counters.NotCalled[i]} not called)")) +
                               $" ({operationClock.Elapsed.TotalSeconds:F1} s)";
                    log.WriteLine(line);
                    log.Flush();

                    for (var i = 0; i < operation.Variants.Length; i++)
                    {
                        var key = operation.Name + "\t" + operation.Variants[i].Label;
                        totals[key] = totals.GetValueOrDefault(key) + counters.Differ[i];
                    }

                    summary.Add(new Dictionary<string, object?>
                    {
                        ["operation"] = operation.Name,
                        ["class"] = name,
                        ["inputs"] = counters.Inputs,
                        ["cases"] = counters.Cases,
                        ["sharedRuns"] = counters.SharedRuns,
                        ["seconds"] = Math.Round(operationClock.Elapsed.TotalSeconds, 1),
                        ["variants"] = operation.Variants.Select((v, i) => new Dictionary<string, object?>
                        {
                            ["arm"] = v.Arm,
                            ["variant"] = v.Name,
                            ["ran"] = counters.Ran[i],
                            ["differ"] = counters.Differ[i],
                            ["notCalled"] = counters.NotCalled[i],
                        }).ToList(),
                    });
                }
            }
        }

        var listingDigest = Convert.ToHexStringLower(hash.GetHashAndReset());
        var document = new Dictionary<string, object?>
        {
            ["rule"] = "docs/evidence/sjl-7-001/reference-arms.md",
            ["mode"] = options.Mode,
            ["void"] = options.Void,
            ["lineLimit"] = options.LineLimit,
            ["operationFilter"] = options.Operations?.Order(StringComparer.Ordinal).ToList(),
            ["classFilter"] = options.Classes?.Order(StringComparer.Ordinal).ToList(),
            ["started"] = started.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            ["seconds"] = Math.Round(clock.Elapsed.TotalSeconds),
            ["environment"] = environment,
            ["population"] = new Dictionary<string, object?>
            {
                ["directory"] = population.Directory,
                ["manifestSha256"] = population.ManifestDigest,
                ["manifest"] = population.ManifestText is { } text ? JsonDocument.Parse(text).RootElement : null,
                ["files"] = population.Files,
                ["U1"] = "implicit: every code point from U+0000 to U+10FFFF, in every code point operation, and alone in casing, normalization and the properties of strings",
            },
            ["names"] = new Dictionary<string, object?>
            {
                ["binary"] = Arms.BinaryNames,
                ["generalCategory"] = Arms.CategoryNames,
                ["script"] = Arms.ScriptNames,
                ["stringProperties"] = Arms.StringPropertyNames,
                ["unresolved"] = Arms.Unresolved.Select(static u => new Dictionary<string, string>
                {
                    ["arm"] = u.Arm, ["operation"] = u.Operation, ["name"] = u.Name, ["answer"] = u.Answer,
                }).ToList(),
            },
            ["notImplemented"] = Arms.NotImplemented.Select(static n => new Dictionary<string, string>
            {
                ["arm"] = n.Arm, ["operation"] = n.Operation, ["reason"] = n.Reason,
            }).ToList(),
            ["operations"] = summary,
            ["differences"] = new Dictionary<string, object?>
            {
                ["total"] = lines,
                ["byOperationAndArm"] = totals.Select(static t => new Dictionary<string, object?>
                {
                    ["operation"] = t.Key.Split('\t')[0], ["arm"] = t.Key.Split('\t')[1], ["differ"] = t.Value,
                }).ToList(),
                ["listing"] = new Dictionary<string, object?>
                {
                    ["file"] = "differences.tsv.gz",
                    ["lines"] = lines,
                    ["sha256OfUncompressedText"] = listingDigest,
                },
            },
            ["negativeControl"] = options.Mode == "negative-control"
                ? planted.Select(static p => new Dictionary<string, object?> { ["planted"] = p.Key, ["reportedAsADifference"] = p.Value }).ToList()
                : null,
        };

        File.WriteAllText(
            Path.Combine(outputDirectory, "summary.json"),
            JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }) + "\n",
            new UTF8Encoding(false));
        log.WriteLine($"{lines} differences; listing sha256 {listingDigest}; {clock.Elapsed.TotalSeconds:F0} s");
        foreach (var (plant, found) in planted)
            log.WriteLine($"negative control: {plant}: {(found ? "reported as a difference" : "NOT reported")}");
    }

    /// <summary>A class's inputs in chunks: U1 generated, the others read; an input past 2^20 code units alone.</summary>
    private IEnumerable<List<Item>> Chunks(string name)
    {
        if (name == "U1")
        {
            for (var first = 0; first <= Population.MaxCodePoint; first += CodePointChunk)
                yield return Population.CodePoints(first, CodePointChunk);
            yield break;
        }

        var current = new List<Item>(ItemChunk);
        foreach (var item in population.Items(name))
        {
            if (item.Text.Length > AloneLength)
            {
                if (current.Count > 0)
                    yield return current;
                yield return [item];
                current = new List<Item>(ItemChunk);
                continue;
            }

            current.Add(item);
            if (current.Count == ItemChunk)
            {
                yield return current;
                current = new List<Item>(ItemChunk);
            }
        }

        if (current.Count > 0)
            yield return current;
    }

    private void RunChunk(Operation operation, List<Item> chunk, Counters counters, Action<string> write)
    {
        var width = operation.Variants.Length;
        var rows = new List<string>?[chunk.Count];
        var counts = new long[chunk.Count, (3 * width) + 2];
        var sharedKeys = operation.Variants.Select(static v => v.SharedKey).Distinct(StringComparer.Ordinal).ToArray();
        var sharedIndex = operation.Variants.Select(v => Array.IndexOf(sharedKeys, v.SharedKey)).ToArray();
        var options = new ParallelOptions { MaxDegreeOfParallelism = chunk.Count == 1 ? 1 : Environment.ProcessorCount };

        Parallel.For(0, chunk.Count, options, at =>
        {
            var item = chunk[at];
            var shared = new Result?[sharedKeys.Length];
            foreach (var input in operation.Expand(item))
            {
                counts[at, 3 * width]++;
                Array.Clear(shared);
                for (var v = 0; v < width; v++)
                {
                    var variant = operation.Variants[v];
                    var result = Arms.Safe(variant.Run, input);
                    if (result is not { } answer)
                    {
                        counts[at, (3 * v) + 2]++;
                        continue;
                    }

                    if (shared[sharedIndex[v]] is not { } expected)
                    {
                        expected = Arms.Safe(c => variant.Shared(c), input) ?? Result.Refused("null");
                        shared[sharedIndex[v]] = expected;
                        counts[at, (3 * width) + 1]++;
                    }

                    counts[at, 3 * v]++;
                    if (answer != expected)
                    {
                        counts[at, (3 * v) + 1]++;
                        (rows[at] ??= []).Add(string.Join('\t',
                            operation.Name, item.Class, item.Shown, input.Parameter, variant.Label, expected.Render(), answer.Render()));
                        if (planted.Count > 0)
                            Plant(operation, input, variant);
                    }
                }
            }
        });

        for (var at = 0; at < chunk.Count; at++)
        {
            counters.Inputs++;
            counters.Cases += counts[at, 3 * width];
            counters.SharedRuns += counts[at, (3 * width) + 1];
            for (var v = 0; v < width; v++)
            {
                counters.Ran[v] += counts[at, 3 * v];
                counters.Differ[v] += counts[at, (3 * v) + 1];
                counters.NotCalled[v] += counts[at, (3 * v) + 2];
            }

            if (rows[at] is { } found)
            {
                foreach (var row in found)
                    write(row);
            }
        }
    }

    /// <summary>Marks a planted change of the negative control as reported, when this difference is it.</summary>
    private void Plant(Operation operation, Case input, Variant variant)
    {
        lock (planted)
        {
            if (operation.Name == "normalize" && variant.Arm == "V" && input.Item.Class.StartsWith("U4", StringComparison.Ordinal)
                && input.Item.Line == 1 && input.Parameter == Arms.PlantedVParameter)
                planted["V: normalize, U4 line 1, NFC/none, one unit more on the first charge"] = true;

            if (operation.Name == "IdentifierStart" && variant.Name == "CharExtensions.IsIdentifierStart"
                && input.Item.Class == "U1" && input.Item.CodePoint == Arms.PlantedJCodePoint)
                planted["J: IdentifierStart, U1 U+0041, CharExtensions.IsIdentifierStart negated"] = true;
        }
    }

    private sealed class Counters(int variants)
    {
        public long Inputs;
        public long Cases;
        public long SharedRuns;
        public readonly long[] Ran = new long[variants];
        public readonly long[] Differ = new long[variants];
        public readonly long[] NotCalled = new long[variants];
    }
}
