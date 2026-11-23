namespace Schlupfwerk.Web.Localization;

/// <summary>The site's pages. Each one has a path per language, e.g. /pt/sobre and /en/about.</summary>
public enum SitePage
{
    Home,
    About,
    Games,
    Cafe,
    Privacy,
}

/// <summary>
/// Single source of truth for page paths. The @page directives must match these paths;
/// a test checks that they do.
/// </summary>
public static class SiteRoutes
{
    public const string BaseUrl = "https://schlupfwerk.com";

    private static readonly Dictionary<SitePage, (string Pt, string En)> Slugs = new()
    {
        [SitePage.Home] = ("", ""),
        [SitePage.About] = ("sobre", "about"),
        [SitePage.Games] = ("jogos", "games"),
        [SitePage.Cafe] = ("cafe", "cafe"),
        [SitePage.Privacy] = ("privacidade", "privacy"),
    };

    public static IEnumerable<SitePage> AllPages => Slugs.Keys;

    /// <summary>The absolute path of a page, e.g. "/pt/sobre" or "/en".</summary>
    public static string PathOf(SitePage page, Lang lang)
    {
        var (pt, en) = Slugs[page];
        var slug = lang == Lang.Pt ? pt : en;
        return slug.Length == 0 ? $"/{lang.Code()}" : $"/{lang.Code()}/{slug}";
    }

    public static string AbsoluteUrlOf(SitePage page, Lang lang) => BaseUrl + PathOf(page, lang);

    /// <summary>The language from the first path segment, if it is "pt" or "en".</summary>
    public static Lang? LangOf(string relativePath)
    {
        var first = Segments(relativePath).FirstOrDefault();
        return LangExtensions.TryParse(first, out var lang) ? lang : null;
    }

    /// <summary>Finds the page for a base-relative path such as "pt/sobre" or "en/about?x=1".</summary>
    public static bool TryMatch(string relativePath, out SitePage page, out Lang lang)
    {
        var segments = Segments(relativePath);
        page = SitePage.Home;
        lang = Lang.En;
        if (segments.Length is < 1 or > 2 || !LangExtensions.TryParse(segments[0], out lang))
        {
            return false;
        }

        var slug = segments.Length == 2 ? segments[1] : "";
        foreach (var (candidate, (pt, en)) in Slugs)
        {
            if (string.Equals(slug, lang == Lang.Pt ? pt : en, StringComparison.OrdinalIgnoreCase))
            {
                page = candidate;
                return true;
            }
        }

        return false;
    }

    private static string[] Segments(string relativePath)
    {
        var path = relativePath.Split('?', '#')[0];
        return path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }
}
