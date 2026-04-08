---
name: code-review
description: Use when reviewing or preparing a pull request for contentstack-management-dotnet.
---

# Code review – Contentstack Management .NET SDK

## When to use

- Before requesting review or merging a PR.
- When auditing changes for API safety, tests, and repo policies.

## Instructions

### Branch and merge expectations

- **Typical PRs** should target **`development`**. Use **`main`** as the base branch only for **hotfixes**.
- **When the base is `main`:** only PRs from **`staging`** are allowed (enforced by [`.github/workflows/check-branch.yml`](../../.github/workflows/check-branch.yml)). Confirm head/base match team intent before approving.

### Summary checklist

- **Purpose:** Change matches the ticket/PR description; no unrelated refactors.
- **Tests:** Unit tests updated or added for behavior changes; run `sh ./Scripts/run-unit-test-case.sh` locally for core changes. Integration tests only when behavior depends on live API—coordinate credentials.
- **API compatibility:** Public surface (`ContentstackClient`, options, models) changes are intentional and versioned appropriately; avoid breaking changes without major bump and changelog.
- **Security:** No secrets, tokens, or keys in source or commits; `appsettings.json` with real data must not be committed.
- **Signing:** If assembly signing is affected, confirm `CSManagementSDK.snk` usage matches [`../framework/SKILL.md`](../framework/SKILL.md).
- **Style:** Follow [`../csharp-style/SKILL.md`](../csharp-style/SKILL.md); match surrounding code.
- **Documentation:** User-visible behavior changes reflected in `README.md` or package release notes when needed.

For markdown blocks to paste into PRs, copy from **PR review checklist (copy-paste)** below.

### Severity (optional labels)

- **Blocker:** Build or CI broken; security issue; violates branch policy.
- **Major:** Missing tests for risky logic; breaking API without process.
- **Minor:** Naming nits, non-user-facing cleanup.

### PR review checklist (copy-paste)

Copy sections into a PR comment when useful. This checklist is for **this** repo (`HttpClient` + pipeline + MSTest), **not** the Content Delivery .NET SDK.

#### Branch policy

```markdown
- [ ] **Default:** PR targets **`development`** unless this is a documented **hotfix** to **`main`**
- [ ] If base is **`main`**: head branch is **`staging`** (see `.github/workflows/check-branch.yml`)
```

#### Breaking changes

```markdown
- [ ] No public method/property removed or narrowed without deprecation / major version plan
- [ ] `JsonProperty` / JSON names for API-facing models unchanged unless intentional and documented
- [ ] New required `ContentstackClientOptions` fields have safe defaults or are optional
- [ ] Strong naming: assembly signing still consistent if keys or `csproj` changed
```

#### HTTP and pipeline

```markdown
- [ ] New or changed HTTP calls go through existing client/pipeline (`ContentstackClient` → `IContentstackService` → pipeline), not ad-hoc `HttpClient` usage inside services without justification
- [ ] Retry-sensitive changes reviewed alongside `RetryHandler` / `DefaultRetryPolicy` and unit tests under `Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/`
- [ ] Headers, query params, and path segments align with CMA docs; no hardcoded production URLs where options.Host should be used
```

#### Services and query API

```markdown
- [ ] `IContentstackService` implementations set `ResourcePath`, `HttpMethod`, `Parameters` / `QueryResources` / `PathResources` / `Content` consistently with sibling services
- [ ] New fluent `Query` methods only add to `ParameterCollection` with correct API parameter names
```

#### Tests

```markdown
- [ ] Unit tests use MSTest; `sh ./Scripts/run-unit-test-case.sh` passes for core changes
- [ ] Integration tests only when needed; no secrets committed (`appsettings.json` stays local)
```

#### Security and hygiene

```markdown
- [ ] No API keys, tokens, or passwords in source or test data checked into git
- [ ] OAuth / token handling does not log secrets
```

#### Documentation

```markdown
- [ ] User-visible behavior reflected in `README.md` or release notes when appropriate
- [ ] `skills/` updated if agent/contributor workflow changed
```
