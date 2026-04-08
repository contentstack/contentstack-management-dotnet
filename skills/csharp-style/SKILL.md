---
name: csharp-style
description: Use for C# language level, nullable usage, and file/folder layout consistent with Contentstack.Management.Core.
---

# C# style – Contentstack Management .NET SDK

## When to use

- Writing new C# code in `Contentstack.Management.Core` or test projects.
- Choosing names, nullability, and structure for PRs.

## Instructions

### Language version

- Core library uses **C# 8.0** (`LangVersion` in [`contentstack.management.core.csproj`](../../Contentstack.Management.Core/contentstack.management.core.csproj)).
- **Nullable reference types** are enabled (`Nullable` is `enable`). Prefer explicit nullability on public APIs (`?`, `!` only when justified and consistent with existing code).

### Project layout (core)

Follow existing top-level folders:

- `Services/` — API-facing service classes by domain (stack, organization, OAuth, etc.).
- `Models/` — DTOs and domain models (including `Models/Fields/`).
- `Runtime/` — pipeline, HTTP handlers, execution context.
- `Exceptions/` — SDK exception types.
- `Abstractions/`, `Queryable/`, `Utils/` — as used today.

New features should land in the same folder that similar code uses; avoid new root folders without team agreement.

### Naming and patterns

- Match existing naming: PascalCase for public types and methods; private fields often camelCase with underscore prefix where already used in the file.
- Prefer `async`/`await` patterns consistent with neighboring methods when adding asynchronous APIs.
- Keep XML doc comments on public surface when the rest of the type is documented that way.

### Tests

- MSTest: `[TestClass]`, `[TestMethod]`, `[ClassInitialize]` where the suite requires shared setup (see integration tests).
- Use existing assertion helpers (e.g. `AssertLogger` in integration tests) where applicable.

### What not to do

- Do not introduce a different language version or disable nullable without an explicit team decision.
- Do not impose style rules that contradict the majority of files in the same directory.
