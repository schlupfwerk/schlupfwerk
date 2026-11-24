# 0008 — Donations: Ko-fi and a static Pix QR code

- Status: Accepted
- Date: 2026-11-23

## Context

The games have no ads, so the studio accepts donations ("cafezinho") on `/pt/cafe` and `/en/cafe`.
International visitors need a card-based option; Brazilians mostly use Pix. The site has no backend, so
nothing can be generated per visit.

## Decision

- **Ko-fi** (`https://ko-fi.com/schlupfwerk`, with Stripe) for international donations: a plain link.
- **Pix** through a **static BR Code QR image**, without a fixed amount (the donor types it):
  - random key `d5555f67-5b99-4471-a9a0-3af5881e61f9` (replaced `ad349d7a-…` on 2026-11-23);
  - merchant name `BRAVO CANDIA CONSULTORIA`, city `BRASIL`, txid `***`;
  - under the QR code, the beneficiary as the bank shows it:
    "MANUEL ANDREZ BRAVO CANDIA CONSULTORIA EM TI LTDA";
  - **no "copia e cola" code**, only the QR code, to keep the page clean;
  - the CNPJ is **not** a registered Pix key and must not be used.
- The image is `profile/pix-qr.png` (shown in the README); `wwwroot/img/pix-qr.png` is a copy and a test
  checks both are identical. The full payload is recorded in `site/CLAUDE.md`.
- **Changing the key:** rebuild the payload changing only the key, recompute the CRC16-CCITT, check the
  builder reproduces the previous payload exactly, decode the new image back, update both copies, and
  test with a real payment before announcing it. Both keys so far were tested with a real payment.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Static QR, random key** ✅ | No backend; the key doesn't expose a phone, e-mail or CNPJ | Donor types the amount; changing the key means a new image |
| Dynamic Pix (per-donation QR with amount) | Pre-filled amount, reconciliation | Needs a PSP integration and a backend |
| "Copia e cola" text next to the QR | Easier on the same phone | Cluttered page; was removed on purpose |

## Consequences

- ✅ Works on a fully static site; no payment data passes through the studio
- ⚠️ The image must never be regenerated casually: a wrong QR code sends money nowhere or to the wrong place
