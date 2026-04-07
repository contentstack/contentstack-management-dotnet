# Query and parameters (fluent list API)

The management SDK exposes a **fluent `Query`** type for listing resources (stacks, entries, assets, roles, etc.). It is separate from the Content Delivery SDK’s query DSL.

## Types

| Type | Location | Role |
| ---- | -------- | ---- |
| **`Query`** | [`Queryable/Query.cs`](../../../Contentstack.Management.Core/Queryable/Query.cs) | Fluent methods add entries to an internal `ParameterCollection`, then `Find` / `FindAsync` runs `QueryService`. |
| **`ParameterCollection`** | [`Queryable/ParameterCollection.cs`](../../../Contentstack.Management.Core/Queryable/ParameterCollection.cs) | `SortedDictionary<string, QueryParamValue>` with overloads for `string`, `double`, `bool`, `List<string>`, etc. |

## Typical usage pattern

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

## Extra parameters on `Find`

`Find(ParameterCollection collection = null)` and `FindAsync` merge an optional **`ParameterCollection`** into the query before building **`QueryService`**. Use this when ad-hoc parameters are not exposed as fluent methods.

## Implementing new fluent methods

1. Add a method on **`Query`** that calls `_collection.Add("api_key", value)` (or the appropriate `ParameterCollection` overload).
2. Return **`this`** for chaining.
3. Add XML documentation with a short `<example>` consistent with existing **`Query`** members.
4. Add unit tests if serialization or parameter keys are non-trivial.

## Relationship to `IContentstackService`

List operations ultimately construct a **`QueryService`** that implements **`IContentstackService`** and is executed through **`ContentstackClient.InvokeSync` / `InvokeAsync`**. Path and HTTP details live on the service; the fluent API only shapes **`ParameterCollection`**.

For full request flow, see [`cma-architecture.md`](cma-architecture.md).
