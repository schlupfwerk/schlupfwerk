# 0009 — A real 404 status for unknown paths

- Status: Proposed ([PR #1](https://github.com/schlupfwerk/schlupfwerk/pull/1), to be checked on its preview)
- Date: 2026-11-23

## Context

The first version used SWA's `navigationFallback`, which serves `index.html` with **200** for any path.
Blazor then showed the NotFound page, but to a search engine `/pt/nao-existe` looked like a real page
(a "soft 404"), and typos or old links could get indexed.

## Decision

- **No `navigationFallback`** in `staticwebapp.config.json`.
- Each page (every `SiteRoutes` path in both languages, plus the short link `/cafe`) is an explicit route
  that rewrites to `/index.html` (200).
- Everything else falls into `responseOverrides` → `404` → rewrite to `/index.html` with **status 404**;
  Blazor renders the NotFound page as before.
- `trailingSlash: "never"`, so `/pt/sobre/` redirects to `/pt/sobre` instead of becoming a 404.
- A test checks the rewrite routes against `SiteRoutes`, like the sitemap test.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Explicit routes + 404 override** ✅ | Real status for search engines; same page for visitors | Every new page needs its routes (enforced by a test) |
| Keep `navigationFallback` | Zero maintenance | Soft 404s |
| Wildcards (`/pt/*`, `/en/*`) | Fewer lines | Unknown paths under a language would still answer 200 |
| A separate static `404.html` | Doesn't load the app | A second, non-Blazor page to keep in two languages and in the site's style |

## Consequences

- ✅ Unknown paths answer 404 while still showing the site's own NotFound page
- ⚠️ A missing asset (e.g. a wrong image path) also gets `index.html` with 404 instead of an empty 404;
  harmless, but it shows up as HTML in the network panel
- ⚠️ This changes server routing, which can't be run locally; check it on the PR preview before merging
