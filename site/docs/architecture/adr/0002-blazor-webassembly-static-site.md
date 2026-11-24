# 0002 — A static site in Blazor WebAssembly on .NET 10

- Status: Accepted
- Date: 2026-11-23

## Context

The studio needs an institutional site: who we are, our values, our games, donations and a privacy
policy that the app stores require. It has no logged-in area and no data to store. Manuel works in
.NET and maintains the site alone, and the site must open fast on ordinary phones.

## Decision

- **Blazor WebAssembly standalone on .NET 10**, published as plain static files. No backend, no database.
- The SDK is pinned in `global.json`; package versions are central (`Directory.Packages.props`).
- **Quality gate** in `Directory.Build.props`: warnings are errors, analyzers and code style are on;
  CI also runs `dotnet format --verify-no-changes`. Tests use xUnit v3 and bUnit.
- **Small download:** invariant globalization, no time-zone data, default trimming
  (**not** `TrimMode=full`, which removes the site's own components and routes), Brotli served by the host.
- **Texts are data:** `Localization/texts.pt.json` and `texts.en.json`, embedded with
  `WithCulture="false"` (without it, the `.pt`/`.en` in the name turns them into satellite assemblies
  and the texts disappear).
- **SEO:** WebAssembly renders after download, so `index.html` carries the `<title>`, `meta description`
  and Open Graph tags itself, plus `sitemap.xml` and `robots.txt`.
- **Self-hosted font** (Nunito, OFL) in `wwwroot/fonts/v1/`. Files there are cached as immutable, so a new
  font file goes to `fonts/v2/`.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Blazor WASM, static** ✅ | Manuel's stack; tests in C#; free static hosting | Larger first download than plain HTML; content appears after WASM starts |
| Blazor Web App with server or prerendering | Better first paint and SEO | Needs a server or a prerender step; more moving parts for a small site |
| Plain HTML/CSS or a static generator | Smallest, best SEO | Another stack to maintain; no shared C# with the rest of Manuel's work |

## Consequences

- ✅ Free static hosting, no server to patch
- ✅ Routes, texts and the sitemap are checked by tests against one source (`SiteRoutes`)
- ⚠️ Search engines see the content only after WASM runs; if SEO turns out weak, static prerendering is the next step
