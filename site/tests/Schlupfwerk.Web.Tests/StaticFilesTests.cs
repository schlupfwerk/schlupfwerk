using System.Text.Json;
using System.Xml.Linq;
using Schlupfwerk.Web.Localization;

namespace Schlupfwerk.Web.Tests;

public sealed class StaticFilesTests
{
    [Fact]
    public void PixQrCode_IsTheSameAsTheOneInTheReadme()
    {
        var site = File.ReadAllBytes(Path.Combine(SiteFiles.WebRoot, "img", "pix-qr.png"));
        var readme = File.ReadAllBytes(Path.Combine(SiteFiles.RepoRoot, "profile", "pix-qr.png"));

        Assert.Equal(readme, site);
    }

    [Fact]
    public void Sitemap_ListsEveryPageInBothLanguages()
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var sitemap = XDocument.Load(Path.Combine(SiteFiles.WebRoot, "sitemap.xml"));
        var listed = sitemap.Descendants(ns + "loc").Select(e => e.Value).Order();

        var expected =
            from page in SiteRoutes.AllPages
            from lang in new[] { Lang.Pt, Lang.En }
            select SiteRoutes.AbsoluteUrlOf(page, lang);

        Assert.Equal(expected.Order(), listed);
    }

    [Fact]
    public void StaticWebAppConfig_HasTheSpaFallback_AndTheCspPlaceholder()
    {
        var path = Path.Combine(SiteFiles.WebRoot, "staticwebapp.config.json");
        using var json = JsonDocument.Parse(File.ReadAllText(path));
        var root = json.RootElement;

        Assert.Equal("/index.html", root.GetProperty("navigationFallback").GetProperty("rewrite").GetString());
        var csp = root.GetProperty("globalHeaders").GetProperty("Content-Security-Policy").GetString();
        // tools/csp-hashes.cs swaps the placeholder for the hashes of the inline scripts at publish time.
        Assert.Contains("__INLINE_SCRIPT_HASHES__", csp, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("favicon.ico")]
    [InlineData("favicon.svg")]
    [InlineData("apple-touch-icon.png")]
    [InlineData("og-image.png")]
    [InlineData("img/logo-header.svg")]
    [InlineData("img/logo-header-dark.svg")]
    [InlineData("img/icon-192.png")]
    [InlineData("img/icon-512.png")]
    [InlineData("fonts/v1/nunito-latin-wght.woff2")]
    [InlineData("img/swanrace/swanrace-race.webp")]
    [InlineData("img/swanrace/swanrace-customize.webp")]
    [InlineData("img/swanrace/swanrace-title.webp")]
    public void BrandFiles_ArePublished(string file) =>
        Assert.True(File.Exists(Path.Combine(SiteFiles.WebRoot, file)), file);
}
