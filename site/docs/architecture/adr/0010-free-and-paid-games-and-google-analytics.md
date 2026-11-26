# 0010 — Free and paid games, and Google Analytics in the games

- Status: Accepted
- Date: 2026-11-26
- Supersedes: part of [0007](0007-privacy-no-tracking.md) (the "no analytics" promise for the games)

## Context

Until now the site, the README and the privacy page described the studio as making only free,
family-friendly, open-source (GPLv3) games, with "no ads, no tracking" and no analytics anywhere.
Two things changed:

- **Analytics in the games.** Swan Race needs a way to learn about errors on players' devices.
  [0007](0007-privacy-no-tracking.md) asked for a new ADR and a privacy page update before any analytics or
  crash reporting ships in a game.
- **Paid games.** The studio also wants to sell games (e.g. on Steam, for around R$ 9–10), such as an
  action platformer in the metroidvania style (Castlevania, Blasphemous). Those games are closed source
  and are not made for children.

## Decision

- **Two kinds of games:**
  - **Free games** (Swan Race and future free games): free forever, no purchases or microtransactions,
    family friendly, open source (GPLv3 code, CC BY-NC-SA 4.0 art).
  - **Paid games**: sold in the stores (Steam, App Store, Google Play), which process the payment.
    Closed source, with light code protection (e.g. R8 on Android). They may have action, shooting and
    combat violence for older players, with the store's age rating. **Never sexual content.**
  - **Fair play holds for both:** nothing that gives an advantage is for sale.
- **The promise becomes "no ads, no selling data"** (PT: "sem anúncios e sem venda de dados"). We no
  longer say "no tracking" or "no analytics" about the games.
- **Google Analytics is the only data tool in the games.** No Firebase, no Crashlytics, no ad SDKs,
  no other third-party SDK that collects data. It is used only to find errors and improve the games, and
  it must stay anonymous:
  - no name, email, contacts, precise location or advertising ID, and no user ID;
  - nothing is used for ads (no Google Signals, no ads personalization, no linking to Google Ads);
  - nothing is sent without internet; the game still works fully offline.
- **The site does not change:** no cookies, no analytics, no third-party scripts ([0007](0007-privacy-no-tracking.md)).
- The privacy page gets a **"Google Analytics in our games"** section (PT/EN) and the values, the home page,
  the About page, the README and the `meta description` say the same thing.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Google Analytics only, anonymous** ✅ | Error and usage reports with one tool; nothing else to disclose | Breaks the old "no tracking" slogan; Google receives data; needs care with the stores' family rules |
| Firebase Crashlytics | Built for crash reports | Another SDK and more data; the owner doesn't want it |
| No analytics at all | Simplest promise | No way to learn about errors on players' devices |
| All games free and GPL | One simple story | The studio can't earn anything from its games |

## Consequences

- ✅ The site tells the truth about analytics and about paid, older-audience games
- ✅ Swan Race stays free, family friendly and open source
- ⚠️ Swan Race is family friendly and may reach children: Google Analytics must be configured to match the
  privacy page (anonymous, no advertising ID, no ads features), and the Google Play Families policy and
  the LGPD (art. 14) must be checked before it ships on Android
- ⚠️ Any new data, SDK or online feature in a game still needs a privacy review and a page update first
