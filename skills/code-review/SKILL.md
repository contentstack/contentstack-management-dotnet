---
name: code-review
description: Use when reviewing or preparing a pull request for contentstack-management-dotnet.
---

# Code review – Contentstack Management .NET SDK

**Deep dive:** [`references/checklist.md`](references/checklist.md) (copy-paste PR checklist sections).

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

For markdown blocks to paste into PRs, use [`references/checklist.md`](references/checklist.md).

### Severity (optional labels)

- **Blocker:** Build or CI broken; security issue; violates branch policy.
- **Major:** Missing tests for risky logic; breaking API without process.
- **Minor:** Naming nits, non-user-facing cleanup.

## References

- [`references/checklist.md`](references/checklist.md) — detailed PR checklist.
- [`../dev-workflow/SKILL.md`](../dev-workflow/SKILL.md) — CI and branches.
- [`../contentstack-management-dotnet-sdk/SKILL.md`](../contentstack-management-dotnet-sdk/SKILL.md) — API boundaries.
