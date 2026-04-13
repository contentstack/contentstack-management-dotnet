# Skills – Contentstack Management .NET SDK

Source of truth for detailed guidance. Read [`AGENTS.md`](../AGENTS.md) first, then open the skill that matches your task.

## Project context

| Area | This repo (CMA SDK) |
| ---- | ------------------- |
| **API** | [Content Management API (CMA)](https://www.contentstack.com/docs/developers/apis/content-management-api/) — not the Content Delivery API (CDA). |
| **Packages** | `contentstack.management.csharp` (core), `contentstack.management.aspnetcore` (DI helpers). |
| **HTTP** | `HttpClient` through [`ContentstackClient`](../Contentstack.Management.Core/ContentstackClient.cs) → runtime pipeline (`HttpHandler`, `RetryHandler`). |
| **Language / tests** | C# 8, nullable enabled. **MSTest** on **net7.0**; unit tests use **Moq** and **AutoFixture** where existing tests do. |

## How to use these skills

- **In the repo:** open `skills/<folder>/SKILL.md` for the topic you need.
- **In Cursor / other AI chats:** reference a skill by path, e.g. `skills/http-pipeline/SKILL.md` or `@skills/http-pipeline` if your tooling resolves that alias to this folder.

## Example prompts

- “Add a new CMA endpoint wrapper following `skills/contentstack-management-dotnet-sdk/SKILL.md`.”
- “Adjust retry behavior for 429 responses using `skills/http-pipeline/SKILL.md` and update unit tests under `Contentstack.Management.Core.Unit.Tests/Runtime/Pipeline/`.”
- “Write a unit test with MSTest + Moq following `skills/testing/SKILL.md`.”

## When to use which skill

| Skill folder | Use when |
| ------------ | -------- |
| [`dev-workflow/`](dev-workflow/) | Git branches, CI workflows, running build/test scripts, release/NuGet flow. |
| [`contentstack-management-dotnet-sdk/`](contentstack-management-dotnet-sdk/) | `ContentstackClient`, options, authentication, public API and package boundaries. |
| [`testing/`](testing/) | Writing or running unit/integration tests, MSTest, coverlet, local credentials. |
| [`code-review/`](code-review/) | Preparing or reviewing a PR against this repository. |
| [`framework/`](framework/) | Target frameworks, signing, NuGet packaging, OS-specific builds, high-level HTTP/runtime stack. |
| [`csharp-style/`](csharp-style/) | C# language version, nullable usage, naming and folder layout consistent with the repo. |
| [`http-pipeline/`](http-pipeline/) | Changing `HttpHandler`, `RetryHandler`, retry policy, or pipeline ordering. |
| [`aspnetcore-integration/`](aspnetcore-integration/) | `Contentstack.Management.ASPNETCore` package, `IHttpClientFactory`, DI registration. |
| [`documentation/`](documentation/) | Building or updating DocFX API documentation under `docfx_project/`. |

Each folder contains **`SKILL.md`** with YAML frontmatter (`name`, `description`).
