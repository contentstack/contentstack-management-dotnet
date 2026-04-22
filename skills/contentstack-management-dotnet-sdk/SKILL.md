---
name: contentstack-management-dotnet-sdk
description: Use when changing or using the CMA client API, authentication, or NuGet package surface for Contentstack.Management.Core.
---

# Contentstack Management .NET SDK (CMA)

## When to use

- Adding or changing `ContentstackClient` behavior, options, or service entry points.
- Documenting how consumers authenticate (management token, authtoken, login).
- Reviewing breaking vs compatible changes for the NuGet package `contentstack.management.csharp`.

## Instructions

Official API reference: [Content Management API](https://www.contentstack.com/docs/developers/apis/content-management-api/).

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
- Retries, handlers, and pipeline details: [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md). TFMs and packaging: [`../framework/SKILL.md`](../framework/SKILL.md).

### Architecture

This describes how requests flow through **this** SDK. It is **not** the Content Delivery (CDA) client: there is no `HttpWebRequest`-only layer or delivery-token query-string-only rule set here.

#### Entry and configuration

1. **`ContentstackClient`** ([`ContentstackClient.cs`](../../Contentstack.Management.Core/ContentstackClient.cs)) is the public entry point.
2. **`ContentstackClientOptions`** ([`ContentstackClientOptions.cs`](../../Contentstack.Management.Core/ContentstackClientOptions.cs)) holds `Host`, tokens, proxy, retry settings, and optional custom `RetryPolicy`.
3. The client builds a **`ContentstackRuntimePipeline`** in `BuildPipeline()`: **`HttpHandler`** → **`RetryHandler`** (see [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md)).

#### Stack-scoped API

- **`Stack`** ([`Models/Stack.cs`](../../Contentstack.Management.Core/Models/Stack.cs)) is obtained from the client (e.g. `client.Stack(apiKey, managementToken)` or overloads). Most management operations are stack-relative.
- Domain types under **`Models/`** (entries, assets, content types, etc.) expose methods that construct or call **`IContentstackService`** implementations.

#### Service interface and invocation

- **`IContentstackService`** ([`Services/IContentstackService.cs`](../../Contentstack.Management.Core/Services/IContentstackService.cs)) defines one CMA operation: resource path, HTTP method, headers, query/path resources, body (`HttpContent`), and hooks to build the outbound request and handle the response.
- The client executes services via **`InvokeSync`** / **`InvokeAsync`**, which run the pipeline and return **`ContentstackResponse`**.

Important members on `IContentstackService`:

- **`Parameters`** — [`ParameterCollection`](../../Contentstack.Management.Core/Queryable/ParameterCollection.cs) for typed query/body parameters (used by list/query flows).
- **`QueryResources`**, **`PathResources`**, **`AddQueryResource`**, **`AddPathResource`** — URL composition.
- **`UseQueryString`** — some operations send parameters as query string instead of body.
- **`CreateHttpRequest`** / **`OnResponse`** — integrate with [`ContentstackHttpRequest`](../../Contentstack.Management.Core/Http/ContentstackHttpRequest.cs) and response parsing.

Concrete services live under **`Services/`** (including nested folders by domain, e.g. stack, organization, OAuth).

#### OAuth and auth

Implementation details span **`OAuthHandler`**, **`Services/OAuth/`**, and client token dictionaries on **`ContentstackClient`** (high-level behavior is under **Authentication** above).

#### Adding a feature (checklist)

1. Confirm the CMA contract (path, method, query vs body) from [official API docs](https://www.contentstack.com/docs/developers/apis/content-management-api/).
2. Implement or extend an **`IContentstackService`** (or reuse patterns from a sibling service in `Services/`).
3. Expose a method on the appropriate **`Stack`** / model type; keep public API consistent with existing naming.
4. Add **unit tests** (MSTest + mocks); add **integration** tests only when live API coverage is required.
5. If behavior touches HTTP retries or status codes, coordinate with [`../http-pipeline/SKILL.md`](../http-pipeline/SKILL.md).

### Query and parameters (fluent list API)

The management SDK exposes a **fluent `Query`** type for listing resources (stacks, entries, assets, roles, etc.). It is separate from the Content Delivery SDK’s query DSL.

#### Types

| Type | Location | Role |
| ---- | -------- | ---- |
| **`Query`** | [`Queryable/Query.cs`](../../Contentstack.Management.Core/Queryable/Query.cs) | Fluent methods add entries to an internal `ParameterCollection`, then `Find` / `FindAsync` runs `QueryService`. |
| **`ParameterCollection`** | [`Queryable/ParameterCollection.cs`](../../Contentstack.Management.Core/Queryable/ParameterCollection.cs) | `SortedDictionary<string, QueryParamValue>` with overloads for `string`, `double`, `bool`, `List<string>`, etc. |

#### Typical usage pattern

Models such as **`Entry`**, **`Asset`**, **`Role`**, **`Stack`** expose **`Query()`** returning a **`Query`** bound to that resource path. Chain parameters, then call **`Find()`** or **`FindAsync()`**:

```csharp
// Illustrative — see XML examples on Query/Stack/Entry in the codebase.
ContentstackResponse response = client
    .Stack("<API_KEY>", "<MANAGEMENT_TOKEN>")
    .ContentType("<CONTENT_TYPE_UID>")
    .Entry()
    .Query()
    .Limit(10)
    .Skip(0)
    .Find();
```

Requirements enforced by **`Query`**:

- Stack must be logged in where applicable (`ThrowIfNotLoggedIn`).
- Stack API key must be present for the call (`ThrowIfAPIKeyEmpty`).

#### Extra parameters on `Find`

`Find(ParameterCollection collection = null)` and `FindAsync` merge an optional **`ParameterCollection`** into the query before building **`QueryService`**. Use this when ad-hoc parameters are not exposed as fluent methods.

#### Implementing new fluent methods

1. Add a method on **`Query`** that calls `_collection.Add("api_key", value)` (or the appropriate `ParameterCollection` overload).
2. Return **`this`** for chaining.
3. Add XML documentation with a short `<example>` consistent with existing **`Query`** members.
4. Add unit tests if serialization or parameter keys are non-trivial.

#### Relationship to `IContentstackService`

List operations ultimately construct a **`QueryService`** that implements **`IContentstackService`** and is executed through **`ContentstackClient.InvokeSync` / `InvokeAsync`**. Path and HTTP details live on the service; the fluent API only shapes **`ParameterCollection`**. For the overall request path, see **Architecture** above.
