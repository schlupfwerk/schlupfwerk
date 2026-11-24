# 0001 — Record architecture decisions

- Status: Accepted
- Date: 2026-11-23

## Context

The site is small, but several of its choices aren't obvious from the code: why there is no
`navigationFallback`, why the CSP has a hash in it, why the Pix QR code must not be regenerated freely,
why GitHub Actions was removed. Without a written reason, the next change (by Manuel or by an AI
assistant) can undo one of them by accident.

## Decision

Keep architecture decision records in `site/docs/architecture/adr/`, in English (like the code
comments), using the same format as Manuel's portfolio (`Bravodin/portifolio_bravo`,
`docs/architecture/adr/`). `site/CLAUDE.md` stays the short working guide; the ADRs hold the reasons.

## Consequences

- ✅ Each non-obvious choice has its context and trade-offs in one place
- ⚠️ A record must be written or updated in the same change that makes or reverses a decision
