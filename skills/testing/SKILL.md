---
name: testing
description: Use for MSTest projects, unit vs integration tests, coverlet/TRX output, and local credentials in contentstack-management-dotnet.
---

# Testing – Contentstack Management .NET SDK

**Deep dive:** [`references/mstest-patterns.md`](references/mstest-patterns.md) (MSTest, AutoFixture, Moq templates).

## When to use

- Adding or fixing unit or integration tests.
- Reproducing CI test failures (TRX, coverage).
- Setting up `appsettings.json` for integration tests.

## Instructions

### Projects

| Project | Path | Purpose |
| ------- | ---- | ------- |
| Unit tests | [`Contentstack.Management.Core.Unit.Tests/`](../../Contentstack.Management.Core.Unit.Tests/) | Fast, isolated tests; **this is what CI runs** via [`Scripts/run-unit-test-case.sh`](../../Scripts/run-unit-test-case.sh). |
| Integration tests | [`Contentstack.Management.Core.Tests/`](../../Contentstack.Management.Core.Tests/) | Real API tests under `IntegrationTest/`; requires credentials. |

- **Framework:** MSTest (`Microsoft.VisualStudio.TestTools.UnitTesting`).
- **Target framework:** `net7.0` for both test projects.
- **Coverage:** `coverlet.collector` with `--collect:"XPlat code coverage"` in the unit test script.

### CI parity (unit)

From repo root:

```bash
sh ./Scripts/run-unit-test-case.sh
```

- TRX output: `Contentstack.Management.Core.Unit.Tests/TestResults/Report-Contentstack-DotNet-Test-Case.trx` (logger file name in script).
- The script deletes `Contentstack.Management.Core.Unit.Tests/TestResults` before running.

### Integration tests and credentials

- Helper: [`Contentstack.Management.Core.Tests/Contentstack.cs`](../../Contentstack.Management.Core.Tests/Contentstack.cs) loads **`appsettings.json`** via `ConfigurationBuilder` and exposes `Contentstack.CreateAuthenticatedClient()` (login using credentials from config—**never commit real secrets**).
- Add `appsettings.json` locally (it is not checked in; keep secrets out of git). Structure includes `Contentstack` section and nested `Contentstack:Credentials`, `Contentstack:Organization` as used by the tests.
- Integration tests use `[ClassInitialize]` / `[DoNotParallelize]` in many classes; follow existing patterns when adding scenarios.

### Hygiene

- Do not commit TRX zips, ad-hoc `TestResults` archives, or credential files.
- Prefer deterministic unit tests with mocks (see existing `Mokes/` / `Mock/` usage in unit tests).

## References

- [`references/mstest-patterns.md`](references/mstest-patterns.md) — unit test templates and tooling.
- [`../dev-workflow/SKILL.md`](../dev-workflow/SKILL.md) — CI workflow that runs unit tests.
- [`../../Scripts/run-unit-test-case.sh`](../../Scripts/run-unit-test-case.sh) — exact `dotnet test` arguments.
