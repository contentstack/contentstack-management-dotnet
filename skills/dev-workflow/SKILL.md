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

- **Default workflow:** open PRs against **`development`** for regular feature and fix work. **`main`** is reserved for **hotfixes** (PRs raised directly to `main` only when patching production).
- **When the PR target is `main`:** GitHub Actions requires the head branch to be **`staging`**—other head branches are rejected by [`.github/workflows/check-branch.yml`](../../.github/workflows/check-branch.yml). Coordinate with SRE/release if a hotfix must use a different flow.
- Do not bypass enforced checks without org approval.

### Key workflows

| Workflow | Role |
| -------- | ---- |
| [`unit-test.yml`](../../.github/workflows/unit-test.yml) | On PR and push: runs [`Scripts/run-unit-test-case.sh`](../../Scripts/run-unit-test-case.sh) (unit tests + TRX + coverlet). |
| [`check-branch.yml`](../../.github/workflows/check-branch.yml) | For PRs **into `main`**, enforces head branch **`staging`**. |
| [`nuget-publish.yml`](../../.github/workflows/nuget-publish.yml) | On release: `dotnet pack -c Release -o out` and push to NuGet / GitHub Packages. |
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

## References

- [`../framework/SKILL.md`](../framework/SKILL.md) — TFMs, pack, signing.
- [`../testing/SKILL.md`](../testing/SKILL.md) — test projects and credentials.
- [`../../AGENTS.md`](../../AGENTS.md) — top-level commands table.
