using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Broiler.Unicode.Architecture.Tests;

/// <summary>What a project of the programme in this repository is, for the rules that hold it.</summary>
internal enum SharedProjectKind
{
    /// <summary>
    /// A library the engines consume: Broiler.Unicode.Algorithms and Broiler.Unicode.Segmentation. Every
    /// rule holds it.
    /// </summary>
    Library,

    /// <summary>
    /// A library holding generated data, Broiler.Unicode.Tables: the tables the generator writes, for this
    /// repository's tests and probe and for consumers other than the engines, which embed their own
    /// (decision SJL-D-6 (c)). Every rule but SL8 holds it; the assurance scheme covers the packages the
    /// engines consume and the generator.
    /// </summary>
    Tables,

    /// <summary>
    /// The test-only generator package, Broiler.Unicode.Generation. SL1, SL2, SL4, SL5, SL6, SL8 and SL10
    /// hold it. SL3 does not: the package carries this repository's pin as an embedded resource and reads
    /// it through <c>Assembly.GetManifestResourceStream</c> and a <c>StreamReader</c>, which SL3's
    /// reflection and I/O clauses refuse (<c>UnicodePin.Owner</c>, for Broiler.VM's rule I4); a test holds
    /// the exemption to exactly those findings. SL5 holds it to everything but the Native AOT and trim
    /// properties, since it is not Native AOT code and declares neither flag of convention 12. It reaches
    /// each engine's tests only, never a product closure. SL7 holds what it writes and SL9 the libraries
    /// the engines consume, neither of which is the generator itself.
    /// </summary>
    Generator,

    /// <summary>The Native AOT probe: an executable. SL3 but for I/O, SL5's probe properties and SL10 hold it.</summary>
    Probe,

    /// <summary>A test project. SL10 holds it.</summary>
    Test,
}

/// <summary>One project of the programme, read from its project file; <see cref="Name"/> is its assembly's.</summary>
internal sealed record SharedProject(string Name, string Path, SharedProjectKind Kind, XDocument Document)
{
    internal string Directory => System.IO.Path.GetDirectoryName(Path)!;

    internal IEnumerable<string> Items(string itemType) => Document.Descendants(itemType)
        .Select(static element => (string?)element.Attribute("Include"))
        .Where(static include => include is not null)
        .Select(static include => include!);
}

/// <summary>
/// The repository as the rules read it: the programme's projects, the documents and pin text beside
/// them, and the build output the test project's references put beside this assembly.
/// </summary>
/// <remarks>
/// Broiler.JS keeps the programme's projects in one folder; this repository keeps them beside its older
/// packages, so the rules read a set of directories instead: <c>src/Unicode.*</c>, <c>probes/*</c> and
/// <c>tests/Unicode.*</c>. The older projects, <c>src/UnicodeProperties</c>, <c>src/UnicodeEmoji.*</c>,
/// <c>src/UnicodeCldr.*</c> and their tests, are named without the dot, are not the programme's, and
/// nothing here reads them. A new project in one of these directories is read the day it is added, so a
/// new library cannot pass by being unknown: SL1's layering table has no row for it.
/// </remarks>
internal static class SharedTree
{
    /// <summary>The repository root: the first directory above the test assembly that holds the solution.</summary>
    internal static string Root { get; } = FindRoot();

    internal static string TestProjectDirectory => Path.Combine(Root, "tests", "Unicode.Architecture.Tests");

    internal static string Witnesses => Path.Combine(TestProjectDirectory, "witnesses");

    /// <summary>The programme's documents, which SL10 holds to LF line endings.</summary>
    internal static string Documents => Path.Combine(Root, "docs");

    /// <summary>The pin's directory. Only its own text is read here: <see cref="IsPinText"/>; the archived files are binary and never read.</summary>
    internal static string Pins => Path.Combine(Root, "data", "unicode", "pins");

    /// <summary>Where the programme's project directories are, and the prefix each name in it carries.</summary>
    private static readonly (string Parent, string Prefix)[] ProjectAreas = [("src", "Unicode."), ("probes", string.Empty), ("tests", "Unicode.")];

    /// <summary>The libraries that are not consumed by the engines, by assembly name, with their kind; every other library is.</summary>
    private static readonly FrozenDictionary<string, SharedProjectKind> LibraryKinds = new Dictionary<string, SharedProjectKind>
    {
        ["Broiler.Unicode.Tables"] = SharedProjectKind.Tables,
        ["Broiler.Unicode.Generation"] = SharedProjectKind.Generator,
    }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>Every directory that holds a project of the programme, in ordinal order.</summary>
    internal static IReadOnlyList<string> ProjectDirectories { get; } = ProjectAreas
        .SelectMany(static area => Directory.EnumerateDirectories(Path.Combine(Root, area.Parent))
            .Where(directory => Path.GetFileName(directory).StartsWith(area.Prefix, StringComparison.Ordinal)))
        .Order(StringComparer.Ordinal)
        .ToList();

