using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.Unicode.Generation;

/// <summary>One file a Unicode pin names: how the pin writes it, how long it is and what it hashes to.</summary>
/// <param name="Kind"><c>file</c> for a UCD file, an emoji file or the licence text; <c>spec-table</c> for an ECMAScript property table.</param>
/// <param name="Name">The path as the pin writes it: relative to the pin's directory for a <c>file</c>, to the repository's root for a <c>spec-table</c>.</param>
/// <param name="Bytes">The file's length.</param>
/// <param name="Sha256">The file's SHA-256, in lower-case hexadecimal.</param>
public sealed record UnicodePinEntry(string Kind, string Name, long Bytes, string Sha256)
{
    /// <summary>The file's name without its directories, which a pin comparison across repositories reads.</summary>
    public string FileName => Name[(Name.LastIndexOf('/') + 1)..];
}

/// <summary>
/// A Unicode data pin - Broiler.Unicode's <c>data/unicode/pins/unicode.pin</c>, or Broiler.VM's
/// <c>src/tests/unicode/pins/unicode.pin</c> it was copied from - parsed, and the checks that stand
/// between an archived byte and a generated table.
/// </summary>
/// <remarks>
/// <b>The generator reads nothing this type has not verified</b>: every file's length, SHA-256 and
/// version header is compared with what the pin records, a file the pin does not name is never read,
/// and a failure is an exception rather than a warning (Broiler.VM's JSD-0031 section 5, which this
/// port keeps). Each check is a pure function over bytes, so a test can hand it a changed byte.
/// </remarks>
public sealed class UnicodePin
{
    /// <summary>The one version the tables are generated from.</summary>
    public const string PinnedVersion = "17.0.0";

    private UnicodePin(IReadOnlyDictionary<string, string> values, IReadOnlyList<UnicodePinEntry> entries)
    {
        Values = values;
        Entries = entries;
    }

    /// <summary>The single-valued keys: <c>version</c>, <c>source</c>, <c>archived</c> and the rest.</summary>
    public IReadOnlyDictionary<string, string> Values { get; }

    /// <summary>Every file the pin names, in the order it names them.</summary>
    public IReadOnlyList<UnicodePinEntry> Entries { get; }

    /// <summary>The version line, or the empty string when there is none.</summary>
    public string Version => Values.TryGetValue("version", out var version) ? version : string.Empty;

    /// <summary>
    /// Broiler.Unicode's own pin, <c>data/unicode/pins/unicode.pin</c>, as this package carries it: the pin an
    /// engine's own must equal, by file name, length and hash (Broiler.VM's rule I4).
    /// </summary>
    public static UnicodePin Owner()
    {
        using var stream = typeof(UnicodePin).Assembly.GetManifestResourceStream("Broiler.Unicode.Generation.unicode.pin")
            ?? throw new InvalidDataException("this package does not carry Broiler.Unicode's pin");
        using var reader = new StreamReader(stream, new UTF8Encoding(false, true));

        return Parse(reader.ReadToEnd());
    }

