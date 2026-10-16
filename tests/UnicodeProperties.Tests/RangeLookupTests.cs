using Broiler.Unicode.Properties;
using Xunit;

namespace Broiler.Unicode.Tests;

/// <summary>
/// Broiler.Unicode's roadmap, items U2 and U6: no lookup hands out a mutable reference to shared data, and
/// the package states its Unicode version.
/// </summary>
public class RangeLookupTests
{
    [Fact]
    public void A_caller_that_changes_its_ranges_does_not_change_the_next_callers()
    {
        var first = UnicodeProperties.GetBinaryProperty("ASCII")!;
        var expected = ((int, int)[])first.Clone();

        first[0] = (0x10FFFF, 0x10FFFF);

        Assert.Equal(expected, UnicodeProperties.GetBinaryProperty("ASCII"));
        Assert.NotSame(UnicodeProperties.GetScript("Latin"), UnicodeProperties.GetScript("Latin"));
        Assert.NotSame(UnicodeProperties.GetScriptExtensions("Latin"), UnicodeProperties.GetScriptExtensions("Latin"));
        Assert.NotSame(UnicodeProperties.GetGeneralCategory("Lu"), UnicodeProperties.GetGeneralCategory("Lu"));
    }

    [Fact]
    public void The_package_states_its_Unicode_version()
    {
        Assert.Equal("17.0.0", UnicodeProperties.UnicodeVersion);
    }
}
