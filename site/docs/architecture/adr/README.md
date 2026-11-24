# Architecture decision records

Decisions about the Schlupfwerk website (`site/`), one file per decision, in the order they were made.
Each record says what was decided, why, what else was considered and what it costs.

| # | Decision | Status |
|---|---|---|
| [0001](0001-record-architecture-decisions.md) | Record architecture decisions | Accepted |
| [0002](0002-blazor-webassembly-static-site.md) | A static site in Blazor WebAssembly on .NET 10 | Accepted |
| [0003](0003-azure-pipelines-and-static-web-apps.md) | CI/CD on Azure Pipelines, hosting on Azure Static Web Apps (Free) | Accepted |
| [0004](0004-domains-and-dns.md) | `schlupfwerk.com` as the only address, DNS on Cloudflare | Accepted |
| [0005](0005-bilingual-routes-and-language-choice.md) | Two languages with translated paths, chosen in the browser | Accepted |
| [0006](0006-strict-csp-with-hashed-import-map.md) | A strict CSP, with the import map allowed by hash | Accepted |
| [0007](0007-privacy-no-tracking.md) | No tracking, no cookies, no cookie banner | Accepted |
| [0008](0008-pix-static-qr-code.md) | Donations: Ko-fi and a static Pix QR code | Accepted |
| [0009](0009-real-404-for-unknown-paths.md) | A real 404 status for unknown paths | Proposed |

## Writing a new one

Copy the shape of an existing record: title, status, date, then **Context**, **Decision**,
**Alternatives considered** and **Consequences** (✅ gains, ⚠️ costs). Number it with the next free
number and add it to the table above. A record that changes an earlier decision says
`Supersedes: [NNNN](...)`, and the earlier one gets `Superseded by`.
