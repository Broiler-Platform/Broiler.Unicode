// REFERENCE ARM V: a test-only copy of Broiler.VM's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.VM at 24632b1 (the product sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at its revision 16a35b2):
//   src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeCaseFolding.cs
//     blob 53c4de8d631ba773b2d9e43f92ddb53dd2f63318: the whole file: JsUnicodeCaseFolding (SimpleFold, Orbit, SimpleUpper, UpperOrbit and their helpers; :57-302)
//
// Changes from the source: none. Every byte after the blank line that ends this header is the blob's: the
// namespace, the assurance header and annotations included, so that the generated half of each partial class
// compiles against it unchanged.

// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           0
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// Unicode 17.0.0 simple case folding - <c>CaseFolding.txt</c>'s status <c>C</c> and <c>S</c>
/// mappings - over the whole code space, and the reverse relation a case-insensitive class needs.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the table decision JSD-0031 section 7 moves the <c>u</c>-mode Canonicalize onto.</b>
/// ES2026 Canonicalize under <c>u</c> (without <c>v</c>) is "the simple or common case folding
/// mapping" of <c>CaseFolding.txt</c>, or the character itself. <c>JsRegExpCase</c> reads
/// <see cref="SimpleFold"/> as that Canonicalize and <see cref="Orbit"/> as its reverse, for
/// literals, classes, back-references and property escapes alike, so one pattern cannot answer two
/// ways (slice U4, JSeal F09).
/// </para>
/// <para>
/// <b>The non-<c>u</c> Canonicalize reads here too.</b> Without <c>u</c> or <c>v</c>, ES2026
/// Canonicalize upper-cases one code unit and keeps the code unit when the result is not one code
/// unit or would map a non-ASCII code unit to ASCII. <see cref="SimpleUpper"/> is that mapping
/// over UnicodeData.txt's simple upper-case field, those two rules applied by the generator, and
/// <see cref="UpperOrbit"/> its reverse (JSeal slice JSD-0031-later). The specification's
/// upper-casing is the full one; the simple field agrees with it except for the 27 Greek letters
/// with a ypogegrammeni or prosgegrammeni (U+1F80 to U+1FAF's lower-case halves, U+1FB3, U+1FC3,
/// U+1FF3), whose full upper case is two code points and which therefore canonicalize to
/// themselves in the language. <c>SpecialCasing.txt</c>, which would say so, is not archived, so
/// those 27 map to their title-case partner here, as they did on the platform's data before.
/// </para>
/// <para>
/// <b>Invariants the generator checks.</b> Folding is idempotent: no folding target has a folding
/// of its own, so <c>SimpleFold(SimpleFold(c)) == SimpleFold(c)</c>. Status <c>F</c> (full) and
/// <c>T</c> (Turkic) rows are not read, so U+0130 and U+0131 fold to themselves. The non-<c>u</c>
/// mapping is idempotent in the same way, and every code point with a simple upper case is
/// <c>Changes_When_Uppercased</c>.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F79A49
// Broiler-Human:        PENDING
internal static partial class JsUnicodeCaseFolding
{
    /// <summary>The simple case folding of <paramref name="codePoint"/>, or the code point itself.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9517DB
    // Broiler-Human:        PENDING
    internal static int SimpleFold(int codePoint)
    {
        var entry = Search(codePoint);

        return entry < 0 ? codePoint : JsUnicodeProperties.ReadInt24(FoldData, (entry * 6) + 3);
    }

    /// <summary>
    /// Writes every code point whose simple folding equals that of <paramref name="codePoint"/> -
    /// the folding itself and <paramref name="codePoint"/> included - into
    /// <paramref name="destination"/> in ascending order, and answers how many there are.
    /// </summary>
    /// <remarks>
    /// A code point with no folding and that nothing folds to answers one: itself. The destination
    /// must hold <see cref="MaxOrbit"/> entries; a shorter one throws rather than truncating.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0C7324
    // Broiler-Human:        PENDING
    internal static int Orbit(int codePoint, Span<int> destination)
    {
        if (destination.Length < MaxOrbit)
        {
            throw new ArgumentException("the destination is shorter than MaxOrbit", nameof(destination));
        }

        var target = SimpleFold(codePoint);
        var count = 0;

        destination[count++] = target;

        // The orbit index is ordered by folding, so the members sharing this one are contiguous:
        // find the first, then read forward while the folding still matches.
        var low = 0;
        var high = (OrbitIndex.Length / 2) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (OrbitTarget(middle) < target)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        for (var position = low; position < OrbitIndex.Length / 2 && OrbitTarget(position) == target; position++)
        {
            destination[count++] = JsUnicodeProperties.ReadInt24(FoldData, OrbitEntry(position) * 6);
        }

        // At most MaxOrbit entries: an insertion sort is the whole of the ordering needed.
        for (var sorted = 1; sorted < count; sorted++)
        {
            var value = destination[sorted];
            var slot = sorted - 1;

            while (slot >= 0 && destination[slot] > value)
            {
                destination[slot + 1] = destination[slot];
                slot--;
            }

            destination[slot + 1] = value;
        }

        return count;
    }

