---
name: testing
description: Use for MSTest projects, unit vs integration tests, coverlet/TRX output, and local credentials in contentstack-management-dotnet.
---

# Testing – Contentstack Management .NET SDK

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

### MSTest patterns (unit tests)

This repo uses **MSTest** (`Microsoft.VisualStudio.TestTools.UnitTesting`), **not** xUnit. Many tests also use **AutoFixture**, **AutoFixture.AutoMoq**, and **Moq**.

#### Basic test class

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.YourArea
{
    [TestClass]
    public class YourFeatureTest
    {
        [TestInitialize]
        public void Setup()
        {
            // Per-test setup
        }

        [TestMethod]
        public void YourScenario_DoesExpectedThing()
        {
            Assert.IsNotNull(result);
        }
    }
}
```

#### AutoFixture + AutoMoq

Common in HTTP and service tests ([`ContentstackHttpRequestTest.cs`](../../Contentstack.Management.Core.Unit.Tests/Http/ContentstackHttpRequestTest.cs)):

```csharp
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class YourFeatureTest
{
    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

    [TestMethod]
    public void Example()
    {
        var value = _fixture.Create<string>();
        Assert.IsFalse(string.IsNullOrEmpty(value));
    }
}
```

#### Pipeline / handler tests

Pipeline tests often build **`ExecutionContext`**, **`RequestContext`**, **`ResponseContext`**, attach a **`RetryPolicy`**, and use test doubles from **`Mokes/`** (e.g. `MockHttpHandlerWithRetries`, `MockService`). See [`RetryHandlerTest.cs`](../../Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/RetryHandler/RetryHandlerTest.cs).

#### Integration tests (separate project)

Integration tests live in **`Contentstack.Management.Core.Tests`**, use **`[ClassInitialize]`**, **`[DoNotParallelize]`** in many classes, and **`Contentstack.CreateAuthenticatedClient()`** from [`Contentstack.cs`](../../Contentstack.Management.Core.Tests/Contentstack.cs). Do **not** use this pattern in the unit test project for network I/O.

#### Commands

- **CI parity (unit):** `sh ./Scripts/run-unit-test-case.sh` from repo root.
- **Single project:** `dotnet test Contentstack.Management.Core.Unit.Tests/Contentstack.Management.Core.Unit.Tests.csproj`

Exact `dotnet test` arguments are in [`Scripts/run-unit-test-case.sh`](../../Scripts/run-unit-test-case.sh).
