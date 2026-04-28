---
name: dev-workflow
description: Use for branches, CI, build/test scripts, and NuGet release flow in contentstack-management-dotnet.
---

# Dev workflow – Contentstack Management .NET SDK

## When to use

- Changing or debugging GitHub Actions workflows.
- Running the same build/test commands as CI locally.
- Preparing a release or understanding how packages are published.

## Instructions

### Branch policy

- **Default:** open PRs against **`development`** for feature and fix work.
- **Releases:** open a **release PR `development` → `main`** (no `staging`). After `main` is updated, [`.github/workflows/back-merge-pr.yml`](../../.github/workflows/back-merge-pr.yml) opens **`main` → `development`** when needed so branches stay aligned.
- **Publishing:** create a **GitHub Release** (after the release commit is on `main`) to trigger [`.github/workflows/nuget-publish.yml`](../../.github/workflows/nuget-publish.yml) (`release: created`).
- **Version gate:** PRs that touch product code or `Directory.Build.props` need matching bumps in `Directory.Build.props` and `CHANGELOG.md` per [`.github/workflows/check-version-bump.yml`](../../.github/workflows/check-version-bump.yml).

### Key workflows

| Workflow | Role |
| -------- | ---- |
| [`unit-test.yml`](../../.github/workflows/unit-test.yml) | On PR and push: runs [`Scripts/run-unit-test-case.sh`](../../Scripts/run-unit-test-case.sh) (unit tests + TRX + coverlet). |
| [`back-merge-pr.yml`](../../.github/workflows/back-merge-pr.yml) | After pushes to **`main`**, opens **`main` → `development`** PR if needed. |
| [`check-version-bump.yml`](../../.github/workflows/check-version-bump.yml) | On PR: requires version + changelog when SDK sources / props change. |
| [`nuget-publish.yml`](../../.github/workflows/nuget-publish.yml) | On **GitHub Release** (`created`): `dotnet pack -c Release -o out` and push to NuGet / GitHub Packages. |
| [`policy-scan.yml`](../../.github/workflows/policy-scan.yml), [`sca-scan.yml`](../../.github/workflows/sca-scan.yml) | Security / compliance scans. |

### Local commands

- **Build:** `dotnet build Contentstack.Management.Core.sln`
- **Unit tests (matches CI):** `sh ./Scripts/run-unit-test-case.sh` from repo root (cleans `Contentstack.Management.Core.Unit.Tests/TestResults` first).
- **Integration tests:** separate project; see [`../testing/SKILL.md`](../testing/SKILL.md).

### Scripts

- [`Scripts/run-unit-test-case.sh`](../../Scripts/run-unit-test-case.sh) — unit test entrypoint used in CI.
- [`Scripts/generate_integration_test_report.py`](../../Scripts/generate_integration_test_report.py) — integration test reporting helper (if used by the team).

### Signing and secrets

- Contributors need `CSManagementSDK.snk` for a full signed build matching the repo; see [`../framework/SKILL.md`](../framework/SKILL.md).
- NuGet push uses repository secrets (`NUGET_API_KEY`, etc.)—never commit keys.

Top-level commands: [`../../AGENTS.md`](../../AGENTS.md).
