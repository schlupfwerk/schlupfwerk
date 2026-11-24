# 0004 — `schlupfwerk.com` as the only address, DNS on Cloudflare

- Status: Accepted
- Date: 2026-11-23

## Context

The studio owns `schlupfwerk.com` and `schlupfwerk.com.br`, both with DNS on Cloudflare. The site's
canonical URLs (`og:url`, `sitemap.xml`, `SiteRoutes.BaseUrl`) use `https://schlupfwerk.com`. The SWA
Free plan allows two custom domains. An apex domain can't be a plain CNAME, and SWA validates an apex
with a TXT record.

## Decision

- **`schlupfwerk.com`** (apex) is the SWA **default domain**: `CNAME @ → <app>.azurestaticapps.net`
  (Cloudflare flattens it), validated with `TXT _dnsauth`.
- **`www.schlupfwerk.com`**: SWA custom domain, `CNAME www → <app>.azurestaticapps.net`, validated by CNAME.
- Because the apex is the default domain, SWA redirects (301) `www` and the `*.azurestaticapps.net`
  address to it, keeping the path.
- Both records point at Azure **DNS only (grey cloud)**. A proxied (orange) record would stop SWA from
  validating and issuing the certificate, and Cloudflare's proxy features (Web Analytics, Rocket Loader,
  email obfuscation) inject scripts that break the CSP and the no-tracking rule
  ([0007](0007-privacy-no-tracking.md)).
- **`schlupfwerk.com.br`** never reaches Azure: proxied `A @` and `A www` to the placeholder
  `192.0.2.1`, and a Cloudflare **Redirect Rule** sends both hosts to
  `concat("https://schlupfwerk.com", http.request.uri.path)` with **301**. The query string is not kept;
  the site doesn't use one.
- MX and SPF/DKIM/DMARC records of the zones are for e-mail (`contact@schlupfwerk.com`) and are left alone.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Apex as canonical** ✅ | Short address; matches what the site already prints | Needs CNAME flattening (Cloudflare has it) and TXT validation |
| `www` as canonical | Plain CNAME everywhere | Every canonical URL in the site would change |
| `.com.br` as a third SWA domain | — | The Free plan allows only two; a redirect is all it needs |

## Consequences

- ✅ One address for search engines and links; every other host redirects with the path kept
- ✅ HTTPS certificates for both `.com` hosts are issued and renewed by SWA for free
- ⚠️ The `.com` records must stay grey; turning on the proxy breaks certificate renewal
- ⚠️ The `_dnsauth` TXT record can stay; removing it doesn't break the site, but keeping it is simpler
