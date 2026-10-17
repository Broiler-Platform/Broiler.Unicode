using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Broiler.Unicode.Tables;

namespace Broiler.Unicode.Algorithms.Benchmarks;

/// <summary>
/// Casing: the workload's four calls per sample (upper and lower case, Turkic upper case and Lithuanian
/// lower case, forty calls per operation), and the two single calls the SJL-7 direct timing named. The
/// owner is metered as Broiler.VM meters it, and the workload also without a meter, as Broiler.JS calls
/// it; the runtime reads the invariant culture and ICU's tr and lt.
/// </summary>
[MemoryDiagnoser]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class CasingBenchmarks
{
    private static readonly TextInfo Turkish = CultureInfo.GetCultureInfo("tr").TextInfo;

    private static readonly TextInfo Lithuanian = CultureInfo.GetCultureInfo("lt").TextInfo;

    private readonly UnicodeCaseMapping casing = new(UnicodeTables.Instance);

    private readonly UnicodeWorkCharge charge = new Meter().Charge;

    /// <summary>The owner over the workload's four calls per sample.</summary>
    [Benchmark]
    [BenchmarkCategory("Workload")]
    public int OwnerWorkload()
    {
        var total = 0;

        foreach (var sample in Samples.Casing)
        {
            casing.ToUpperCase(sample, CaseLanguage.Default, int.MaxValue, charge, out var upper);
            casing.ToLowerCase(sample, CaseLanguage.Default, int.MaxValue, charge, out var lower);
            casing.ToUpperCase(sample, CaseLanguage.Turkic, int.MaxValue, charge, out var turkic);
            casing.ToLowerCase(sample, CaseLanguage.Lithuanian, int.MaxValue, charge, out var lithuanian);
            total += upper!.Length + lower!.Length + turkic!.Length + lithuanian!.Length;
        }

        return total;
    }

    /// <summary>The owner over the workload's four calls per sample without a meter, as Broiler.JS calls it.</summary>
    [Benchmark]
    [BenchmarkCategory("Workload")]
    public int OwnerWorkloadUnmetered()
    {
        var total = 0;

        foreach (var sample in Samples.Casing)
        {
            casing.ToUpperCase(sample, CaseLanguage.Default, int.MaxValue, null, out var upper);
            casing.ToLowerCase(sample, CaseLanguage.Default, int.MaxValue, null, out var lower);
            casing.ToUpperCase(sample, CaseLanguage.Turkic, int.MaxValue, null, out var turkic);
            casing.ToLowerCase(sample, CaseLanguage.Lithuanian, int.MaxValue, null, out var lithuanian);
            total += upper!.Length + lower!.Length + turkic!.Length + lithuanian!.Length;
        }

        return total;
    }

    /// <summary>The runtime over the workload's four calls per sample.</summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Workload")]
    public int RuntimeWorkload()
    {
        var total = 0;

        foreach (var sample in Samples.Casing)
        {
            total += sample.ToUpperInvariant().Length + sample.ToLowerInvariant().Length +
                Turkish.ToUpper(sample).Length + Lithuanian.ToLower(sample).Length;
        }

        return total;
    }

    /// <summary>The owner's default upper case of an ASCII title.</summary>
    [Benchmark]
    [BenchmarkCategory("Single")]
    public string? OwnerUpperAscii()
    {
        casing.ToUpperCase("The Quick Brown Fox", CaseLanguage.Default, int.MaxValue, charge, out var result);
        return result;
    }

    /// <summary>The runtime's invariant upper case of an ASCII title.</summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Single")]
    public string RuntimeUpperAscii() => "The Quick Brown Fox".ToUpperInvariant();

    /// <summary>The owner's default lower case of an ASCII text casing leaves as it is.</summary>
    [Benchmark]
    [BenchmarkCategory("Single")]
    public string? OwnerLowerUnchanged()
    {
        casing.ToLowerCase("plain ascii text 0123", CaseLanguage.Default, int.MaxValue, charge, out var result);
        return result;
    }

    /// <summary>The runtime's invariant lower case of an ASCII text casing leaves as it is.</summary>
    [Benchmark]
    [BenchmarkCategory("Single")]
    public string RuntimeLowerUnchanged() => "plain ascii text 0123".ToLowerInvariant();
}
