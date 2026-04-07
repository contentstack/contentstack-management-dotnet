---
name: http-pipeline
description: Use when changing HTTP handlers, retry behavior, or pipeline ordering in Contentstack.Management.Core.Runtime.Pipeline.
---

# HTTP pipeline and retries – Contentstack Management .NET SDK

**Deep dive:** [`references/retry-and-handlers.md`](references/retry-and-handlers.md) (handler order, configuration, tests).

## When to use

- Modifying [`RetryHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryHandler.cs), [`DefaultRetryPolicy`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/DefaultRetryPolicy.cs), or [`RetryConfiguration`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryConfiguration.cs).
- Adding or reordering pipeline handlers in [`ContentstackClient.BuildPipeline`](../../Contentstack.Management.Core/ContentstackClient.cs).
- Debugging retry loops, HTTP status codes treated as retryable, or network error classification.

## Instructions

### Pipeline construction

- [`ContentstackClient.BuildPipeline`](../../Contentstack.Management.Core/ContentstackClient.cs) creates:
  1. [`HttpHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/HttpHandler/HttpHandler.cs) — wraps the SDK `HttpClient`.
  2. [`RetryHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryHandler.cs) — applies `RetryPolicy` (custom from options, or `DefaultRetryPolicy` from `RetryConfiguration.FromOptions`).

- Custom policies: `ContentstackClientOptions.RetryPolicy` can supply a user-defined `RetryPolicy`; otherwise `DefaultRetryPolicy` + `RetryConfiguration` apply.

### Retry configuration

- Defaults and toggles (retry limit, delay, which error classes to retry) live in [`RetryConfiguration`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryConfiguration.cs).
- HTTP status codes handled by default policy include 5xx, 429, timeouts, and related cases—see `DefaultRetryPolicy` for the authoritative set.

### Tests

- Unit tests under [`Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/`](../../Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/) (e.g. `RetryHandler`, `RetryDelayCalculator`) should be updated when behavior changes.

### Relationship to other docs

- [`../framework/SKILL.md`](../framework/SKILL.md) has a short overview; this skill is the **detailed** place for pipeline edits.

## References

- [`references/retry-and-handlers.md`](references/retry-and-handlers.md) — retries and handlers detail.
- [`../framework/SKILL.md`](../framework/SKILL.md) — TFMs and packaging.
- [`../contentstack-management-dotnet-sdk/SKILL.md`](../contentstack-management-dotnet-sdk/SKILL.md) — client options and public API.
