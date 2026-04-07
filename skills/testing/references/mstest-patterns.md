# MSTest patterns (unit tests)

This repo uses **MSTest** (`Microsoft.VisualStudio.TestTools.UnitTesting`), **not** xUnit. Many tests also use **AutoFixture**, **AutoFixture.AutoMoq**, and **Moq**.

## Basic test class

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

## AutoFixture + AutoMoq

Common in HTTP and service tests ([`ContentstackHttpRequestTest.cs`](../../../Contentstack.Management.Core.Unit.Tests/Http/ContentstackHttpRequestTest.cs)):

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

## Pipeline / handler tests

Pipeline tests often build **`ExecutionContext`**, **`RequestContext`**, **`ResponseContext`**, attach a **`RetryPolicy`**, and use test doubles from **`Mokes/`** (e.g. `MockHttpHandlerWithRetries`, `MockService`). See [`RetryHandlerTest.cs`](../../../Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/RetryHandler/RetryHandlerTest.cs).

## Integration tests (separate project)

Integration tests live in **`Contentstack.Management.Core.Tests`**, use **`[ClassInitialize]`**, **`[DoNotParallelize]`** in many classes, and **`Contentstack.CreateAuthenticatedClient()`** from [`Contentstack.cs`](../../../Contentstack.Management.Core.Tests/Contentstack.cs). Do **not** use this pattern in the unit test project for network I/O.

## Commands

- **CI parity (unit):** `sh ./Scripts/run-unit-test-case.sh` from repo root.
- **Single project:** `dotnet test Contentstack.Management.Core.Unit.Tests/Contentstack.Management.Core.Unit.Tests.csproj`

See [`../SKILL.md`](../SKILL.md) for TRX, coverlet, and `appsettings.json` for integration runs.
