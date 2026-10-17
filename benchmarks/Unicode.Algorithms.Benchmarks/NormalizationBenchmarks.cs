using System.Text;
using BenchmarkDotNet.Attributes;
using Broiler.Unicode.Tables;

namespace Broiler.Unicode.Algorithms.Benchmarks;

/// <summary>
/// All ten normalization samples in all four forms, forty calls per operation: the owner, metered as
/// Broiler.VM meters it and without a meter as Broiler.JS calls it, against the runtime's
/// <see cref="string.Normalize(NormalizationForm)"/>.
/// </summary>
[MemoryDiagnoser]
public class NormalizationBenchmarks
{
    private static readonly NormalForm[] Forms = [NormalForm.Nfc, NormalForm.Nfd, NormalForm.Nfkc, NormalForm.Nfkd];

    private static readonly NormalizationForm[] RuntimeForms =
        [NormalizationForm.FormC, NormalizationForm.FormD, NormalizationForm.FormKC, NormalizationForm.FormKD];

    private readonly UnicodeNormalization normalization = new(UnicodeTables.Instance);

    private readonly UnicodeWorkCharge charge = new Meter().Charge;

    /// <summary>The owner over every sample and form.</summary>
    [Benchmark]
    public int Owner()
    {
        var total = 0;

        foreach (var sample in Samples.Normalize)
        {
            foreach (var form in Forms)
            {
                normalization.Transform(sample, form, int.MaxValue, charge, out var result);
                total += result!.Length;
            }
        }

        return total;
    }

    /// <summary>The owner over every sample and form without a meter, as Broiler.JS calls it.</summary>
    [Benchmark]
    public int OwnerUnmetered()
    {
        var total = 0;

        foreach (var sample in Samples.Normalize)
        {
            foreach (var form in Forms)
            {
                normalization.Transform(sample, form, int.MaxValue, null, out var result);
                total += result!.Length;
            }
        }

        return total;
    }

    /// <summary>The runtime (ICU on Linux, NLS or ICU on Windows) over every sample and form.</summary>
    [Benchmark(Baseline = true)]
    public int Runtime()
    {
        var total = 0;

        foreach (var sample in Samples.Normalize)
        {
            foreach (var form in RuntimeForms)
            {
                total += sample.Normalize(form).Length;
            }
        }

        return total;
    }
}
