---
name: framework
description: Use for target frameworks, assembly signing, NuGet packaging, OS-specific builds, and HTTP pipeline overview in Contentstack.Management.Core.
---

# Framework and platform – Contentstack Management .NET SDK

## When to use

- Changing `TargetFrameworks`, signing, or package metadata.
- Debugging build differences between Windows and macOS/Linux.
- Understanding how `ContentstackClient` wires `HttpClient` and the runtime pipeline (before diving into retry details).

## Instructions

### Target frameworks

- [`Contentstack.Management.Core/contentstack.management.core.csproj`](../../Contentstack.Management.Core/contentstack.management.core.csproj):
  - **Windows:** `netstandard2.0;net471;net472`
  - **Non-Windows:** `netstandard2.0` only (full framework TFMs need Windows reference assemblies).
- Test projects target **`net7.0`**.
- [`Contentstack.Management.ASPNETCore/contentstack.management.aspnetcore.csproj`](../../Contentstack.Management.ASPNETCore/contentstack.management.aspnetcore.csproj): `netstandard2.1`.

### Assembly signing

- Core and test projects reference **`CSManagementSDK.snk`** via `SignAssembly` / `AssemblyOriginatorKeyFile`.
- Keep strong-name policy consistent when adding new shipped assemblies.

### NuGet

- Core package ID: **`contentstack.management.csharp`** (see `PackageId` in core `.csproj`).
- ASP.NET Core package ID: **`contentstack.management.aspnetcore`**.
- Local pack: `dotnet pack -c Release -o out` (see [`.github/workflows/nuget-publish.yml`](../../.github/workflows/nuget-publish.yml)).

### HTTP stack overview

- [`ContentstackClient.BuildPipeline`](../../Contentstack.Management.Core/ContentstackClient.cs) constructs a **`ContentstackRuntimePipeline`** with:
  - [`HttpHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/HttpHandler/HttpHandler.cs) — sends HTTP requests.
  - [`RetryHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryHandler.cs) — applies retry policy (default or custom).
- Options: [`RetryConfiguration`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryConfiguration.cs) and `ContentstackClientOptions` retry-related settings.
- For deep changes to retry rules, handler ordering, or custom `RetryPolicy`, see [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md).

## References

- [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md) — pipeline and retries in detail.
- [`../dev-workflow/SKILL.md`](../dev-workflow/SKILL.md) — CI and pack commands.
- [`../contentstack-management-dotnet-sdk/SKILL.md`](../contentstack-management-dotnet-sdk/SKILL.md) — public API.
