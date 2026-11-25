# 0007 — No tracking, no cookies, no cookie banner

- Status: Accepted
- Date: 2026-11-23

## Context

"Your time and your privacy are not our product" is one of the studio's values. The first version of the
site promised **"no data collection"**. While planning the games it became clear that optional online
features (friends leaderboards, achievements, cloud saves, purchases) do involve personal data, even if
the studio never receives it, so that promise wasn't accurate. The audience includes children, so the
LGPD (art. 14) and the stores' family rules apply.

## Decision

- **The promise is "no ads, no tracking"** (PT: "sem anúncios e sem rastreamento"): no analytics, no
  selling data, and the studio doesn't keep personal data. Don't write "no data collection".
- **The site** sets no cookies and runs no analytics or third-party scripts. The only thing it stores is the
  language the visitor picks, in `localStorage`, and only after they pick it ([0005](0005-bilingual-routes-and-language-choice.md)).
- **No cookie banner.** The LGPD and the EU rules ask for consent for non-essential cookies (tracking,
  analytics, ads). A preference the visitor set themselves is functional, and the privacy page explains it.
  A banner about cookies that don't exist would only get in the way.
- **The games' 1.0 plan** keeps that promise without a studio server:
  - single player against AI, works offline, no account;
  - ~~cosmetic skins and an in-game currency (earned by playing or bought through the stores)~~
    **Revised 2026-11-25: Swan Race is completely free, with no purchases**; everything is unlocked from the
    start. If a game ever sells something, the stores process the payment (StoreKit 2 / Play Billing / Steam).
    Progress is saved in iCloud or Google Play Games;
  - friends-only leaderboards and achievements through Game Center and Google Play Games;
  - invitations by share link (e.g. WhatsApp). **The game never reads the phone's contacts.**
  - Real-time multiplayer and referral rewards are **2.0**: they need a server and would add data (IP,
    nickname, who invited whom). The privacy policy must describe that data before such a feature ships.
- The privacy page (PT/EN) says this in plain language: the site, the games, leaderboards/achievements/progress
  (Apple and Google), purchases (App Store/Google Play), donations (Ko-fi/Stripe or Pix), children, LGPD, contact.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **"No tracking" + data with Apple/Google** ✅ | True today and for 1.0; no server; no data to protect | Features limited to what Game Center / Play Games offer |
| Keep "no data collection" | Simpler slogan | Not true once there are leaderboards or purchases |
| Studio accounts and server from 1.0 | Cross-platform multiplayer, referrals | Cost, security and LGPD duties (children's data) for a one-person studio |
| Cookie banner "just in case" | — | Nothing to consent to; worse experience |

## Consequences

- ✅ The value statement, the privacy page, the README and `meta description` say the same, true thing
- ✅ No personal data to store, secure or delete on the studio side
- ⚠️ Every new online feature needs a privacy review and a policy update before it ships. That includes
  any analytics or crash reporting (e.g. Firebase) in the games: it would change the "no tracking" promise,
  so it needs a new ADR and a privacy page update before it ships
