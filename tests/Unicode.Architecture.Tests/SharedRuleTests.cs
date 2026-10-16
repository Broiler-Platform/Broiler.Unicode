using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Algorithms.Tests;
using Broiler.Unicode.Segmentation;
using Broiler.Unicode.Tables;
using Xunit;

namespace Broiler.Unicode.Architecture.Tests;

/// <summary>
/// SL1 to SL10. Each rule has a clean direction over the programme's projects as they stand, which also
/// asserts that the rule read a real subject, and a witness direction over an input made to break it. A
/// test method is named for its rule, so the register can find it. Which libraries a rule holds is
/// <see cref="SharedTree.Holds"/>'s answer, and the reasons are on <see cref="SharedProjectKind"/>.
/// </summary>
public class SharedRuleTests
{
    private static IReadOnlyDictionary<string, AssemblyFacts> Assemblies { get; } =
        SharedTree.Libraries.ToDictionary(static p => p.Name, static p => AssemblyFacts.Read(SharedTree.AssemblyPath(p.Name)), StringComparer.Ordinal);

    private static IEnumerable<AssemblyFacts> AssembliesHeldBy(string rule) =>
        SharedTree.LibrariesHeldBy(rule).Select(static p => Assemblies[p.Name]);

    private static readonly string[] SuppressionProperties = ["NoWarn", "WarningsNotAsErrors", "SuppressTrimAnalysisWarnings"];

