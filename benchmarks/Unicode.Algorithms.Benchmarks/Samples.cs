namespace Broiler.Unicode.Algorithms.Benchmarks;

/// <summary>
/// The sample strings of the SJL-7 Unicode workloads (Broiler.JS's <c>UnicodeBenchmarks</c>, which uses
/// Broiler.VM's), written as escapes so that no editor or tool can change their form.
/// </summary>
internal static class Samples
{
    /// <summary>
    /// <c>normalize</c>'s samples: Latin composed and decomposed, Hangul syllables and jamo, compatibility
    /// characters, Greek with diacritics, combining marks out of order, supplementary-plane mappings,
    /// ASCII and half-width forms.
    /// </summary>
    internal static readonly string[] Normalize =
    [
        "été café",
        "été café",
        "가나다라",
        "가나다",
        "ﬁﬂ Ω Å ㌀",
        "ᾀᾳΐ άέ",
        "Ǻ q̣̇ ẛ̣",
        "\U0001D15E\U0001D15F",
        "plain ascii text 0123",
        "ｶﾞﾊﾟ ½ ①",
    ];

    /// <summary>
    /// Casing's samples: ASCII, Latin with diacritics and sharp s, Greek with final sigma, Cyrillic,
    /// Armenian and Latin ligatures, supplementary-plane letters, Turkic and Lithuanian letters, and
    /// title-case letters.
    /// </summary>
    internal static readonly string[] Casing =
    [
        "The Quick Brown Fox",
        "Straße ÄÖÜ äöü",
        "ΑΣΤΡΟΣ λόγος",
        "Привет мир",
        "և ﬀﬁ ŉ",
        "\U00010400\U00010401 \U00010D50\U00010D51",
        "İstanbul ışık",
        "İ J̀ ÌÍ",
        "plain ascii text 0123",
        "ǅǈǋ ᾀᾈ",
    ];
}
