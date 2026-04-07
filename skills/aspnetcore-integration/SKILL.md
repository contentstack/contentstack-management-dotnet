---
name: aspnetcore-integration
description: Use for the contentstack.management.aspnetcore package, HttpClient/DI registration with ASP.NET Core.
---

# ASP.NET Core integration – Contentstack Management .NET SDK

## When to use

- Changing [`Contentstack.Management.ASPNETCore/`](../../Contentstack.Management.ASPNETCore/) or the NuGet package **`contentstack.management.aspnetcore`**.
- Registering `ContentstackClient` with `IHttpClientFactory` / `IServiceCollection`.

## Instructions

### Package

- **Package ID:** `contentstack.management.aspnetcore`
- **Target:** `netstandard2.1`
- **Project:** [`contentstack.management.aspnetcore.csproj`](../../Contentstack.Management.ASPNETCore/contentstack.management.aspnetcore.csproj)

### Registration APIs

- [`ServiceCollectionExtensions`](../../Contentstack.Management.ASPNETCore/ServiceCollectionExtensions.cs) in namespace `Microsoft.Extensions.DependencyInjection`:
  - `AddContentstackClient(IServiceCollection, ContentstackClientOptions)` / `TryAddContentstackClient` — extend here when wiring options-based registration; keep behavior aligned with DI conventions.
  - `AddContentstackClient(IServiceCollection, Action<HttpClient>)` — registers `ContentstackClient` with **`AddHttpClient<ContentstackClient>`** for typed client configuration.

When extending DI support, align with Microsoft.Extensions.DependencyInjection and `Microsoft.Extensions.Http` patterns already referenced in the project file.

### Core dependency

- The ASP.NET Core project references the core management package; public types come from [`Contentstack.Management.Core`](../../Contentstack.Management.Core/).

## References

- [`../contentstack-management-dotnet-sdk/SKILL.md`](../contentstack-management-dotnet-sdk/SKILL.md) — core client and options.
- [`../framework/SKILL.md`](../framework/SKILL.md) — NuGet and TFMs.