    /// <summary>Every project of the programme, in ordinal order of assembly name.</summary>
    internal static IReadOnlyList<SharedProject> Projects { get; } = ReadProjects();

    /// <summary>Every library: a project that is neither the probe nor a test, whatever its kind.</summary>
    internal static IReadOnlyList<SharedProject> Libraries { get; } =
        Projects.Where(static p => p.Kind is SharedProjectKind.Library or SharedProjectKind.Tables or SharedProjectKind.Generator).ToList();

    internal static SharedProject Project(string name) => Projects.Single(p => p.Name == name);

    /// <summary>The libraries rule <paramref name="rule"/> holds.</summary>
    internal static IEnumerable<SharedProject> LibrariesHeldBy(string rule) => Libraries.Where(p => Holds(p.Kind, rule));

    /// <summary>Whether rule <paramref name="rule"/> holds a project of <paramref name="kind"/>; the reasons are on <see cref="SharedProjectKind"/>.</summary>
    internal static bool Holds(SharedProjectKind kind, string rule) => kind switch
    {
        SharedProjectKind.Library => true,
        SharedProjectKind.Tables => rule != "SL8",
        SharedProjectKind.Generator => rule is "SL1" or "SL2" or "SL4" or "SL5" or "SL6" or "SL8" or "SL10",
        SharedProjectKind.Probe => rule is "SL3" or "SL5" or "SL10",
        SharedProjectKind.Test => rule == "SL10",
        _ => false,
    };

    /// <summary>
    /// The files a rule about sources reads: every file of the programme's project directories outside
    /// build output and outside the witnesses, which exist to break the rules; the documents; and the
    /// pin's own text.
    /// </summary>
    internal static IEnumerable<string> SourceFiles(string pattern) =>
        ProjectDirectories.Append(Documents)
            .SelectMany(directory => Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
            .Concat(Directory.EnumerateFiles(Pins, pattern, SearchOption.TopDirectoryOnly).Where(IsPinText))
            .Where(static path => !IsExcluded(path))
            .Order(StringComparer.Ordinal);

    internal static string Relative(string path) =>
        Path.GetRelativePath(Root, path).Replace(Path.DirectorySeparatorChar, '/');

    /// <summary>The built assembly of a library, from this test's output directory.</summary>
    internal static string AssemblyPath(string assemblyName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, assemblyName + ".dll");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                assemblyName + ".dll is not in the test output. Every library of the programme is referenced by "
                + "this project so that the rules read the build that just happened.", path);
        }

        return path;
    }

    /// <summary>The pin's own text beside the archive: its README and the pin file. The archive's files are binary.</summary>
    private static bool IsPinText(string path) =>
        path.EndsWith(".md", StringComparison.Ordinal) || Path.GetFileName(path) == "unicode.pin";

    private static bool IsExcluded(string path)
    {
        var relative = Relative(path);
        var segments = relative.Split('/');
        return segments.Any(static s => s is "bin" or "obj" or "TestResults")
            || relative.StartsWith("tests/Unicode.Architecture.Tests/witnesses/", StringComparison.Ordinal);
    }

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Broiler.Unicode.slnx")))
                return directory.FullName;
        }

        throw new DirectoryNotFoundException("no Broiler.Unicode.slnx above " + AppContext.BaseDirectory);
    }

    private static List<SharedProject> ReadProjects()
    {
        var projects = new List<SharedProject>();
        foreach (var path in ProjectDirectories.SelectMany(static d => Directory.EnumerateFiles(d, "*.csproj", SearchOption.TopDirectoryOnly)))
        {
            var document = XDocument.Load(path);
            var assemblyName = document.Descendants("AssemblyName").Select(static e => e.Value.Trim()).FirstOrDefault();
            var name = string.IsNullOrEmpty(assemblyName) ? Path.GetFileNameWithoutExtension(path) : assemblyName;
            var isTest = document.Descendants("IsTestProject").Any(static e => e.Value.Trim() == "true");
            var isExe = document.Descendants("OutputType").Any(static e => e.Value.Trim() == "Exe");
            var kind = isTest ? SharedProjectKind.Test
                : isExe ? SharedProjectKind.Probe
                : LibraryKinds.GetValueOrDefault(name, SharedProjectKind.Library);
            projects.Add(new SharedProject(name, path, kind, document));
        }

        projects.Sort(static (a, b) => string.CompareOrdinal(a.Name, b.Name));
        return projects;
    }
}
