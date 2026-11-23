using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Schlupfwerk.Web.Layout;
using Schlupfwerk.Web.Localization;
using Schlupfwerk.Web.Pages;

namespace Schlupfwerk.Web.Tests;

public sealed class LayoutAndRedirectTests : BunitContext
{
    public LayoutAndRedirectTests()
    {
        Services.AddSingleton<Texts>();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<MainLayout> RenderLayoutAt(string path)
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(path);
        return Render<MainLayout>(p => p.Add(x => x.Body, (RenderFragment)(b => b.AddContent(0, "body"))));
    }

    [Fact]
    public void LanguageSwitch_PointsToTheSamePageInTheOtherLanguage()
    {
        var cut = RenderLayoutAt("/pt/sobre");

        var links = cut.FindAll(".lang-switch a");
        Assert.Equal("/pt/sobre", links[0].GetAttribute("href"));
        Assert.Equal("/en/about", links[1].GetAttribute("href"));
        Assert.Equal("true", links[0].GetAttribute("aria-current"));
    }

    [Fact]
    public void Menu_MarksTheCurrentPage_InTheUrlLanguage()
    {
        var cut = RenderLayoutAt("/en/games");

        var current = cut.Find(".main-nav a[aria-current=page]");
        Assert.Equal("/en/games", current.GetAttribute("href"));
        Assert.Equal("Games", current.TextContent);
    }

    [Fact]
    public void Layout_SetsTheHtmlLang()
    {
        RenderLayoutAt("/pt");

        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "schlupfwerk.setDocumentLang" && Equals(i.Arguments[0], "pt-BR"));
    }

    [Fact]
    public void ChoosingALanguage_SavesIt()
    {
        var cut = RenderLayoutAt("/pt");

        cut.FindAll(".lang-switch a")[1].Click();

        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "schlupfwerk.saveLang" && Equals(i.Arguments[0], "en"));
    }

    [Theory]
    [InlineData("/", "pt", "/pt")]
    [InlineData("/", "en", "/en")]
    [InlineData("/", null, "/en")]
    [InlineData("/cafe", "pt", "/pt/cafe")]
    public void Root_RedirectsToThePreferredLanguage(string from, string? preferred, string expected)
    {
        JSInterop.Setup<string?>("schlupfwerk.preferredLang").SetResult(preferred);
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo(from);

        Render<LanguageRedirect>();

        Assert.Equal(expected, "/" + nav.ToBaseRelativePath(nav.Uri));
    }
}
