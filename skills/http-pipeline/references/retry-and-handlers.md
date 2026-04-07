# Retry handlers and pipeline (deep dive)

## Handler order

[`ContentstackClient.BuildPipeline`](../../../Contentstack.Management.Core/ContentstackClient.cs) registers handlers in this **outer-to-inner** order for execution:

1. **`HttpHandler`** ([`HttpHandler.cs`](../../../Contentstack.Management.Core/Runtime/Pipeline/HttpHandler/HttpHandler.cs)) — sends the `HttpRequestMessage` via the SDK’s `HttpClient`.
2. **`RetryHandler`** ([`RetryHandler.cs`](../../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryHandler.cs)) — wraps the inner handler and applies **`RetryPolicy`**.

Incoming calls traverse **RetryHandler** first, which delegates to **HttpHandler** for the actual HTTP call, then inspects success/failure and may retry.

## Policy selection

- If **`ContentstackClientOptions.RetryPolicy`** is set, that instance is used.
- Otherwise **`RetryConfiguration.FromOptions(contentstackOptions)`** builds a **`RetryConfiguration`**, then **`new DefaultRetryPolicy(retryConfiguration)`**.

## `RetryConfiguration` (high level)

[`RetryConfiguration.cs`](../../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/RetryConfiguration.cs) controls:

- **`RetryOnError`**, **`RetryLimit`**, **`RetryDelay`**
- Network vs HTTP retries: **`RetryOnNetworkFailure`**, **`RetryOnDnsFailure`**, **`RetryOnSocketFailure`**, **`MaxNetworkRetries`**, **`NetworkRetryDelay`**, **`RetryOnHttpServerError`**, etc.

Exact defaults and edge cases belong in code comments and unit tests—**do not duplicate** the full matrix here; change the source and tests together.

## HTTP status codes (default policy)

[`DefaultRetryPolicy`](../../../Contentstack.Management.Core/Runtime/Pipeline/RetryHandler/DefaultRetryPolicy.cs) maintains a set of status codes that may trigger HTTP retries (e.g. selected 5xx, 429, timeouts, **Unauthorized** in the default set). When adjusting this list, consider:

- Risk of retrying non-idempotent operations.
- Interaction with auth refresh / OAuth flows on **`ContentstackClient`**.

## Where to add tests

- **`Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/`** — `RetryHandler`, `RetryDelayCalculator`, `DefaultRetryPolicy`, `NetworkErrorDetector`, etc.
- Keep tests deterministic (short delays, mocked inner handlers).

## Related skills

- [`../SKILL.md`](../SKILL.md) — summary for agents.
- [`../../framework/SKILL.md`](../../framework/SKILL.md) — TFMs and packaging.
