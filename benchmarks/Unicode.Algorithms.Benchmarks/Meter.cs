namespace Broiler.Unicode.Algorithms.Benchmarks;

/// <summary>A meter in the engines' shape: it sums what it is charged and never stops the work.</summary>
internal sealed class Meter
{
    /// <summary>The units charged so far.</summary>
    internal ulong Units { get; private set; }

    /// <summary>Charges <paramref name="units"/>.</summary>
    internal void Charge(ulong units) => Units += units;
}
