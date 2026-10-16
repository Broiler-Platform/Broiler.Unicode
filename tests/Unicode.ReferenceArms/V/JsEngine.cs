// REFERENCE ARM V: the engine the copies of Broiler.VM's members run against. It is not a copy: it stands for
// the three members of Broiler.VM's JsEngine at 24632b1 the copied members call, and records what they do.
//
//   src/Broiler.VM.Profile.JavaScript/JsEngine.Metering.cs: Charge(ulong units) (:155), which charges the
//     engine's fuel, and Error(string kind, string message) (:270), which builds the JsThrow a member throws;
//   src/Broiler.VM.Profile.JavaScript/JsEngine.cs: Intl (:183), the realm's decoded Intl tables, which
//     LanguageCasing reads for Soft_Dotted (JsRealm.String.cs:1228).
//
// Here Charge records every charge, its units and their total, and never runs out; Error is the recorded
// exception JsThrow carrying the error's kind; and Intl is arm V's JsIntlTables over the tables copied into
// JsCldrTables.g.cs, through JsCldrData as the source's Intl assembly answers them.

using System;
using Broiler.VM.Profile.JavaScript.Intl;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The arm's engine: the meter Broiler.VM's members charge, the errors they raise, and the Intl tables.</summary>
internal sealed class JsEngine
{
    /// <summary>The Intl tables every arm V engine reads, decoded once as the source decodes them once per process.</summary>
    private static readonly JsIntlTables SharedIntl = new(JsCldrData.Instance);

    /// <summary>
    /// The negative control's planted change, when set: the first charge this engine records is one unit more
    /// than the member charged. Only the comparison's <c>--negative-control</c> run sets it, for one input.
    /// </summary>
    private bool plant;

    /// <summary>An engine for one call of a copied member.</summary>
    /// <param name="plantFirstCharge">Whether this is the negative control's input.</param>
    public JsEngine(bool plantFirstCharge = false) => plant = plantFirstCharge;

    /// <summary>The total of every charge recorded.</summary>
    public ulong Charged { get; private set; }

    /// <summary>How many charges were recorded.</summary>
    public int Charges { get; private set; }

    /// <summary>The source's JsEngine.Intl: never null here, since the arm's realm always has Intl.</summary>
    internal JsIntlTables? Intl => SharedIntl;

    /// <summary>The source's JsEngine.Charge: records the charge.</summary>
    internal void Charge(ulong units)
    {
        if (plant)
        {
            units++;
            plant = false;
        }

        Charged += units;
        Charges++;
    }

    /// <summary>The source's JsEngine.Error: the recorded exception the member throws.</summary>
    internal JsThrow Error(string kind, string message) => new(kind, message);
}

/// <summary>The arm's recorded exception: an engine error by its kind (its constructor's name).</summary>
internal sealed class JsThrow(string kind, string message) : Exception(message)
{
    public string Kind { get; } = kind;
}
