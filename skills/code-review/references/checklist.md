# PR review checklist (CMA Management SDK)

Copy sections into a PR comment when useful. This checklist is for **this** repo (`HttpClient` + pipeline + MSTest), **not** the Content Delivery .NET SDK.

## Branch policy

```markdown
- [ ] **Default:** PR targets **`development`** unless this is a documented **hotfix** to **`main`**
- [ ] If base is **`main`**: head branch is **`staging`** (see `.github/workflows/check-branch.yml`)
```

## Breaking changes

```markdown
- [ ] No public method/property removed or narrowed without deprecation / major version plan
- [ ] `JsonProperty` / JSON names for API-facing models unchanged unless intentional and documented
- [ ] New required `ContentstackClientOptions` fields have safe defaults or are optional
- [ ] Strong naming: assembly signing still consistent if keys or `csproj` changed
```

## HTTP and pipeline

```markdown
- [ ] New or changed HTTP calls go through existing client/pipeline (`ContentstackClient` → `IContentstackService` → pipeline), not ad-hoc `HttpClient` usage inside services without justification
- [ ] Retry-sensitive changes reviewed alongside `RetryHandler` / `DefaultRetryPolicy` and unit tests under `Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/`
- [ ] Headers, query params, and path segments align with CMA docs; no hardcoded production URLs where options.Host should be used
```

## Services and query API

```markdown
- [ ] `IContentstackService` implementations set `ResourcePath`, `HttpMethod`, `Parameters` / `QueryResources` / `PathResources` / `Content` consistently with sibling services
- [ ] New fluent `Query` methods only add to `ParameterCollection` with correct API parameter names
```

## Tests

```markdown
- [ ] Unit tests use MSTest; `sh ./Scripts/run-unit-test-case.sh` passes for core changes
- [ ] Integration tests only when needed; no secrets committed (`appsettings.json` stays local)
```

## Security and hygiene

```markdown
- [ ] No API keys, tokens, or passwords in source or test data checked into git
- [ ] OAuth / token handling does not log secrets
```

## Documentation

```markdown
- [ ] User-visible behavior reflected in `README.md` or release notes when appropriate
- [ ] `skills/` or `references/` updated if agent/contributor workflow changed
```
