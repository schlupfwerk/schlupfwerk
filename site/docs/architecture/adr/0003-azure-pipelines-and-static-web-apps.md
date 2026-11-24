# 0003 — CI/CD on Azure Pipelines, hosting on Azure Static Web Apps (Free)

- Status: Accepted
- Date: 2026-11-23

## Context

The site is static and needs custom headers (CSP, immutable caching), compression, HTTPS on a custom
domain and, ideally, a preview of each change. The code is public on GitHub, in `site/` of
`schlupfwerk/schlupfwerk` (the same repository as the organization's profile README), and doesn't
accept contributions. This follows ADR 0009 of Manuel's portfolio, which made the same choice for the
same reasons.

## Decision

- **Hosting:** Azure Static Web Apps, **Free** plan, resource `schlupfwerk` in `schlupfwerk_group`
  (Central US; static content is served globally regardless of the region). The deployment
  authorization policy is the **deployment token**, not GitHub identity.
- **CI/CD:** **Azure Pipelines**, `azure-pipelines.yml` at the repository root, triggered only by
  changes under `site/` or to the pipeline file. Stages:
  1. **CI:** restore → build (`-warnaserror`) → format check → tests → publish → `tools/csp-hashes.cs` → artifact.
  2. **CD production:** pushes to `main` deploy with `AzureStaticWebApp@0`
     (`skip_app_build: true`, `skip_api_build: true`).
  3. **CD preview:** pull requests deploy to a named environment `pr<number>`.
- **Secret:** the SWA deployment token lives only in the secret pipeline variable `SWA_DEPLOYMENT_TOKEN`.
  It was reset once after being exposed in a chat, and the pipeline was re-run to confirm the new one.
- **Pull requests from forks are not built** (pipeline triggers → pull request validation → forks off).
  The repository doesn't accept contributions, and a fork's code must never run on our agent.
- **No GitHub Actions.** Because the SWA was first linked to the GitHub repository, Azure committed a
  workflow (`.github/workflows/azure-static-web-apps-*.yml`) and a repository secret with a token. Both
  were removed: the workflow would build `site/` its own way and deploy over production without the
  CSP step (see [0006](0006-strict-csp-with-hashed-import-map.md)).

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Azure Pipelines + SWA Free** ✅ | Headers, Brotli, managed HTTPS, PR previews, free; matches Manuel's Azure profile | A second account (Azure DevOps); previews aren't deleted automatically |
| GitHub Actions + SWA (Azure's default) | Generated for you; previews cleaned up on PR close | Builds the app without our CSP step; doesn't show Azure DevOps |
| GitHub Pages | Simplest | No custom headers, no real 404 status, no previews |

## Consequences

- ✅ One deploy path, with the same quality gate on every change
- ✅ Each PR can be opened in a preview before merging
- ⚠️ The Free plan allows **3** preview environments and they are **not** removed when the PR closes;
  delete stale ones in the portal (Static Web App → Environments)
- ⚠️ If Azure re-links the app to GitHub, check for a new `.github/workflows/azure-static-web-apps-*.yml`
  and a new repository secret, and remove both
- ⚠️ `main` has no branch protection: Manuel is the only one with write access and merges his own PRs
