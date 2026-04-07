---
name: contentstack-management-dotnet-sdk
description: Use when changing or using the CMA client API, authentication, or NuGet package surface for Contentstack.Management.Core.
---

# Contentstack Management .NET SDK (CMA)

**Deep dive:** [`references/cma-architecture.md`](references/cma-architecture.md) (request flow, `IContentstackService`), [`references/query-and-parameters.md`](references/query-and-parameters.md) (fluent `Query`, `ParameterCollection`).

## When to use

- Adding or changing `ContentstackClient` behavior, options, or service entry points.
- Documenting how consumers authenticate (management token, authtoken, login).
- Reviewing breaking vs compatible changes for the NuGet package `contentstack.management.csharp`.

## Instructions

### Packages and entry points

| Package ID | Project | Role |
| ---------- | ------- | ---- |
| `contentstack.management.csharp` | [`Contentstack.Management.Core/`](../../Contentstack.Management.Core/) | Main SDK; `ContentstackClient`, models, services. |
| `contentstack.management.aspnetcore` | [`Contentstack.Management.ASPNETCore/`](../../Contentstack.Management.ASPNETCore/) | ASP.NET Core registration helpers; see [`../aspnetcore-integration/SKILL.md`](../aspnetcore-integration/SKILL.md). |

- Primary type: [`ContentstackClient`](../../Contentstack.Management.Core/ContentstackClient.cs) (`IContentstackClient`).
- Configuration: [`ContentstackClientOptions`](../../Contentstack.Management.Core/ContentstackClientOptions.cs) (and related options types).

### Authentication (high level)

- **Management token:** stack-scoped token; typical pattern `client.Stack(apiKey, managementToken)` per product docs.
- **Authtoken:** user/session token on the client options.
- **Login with credentials:** `ContentstackClient.Login` / `LoginAsync` with `NetworkCredential` (see root [`README.md`](../../README.md) examples).

OAuth-related code lives under [`Services/OAuth/`](../../Contentstack.Management.Core/Services/OAuth/), [`OAuthHandler.cs`](../../Contentstack.Management.Core/OAuthHandler.cs), and [`Utils/PkceHelper.cs`](../../Contentstack.Management.Core/Utils/PkceHelper.cs). Prefer small, testable changes; preserve existing public contracts unless doing a major version bump.

### Serialization and models

- JSON serialization and converters are configured in `ContentstackClient` (e.g. custom `JsonConverter` types for fields and nodes).
- Domain models live under [`Models/`](../../Contentstack.Management.Core/Models/). Follow existing patterns when adding types.

### Errors

- Exceptions and error mapping: [`Exceptions/`](../../Contentstack.Management.Core/Exceptions/). Keep messages and HTTP status handling consistent with existing patterns.

### Integration boundaries

- The SDK talks to Contentstack **Management** HTTP APIs. Do not confuse with Delivery API clients.
- HTTP behavior (retries, handlers) is documented under [`../framework/SKILL.md`](../framework/SKILL.md) and [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md).

## References

- [`references/cma-architecture.md`](references/cma-architecture.md) — architecture and invocation flow.
- [`references/query-and-parameters.md`](references/query-and-parameters.md) — fluent query API.
- [`../framework/SKILL.md`](../framework/SKILL.md) — TFMs, NuGet, pipeline overview.
- [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md) — retries and handlers.
- [`../csharp-style/SKILL.md`](../csharp-style/SKILL.md) — C# conventions.
- [Content Management API](https://www.contentstack.com/docs/developers/apis/content-management-api/) (official docs).
