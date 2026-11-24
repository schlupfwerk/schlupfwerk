# 0005 — Two languages with translated paths, chosen in the browser

- Status: Accepted
- Date: 2026-11-23

## Context

The audience is Brazilian families and an international one. Each page must be linkable and indexable
in each language, and the visitor should land in their language without being asked. The site has no
server and promises no tracking ([0007](0007-privacy-no-tracking.md)).

## Decision

- **Portuguese (pt-BR) and English**, every path starts with the language and the rest is translated:
  `/pt/sobre` ↔ `/en/about`, `/pt/jogos` ↔ `/en/games`, `/pt/privacidade` ↔ `/en/privacy`,
  `/pt/cafe` ↔ `/en/cafe`. `SiteRoutes` is the single source of these paths; tests check the `@page`
  directives and `sitemap.xml` against it (and the SWA routes, with [0009](0009-real-404-for-unknown-paths.md)).
- **`/` and the short link `/cafe`** send the visitor to their language. `js/site.js` does it before
  Blazor downloads; `LanguageRedirect.razor` covers navigation inside the app. The order is:
  1. the language the visitor picked in the PT/EN switch, saved in `localStorage` (if storage is
     blocked, the choice just isn't remembered);
  2. otherwise the browser's first preferred language (`navigator.languages`): `pt*` → `/pt`, anything else → `/en`.
- **No IP geolocation.** The browser language says which language a person reads; the IP only says where
  they are (a Brazilian abroad would get English), and it would need a server or a third party.
- Texts live in `texts.pt.json` and `texts.en.json`, never in components; a test checks both files have the same keys.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Browser language + saved choice** ✅ | No server, no data sent anywhere; respects the person's own setting | A visitor with an English browser who prefers Portuguese clicks PT once |
| IP geolocation | Works without browser settings | Needs a server or a third party; guesses country, not language; a privacy cost |
| Same path, language in a query or cookie | Fewer routes | Pages can't be indexed per language; cookies would contradict [0007](0007-privacy-no-tracking.md) |

## Consequences

- ✅ Every page has a stable, shareable URL in each language
- ✅ Nothing about the visitor leaves the browser
- ⚠️ A new page needs both paths in `SiteRoutes`, both `@page` directives, both texts, the sitemap and,
  with [0009](0009-real-404-for-unknown-paths.md), the SWA routes; the tests point out what is missing
