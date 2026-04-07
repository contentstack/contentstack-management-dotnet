# CMA SDK architecture (Management .NET)

This document describes how requests flow through **this** SDK. It is **not** the Content Delivery (CDA) client: there is no `HttpWebRequest`-only layer or delivery-token query-string-only rule set here.

## Entry and configuration

1. **`ContentstackClient`** ([`ContentstackClient.cs`](../../../Contentstack.Management.Core/ContentstackClient.cs)) is the public entry point.
2. **`ContentstackClientOptions`** ([`ContentstackClientOptions.cs`](../../../Contentstack.Management.Core/ContentstackClientOptions.cs)) holds `Host`, tokens, proxy, retry settings, and optional custom `RetryPolicy`.
3. The client builds a **`ContentstackRuntimePipeline`** in `BuildPipeline()` (see [`../../framework/SKILL.md`](../../framework/SKILL.md) and [`../../http-pipeline/SKILL.md`](../../http-pipeline/SKILL.md)): **`HttpHandler`** → **`RetryHandler`**.

## Stack-scoped API

- **`Stack`** ([`Models/Stack.cs`](../../../Contentstack.Management.Core/Models/Stack.cs)) is obtained from the client (e.g. `client.Stack(apiKey, managementToken)` or overloads). Most management operations are stack-relative.
- Domain types under **`Models/`** (entries, assets, content types, etc.) expose methods that construct or call **`IContentstackService`** implementations.

## Service interface and invocation

- **`IContentstackService`** ([`Services/IContentstackService.cs`](../../../Contentstack.Management.Core/Services/IContentstackService.cs)) defines one CMA operation: resource path, HTTP method, headers, query/path resources, body (`HttpContent`), and hooks to build the outbound request and handle the response.
- The client executes services via **`InvokeSync`** / **`InvokeAsync`**, which run the pipeline and return **`ContentstackResponse`**.

Important members on `IContentstackService`:

- **`Parameters`** — [`ParameterCollection`](../../../Contentstack.Management.Core/Queryable/ParameterCollection.cs) for typed query/body parameters (used by list/query flows).
- **`QueryResources`**, **`PathResources`**, **`AddQueryResource`**, **`AddPathResource`** — URL composition.
- **`UseQueryString`** — some operations send parameters as query string instead of body.
- **`CreateHttpRequest`** / **`OnResponse`** — integrate with [`ContentstackHttpRequest`](../../../Contentstack.Management.Core/Http/ContentstackHttpRequest.cs) and response parsing.

Concrete services live under **`Services/`** (including nested folders by domain, e.g. stack, organization, OAuth).

## Fluent list queries (`Queryable`)

- **`Query`** ([`Queryable/Query.cs`](../../../Contentstack.Management.Core/Queryable/Query.cs)) provides a fluent API (`Limit`, `Skip`, `IncludeCount`, …) backed by an internal **`ParameterCollection`**.
- **`Find()`** / **`FindAsync()`** merge an optional extra `ParameterCollection`, construct **`QueryService`**, and invoke the client sync/async. See [`query-and-parameters.md`](query-and-parameters.md).

## OAuth and auth

- User/session auth, management tokens, and login flows are described at a high level in [`../SKILL.md`](../SKILL.md). Implementation details span **`OAuthHandler`**, **`Services/OAuth/`**, and client token dictionaries on **`ContentstackClient`**.

## Adding a feature (checklist)

1. Confirm the CMA contract (path, method, query vs body) from official API docs.
2. Implement or extend an **`IContentstackService`** (or reuse patterns from a sibling service in `Services/`).
3. Expose a method on the appropriate **`Stack`** / model type; keep public API consistent with existing naming.
4. Add **unit tests** (MSTest + mocks); add **integration** tests only when live API coverage is required.
5. If behavior touches HTTP retries or status codes, coordinate with [`../../http-pipeline/SKILL.md`](../../http-pipeline/SKILL.md).
