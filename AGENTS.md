# Contentstack Management .NET SDK – Agent guide

**Universal entry point** for contributors and AI agents. Detailed conventions live in **`skills/*/SKILL.md`**.

## What this repo is

| Field | Detail |
| ----- | ------ |
| **Name:** | [contentstack/contentstack-management-dotnet](https://github.com/contentstack/contentstack-management-dotnet) |
| **Purpose:** | .NET SDK for the [Content Management API (CMA)](https://www.contentstack.com/docs/developers/apis/content-management-api/)—manage stacks, content types, entries, assets, and related resources. |
| **Out of scope (if any):** | Content **delivery** to end users should use the Content Delivery API and its SDKs, not this package. This repo does not implement the CDA. |

## Tech stack (at a glance)

| Area | Details |
| ---- | ------- |
| **Language** | C# 8.0, nullable reference types enabled (`LangVersion` in core project). |
| **Build** | .NET SDK; main solution [`Contentstack.Management.Core.sln`](Contentstack.Management.Core.sln). Core library targets `netstandard2.0` (and `net471` / `net472` on Windows—see [`skills/framework/SKILL.md`](skills/framework/SKILL.md)). |
| **Tests** | MSTest; test projects target `net7.0`. Unit tests: [`Contentstack.Management.Core.Unit.Tests/`](Contentstack.Management.Core.Unit.Tests/). Integration tests: [`Contentstack.Management.Core.Tests/`](Contentstack.Management.Core.Tests/) (includes `IntegrationTest/`). |
| **Lint / coverage** | No repo-wide `dotnet format` / lint script at root. Rely on **.NET SDK analyzers** and IDE analysis. Tests use **coverlet** (`XPlat code coverage`) when run as in CI. |
| **Other** | NuGet packages: `contentstack.management.csharp` (core), `contentstack.management.aspnetcore` (ASP.NET Core helpers). Assembly signing via `CSManagementSDK.snk`. |

## Commands (quick reference)

| Command type | Command |
| ------------ | ------- |
| **Build** | `dotnet build Contentstack.Management.Core.sln` |
| **Test (CI-parity, unit)** | `sh ./Scripts/run-unit-test-case.sh` — runs `dotnet test` on [`Contentstack.Management.Core.Unit.Tests/Contentstack.Management.Core.Unit.Tests.csproj`](Contentstack.Management.Core.Unit.Tests/Contentstack.Management.Core.Unit.Tests.csproj) with TRX logger and coverlet. |
| **Test (integration)** | `dotnet test Contentstack.Management.Core.Tests/Contentstack.Management.Core.Tests.csproj` — requires local `appsettings.json` with credentials (see [`skills/testing/SKILL.md`](skills/testing/SKILL.md)). |
| **Pack (release)** | `dotnet pack -c Release -o out` (as in [`.github/workflows/nuget-publish.yml`](.github/workflows/nuget-publish.yml)). |

**CI:** [`.github/workflows/unit-test.yml`](.github/workflows/unit-test.yml) (unit tests on PR/push). **Branches:** feature work merges to **`development`**; **release PRs** are **`development` → `main`** (no `staging`). After `main` advances, [`.github/workflows/back-merge-pr.yml`](.github/workflows/back-merge-pr.yml) can open **`main` → `development`**. **Releases:** create a **GitHub Release** to run [`.github/workflows/nuget-publish.yml`](.github/workflows/nuget-publish.yml) (`release: created`). [`.github/workflows/check-version-bump.yml`](.github/workflows/check-version-bump.yml) enforces version + `CHANGELOG.md` on relevant PRs.

## Where the documentation lives: skills

| Skill | Path | What it covers |
| ----- | ---- | -------------- |
| Dev workflow | [`skills/dev-workflow/SKILL.md`](skills/dev-workflow/SKILL.md) | Branches, CI, scripts, when to run which tests. |
| SDK (CMA) | [`skills/contentstack-management-dotnet-sdk/SKILL.md`](skills/contentstack-management-dotnet-sdk/SKILL.md) | Public API, auth, package boundaries. |
| Testing | [`skills/testing/SKILL.md`](skills/testing/SKILL.md) | MSTest layout, unit vs integration, credentials, coverage. |
| Code review | [`skills/code-review/SKILL.md`](skills/code-review/SKILL.md) | PR expectations and checklist. |
| Framework / platform | [`skills/framework/SKILL.md`](skills/framework/SKILL.md) | TFMs, signing, NuGet, HTTP pipeline overview. |
| C# style | [`skills/csharp-style/SKILL.md`](skills/csharp-style/SKILL.md) | Language and layout conventions for this repo. |
| HTTP pipeline (retries) | [`skills/http-pipeline/SKILL.md`](skills/http-pipeline/SKILL.md) | Handlers, retry policy, pipeline behavior. |
| ASP.NET Core integration | [`skills/aspnetcore-integration/SKILL.md`](skills/aspnetcore-integration/SKILL.md) | `contentstack.management.aspnetcore` package and DI. |
| Documentation (DocFX) | [`skills/documentation/SKILL.md`](skills/documentation/SKILL.md) | API docs under `docfx_project/`. |

An index with **when to use** hints is in [`skills/README.md`](skills/README.md).

## Using Cursor (optional)

If you use **Cursor**, [`.cursor/rules/README.md`](.cursor/rules/README.md) points to **`AGENTS.md`** and **`skills/`**—same docs as everyone else; no duplicated prose in `.cursor`.
