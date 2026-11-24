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
    public void StaticWebAppConfig_ServesEveryPage_AndUnknownPathsAs404()
    {
        using var json = ReadStaticWebAppConfig();
        var root = json.RootElement;

        // No navigation fallback: it would answer 200 for any path, so search engines would index
        // the "not found" page. Known pages are listed one by one instead.
        Assert.False(root.TryGetProperty("navigationFallback", out _));

        var rewritten =
            from route in root.GetProperty("routes").EnumerateArray()
            where route.TryGetProperty("rewrite", out var target) && target.GetString() == "/index.html"
            select route.GetProperty("route").GetString();

        var expected =
            (from page in SiteRoutes.AllPages
             from lang in new[] { Lang.Pt, Lang.En }
             select SiteRoutes.PathOf(page, lang))
            .Append("/cafe"); // short link, redirected to the visitor's language by js/site.js

        Assert.Equal(expected.Order(), rewritten.Order());

        // Anything else gets the app (which shows the NotFound page) with a real 404 status.
        var notFound = root.GetProperty("responseOverrides").GetProperty("404");
        Assert.Equal("/index.html", notFound.GetProperty("rewrite").GetString());
        Assert.Equal(404, notFound.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public void StaticWebAppConfig_HasTheCspPlaceholder()
    {
        using var json = ReadStaticWebAppConfig();
        var root = json.RootElement;

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
    public void BrandFiles_ArePublished(string file) =>
        Assert.True(File.Exists(Path.Combine(SiteFiles.WebRoot, file)), file);

    private static JsonDocument ReadStaticWebAppConfig() =>
        JsonDocument.Parse(File.ReadAllText(Path.Combine(SiteFiles.WebRoot, "staticwebapp.config.json")));
}
