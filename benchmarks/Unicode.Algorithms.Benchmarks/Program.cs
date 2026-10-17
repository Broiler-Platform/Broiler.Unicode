using BenchmarkDotNet.Running;
using Broiler.Unicode.Algorithms.Benchmarks;

// Run every benchmark, or a subset through BenchmarkDotNet's filter, e.g.:
//   dotnet run -c Release --project benchmarks/Unicode.Algorithms.Benchmarks -- --filter *Casing*
BenchmarkSwitcher
    .FromTypes([typeof(NormalizationBenchmarks), typeof(CasingBenchmarks)])
    .Run(args);
