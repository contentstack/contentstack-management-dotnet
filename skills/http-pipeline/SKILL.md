---
name: http-pipeline
description: Use when changing HTTP handlers, retry behavior, or pipeline ordering in Contentstack.Management.Core.Runtime.Pipeline.
---

# HTTP pipeline and retries – Contentstack Management .NET SDK

## When to use

- Modifying [`RetryHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryHandler.cs), [`DefaultRetryPolicy`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/DefaultRetryPolicy.cs), or [`RetryConfiguration`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryConfiguration.cs).
- Adding or reordering pipeline handlers in [`ContentstackClient.BuildPipeline`](../../Contentstack.Management.Core/ContentstackClient.cs).
- Debugging retry loops, HTTP status codes treated as retryable, or network error classification.

## Instructions

### Pipeline construction and handler order

[`ContentstackClient.BuildPipeline`](../../Contentstack.Management.Core/ContentstackClient.cs) registers handlers in this **outer-to-inner** order for execution:

1. [`HttpHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/HttpHandler/HttpHandler.cs) — sends the `HttpRequestMessage` via the SDK’s `HttpClient`.
2. [`RetryHandler`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryHandler.cs) — wraps the inner handler and applies **`RetryPolicy`**.

Incoming calls traverse **RetryHandler** first, which delegates to **HttpHandler** for the actual HTTP call, then inspects success/failure and may retry.

### Policy selection

- If **`ContentstackClientOptions.RetryPolicy`** is set, that instance is used.
- Otherwise **`RetryConfiguration.FromOptions(contentstackOptions)`** builds a **`RetryConfiguration`**, then **`new DefaultRetryPolicy(retryConfiguration)`**.

### Retry configuration

[`RetryConfiguration`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryConfiguration.cs) holds defaults and toggles, including:

- **`RetryOnError`**, **`RetryLimit`**, **`RetryDelay`**
- Network vs HTTP retries: **`RetryOnNetworkFailure`**, **`RetryOnDnsFailure`**, **`RetryOnSocketFailure`**, **`MaxNetworkRetries`**, **`NetworkRetryDelay`**, **`RetryOnHttpServerError`**, etc.

Exact defaults and edge cases belong in code comments and unit tests—**do not duplicate** the full matrix here; change the source and tests together.

### HTTP status codes (default policy)

[`DefaultRetryPolicy`](../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/DefaultRetryPolicy.cs) maintains a set of status codes that may trigger HTTP retries (e.g. selected 5xx, 429, timeouts, **Unauthorized** in the default set). When adjusting this list, consider:

- Risk of retrying non-idempotent operations.
- Interaction with auth refresh / OAuth flows on **`ContentstackClient`**.

### Tests

- Unit tests under [`Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/`](../../Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/) — e.g. `RetryHandler`, `RetryDelayCalculator`, `DefaultRetryPolicy`, `NetworkErrorDetector`, etc. Update these when behavior changes.
- Keep tests deterministic (short delays, mocked inner handlers).
