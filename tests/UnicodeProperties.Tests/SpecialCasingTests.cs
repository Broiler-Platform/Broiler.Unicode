using System.Globalization;
using Broiler.Unicode.Properties;
using Xunit;

namespace Broiler.Unicode.Tests;

public class SpecialCasingTests
{
    [Fact]
    public void Lone_Surrogate_With_Special_Casing_Does_Not_Throw_And_Preserves_Surrogate()
    {
        // Issue identified in docs/component-reuse-investigation.md:
        // "\uD800\u00DF" previously threw ArgumentException from char.ConvertToUtf32.
        var input = "\uD800\u00DF";
        var upper = JSStringSpecialCasing.ToUpper(input);
        Assert.Equal("\uD800SS", upper);
    }

    [Fact]
    public void Low_Surrogate_Lone_Surrogate_Preserved()
    {
        var input = "\uDC00\u00DF";
        var upper = JSStringSpecialCasing.ToUpper(input);
        Assert.Equal("\uDC00SS", upper);
    }

    [Fact]
    public void Greek_Sigma_With_Surrogate_Does_Not_Throw()
    {
        var input = "\uD800\u03A3\uDC00";
        var lower = JSStringSpecialCasing.ToLower(input);
        Assert.NotNull(lower);
    }

    [Fact]
    public void Standard_Special_Casing_Works()
    {
        Assert.Equal("STRASSE", JSStringSpecialCasing.ToUpper("straße"));
        Assert.Equal("strasse", JSStringSpecialCasing.ToLower("STRASSE"));
        Assert.Equal("i", JSStringSpecialCasing.ToLocaleLower("İ", new CultureInfo("tr-TR")));
        Assert.Equal("ı", JSStringSpecialCasing.ToLocaleLower("I", new CultureInfo("tr-TR")));
    }
}
