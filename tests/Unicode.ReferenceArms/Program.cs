using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace Broiler.Unicode.ReferenceArms;

/// <summary>
/// The entry point of bundle sjl-7-001's reference-arm comparison (docs/evidence/sjl-7-001/reference-arms.md),
/// run in Release under the JIT with the invariant culture, as the rule's configuration asks.
/// </summary>
/// <remarks>
/// <code>
/// dotnet Unicode.ReferenceArms.dll compare [--population DIR] --out DIR [--culture NAME] [--negative-control]
///     [--pins DIR] [--limit LINES] [--operations NAME,NAME] [--classes U1,U2]
/// </code>
/// Without <c>--population</c> only U1 runs. <c>--culture tr-TR</c> or <c>lt-LT</c> is the rule's second run
/// of arm J's culture-dependent members. <c>--negative-control</c> runs the two planted changes. A run with
/// <c>--limit</c>, <c>--operations</c> or <c>--classes</c> is void, and exists to try the comparison out.
/// </remarks>
internal static class Program
{
    /// <summary>The commits each arm was copied from, and the correspondence's revisions.</summary>
    private static readonly Dictionary<string, string> Arms_ = new(StringComparer.Ordinal)
    {
        ["V"] = "Broiler.VM 24632b1 (correspondence read at Broiler.VM 16a35b2), under tests/Unicode.ReferenceArms/V/",
        ["J"] = "Broiler.JS 4d70821e (correspondence read at Broiler.JS bdb1a3ca), under tests/Unicode.ReferenceArms/J/; packages Broiler.Unicode.Properties 0.1.0-preview.3 and Broiler.Unicode.Emoji.StringProperties 0.1.0-preview.3",
        ["S"] = "Broiler.Unicode's src/Unicode.Algorithms, src/Unicode.Segmentation and src/Unicode.Tables at the commit under test, read through UnicodeTables.Instance",
    };

