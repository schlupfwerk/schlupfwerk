# 0006 — A strict CSP, with the import map allowed by hash

- Status: Accepted
- Date: 2026-11-23

## Context

The site loads nothing from third parties and wants a Content-Security-Policy that keeps it that way.
`.NET` writes an **inline** `<script type="importmap">` into the published `index.html`, and its content
(fingerprinted file names) changes on every build. A CSP without `'unsafe-inline'` blocks it, and then
Blazor never starts: the page stays blank.

## Decision

- The CSP is sent as a header from `staticwebapp.config.json` (`globalHeaders`), with
  `default-src 'self'`, `script-src 'self' 'wasm-unsafe-eval' <hashes>`, `style-src 'self'`,
  `object-src 'none'`, `base-uri 'self'`, `form-action 'self'`, `frame-ancestors 'none'`, plus
  `X-Content-Type-Options`, `Referrer-Policy` and `Permissions-Policy`.
- The config holds the placeholder `__INLINE_SCRIPT_HASHES__`. After `dotnet publish`, the pipeline runs
  `tools/csp-hashes.cs`, which hashes every inline script of the published `index.html` (SHA-256, with line
  breaks normalized to LF as the browser does) and writes the hashes in place of the placeholder.
- Any other inline script must go through the same tool; inline styles are not allowed at all.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Hash written at publish** ✅ | No `'unsafe-inline'`; exact match to what is served | A post-publish step that every deploy path must run |
| `'unsafe-inline'` for scripts | No extra step | Allows any injected inline script; defeats the point |
| Nonces | Standard for dynamic pages | Need a server to generate a nonce per response |

## Consequences

- ✅ A third-party or injected script can't run on the site
- ⚠️ A deploy that skips `tools/csp-hashes.cs` (for example, Azure's default GitHub Actions workflow)
  produces a blank site; this is one reason for a single deploy path ([0003](0003-azure-pipelines-and-static-web-apps.md))
- ⚠️ Proxies that inject scripts (Cloudflare's orange cloud features) are incompatible ([0004](0004-domains-and-dns.md))
