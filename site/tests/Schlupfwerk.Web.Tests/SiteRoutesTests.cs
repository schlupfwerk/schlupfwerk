using System.Reflection;
using Microsoft.AspNetCore.Components;
using Schlupfwerk.Web.Localization;

namespace Schlupfwerk.Web.Tests;

public sealed class SiteRoutesTests
{
    /// <summary>Routes that are not content pages: the language redirect and the not-found page.</summary>
    private static readonly string[] SpecialRoutes = ["/", "/cafe", "/not-found"];

    private static IEnumerable<string> PageDirectives() =>
        typeof(App).Assembly.GetTypes()
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>())
            .Select(r => r.Template)
            .Where(t => !SpecialRoutes.Contains(t));

    private static IEnumerable<string> RoutePaths() =>
        from page in SiteRoutes.AllPages
        from lang in new[] { Lang.Pt, Lang.En }
        select SiteRoutes.PathOf(page, lang);

    [Fact]
    public void PageDirectives_MatchSiteRoutes_OneToOne() =>
        Assert.Equal(RoutePaths().Order(), PageDirectives().Order());

    [Theory]
    [InlineData(SitePage.Home, Lang.Pt, "/pt")]
    [InlineData(SitePage.Home, Lang.En, "/en")]
    [InlineData(SitePage.About, Lang.Pt, "/pt/sobre")]
    [InlineData(SitePage.About, Lang.En, "/en/about")]
    [InlineData(SitePage.Cafe, Lang.Pt, "/pt/cafe")]
    [InlineData(SitePage.Privacy, Lang.En, "/en/privacy")]
    public void PathOf_BuildsTheLocalizedPath(SitePage page, Lang lang, string expected) =>
        Assert.Equal(expected, SiteRoutes.PathOf(page, lang));

    [Fact]
    public void TryMatch_FindsEveryPathItBuilds()
    {
        foreach (var page in SiteRoutes.AllPages)
        {
            foreach (var lang in new[] { Lang.Pt, Lang.En })
            {
                var relative = SiteRoutes.PathOf(page, lang).TrimStart('/');
                Assert.True(SiteRoutes.TryMatch(relative, out var foundPage, out var foundLang), relative);
                Assert.Equal((page, lang), (foundPage, foundLang));
            }
        }
    }

    [Theory]
    [InlineData("en/about/?utm=x")]
    [InlineData("en/about#top")]
    [InlineData("EN/About")]
    public void TryMatch_IgnoresQueryHashAndCase(string relative)
    {
        Assert.True(SiteRoutes.TryMatch(relative, out var page, out var lang));
        Assert.Equal((SitePage.About, Lang.En), (page, lang));
    }

    [Theory]
    [InlineData("")]
    [InlineData("cafe")]
    [InlineData("de")]
    [InlineData("pt/about")]
    [InlineData("pt/sobre/extra")]
    public void TryMatch_RejectsUnknownPaths(string relative) =>
        Assert.False(SiteRoutes.TryMatch(relative, out _, out _));

    [Fact]
    public void LangOf_ReadsTheFirstSegment()
    {
        Assert.Equal(Lang.Pt, SiteRoutes.LangOf("pt/qualquer-coisa"));
        Assert.Equal(Lang.En, SiteRoutes.LangOf("en"));
        Assert.Null(SiteRoutes.LangOf("xyz"));
    }
}