    public static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] != "compare")
        {
            Console.Error.WriteLine("usage: compare [--population DIR] --out DIR [--culture NAME] [--negative-control] [--pins DIR] [--limit LINES] [--operations NAME,NAME] [--classes U1,U2]");
            return 2;
        }

        string? Option(string name)
        {
            var at = Array.IndexOf(args, name);
            return at >= 0 && at + 1 < args.Length ? args[at + 1] : null;
        }

        var cultureName = Option("--culture");
        var negativeControl = args.Contains("--negative-control");

        // Every thread of the run, the parallel workers' included, under one culture: the invariant one, or the
        // one --culture names for the rule's second run of arm J's culture-dependent members.
        var culture = cultureName is null ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        var environment = Environment_(culture);
        if (environment["globalizationInvariant"] is true)
        {
            Console.Error.WriteLine("The process runs in invariant globalization mode; arm J's host operations need the host's ICU.");
            return 3;
        }

        var output = Option("--out") ?? throw new ArgumentException("--out is required");
        var pins = Option("--pins") ?? Path.Combine(FindRoot(), "data", "unicode", "pins");
        Arms.ReadNames(pins);
        Arms.NegativeControl = negativeControl;

        long? limit = Option("--limit") is { } text ? long.Parse(text, CultureInfo.InvariantCulture) : null;
        var operations = Option("--operations")?.Split(',').ToHashSet(StringComparer.Ordinal);
        var classes = Option("--classes")?.Split(',').ToHashSet(StringComparer.Ordinal);
        var mode = negativeControl ? "negative-control" : cultureName is not null ? "culture" : "comparison";
        var population = Population.Read(Option("--population"), limit);
        environment["pins"] = pins;
        environment["arms"] = Arms_;

        Console.Out.WriteLine($"mode {mode}; culture {(culture.Name.Length == 0 ? "invariant" : culture.Name)}; ICU {environment["icuVersion"]} (Unicode {environment["icuUnicodeVersion"]}); population {population.Directory ?? "(U1 only)"}");
        new Comparison(population, Arms.Operations(cultureName is not null, negativeControl), new RunOptions(mode, limit, operations, classes), Console.Out)
            .Run(output, environment);
        return 0;
    }

    /// <summary>The repository root: the first directory above the working directory or the binary that holds the solution.</summary>
    private static string FindRoot()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Broiler.Unicode.slnx")))
                    return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("no Broiler.Unicode.slnx above the working directory or the binary; pass --pins");
    }

    // ---- the run's environment -------------------------------------------------------------------

    /// <summary>
    /// The run's environment: the runtime, the machine, the culture and globalization mode, and the host's ICU
    /// with the Unicode version it carries, each with how it was read.
    /// </summary>
    private static Dictionary<string, object?> Environment_(CultureInfo culture)
    {
        var environment = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["runtime"] = RuntimeInformation.FrameworkDescription,
            ["os"] = RuntimeInformation.OSDescription,
            ["architecture"] = RuntimeInformation.ProcessArchitecture.ToString(),
            ["processors"] = Environment.ProcessorCount,
            ["memoryBytes"] = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
            ["jit"] = System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled ? "JIT (dynamic code compiled)" : "not JIT",
            ["serverGc"] = GCSettings.IsServerGC,
            ["culture"] = culture.Name.Length == 0 ? "invariant" : culture.Name,
            ["uiCulture"] = CultureInfo.CurrentUICulture.Name.Length == 0 ? "invariant" : CultureInfo.CurrentUICulture.Name,
        };

        // Globalization mode: the switch, and the behaviour that shows it (a Turkish TextInfo maps i to U+0130
        // only when the culture data is ICU's).
        AppContext.TryGetSwitch("System.Globalization.Invariant", out var invariantSwitch);
        var turkish = CultureInfo.GetCultureInfo("tr-TR").TextInfo.ToUpper("i");
        environment["globalizationInvariantSwitch"] = invariantSwitch;
        environment["globalizationInvariant"] = invariantSwitch || turkish != "İ";
        environment["turkishUpperOfI"] = "U+" + ((int)turkish[0]).ToString("X4", CultureInfo.InvariantCulture);

        // The ICU the runtime loaded: its version from the runtime itself, through the internal
        // Interop.Globalization.GetICUVersion that System.Private.CoreLib calls (the version packed as
        // major << 24 | minor << 16 | patch << 8 | build); the library's path from /proc/self/maps; and the
        // Unicode version ICU carries from ICU itself, u_getUnicodeVersion in that library.
        var (icuVersion, icuMethod) = IcuVersion();
        environment["icuVersion"] = icuVersion;
        environment["icuVersionMethod"] = icuMethod;
        var library = LoadedLibrary("libicuuc.so");
        environment["icuLibrary"] = library;
        var (unicode, unicodeMethod) = IcuUnicodeVersion(library, icuVersion);
        environment["icuUnicodeVersion"] = unicode;
        environment["icuUnicodeVersionMethod"] = unicodeMethod;

        // The runtime's own character data, which arm J's CharExtensions and Rune calls read (not ICU's).
        environment["runtimeCharacterData"] = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["U+1C89 (assigned in Unicode 16.0)"] = CharUnicodeInfo.GetUnicodeCategory(0x1C89).ToString(),
            ["U+A7CE (assigned in Unicode 17.0)"] = CharUnicodeInfo.GetUnicodeCategory(0xA7CE).ToString(),
        };

        environment["assemblies"] = new[]
            {
                typeof(Broiler.Unicode.Algorithms.UnicodeProperties).Assembly,
                typeof(Broiler.Unicode.Segmentation.UnicodeSegmenter).Assembly,
                typeof(Broiler.Unicode.Tables.UnicodeTables).Assembly,
                typeof(Broiler.Unicode.Properties.JSStringSpecialCasing).Assembly,
                typeof(UnicodeEmoji.StringProperties.EmojiStringProperties).Assembly,
                typeof(Broiler.ECMAScript.Text.Utf16).Assembly,
            }
            .ToDictionary(
                static a => a.GetName().Name ?? "?",
                static a => (object?)(a.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? a.GetName().Version?.ToString()),
                StringComparer.Ordinal);
        return environment;
    }

    private static (string Version, string Method) IcuVersion()
    {
        var interop = typeof(object).Assembly.GetType("Interop+Globalization");
        var method = interop?.GetMethod("GetICUVersion", BindingFlags.NonPublic | BindingFlags.Static);
        if (method?.Invoke(null, null) is int packed && packed != 0)
        {
            var parts = new[] { (packed >> 24) & 0xFF, (packed >> 16) & 0xFF, (packed >> 8) & 0xFF, packed & 0xFF };
            return (string.Join('.', parts.Select(static p => p.ToString(CultureInfo.InvariantCulture))), "the runtime's Interop.Globalization.GetICUVersion, by reflection");
        }

        return ("unknown", "Interop.Globalization.GetICUVersion was not found");
    }

    /// <summary>The path of a shared library the process has mapped, from /proc/self/maps.</summary>
    private static string? LoadedLibrary(string name)
    {
        const string maps = "/proc/self/maps";
        if (!File.Exists(maps))
            return null;

        return File.ReadLines(maps)
            .Select(static line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Where(fields => fields.Length >= 6 && Path.GetFileName(fields[^1]).StartsWith(name, StringComparison.Ordinal))
            .Select(static fields => fields[^1])
            .FirstOrDefault();
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void GetUnicodeVersion(IntPtr versionArray);

    private static (string Version, string Method) IcuUnicodeVersion(string? library, string icuVersion)
    {
        if (library is null || !NativeLibrary.TryLoad(library, out var handle))
            return ("unknown", "the ICU library was not found among the process's mappings");

        var major = icuVersion.Split('.')[0];
        foreach (var symbol in new[] { "u_getUnicodeVersion_" + major, "u_getUnicodeVersion" })
        {
            if (!NativeLibrary.TryGetExport(handle, symbol, out var address))
                continue;

            var buffer = Marshal.AllocHGlobal(4);
            try
            {
                Marshal.GetDelegateForFunctionPointer<GetUnicodeVersion>(address)(buffer);
                var bytes = new byte[4];
                Marshal.Copy(buffer, bytes, 0, 4);
                var version = bytes[3] == 0
                    ? string.Join('.', bytes[..3].Select(static b => b.ToString(CultureInfo.InvariantCulture)))
                    : string.Join('.', bytes.Select(static b => b.ToString(CultureInfo.InvariantCulture)));
                return (version, symbol + " in " + library);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        return ("unknown", "u_getUnicodeVersion was not exported by " + library);
    }
}
