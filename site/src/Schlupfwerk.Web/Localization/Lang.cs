namespace Schlupfwerk.Web.Localization;

/// <summary>The two languages of the site. The URL always starts with the language code.</summary>
public enum Lang
{
    Pt,
    En,
}

public static class LangExtensions
{
    /// <summary>The code used in URLs, in localStorage and in the text file names: "pt" or "en".</summary>
    public static string Code(this Lang lang) => lang == Lang.Pt ? "pt" : "en";

    /// <summary>The value for the html lang and hreflang attributes.</summary>
    public static string HtmlLang(this Lang lang) => lang == Lang.Pt ? "pt-BR" : "en";

    public static Lang Other(this Lang lang) => lang == Lang.Pt ? Lang.En : Lang.Pt;

    public static bool TryParse(string? code, out Lang lang)
    {
        switch (code?.ToLowerInvariant())
        {
            case "pt":
                lang = Lang.Pt;
                return true;
            case "en":
                lang = Lang.En;
                return true;
            default:
                lang = Lang.En;
                return false;
        }
    }
}
