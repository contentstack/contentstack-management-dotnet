---
name: documentation
description: Use when building or updating DocFX API documentation under docfx_project for this repository.
---

# Documentation (DocFX) – Contentstack Management .NET SDK

## When to use

- Regenerating or editing API reference docs.
- Updating [`docfx_project/docfx.json`](../../docfx_project/docfx.json), TOC, or filters.

## Instructions

### Layout

- DocFX project root: [`docfx_project/`](../../docfx_project/)
- Key files: [`docfx.json`](../../docfx_project/docfx.json), [`toc.yml`](../../docfx_project/toc.yml), [`filterRules.yml`](../../docfx_project/filterRules.yml), [`index.md`](../../docfx_project/index.md)

### Metadata source

- `docfx.json` metadata section references project files under `src/**.csproj` in the DocFX config; **verify paths** match this repo’s layout (core library lives under `Contentstack.Management.Core/`, not necessarily `src/`). Update metadata `src` paths if doc generation fails after moves.

### Build

- Install DocFX per [official instructions](https://dotnet.github.io/docfx/), then run from `docfx_project` (typical: `docfx docfx.json` or `docfx build docfx.json`). Exact CLI may vary by DocFX version—use the version your team standardizes on.

### Relationship to product docs

- End-user NuGet and usage examples stay in root [`README.md`](../../README.md). DocFX is for **API reference** material.

[DocFX](https://dotnet.github.io/docfx/) (official).