    /// <summary>Parses pin text: <c>key value</c> lines, <c>#</c> comments, repeated <c>file</c> and <c>spec-table</c> lines.</summary>
    /// <param name="text">The pin's text.</param>
    /// <exception cref="InvalidDataException">A line is malformed or repeats a key.</exception>
    public static UnicodePin Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var entries = new List<UnicodePinEntry>();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] is "file" or "spec-table")
            {
                if (parts.Length != 4 || !long.TryParse(parts[2], out var bytes))
                {
                    throw new InvalidDataException($"unicode.pin: `{line}` is not `{parts[0]} <path> <bytes> <sha256>`");
                }

                entries.Add(new UnicodePinEntry(parts[0], parts[1], bytes, parts[3].ToLowerInvariant()));
                continue;
            }

            if (parts.Length < 2 || !values.TryAdd(parts[0], string.Join(' ', parts.Skip(1))))
            {
                throw new InvalidDataException($"unicode.pin: `{line}` is empty or repeats a key");
            }
        }

        return new UnicodePin(values, entries);
    }

    /// <summary>The entry the pin writes as <paramref name="name"/>.</summary>
    /// <param name="name">The path as the pin writes it.</param>
    public UnicodePinEntry Entry(string name) =>
        Entries.SingleOrDefault(entry => string.Equals(entry.Name, name, StringComparison.Ordinal))
        ?? throw new InvalidDataException($"unicode.pin names no file `{name}`");

    /// <summary>The ECMAScript property table the pin names whose file name is <paramref name="fileName"/>, wherever its repository keeps it.</summary>
    /// <param name="fileName">The table's file name, e.g. <c>table-binary-unicode-properties.html</c>.</param>
    public UnicodePinEntry SpecTable(string fileName) =>
        Entries.SingleOrDefault(entry => entry.Kind == "spec-table" && string.Equals(entry.FileName, fileName, StringComparison.Ordinal))
        ?? throw new InvalidDataException($"unicode.pin names no ECMAScript table `{fileName}`");

    /// <summary>
    /// Every way <paramref name="archive"/> differs from what the pin records: a missing file, a length, a
    /// hash, or a header that does not name the pinned version. Empty when the archive is the pinned one.
    /// </summary>
    /// <param name="archive">Each file's bytes, by the name the pin writes it under.</param>
    public IEnumerable<string> Violations(IReadOnlyDictionary<string, byte[]> archive)
    {
        ArgumentNullException.ThrowIfNull(archive);

        if (!string.Equals(Version, PinnedVersion, StringComparison.Ordinal))
        {
            yield return $"unicode.pin names version `{Version}`, and the tables are generated from {PinnedVersion} only";
        }

        if (!Values.TryGetValue("archived", out var archived) || archived != "yes")
        {
            yield return "unicode.pin does not say `archived yes`, and the generator reads only archived bytes";
        }

        foreach (var entry in Entries)
        {
            if (!archive.TryGetValue(entry.Name, out var bytes))
            {
                yield return $"{entry.Name} is pinned and no file is there";
                continue;
            }

            if (bytes.LongLength != entry.Bytes)
            {
                yield return $"{entry.Name} is {bytes.LongLength} bytes, and the pin records {entry.Bytes}";
            }

            var digest = Convert.ToHexStringLower(SHA256.HashData(bytes));

            if (!string.Equals(digest, entry.Sha256, StringComparison.Ordinal))
            {
                yield return $"{entry.Name} hashes to {digest}, and the pin records {entry.Sha256}";
            }

            if (HeaderViolation(entry.Name, bytes, PinnedVersion) is { } header)
            {
                yield return $"{entry.Name}: {header}";
            }
        }
    }

    /// <summary>The archive, verified, or an exception naming every way it is not the pinned one.</summary>
    /// <param name="archive">Each file's bytes, by the name the pin writes it under.</param>
    /// <exception cref="InvalidDataException">The archive is not the one the pin describes.</exception>
    public IReadOnlyDictionary<string, byte[]> Verified(IReadOnlyDictionary<string, byte[]> archive)
    {
        var violations = Violations(archive).ToArray();

        if (violations.Length > 0)
        {
            throw new InvalidDataException(
                "The Unicode archive is not the one unicode.pin describes, so nothing is generated from it:\n  " +
                string.Join("\n  ", violations));
        }

        return archive;
    }

    /// <summary>
    /// Every way this pin and <paramref name="other"/> differ as data: a file one names and the other does
    /// not, by file name, or a file both name with another length or hash, and the version. Empty when the
    /// two pin the same files by the same hashes, whatever their comments and paths.
    /// </summary>
    /// <param name="other">Another repository's pin, e.g. Broiler.VM's.</param>
    public IEnumerable<string> Differences(UnicodePin other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (!string.Equals(Version, other.Version, StringComparison.Ordinal))
        {
            yield return $"one pin names version `{Version}` and the other `{other.Version}`";
        }

        static string Key(UnicodePinEntry entry) => entry.Kind == "spec-table" ? "spec-table:" + entry.FileName : entry.Name;

        var mine = Entries.ToDictionary(Key, StringComparer.Ordinal);
        var theirs = other.Entries.ToDictionary(Key, StringComparer.Ordinal);

        foreach (var (key, entry) in mine.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (!theirs.TryGetValue(key, out var match))
            {
                yield return $"{key} is pinned here and not there";
            }
            else if (entry.Bytes != match.Bytes || !string.Equals(entry.Sha256, match.Sha256, StringComparison.Ordinal))
            {
                yield return $"{key} is {entry.Bytes} bytes hashing to {entry.Sha256} here and {match.Bytes} bytes hashing to {match.Sha256} there";
            }
        }

        foreach (var key in theirs.Keys.Where(key => !mine.ContainsKey(key)).Order(StringComparer.Ordinal))
        {
            yield return $"{key} is pinned there and not here";
        }
    }

    /// <summary>
    /// Why a UCD or emoji file's header does not name <paramref name="version"/>, or null when it does or
    /// the file has no version header (<c>UnicodeData.txt</c>, the licence text, the ECMAScript tables).
    /// Every UCD file but <c>UnicodeData.txt</c> opens <c># Name-17.0.0.txt</c>; the emoji files state
    /// <c># Version: 17.0</c> within their first lines.
    /// </summary>
    /// <param name="name">The path as the pin writes it.</param>
    /// <param name="bytes">The file's bytes.</param>
    /// <param name="version">The version, e.g. <c>17.0.0</c>.</param>
    public static string? HeaderViolation(string name, byte[] bytes, string version)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(version);

        var fileName = name[(name.LastIndexOf('/') + 1)..];

        if (!(name.StartsWith("ucd-", StringComparison.Ordinal) || name.StartsWith("emoji-", StringComparison.Ordinal)) ||
            string.Equals(fileName, "UnicodeData.txt", StringComparison.Ordinal))
        {
            return null;
        }

        var lines = Decode(bytes).Split('\n', 12);

        if (fileName.StartsWith("emoji-", StringComparison.Ordinal))
        {
            var shortVersion = version[..version.LastIndexOf('.')];

            return lines.Take(11).Any(line => string.Equals(line, $"# Version: {shortVersion}", StringComparison.Ordinal))
                ? null
                : $"the header does not state `# Version: {shortVersion}`";
        }

        var expected = $"# {Path.GetFileNameWithoutExtension(fileName)}-{version}.txt";

        return string.Equals(lines[0], expected, StringComparison.Ordinal)
            ? null
            : $"the first line is `{lines[0]}`, and a {version} file opens `{expected}`";
    }

    /// <summary>Strict UTF-8: a byte sequence that is not UTF-8 is a refusal, not a replacement character.</summary>
    /// <param name="bytes">The bytes.</param>
    public static string Decode(byte[] bytes) =>
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetString(bytes);
}
