using Schlupfwerk.Web.Localization;

namespace Schlupfwerk.Web.Tests;

public sealed class TextsTests
{
    private readonly Texts _texts = new();

    [Fact]
    public void BothLanguages_HaveTheSameKeys()
    {
        var pt = _texts.All(Lang.Pt).Keys.ToHashSet();
        var en = _texts.All(Lang.En).Keys.ToHashSet();

        Assert.Empty(pt.Except(en));
        Assert.Empty(en.Except(pt));
    }

    [Theory]
    [InlineData(Lang.Pt)]
    [InlineData(Lang.En)]
    public void NoText_IsEmpty(Lang lang) =>
        Assert.DoesNotContain(_texts.All(lang), pair => string.IsNullOrWhiteSpace(pair.Value));

    [Fact]
    public void MissingKey_IsEasyToSpot() =>
        Assert.Equal("[[nope]]", _texts.Get(Lang.Pt, "nope"));

    [Fact]
    public void PixBeneficiary_IsTheFullCompanyName()
    {
        const string Company = "MANUEL ANDREZ BRAVO CANDIA CONSULTORIA EM TI LTDA";
        Assert.EndsWith(Company, _texts.Get(Lang.Pt, "cafe.pix.beneficiary"), StringComparison.Ordinal);
        Assert.EndsWith(Company, _texts.Get(Lang.En, "cafe.pix.beneficiary"), StringComparison.Ordinal);
    }
}
