using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Schlupfwerk.Web.Localization;
using Schlupfwerk.Web.Pages;

namespace Schlupfwerk.Web.Tests;

public sealed class PageRenderingTests : BunitContext
{
    public PageRenderingTests()
    {
        Services.AddSingleton<Texts>();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    public static TheoryData<Type, Lang> Pages()
    {
        var data = new TheoryData<Type, Lang>();
        foreach (var page in new[] { typeof(Home), typeof(About), typeof(Games), typeof(Cafe), typeof(Privacy), typeof(NotFound) })
        {
            data.Add(page, Lang.Pt);
            data.Add(page, Lang.En);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(Pages))]
    public void Page_RendersOneH1_AndNoMissingText(Type page, Lang lang)
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<Lang>>(0);
            builder.AddComponentParameter(1, "Value", lang);
            builder.AddComponentParameter(2, "ChildContent", (RenderFragment)(b =>
            {
                b.OpenComponent(0, page);
                b.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.DoesNotContain("[[", cut.Markup, StringComparison.Ordinal);
        Assert.Single(cut.FindAll("h1"));
    }

    [Theory]
    [InlineData(Lang.Pt, "/pt/sobre")]
    [InlineData(Lang.En, "/en/about")]
    public void Home_LinksToTheAboutPageInItsLanguage(Lang lang, string aboutPath)
    {
        var cut = Render<Home>(p => p.AddCascadingValue(lang));

        Assert.Contains(cut.FindAll("a"), a => a.GetAttribute("href") == aboutPath);
    }

    [Theory]
    [InlineData(Lang.Pt, "pix")]
    [InlineData(Lang.En, "kofi")]
    public void Cafe_ShowsTheVisitorsUsualOptionFirst(Lang lang, string first)
    {
        var cut = Render<Cafe>(p => p.AddCascadingValue(lang));

        Assert.Equal(first, cut.FindAll(".donate-card h2")[0].Id);
    }

    [Theory]
    [InlineData(Lang.Pt)]
    [InlineData(Lang.En)]
    public void Cafe_KofiButton_IsInTheCenteredActionRow(Lang lang)
    {
        var cut = Render<Cafe>(p => p.AddCascadingValue(lang));

        // .donate-action centers the button (app.css), like the Pix QR code in the other card.
        var button = cut.Find(".donate-action a.button");
        Assert.Equal("https://ko-fi.com/schlupfwerk", button.GetAttribute("href"));
    }

    [Fact]
    public void Cafe_ShowsThePixQrCode_WithTheBeneficiary()
    {
        var cut = Render<Cafe>(p => p.AddCascadingValue(Lang.Pt));

        Assert.Equal("img/pix-qr.png", cut.Find(".pix img").GetAttribute("src"));
        Assert.Contains("MANUEL ANDREZ BRAVO CANDIA CONSULTORIA EM TI LTDA", cut.Find(".pix figcaption").TextContent, StringComparison.Ordinal);
    }
}