    [Fact]
    public void The_rules_read_every_library_of_the_programme_and_the_probe()
    {
        Assert.Equal(
            [
                ("Broiler.Unicode.Algorithms", SharedProjectKind.Library), ("Broiler.Unicode.Generation", SharedProjectKind.Generator),
                ("Broiler.Unicode.Segmentation", SharedProjectKind.Library), ("Broiler.Unicode.Tables", SharedProjectKind.Tables),
            ],
            SharedTree.Libraries.Select(static p => (p.Name, p.Kind)));
        Assert.Equal("Broiler.Unicode.Probe", Assert.Single(SharedTree.Projects, static p => p.Kind == SharedProjectKind.Probe).Name);
        Assert.Equal(["Unicode.Algorithms.Tests", "Unicode.Architecture.Tests"], SharedTree.Projects.Where(static p => p.Kind == SharedProjectKind.Test).Select(static p => p.Name));
        Assert.All(Assemblies.Values, static a => Assert.NotEmpty(a.TypeReferences));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL1_No_library_references_an_engine_Broiler_VM_or_anything_outside_the_layering()
    {
        var violations = new List<string>();
        foreach (var library in SharedTree.LibrariesHeldBy("SL1"))
        {
            violations.AddRange(SharedRules.SL1(library.Name, library.Directory, library.Document));
            using var assets = JsonDocument.Parse(File.ReadAllText(Path.Combine(library.Directory, "obj", "project.assets.json")));
            violations.AddRange(SharedRules.SL1Restore(library.Name, assets));
        }

        violations.AddRange(AssembliesHeldBy("SL1").SelectMany(SharedRules.SL1Assembly));
        Assert.Empty(violations);

        // Not vacuous: three of the four libraries reference the algorithms library, by project and by
        // assembly, and the layering gives each that edge.
        Assert.Contains("Broiler.Unicode.Algorithms", Assemblies["Broiler.Unicode.Segmentation"].AssemblyReferences);
        Assert.Equal(3, SharedTree.LibrariesHeldBy("SL1").Count(static p => p.Items("ProjectReference").Any()));
    }

    [Theory]
    [InlineData("SL1-engine-project-reference.csproj.witness", "references the project ..\\..\\..\\broiler.js")]
    [InlineData("SL1-broiler-vm-package-reference.csproj.witness", "references the package Broiler.VM.Abstractions")]
    [InlineData("SL1-against-the-layering.csproj.witness", "references the package System.Text.Json")]
    [InlineData("SL1-against-the-layering.csproj.witness", "references Broiler.Unicode.Segmentation, against the layering")]
    public void SL1_A_witness_project_is_refused(string witness, string expected)
    {
        var algorithms = SharedTree.Project("Broiler.Unicode.Algorithms");
        var violations = SharedRules.SL1(algorithms.Name, algorithms.Directory, XDocument.Parse(Witness.Text(witness))).ToList();
        Assert.Contains(violations, v => v.Contains(expected, StringComparison.Ordinal));
    }

    [Fact]
    public void SL1_The_assembly_half_refuses_a_reference_outside_the_framework_and_the_layering()
    {
        // This test assembly references xunit and the libraries, none of which the layering gives a
        // library, so it is the witness for the assembly half as Broiler.VM's is for B1.
        var self = AssemblyFacts.Read(typeof(SharedRuleTests).Assembly.Location);
        var violations = SharedRules.SL1Assembly(self).ToList();
        Assert.Contains(violations, static v => v.EndsWith("references the assembly xunit.core", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.EndsWith("references the assembly Broiler.Unicode.Generation", StringComparison.Ordinal));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL2_No_library_holds_mutable_static_state()
    {
        Assert.Empty(AssembliesHeldBy("SL2").SelectMany(SharedRules.SL2));

        // Not vacuous: the algorithms library declares its constants - the Unicode version and the
        // largest code point among them - which SL2 reads as static fields and passes as literals.
        var algorithms = Assemblies["Broiler.Unicode.Algorithms"];
        Assert.Contains(algorithms.StaticFields, static f => f.Type == "Broiler.Unicode.Algorithms.UnicodeAlgorithms" && f.Name == "MaxCodePoint" && f.IsLiteral);
        Assert.Contains(algorithms.StaticFields, static f => f.Type == "Broiler.Unicode.Algorithms.UnicodeAlgorithms" && f.Name == "UnicodeVersion" && f.IsLiteral);
    }

    [Fact]
    public void SL2_The_witness_is_refused_for_each_kind_of_state()
    {
        var violations = SharedRules.SL2(Witness.Compile("SL2-mutable-static-state.cs.witness")).ToList();
        Assert.Contains(violations, static v => v.Contains("AmbientState.Calls is a mutable static field", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.Contains("AmbientState.Table is a static array", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.Contains("AmbientState.Cache is a static System.Collections.Generic.Dictionary`2", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.Contains("AmbientState.Mode is a settable static property", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.Contains("[ThreadStatic]", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.EndsWith("names System.Threading.ThreadLocal`1", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.EndsWith("names System.Threading.AsyncLocal`1", StringComparison.Ordinal));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL3_No_library_reaches_dynamic_code_hidden_machinery_IO_or_the_environment()
    {
        Assert.Equal(
            ["Broiler.Unicode.Algorithms", "Broiler.Unicode.Segmentation", "Broiler.Unicode.Tables"],
            SharedTree.LibrariesHeldBy("SL3").Select(static p => p.Name));
        Assert.Empty(AssembliesHeldBy("SL3").SelectMany(static a => SharedRules.SL3(a)));
    }

    /// <summary>
    /// The generator is not held to SL3 (<see cref="SharedProjectKind.Generator"/>) because it reads the
    /// pin it carries as an embedded resource, and convention 8's I/O clause does not bind it
    /// (docs/conventions.md, "Scope"). The exemption is for that and nothing else: read as the probe is,
    /// with I/O allowed, what SL3 reports of the generator is exactly the reflection type that
    /// <c>UnicodePin.Owner</c> reaches its embedded pin through.
    /// </summary>
    [Fact]
    public void SL3_The_generator_is_exempt_for_reading_its_embedded_pin_alone()
    {
        var generation = Assemblies["Broiler.Unicode.Generation"];
        Assert.Equal(["Broiler.Unicode.Generation names System.Reflection.Assembly: reflection"], SharedRules.SL3(generation, allowIo: true));
        Assert.Contains("System.Reflection.Assembly.GetManifestResourceStream(System.String)", generation.MemberReferences);
        Assert.Contains("System.IO.StreamReader", generation.TypeReferences);
    }

    /// <summary>
    /// The probe is an executable, not a library, and reads Unicode's test files from disk; SL3 holds it
    /// all the same but for I/O, since it is the code that runs under Native AOT. Broiler.JS's probe test
    /// also holds its probe to SL2 and SL4; this repository's conventions hold its probe to conventions 8
    /// (but for I/O), 12 and 17 only (docs/conventions.md, "Scope"), so SL3, SL5 and SL10 are its rules.
    /// </summary>
    [Fact]
    public void SL3_The_probe_reaches_nothing_but_I_O()
    {
        var probe = SharedTree.Projects.Single(static p => p.Kind == SharedProjectKind.Probe);
        var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
        var image = AssemblyFacts.Read(Path.Combine(probe.Directory, "bin", configuration, "net10.0", probe.Name + ".dll"));
        Assert.Contains("System.IO.File", image.TypeReferences);
        Assert.Empty(SharedRules.SL3(image, allowIo: true));
    }

    [Fact]
    public void SL3_The_witness_is_refused_for_each_kind_of_machinery()
    {
        var violations = SharedRules.SL3(Witness.Compile("SL3-dynamic-code-and-io.cs.witness")).ToList();
        string[] expected =
        [
            "calls System.Activator.CreateInstance(System.Type)", "calls System.Type.GetType(System.String)",
            "calls System.Type.MakeGenericType(System.Type[])", "calls System.Enum.GetValues(System.Type)",
            "calls System.Reflection.MethodBase.Invoke(System.Object,System.Object[])", "names System.IO.File",
            "calls System.Environment.GetEnvironmentVariable(System.String): an environment read", "applies a module initializer",
            "declares the platform invoke libc!getpid",
        ];
        foreach (var fragment in expected)
            Assert.Contains(violations, v => v.Contains(fragment, StringComparison.Ordinal));

        // The probe's exemption is for I/O only: the same witness under it still fails on the rest.
        var asProbe = SharedRules.SL3(Witness.Compile("SL3-dynamic-code-and-io.cs.witness"), allowIo: true).ToList();
        Assert.DoesNotContain(asProbe, static v => v.Contains("System.IO.File", StringComparison.Ordinal));
        Assert.Contains(asProbe, static v => v.Contains("applies a module initializer", StringComparison.Ordinal));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL4_No_library_uses_platform_globalization_regular_expressions_or_a_culture()
    {
        Assert.Empty(AssembliesHeldBy("SL4").SelectMany(SharedRules.SL4));

        // Not vacuous: the libraries the engines consume compare strings ordinally, and the generator
        // parses numbers with a format provider; SL4 reads those references with their parameter types
        // and passes them.
        Assert.Contains("System.String.Equals(System.String,System.String,System.StringComparison)", Assemblies["Broiler.Unicode.Algorithms"].MemberReferences);
        Assert.Contains("System.String.Equals(System.String,System.String,System.StringComparison)", Assemblies["Broiler.Unicode.Segmentation"].MemberReferences);
        Assert.Contains("System.Int32.Parse(System.String,System.Globalization.NumberStyles,System.IFormatProvider)", Assemblies["Broiler.Unicode.Generation"].MemberReferences);
    }

    [Fact]
    public void SL4_The_witness_is_refused_for_each_platform_dependence()
    {
        var violations = SharedRules.SL4(Witness.Compile("SL4-platform-globalization.cs.witness")).ToList();
        string[] expected =
        [
            "names System.Text.RegularExpressions.Regex", "calls System.String.Normalize(System.Text.NormalizationForm)",
            "calls System.String.ToUpper()", "calls System.Char.IsLetter(System.Char)",
            "names System.Globalization.CultureInfo: platform globalization", "calls System.Double.Parse(System.String), which parses by the current culture",
            "calls System.String.IndexOf(System.String), which compares by the current culture",
            "interpolates a string without a format provider", "calls System.DateTime.get_Now()", "names System.TimeZoneInfo",
        ];
        foreach (var fragment in expected)
            Assert.Contains(violations, v => v.Contains(fragment, StringComparison.Ordinal));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL5_Every_library_and_the_probe_hold_to_AOT_and_trimming_with_nothing_suppressed()
    {
        var violations = new List<string>();
        foreach (var library in SharedTree.LibrariesHeldBy("SL5"))
        {
            // The generator is held to everything but the AOT and trim properties (SharedRules.GeneratorProperties).
            var required = library.Kind == SharedProjectKind.Generator ? SharedRules.GeneratorProperties : SharedRules.LibraryProperties;
            var evaluated = Tools.Evaluate(library.Path, required.Select(static p => p.Property).Concat(SuppressionProperties));
            violations.AddRange(SharedRules.SL5Properties(library.Name, evaluated, required));
        }

        var probe = SharedTree.Projects.Single(static p => p.Kind == SharedProjectKind.Probe);
        var probeProperties = Tools.Evaluate(probe.Path, SharedRules.ProbeProperties.Select(static p => p.Property).Concat(SuppressionProperties));
        violations.AddRange(SharedRules.SL5Properties(probe.Name, probeProperties, SharedRules.ProbeProperties));

        violations.AddRange(AssembliesHeldBy("SL5").SelectMany(SharedRules.SL5Assembly));
        Assert.Empty(violations);
    }

    /// <summary>
    /// The inventory, in the shape of Broiler.VM's N36: every suppression in the programme's sources and
    /// project files, and in the build files they import, is listed by file in docs/suppressions.md, and
    /// when there is none, the inventory says None.
    /// </summary>
    [Fact]
    public void SL5_The_suppression_inventory_lists_every_suppression_and_says_None_when_there_is_none()
    {
        // The shipped code and the code the probe compiles: the libraries and the probe, and the build
        // files every one of them imports, which here are the repository's own. The test projects are
        // the rules' own sources, whose text names every marker the scan looks for.
        var scanned = SharedTree.Projects.Where(static p => p.Kind != SharedProjectKind.Test).Select(static p => p.Directory + Path.DirectorySeparatorChar).ToList();
        var files = SharedTree.SourceFiles("*.cs")
            .Concat(SharedTree.SourceFiles("*.csproj"))
            .Where(path => scanned.Any(directory => path.StartsWith(directory, StringComparison.Ordinal)))
            .Concat(BuildFiles.Select(static f => Path.Combine(SharedTree.Root, f)))
            .ToList();
        var found = files.SelectMany(static path => SharedRules.Suppressions(SharedTree.Relative(path), File.ReadAllText(path))).ToList();
        var inventory = File.ReadAllText(Path.Combine(SharedTree.Documents, "suppressions.md"));
        var section = inventory[inventory.IndexOf("## Inventory", StringComparison.Ordinal)..];

        if (found.Count == 0)
            Assert.Contains("\nNone.\n", section, StringComparison.Ordinal);
        foreach (var suppression in found)
            Assert.Contains(suppression.Split(':')[0], section, StringComparison.Ordinal);

        // Not vacuous: the scan read every library's and the probe's project file, the sources, and the
        // repository's build files.
        foreach (var project in SharedTree.Projects.Where(static p => p.Kind != SharedProjectKind.Test))
            Assert.Contains(project.Path, files);
        Assert.Contains(files, static p => p.EndsWith("UnicodeNormalization.cs", StringComparison.Ordinal));
        Assert.Contains(files, static p => p.EndsWith("Directory.Build.props", StringComparison.Ordinal));
    }

    /// <summary>The build files every project of the programme imports, relative to the root: the repository's own, not a folder's.</summary>
    private static readonly string[] BuildFiles = ["Directory.Build.props", "Directory.Packages.props", "eng/Broiler.Packaging.props"];

    [Fact]
    public void SL5_The_witnesses_are_refused_in_the_project_in_metadata_and_in_the_inventory()
    {
        var scratch = Directory.CreateTempSubdirectory("sl5-witness-");
        try
        {
            var project = Path.Combine(scratch.FullName, "Witness.csproj");
            File.WriteAllText(project, Witness.Text("SL5-library-suppresses-a-trim-warning.csproj.witness"));
            var evaluated = Tools.Evaluate(project, SharedRules.LibraryProperties.Select(static p => p.Property).Concat(SuppressionProperties));
            var violations = SharedRules.SL5Properties("Witness", evaluated, SharedRules.LibraryProperties).ToList();
            Assert.Contains("Witness: NoWarn suppresses IL2026", violations);
            Assert.Contains("Witness: WarningsNotAsErrors exempts IL3050", violations);
            Assert.DoesNotContain(violations, static v => v.Contains("is '", StringComparison.Ordinal));
        }
        finally
        {
            scratch.Delete(recursive: true);
        }

        Assert.NotEmpty(SharedRules.SL5Assembly(Witness.Compile("SL5-source-suppresses-a-trim-warning.cs.witness")));
        Assert.NotEmpty(SharedRules.Suppressions("x.cs", Witness.Text("SL5-source-suppresses-a-trim-warning.cs.witness")));
        Assert.NotEmpty(SharedRules.Suppressions("x.csproj", Witness.Text("SL5-library-suppresses-a-trim-warning.csproj.witness")));

        // The reset the libraries' project files hold is not a suppression; one code more is.
        Assert.Empty(SharedRules.Suppressions("x.csproj", "<NoWarn>1701;1702</NoWarn>"));
        Assert.NotEmpty(SharedRules.Suppressions("x.csproj", "<NoWarn>1701;1702;IL2026</NoWarn>"));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL6_The_public_API_matches_the_baseline_in_both_directions()
    {
        var update = Environment.GetEnvironmentVariable(ApiListing.UpdateVariable) == "1";
        var violations = new List<string>();
        foreach (var library in SharedTree.LibrariesHeldBy("SL6"))
        {
            var listing = ApiListing.Read(SharedTree.AssemblyPath(library.Name));
            Assert.NotEmpty(listing);
            var baseline = ApiListing.BaselinePath(library);
            if (update)
                File.WriteAllText(baseline, ApiListing.Render(library.Name, listing));
            violations.AddRange(SharedRules.SL6(library.Name, listing, ApiListing.ReadBaseline(baseline)));
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void SL6_Package_validation_is_on_for_every_packed_library()
    {
        foreach (var library in SharedTree.LibrariesHeldBy("SL6"))
        {
            var evaluated = Tools.Evaluate(library.Path, ["IsPackable", "EnablePackageValidation"]);
            if (evaluated["IsPackable"] == "true")
                Assert.True(evaluated["EnablePackageValidation"] == "true", library.Name + " is packed without package validation");
        }
    }

    [Fact]
    public void SL6_A_drift_in_either_direction_is_refused()
    {
        var listing = ApiListing.Read(Witness.Emit("SL6-api-drift.cs.witness"));
        var baseline = Witness.Text("SL6-api-drift.PublicAPI.txt.witness").Split('\n').Where(static l => l.Length > 0 && !l.StartsWith('#')).ToList();
        var violations = SharedRules.SL6("Witness", listing, baseline).ToList();
        Assert.Contains("Witness has public API the baseline does not record: Witness.Status.Overflow = 3", violations);
        Assert.Contains("Witness's baseline records public API the assembly does not have: Witness.Status.Malformed = 1", violations);
        Assert.Equal(2, violations.Count);
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// This repository's generated area is one file, src/Unicode.Tables/UnicodeTableData.g.cs, which
    /// Broiler.Unicode.Generation writes from data/unicode/pins/unicode.pin and the archive beside it. The
    /// test regenerates it in memory, as the algorithms' tests do (it reads the archive by the pin, which
    /// refuses a file whose length or hash moved), and compares it with the committed bytes.
    /// </summary>
    [Fact]
    public void SL7_Generated_data_is_byte_identical_to_what_its_generator_writes()
    {
        var committed = new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            [Archive.TableSourcePath] = File.ReadAllBytes(Archive.Full(Archive.TableSourcePath)),
        };
        var generated = new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            [Archive.TableSourcePath] = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(Archive.TableSource(Archive.Tables)),
        };
        Assert.True(generated[Archive.TableSourcePath].Length > 10_000, "the generator writes the tables");
        Assert.Empty(SharedRules.SL7(committed, generated));

        // The witness: one byte of a committed table changed by hand.
        var edited = new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            [Archive.TableSourcePath] = HandEditedTableWitness.Edit(committed[Archive.TableSourcePath]),
        };
        Assert.Equal([Archive.TableSourcePath + " differs from what the generator writes"], SharedRules.SL7(edited, generated));
    }

    // ---------------------------------------------------------------------------------------------

    private static readonly JsonDocumentOptions Lenient = new() { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

    /// <summary>The directory the assurance tool runs over, <c>--root Broiler.Unicode/src</c>, which holds its configuration and artefacts.</summary>
    private static string AssuranceRoot => Path.Combine(SharedTree.Root, "src");

    private static IEnumerable<string> LibraryProjectPaths =>
        SharedTree.LibrariesHeldBy("SL8").Select(static p => Path.GetRelativePath(AssuranceRoot, p.Path).Replace(Path.DirectorySeparatorChar, '/'));

    [Fact]
    public void SL8_The_assurance_scheme_covers_every_library_it_holds_and_CI_gates_on_it()
    {
        Assert.Equal(
            ["Unicode.Algorithms/Unicode.Algorithms.csproj", "Unicode.Generation/Unicode.Generation.csproj", "Unicode.Segmentation/Unicode.Segmentation.csproj"],
            LibraryProjectPaths);

        using var config = JsonDocument.Parse(File.ReadAllText(Path.Combine(AssuranceRoot, "assurance.config.json")), Lenient);
        Assert.Empty(SharedRules.SL8(config, LibraryProjectPaths));

        foreach (var artefact in (string[])["CODE-ASSURANCE.md", "HUMAN_REVIEW.md", "assurance.manifest.json"])
            Assert.True(File.Exists(Path.Combine(AssuranceRoot, artefact)), "src/" + artefact + " has not been generated");

        // Every source file of every library SL8 holds is in the manifest's files array, which is what
        // makes the record complete: a file the tool did not cover would be absent.
        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(AssuranceRoot, "assurance.manifest.json")));
        var files = manifest.RootElement.GetProperty("files").EnumerateArray().Select(static f => f.GetProperty("file").GetString()).ToHashSet(StringComparer.Ordinal);
        foreach (var library in SharedTree.LibrariesHeldBy("SL8"))
        {
            foreach (var source in SharedTree.SourceFiles("*.cs").Where(p => p.StartsWith(library.Directory + Path.DirectorySeparatorChar, StringComparison.Ordinal)))
                Assert.Contains(Path.GetRelativePath(AssuranceRoot, source).Replace(Path.DirectorySeparatorChar, '/'), files);
        }

        var workflow = File.ReadAllText(Path.Combine(SharedTree.Root, ".github", "workflows", "ci.yml"));
        Assert.Contains("assurance check", workflow, StringComparison.Ordinal);
        Assert.Contains("--root Broiler.Unicode/src", workflow, StringComparison.Ordinal);
    }

    [Fact]
    public void SL8_The_witness_configuration_is_refused()
    {
        using var config = JsonDocument.Parse(Witness.Text("SL8-config-omits-a-library.json.witness"), Lenient);
        var violations = SharedRules.SL8(config, LibraryProjectPaths).ToList();
        Assert.Contains("assurance.config.json does not cover Unicode.Generation/Unicode.Generation.csproj", violations);
        Assert.Contains("assurance.config.json does not forbid preprocessor directives", violations);
        Assert.Contains("assurance.config.json does not use the strict exemption predicate", violations);
        Assert.Contains("assurance.config.json excludes 'Unicode.Algorithms/UnicodeNormalization.cs' without a reason", violations);
        Assert.Contains("assurance.config.json excludes 'Unicode.Algorithms/UnicodeNormalization.cs', which can hide a unit of a library SL8 holds", violations);

        // The configuration's one exclusion, the generated tables of a library SL8 does not hold, with its
        // reason, is accepted.
        using var accepted = JsonDocument.Parse(
            """{ "projects": ["Unicode.Algorithms/Unicode.Algorithms.csproj", "Unicode.Generation/Unicode.Generation.csproj", "Unicode.Segmentation/Unicode.Segmentation.csproj", "Unicode.Tables/Unicode.Tables.csproj"], "exemptionPredicate": "strict", "forbidDirectives": true, "exclude": [{ "glob": "Unicode.Tables/UnicodeTableData.g.cs", "reason": "generated data" }] }""");
        Assert.Empty(SharedRules.SL8(accepted, LibraryProjectPaths));
    }

    // ---------------------------------------------------------------------------------------------

    private static readonly (string File, SegmentGranularity Granularity)[] BreakTests =
    [
        ("GraphemeBreakTest.txt", SegmentGranularity.Grapheme), ("WordBreakTest.txt", SegmentGranularity.Word),
        ("SentenceBreakTest.txt", SegmentGranularity.Sentence),
    ];

    private static readonly NormalForm[] Forms = [NormalForm.Nfc, NormalForm.Nfd, NormalForm.Nfkc, NormalForm.Nfkd];

    /// <summary>
    /// One run of Unicode's own test files through a producer: each file read off disk afresh, every line
    /// of NormalizationTest.txt in its five columns and four forms and every line of the three break tests
    /// at its granularity, and every answer rendered into the report, in order.
    /// </summary>
    private static string Run(IUnicodeWork work)
    {
        var report = new StringBuilder();
        foreach (var line in Lines("NormalizationTest.txt"))
        {
            var fields = line.Split(';');
            if (fields.Length < 5)
                continue;
            report.Append("NormalizationTest.txt");
            for (var column = 0; column < 5; column++)
            {
                var text = Text(fields[column].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                foreach (var form in Forms)
                    report.Append(' ').Append(work.Normalize(text, form));
            }

            report.Append('\n');
        }

        foreach (var (file, granularity) in BreakTests)
        {
            foreach (var line in Lines(Path.Combine("auxiliary", file)))
            {
                var text = Text(line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(static t => t is not ("÷" or "×")));
                report.Append(file).Append(' ').Append(work.Boundaries(text, granularity)).Append('\n');
            }
        }

        return report.ToString();

        static IEnumerable<string> Lines(string file) =>
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(File.ReadAllBytes(Path.Combine(SharedTree.Pins, "ucd-17.0.0", file)))
                .Split('\n')
                .Select(static raw => raw.Split('#')[0].Trim())
                .Where(static line => line.Length > 0 && line[0] != '@');

        static string Text(IEnumerable<string> codePoints)
        {
            var text = new StringBuilder();
            foreach (var codePoint in codePoints.Select(static c => int.Parse(c, NumberStyles.HexNumber, CultureInfo.InvariantCulture)))
                text.Append(codePoint is >= 0xD800 and <= 0xDFFF ? ((char)codePoint).ToString() : char.ConvertFromUtf32(codePoint));
            return text.ToString();
        }
    }

    [Fact]
    public async Task SL9_Two_runs_and_two_threads_over_Unicode_s_test_files_give_identical_output()
    {
        // One producer, shared by the four runs as an engine shares one instance: the normalization and
        // the segmenter the engines consume, over the tables Broiler.Unicode.Tables carries.
        var work = new PackagesWork();
        string[] sequential = [Run(work), Run(work)];
        var threads = await Task.WhenAll(Task.Run(() => Run(work)), Task.Run(() => Run(work)));
        string[] runs = [.. sequential, .. threads];

        Assert.Empty(SharedRules.SL9(runs));
        Assert.True(runs[0].Length > 10_000, "the runs are over every file");
        foreach (var (file, _) in BreakTests)
            Assert.Contains("\n" + file + " ", runs[0], StringComparison.Ordinal);
    }

    [Fact]
    public void SL9_The_witness_producer_is_refused()
    {
        var witness = new NondeterministicWorkWitness();
        Assert.NotEmpty(SharedRules.SL9([Run(witness), Run(witness)]));
    }

    // ---------------------------------------------------------------------------------------------

    [Fact]
    public void SL10_No_source_of_the_programme_holds_a_preprocessor_directive_or_a_CR()
    {
        var sources = SharedTree.SourceFiles("*.cs").ToList();
        Assert.Contains(sources, static s => s.EndsWith("UnicodeTableData.g.cs", StringComparison.Ordinal));
        Assert.Contains(sources, static s => s.EndsWith("Program.cs", StringComparison.Ordinal));
        Assert.Empty(sources.SelectMany(static p => SharedRules.SL10Directives(SharedTree.Relative(p), File.ReadAllText(p))));

        var text = SharedTree.SourceFiles("*").Where(static p => !p.EndsWith(".png", StringComparison.Ordinal)).ToList();
        Assert.Contains(text, static p => p.EndsWith("unicode.pin", StringComparison.Ordinal));
        Assert.Contains(text, static p => p.EndsWith("roadmap.md", StringComparison.Ordinal));
        Assert.Empty(text.SelectMany(static p => SharedRules.SL10LineEndings(SharedTree.Relative(p), File.ReadAllBytes(p))));

        // Read by line: Git checks .gitattributes out with the platform's line endings. Each project
        // directory and the documents are given LF by a line of their own or of a directory above them;
        // the pin's own text by a line naming it.
        var attributes = File.ReadAllLines(Path.Combine(SharedTree.Root, ".gitattributes"));
        foreach (var directory in SharedTree.ProjectDirectories.Append(SharedTree.Documents).Select(SharedTree.Relative))
        {
            var segments = directory.Split('/');
            var covering = Enumerable.Range(1, segments.Length).Select(n => string.Join('/', segments[..n]) + "/** text eol=lf");
            Assert.True(covering.Any(attributes.Contains), directory + " has no text eol=lf line in .gitattributes");
        }

        foreach (var pinText in text.Where(static p => Path.GetDirectoryName(p) == SharedTree.Pins).Select(SharedTree.Relative))
            Assert.Contains(pinText + " text eol=lf", attributes);
    }

    [Fact]
    public void SL10_The_witnesses_are_refused()
    {
        var directives = SharedRules.SL10Directives("w.cs", Witness.Text("SL10-preprocessor-directive.cs.witness")).ToList();
        Assert.Equal(["w.cs:7: #if DEBUG", "w.cs:9: #else", "w.cs:11: #endif", "w.cs:13: #pragma warning disable CS0618", "w.cs:15: #pragma warning restore CS0618"], directives);

        Assert.NotEmpty(SharedRules.SL10LineEndings("w.cs", File.ReadAllBytes(Witness.PathOf("SL10-crlf-line-endings.cs.witness"))));
    }
}

/// <summary>What SL9 runs Unicode's test files through, each answer rendered as text.</summary>
internal interface IUnicodeWork
{
    /// <summary>One normalization case: the status and the code units of the result.</summary>
    string Normalize(string text, NormalForm form);

    /// <summary>One break-test case: the boundaries, in order.</summary>
    string Boundaries(string text, SegmentGranularity granularity);
}

/// <summary>SL9's subject: Broiler.Unicode.Algorithms' normalization and Broiler.Unicode.Segmentation's segmenter, over Broiler.Unicode.Tables.</summary>
internal sealed class PackagesWork : IUnicodeWork
{
    private readonly UnicodeNormalization normalization = new(UnicodeTables.Instance);

    private readonly UnicodeSegmenter segmenter = new(UnicodeTables.Instance);

    public string Normalize(string text, NormalForm form)
    {
        var status = normalization.Transform(text, form, int.MaxValue, null, out var result);
        var rendered = new StringBuilder().Append(status).Append(':');
        foreach (var unit in result ?? string.Empty)
            rendered.Append(((int)unit).ToString("X4", CultureInfo.InvariantCulture));
        return rendered.ToString();
    }

    public string Boundaries(string text, SegmentGranularity granularity) =>
        string.Join(",", segmenter.Boundaries(text, granularity, null).Select(static b => b.ToString(CultureInfo.InvariantCulture)));
}

/// <summary>
/// SL9's witness: a producer whose answer depends on process history, a counter, which is what
/// convention 6 forbids. Two runs through it differ.
/// </summary>
internal sealed class NondeterministicWorkWitness : IUnicodeWork
{
    private int calls;

    public string Normalize(string text, NormalForm form) => (++calls).ToString(CultureInfo.InvariantCulture);

    public string Boundaries(string text, SegmentGranularity granularity) => (++calls).ToString(CultureInfo.InvariantCulture);
}

/// <summary>
/// SL7's witness: the committed generated source with one byte of a table changed by hand - the first
/// digit of CasedRanges' data moved by one, which leaves the file valid C# - the edit that belongs in the
/// generator or the pin and that the rule exists to find.
/// </summary>
internal static class HandEditedTableWitness
{
    internal static byte[] Edit(byte[] committed)
    {
        var changed = (byte[])committed.Clone();
        var table = changed.AsSpan().IndexOf("private static ReadOnlySpan<byte> CasedRanges => new byte[]"u8);
        if (table < 0)
            throw new InvalidOperationException("the committed source has no CasedRanges table to edit");
        var at = table + changed.AsSpan(table).IndexOfAnyInRange((byte)'0', (byte)'9');
        changed[at] = changed[at] == (byte)'9' ? (byte)'8' : (byte)(changed[at] + 1);
        return changed;
    }
}