    /// <summary>How many code points have a simple folding other than themselves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7D8A08
    // Broiler-Human:        PENDING
    internal static int FoldCount => FoldData.Length / 6;

    /// <summary>The code point of fold entry <paramref name="entry"/>, in code point order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=78609B
    // Broiler-Human:        PENDING
    internal static int FoldSource(int entry) => JsUnicodeProperties.ReadInt24(FoldData, entry * 6);

    /// <summary>The simple folding of fold entry <paramref name="entry"/>'s code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3674F3
    // Broiler-Human:        PENDING
    internal static int FoldTarget(int entry) => JsUnicodeProperties.ReadInt24(FoldData, (entry * 6) + 3);

    /// <summary>
    /// The non-<c>u</c> Canonicalize of one code unit: UnicodeData.txt's simple upper case, or the
    /// code unit itself where it has none or where the language keeps it (a mapping to ASCII from
    /// outside it). Anything above the Basic Multilingual Plane answers itself.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=70B120
    // Broiler-Human:        PENDING
    internal static int SimpleUpper(int codeUnit)
    {
        var entry = UpperSearch(codeUnit);

        return entry < 0 ? codeUnit : UpperTarget(entry);
    }

    /// <summary>
    /// Writes every code unit whose non-<c>u</c> canonical form equals that of
    /// <paramref name="codeUnit"/> - the form itself and <paramref name="codeUnit"/> included -
    /// into <paramref name="destination"/> in ascending order, and answers how many there are.
    /// </summary>
    /// <remarks>The destination must hold <see cref="MaxUpperOrbit"/> entries; a shorter one throws.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8733F5
    // Broiler-Human:        PENDING
    internal static int UpperOrbit(int codeUnit, Span<int> destination)
    {
        if (destination.Length < MaxUpperOrbit)
        {
            throw new ArgumentException("the destination is shorter than MaxUpperOrbit", nameof(destination));
        }

        var target = SimpleUpper(codeUnit);
        var count = 0;
        var entries = UpperOrbitIndex.Length / 2;

        destination[count++] = target;

        var low = 0;
        var high = entries - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (UpperTarget(UpperOrbitEntry(middle)) < target)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        for (var position = low; position < entries && UpperTarget(UpperOrbitEntry(position)) == target; position++)
        {
            destination[count++] = UpperSource(UpperOrbitEntry(position));
        }

        // The index lists the other members in ascending order and the form went first, so one
        // pass carries the form to its place.
        for (var sorted = 1; sorted < count && destination[sorted] < destination[sorted - 1]; sorted++)
        {
            (destination[sorted], destination[sorted - 1]) = (destination[sorted - 1], destination[sorted]);
        }

        return count;
    }

    /// <summary>The UpperData entry for <paramref name="codeUnit"/>, or -1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E8D15D
    // Broiler-Human:        PENDING
    private static int UpperSearch(int codeUnit)
    {
        var low = 0;
        var high = (UpperData.Length / 4) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var source = UpperSource(middle);

            if (source == codeUnit)
            {
                return middle;
            }

            if (source < codeUnit)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    /// <summary>The code unit UpperData entry <paramref name="entry"/> moves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=812C7F
    // Broiler-Human:        PENDING
    private static int UpperSource(int entry) => UpperData[entry * 4] | (UpperData[(entry * 4) + 1] << 8);

    /// <summary>The canonical form UpperData entry <paramref name="entry"/> gives.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9AA3AF
    // Broiler-Human:        PENDING
    private static int UpperTarget(int entry) => UpperData[(entry * 4) + 2] | (UpperData[(entry * 4) + 3] << 8);

    /// <summary>The UpperData entry number at <paramref name="position"/> of the upper orbit index.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D3BA25
    // Broiler-Human:        PENDING
    private static int UpperOrbitEntry(int position) => UpperOrbitIndex[position * 2] | (UpperOrbitIndex[(position * 2) + 1] << 8);

    /// <summary>The FoldData entry for <paramref name="codePoint"/>, or -1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4B0CA6
    // Broiler-Human:        PENDING
    private static int Search(int codePoint)
    {
        var low = 0;
        var high = (FoldData.Length / 6) - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;
            var source = JsUnicodeProperties.ReadInt24(FoldData, middle * 6);

            if (source == codePoint)
            {
                return middle;
            }

            if (source < codePoint)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    /// <summary>The FoldData entry number at <paramref name="position"/> of the orbit index.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EDA0B6
    // Broiler-Human:        PENDING
    private static int OrbitEntry(int position) => OrbitIndex[position * 2] | (OrbitIndex[(position * 2) + 1] << 8);

    /// <summary>The folding of the entry at <paramref name="position"/> of the orbit index.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=423D03
    // Broiler-Human:        PENDING
    private static int OrbitTarget(int position) =>
        JsUnicodeProperties.ReadInt24(FoldData, (OrbitEntry(position) * 6) + 3);
}
