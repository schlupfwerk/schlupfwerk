using Microsoft.AspNetCore.Components;
using Schlupfwerk.Web.Localization;

namespace Schlupfwerk.Web.Components;

/// <summary>Base class for components that show text: gives them the current language and T(key).</summary>
public abstract class LocalizedComponent : ComponentBase
{
    /// <summary>Set by MainLayout from the first segment of the URL.</summary>
    [CascadingParameter]
    public Lang Lang { get; set; }

    [Inject]
    public required Texts Texts { get; set; }

    protected string T(string key) => Texts.Get(Lang, key);

    protected string PathOf(SitePage page) => SiteRoutes.PathOf(page, Lang);
}
