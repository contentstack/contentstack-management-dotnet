# Contentstack Management .NET SDK: Comprehensive Migration Guide
## Newtonsoft.Json → System.Text.Json

This document provides a complete migration strategy for the Contentstack Management .NET SDK when moving from **Newtonsoft.Json** to **System.Text.Json**. This represents a **breaking change** requiring a **new major version** of the SDK.

**Audience:** SDK developers, customers using the Management SDK, and teams planning the migration across all modules.

---

## Table of Contents

1. [Migration Status Overview](#migration-status-overview)
2. [Why This Changes Your Code](#why-this-changes-your-code)
3. [Quick Reference Mapping](#quick-reference-mapping)
4. [Module-by-Module Migration](#module-by-module-migration)
5. [Customer Migration Patterns](#customer-migration-patterns)
6. [Asset Module Deep Dive](#asset-module-deep-dive)
7. [Testing Strategy](#testing-strategy)
8. [Implementation Timeline](#implementation-timeline)
9. [Risk Assessment & Mitigation](#risk-assessment--mitigation)
10. [Complete Migration Checklist](#complete-migration-checklist)
11. [Troubleshooting Common Issues](#troubleshooting-common-issues)

---

## Migration Status Overview

The SDK migration spans multiple modules with varying complexity levels. Below is the current status across all modules:

| Module Name | Code Changes with Related Files | Test Cases Changes | Documentation Changes | Implementation Time |
|-------------|--------------------------------|-------------------|----------------------|-------------------|
| **Asset** COMPLETED | **Yes** - 3 JsonProperty attrs, response methods, 62+ test calls | **Yes** - 4,161 lines, 62+ OpenJObjectResponse calls | **Yes** - Migration patterns, examples | **0.5 day** |
| **Stack** PLANNED | **Yes** - StackSettings (3 attrs), Stack model, response handling | **Yes** - 1,191 lines, ~25+ response calls | **Yes** - Core client patterns | **0.5 day** |
| **Organisation** PLANNED | **Yes** - Organization model, user invitations, role management | **Yes** - 578 lines, ~15+ response calls | **Yes** - Auth & management flows | **0.5 day** |
| **Taxonomy** PLANNED | **Yes** - TaxonomyModel (11 attrs), TermModel (19 attrs), complex tree structures | **Yes** - 6,575 lines, ~45+ response calls | **Yes** - Hierarchical data patterns | **0.5 day** |
| **Terms** PLANNED | **Yes** - TermModel integration, taxonomy relationships | **Yes** - Included in Taxonomy tests | **Yes** - Term management examples | **0.5 day** |
| **Users** PLANNED | **Yes** - User model, authentication, profile management | **Yes** - Included in Login/Organization tests | **Yes** - User auth patterns | **0.5 day** |
| **Content Type** IN_PROGRESS | **Yes** - ContentModelling (13 attrs), Field models (40+ attrs), schema validation | **Yes** - 2,273 lines (1,326+947), ~35+ response calls | **Yes** - Schema migration guide | **0.5 day** |
| **Entries** IN_PROGRESS | **Yes** - Entry model, IEntry interface, complex field handling | **Yes** - 5,884 lines, ~50+ response calls | **Yes** - Content management patterns | **0.5 day** |
| **Locale** PLANNED | **Yes** - LocaleModel (3 attrs), localization services | **Yes** - Integrated in Entry/ContentType tests | **Yes** - Localization examples | **0.5 day** |
| **Webhook** PLANNED | **Yes** - WebhookModel (11 attrs), event handling | **Yes** - Integrated with other module tests | **Yes** - Event handling patterns | **0.5 day** |
| **Workflows** PLANNED | **Yes** - WorkflowModel (16 attrs), EntryWorkflowStage (16 attrs), state management | **Yes** - 3,722 lines, ~35+ response calls | **Yes** - Workflow state patterns | **0.5 day** |
| **Entry Variants** PLANNED | **Yes** - EntryVariant models, PublishVariant (2 attrs), complex relationships | **Yes** - 3,460 lines, ~30+ response calls | **Yes** - Variant management | **0.5 day** |
| **Delivery Tokens** PLANNED | **Yes** - DeliveryTokenModel (12 attrs), token management | **Yes** - 2,708 lines, ~25+ response calls | **Yes** - Token auth patterns | **0.5 day** |
| **Variant Group** PLANNED | **Yes** - VariantGroup models, group management | **Yes** - 3,122 lines, ~30+ response calls | **Yes** - Group management | **0.5 day** |
| **Bulk Operation** PLANNED | **Yes** - BulkOperationModels (56 attrs), complex async operations | **Yes** - 4,344 lines, ~40+ response calls | **Yes** - Async operation patterns | **0.5 day** |
| **Global Fields** PLANNED | **Yes** - GlobalField models, GlobalFieldRefs (4 attrs), nested references | **Yes** - 4,586 lines (934+3,652), ~40+ response calls | **Yes** - Global field patterns | **0.5 day** |
| **Release Management** PLANNED | **Yes** - ReleaseModel (13 attrs), release items, publishing workflows | **Yes** - 1,655 lines, ~20+ response calls | **Yes** - Release cycle patterns | **0.5 day** |
| **Roles & Permissions** PLANNED | **Yes** - RoleModel (24 attrs), permission management, AssetRules | **Yes** - 2,891 lines, ~25+ response calls | **Yes** - Permission patterns | **0.5 day** |
| **Environment** PLANNED | **Yes** - EnvironmentModel (7 attrs), environment management | **Yes** - 1,894 lines, ~20+ response calls | **Yes** - Environment setup | **0.5 day** |

### Summary Statistics
- **Total Modules**: 19 modules
- **Total Integration Test Lines**: ~51,000+ lines  
- **Total JsonProperty Attributes**: ~300+ across 49 files
- **Total OpenJObjectResponse Calls**: ~500+ method calls to update
- **Total Estimated Time**: 2-3 weeks for complete migration (9.5 working days)

### Legend
- **COMPLETED**: Full migration implemented and tested
- **IN_PROGRESS**: Migration currently underway
- **PLANNED**: Scheduled for migration (priority order listed)

### Migration Complexity Analysis

**Based on SDK codebase analysis:**
- **49 files** contain JsonProperty attributes requiring updates
- **~51,000+ lines** of integration test code need migration
- **~500+ method calls** to OpenJObjectResponse() need updates
- **19 modules** total requiring systematic migration

**Priority Implementation Order:**
1. **High Priority** (Customer Impact): Entries, Content Types, Assets (COMPLETED)
2. **Medium Priority** (Core Features): Stack, Bulk Operations, Taxonomy  
3. **Lower Priority** (Advanced Features): Workflows, Variants, Webhooks

**Note**: The Asset module (COMPLETED) serves as the reference implementation pattern for all other modules.

---

## Why This Changes Your Code

The SDK currently exposes Newtonsoft.Json types and settings throughout its public API:

| Area | Current Newtonsoft Usage | Impact Level |
|------|--------------------------|--------------|
| **Client configuration** | `ContentstackClient.SerializerSettings` | **HIGH** - Property name changes |
| **Raw JSON handling** | `JObject`, `JToken`, query parameters | **HIGH** - Type changes throughout |
| **Public methods** | `ContentstackResponse.OpenJObjectResponse()`, `ParameterCollection.AddQuery(JObject)` | **CRITICAL** - Method signature changes |
| **Model attributes** | `[JsonProperty(propertyName: "...")]` on all model classes | **HIGH** - Hundreds of attributes |
| **Custom converters** | `JsonConverter<T>` implementations for fields and nodes | **HIGH** - Complete rewrite required |
| **Exception handling** | `Newtonsoft.Json.JsonException` | **LOW** - Namespace change only |

After migration, all Newtonsoft types become **System.Text.Json** equivalents with **no binary or source compatibility**.

---

## Quick Reference Mapping

| Newtonsoft.Json | System.Text.Json | Migration Notes |
|-----------------|------------------|-----------------|
| `JsonSerializerSettings` | `JsonSerializerOptions` | Property configuration changes |
| `JsonConvert.SerializeObject(obj, settings)` | `JsonSerializer.Serialize(obj, options)` | Method name change |
| `JsonConvert.DeserializeObject<T>(json, settings)` | `JsonSerializer.Deserialize<T>(json, options)` | Method name change |
| `JObject.Parse(json)` | `JsonNode.Parse(json)!.AsObject()` | Return type handling |
| `JArray.Parse(json)` | `JsonNode.Parse(json)!.AsArray()` | Return type handling |
| `token.ToObject<T>(serializer)` | `JsonSerializer.Deserialize<T>(token.GetRawText(), options)` | Deserialization pattern |
| `jObject["field"]?.Value<string>()` | `jsonObject["field"]?.GetValue<string>()` | Value extraction method |
| `[JsonProperty("field_uid")]` | `[JsonPropertyName("field_uid")]` | Attribute name change |
| `Newtonsoft.Json.JsonConverter<T>` | `System.Text.Json.Serialization.JsonConverter<T>` | Namespace + rewrite required |

---

## Detailed Change Analysis by Module Type

### High-Complexity Modules (0.5 day each)

#### Entries Module (5,884 test lines)
- **Model Changes**: Entry class, IEntry interface, complex field handling
- **JsonProperty Attributes**: Field-specific serialization across multiple field types
- **Integration Tests**: 50+ OpenJObjectResponse calls, complex content operations
- **Special Considerations**: Content relationships, field validation, publishing workflows

#### Content Types Module (2,273 test lines) 
- **Model Changes**: ContentModelling (13 attrs), Field models (40+ attrs across field types)
- **JsonProperty Attributes**: Schema definition, field metadata, validation rules
- **Integration Tests**: 35+ OpenJObjectResponse calls, schema operations
- **Special Considerations**: Field schema validation, polymorphic field deserialization

#### Taxonomy Module (6,575 test lines)
- **Model Changes**: TaxonomyModel (11 attrs), TermModel (19 attrs), hierarchical structures
- **JsonProperty Attributes**: Tree structure serialization, parent-child relationships  
- **Integration Tests**: 45+ OpenJObjectResponse calls, complex tree operations
- **Special Considerations**: Hierarchical data integrity, term relationships

### Medium-Complexity Modules (0.5 day each)

#### Bulk Operations Module (4,344 test lines)
- **Model Changes**: BulkOperationModels (56 attrs), async operation tracking
- **JsonProperty Attributes**: Operation status, item lists, result tracking
- **Integration Tests**: 40+ OpenJObjectResponse calls, async validation
- **Special Considerations**: Async operation patterns, status polling, error handling

#### Global Fields Module (4,586 test lines)
- **Model Changes**: GlobalField models, GlobalFieldRefs (4 attrs), nested references
- **JsonProperty Attributes**: Field references, nested field structures
- **Integration Tests**: 40+ OpenJObjectResponse calls, reference validation
- **Special Considerations**: Cross-content-type references, field inheritance

### Standard-Complexity Modules (0.5 day each)

#### Standard Pattern for Most Modules:
1. **Model Updates**: Replace `[JsonProperty(propertyName: "name")]` with `[JsonPropertyName("name")]`
2. **Test Updates**: Replace `OpenJObjectResponse()` with `OpenJsonObjectResponse()` 
3. **Navigation Updates**: Replace `?.ToString()` with `?.GetValue<string>()`
4. **Collection Updates**: Initialize with `= new()` syntax

---

## Module-by-Module Migration

### 1. Client Configuration Changes

**All modules are affected by client-level configuration changes.**

#### Before (Newtonsoft — current SDK)
```csharp
using Newtonsoft.Json;

var client = new ContentstackClient(options);

// Configure JSON handling
client.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
client.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
client.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

// Register custom converters
client.SerializerSettings.Converters.Add(new MyNewtonsoftConverter());
```

#### After (System.Text.Json — migrated SDK)
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

var client = new ContentstackClient(options);

// Configure JSON handling (new property name)
client.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
client.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // if needed
// Note: STJ uses ISO 8601 by default for dates

// Register custom converters (must be rewritten for STJ)
client.SerializerOptions.Converters.Add(new MySystemTextJsonConverter());
```

### 2. Response Processing Changes

**Affects all modules that process API responses.**

#### Before (Response Reading)
```csharp
using Newtonsoft.Json.Linq;

ContentstackResponse response = await entry.CreateAsync(model);
JObject raw = response.OpenJObjectResponse();
string uid = raw["entry"]?["uid"]?.Value<string>();
```

#### After (Response Reading)
```csharp
using System.Text.Json.Nodes;

ContentstackResponse response = await entry.CreateAsync(model);
JsonObject raw = response.OpenJsonObjectResponse(); // Method name change
string? uid = raw["entry"]?["uid"]?.GetValue<string>();
```

### 3. Query Parameter Changes

**Affects modules with query functionality (Entries, Assets, Content Types, etc.).**

#### Before (Query Building)
```csharp
using Newtonsoft.Json.Linq;

var query = stack.ContentType("product").Entry().Query();
var filter = new JObject 
{
    ["title"] = "Featured Product",
    ["status"] = "published"
};
query.Parameters.AddQuery(filter);
```

#### After (Query Building)
```csharp
using System.Text.Json.Nodes;

var query = stack.ContentType("product").Entry().Query();
var filter = new JsonObject 
{
    ["title"] = "Featured Product",
    ["status"] = "published"  
};
query.Parameters.AddQuery(filter); // Same method, different parameter type
```

---

## Customer Migration Patterns

### Pattern 1: Basic CRUD Operations

**Applies to: All modules (Assets, Entries, Content Types, etc.)**

#### Before
```csharp
using Newtonsoft.Json.Linq;

// Create operation
var createResponse = await stack.Asset().CreateAsync(assetModel);
var responseObj = createResponse.OpenJObjectResponse();
var uid = responseObj["asset"]["uid"]?.ToString();

// Fetch operation
var fetchResponse = await stack.Asset(uid).FetchAsync();
var assetData = fetchResponse.OpenJObjectResponse();
var title = assetData["asset"]["title"]?.ToString();
```

#### After
```csharp
using System.Text.Json.Nodes;

// Create operation
var createResponse = await stack.Asset().CreateAsync(assetModel);
var responseObj = createResponse.OpenJsonObjectResponse();
var uid = responseObj["asset"]?["uid"]?.GetValue<string>();

// Fetch operation
var fetchResponse = await stack.Asset(uid).FetchAsync();
var assetData = fetchResponse.OpenJsonObjectResponse();
var title = assetData["asset"]?["title"]?.GetValue<string>();
```

### Pattern 2: Complex Query Operations

**Applies to: Entries, Assets, Content Types with filtering**

#### Before
```csharp
using Newtonsoft.Json.Linq;

var query = stack.ContentType("blog").Entry().Query();
var complexFilter = new JObject
{
    ["title"] = new JObject 
    {
        ["$regex"] = "^Featured"
    },
    ["tags"] = new JObject
    {
        ["$in"] = new JArray { "news", "updates" }
    }
};
query.Parameters.AddQuery(complexFilter);

var response = await query.FindAsync();
var entries = response.OpenJObjectResponse()["entries"]?.ToArray();
```

#### After
```csharp
using System.Text.Json.Nodes;

var query = stack.ContentType("blog").Entry().Query();
var complexFilter = new JsonObject
{
    ["title"] = new JsonObject 
    {
        ["$regex"] = "^Featured"
    },
    ["tags"] = new JsonObject
    {
        ["$in"] = new JsonArray { "news", "updates" }
    }
};
query.Parameters.AddQuery(complexFilter);

var response = await query.FindAsync();
var entries = response.OpenJsonObjectResponse()["entries"]?.AsArray();
```

### Pattern 3: Bulk Operations

**Applies to: Bulk Asset, Entry, and Publishing operations**

#### Before
```csharp
using Newtonsoft.Json.Linq;

var bulkItems = new List<BulkPublishAsset>();
foreach (var response in createResponses)
{
    var data = response.OpenJObjectResponse();
    bulkItems.Add(new BulkPublishAsset 
    {
        Uid = data["asset"]["uid"]?.ToString() // Uses [JsonProperty]
    });
}
```

#### After
```csharp
using System.Text.Json.Nodes;

var bulkItems = new List<BulkPublishAsset>();
foreach (var response in createResponses)
{
    var data = response.OpenJsonObjectResponse();
    bulkItems.Add(new BulkPublishAsset 
    {
        Uid = data["asset"]?["uid"]?.GetValue<string>() ?? "" // Uses [JsonPropertyName]
    });
}
```

---

## Asset Module Deep Dive

*The Asset module serves as the reference implementation for all other modules.*

### Complete File Inventory

**Files Requiring Changes:**

| File Path | Changes Required | Impact Level |
|-----------|------------------|--------------|
| `BulkOperationModels.cs` | 3 JsonProperty attributes → JsonPropertyName | **HIGH** |
| `RoleModel.cs` | 2 JsonProperty attributes → JsonPropertyName | **HIGH** |
| `ContentstackResponse.cs` | Method signature + constructor changes | **CRITICAL** |
| `IResponse.cs` | Interface method signature | **HIGH** |
| `ContentstackClient.cs` | Property type change | **HIGH** |
| `Contentstack013_AssetTest.cs` | 62+ OpenJObjectResponse calls | **CRITICAL** |
| `AssetTest.cs` | Mock configuration updates | **MEDIUM** |
| `MockResponse.cs` | Constructor parameter changes | **MEDIUM** |

### Key Code Changes

#### 1. BulkOperationModels.cs Changes

**Before:**
```csharp
using Newtonsoft.Json;

public class BulkPublishAsset
{
    [JsonProperty(propertyName: "uid")]
    public string Uid { get; set; }
}
```

**After:**
```csharp
using System.Text.Json.Serialization;

public class BulkPublishAsset
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = "";
}
```

#### 2. Response Method Changes

**Before:**
```csharp
using Newtonsoft.Json.Linq;

public JObject OpenJObjectResponse()
{
    ThrowIfDisposed();
    return JObject.Parse(OpenResponse());
}
```

**After:**
```csharp
using System.Text.Json.Nodes;

public JsonObject OpenJsonObjectResponse()
{
    ThrowIfDisposed();
    return JsonNode.Parse(OpenResponse())!.AsObject();
}
```

#### 3. Integration Test Updates

**Before:**
```csharp
var responseObject = response.OpenJObjectResponse();
var assetUid = responseObject["asset"]["uid"]?.ToString();
var filename = responseObject["asset"]["filename"]?.ToString();
```

**After:**
```csharp
var responseObject = response.OpenJsonObjectResponse();
var assetUid = responseObject["asset"]?["uid"]?.GetValue<string>();
var filename = responseObject["asset"]?["filename"]?.GetValue<string>();
```

### Asset Migration Time Estimates

| Component | Time Estimate | Complexity |
|-----------|---------------|------------|
| Model attribute changes | 30 minutes | Low |
| Response method changes | 1 hour | Medium |
| Integration test updates | 8-12 hours | High |
| Unit test infrastructure | 2-3 hours | Medium |
| Testing & validation | 4-6 hours | Medium |

**Total Asset Module: 4-6 hours (0.5 day)**

---

## Testing Strategy

### 1. Unit Test Migration

**Pattern for all modules:**

#### Before (Newtonsoft)
```csharp
using Newtonsoft.Json;

[TestMethod]
public void Should_Serialize_Model_Correctly()
{
    var settings = new JsonSerializerSettings();
    settings.Converters.Add(new CustomConverter());
    
    var json = JsonConvert.SerializeObject(model, settings);
    var result = JsonConvert.DeserializeObject<Model>(json, settings);
    
    Assert.AreEqual(expected, result.Property);
}
```

#### After (System.Text.Json)
```csharp
using System.Text.Json;

[TestMethod]
public void Should_Serialize_Model_Correctly()
{
    var options = new JsonSerializerOptions();
    options.Converters.Add(new CustomConverter());
    
    var json = JsonSerializer.Serialize(model, options);
    var result = JsonSerializer.Deserialize<Model>(json, options);
    
    Assert.AreEqual(expected, result.Property);
}
```

### 2. Integration Test Migration

**Pattern for all modules with API responses:**

#### Before
```csharp
[TestMethod]
public async Task Should_Create_Resource_Successfully()
{
    var response = await client.Stack(apiKey, token)
        .Resource()
        .CreateAsync(model);
        
    var responseObject = response.OpenJObjectResponse();
    var resourceUid = responseObject["resource"]["uid"]?.Value<string>();
    
    Assert.IsNotNull(resourceUid);
}
```

#### After
```csharp
[TestMethod] 
public async Task Should_Create_Resource_Successfully()
{
    var response = await client.Stack(apiKey, token)
        .Resource()
        .CreateAsync(model);
        
    var responseObject = response.OpenJsonObjectResponse();
    var resourceUid = responseObject["resource"]?["uid"]?.GetValue<string>();
    
    Assert.IsNotNull(resourceUid);
}
```

### 3. Mock Infrastructure Updates

**Required for all test projects:**

#### Before
```csharp
public static ContentstackResponse CreateMockResponse(string fileName)
{
    var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(File.ReadAllText($"MockData/{fileName}"))
    };
    
    var serializer = JsonSerializer.Create(new JsonSerializerSettings());
    return new ContentstackResponse(httpResponse, serializer);
}
```

#### After
```csharp
public static ContentstackResponse CreateMockResponse(string fileName)
{
    var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(File.ReadAllText($"MockData/{fileName}"))
    };
    
    var options = new JsonSerializerOptions();
    return new ContentstackResponse(httpResponse, options);
}
```

---

## Implementation Timeline

### Phase 1: Core Infrastructure (Week 1)
- [ ] **ContentstackClient.SerializerSettings → SerializerOptions**
- [ ] **ContentstackResponse method signature changes**
- [ ] **IResponse interface updates**
- [ ] **Base test infrastructure updates**

### Phase 2: Model Attributes (Week 2)
- [ ] **Scan all model files for [JsonProperty] attributes**
- [ ] **Replace with [JsonPropertyName] systematically**
- [ ] **Update collection initializers to use new() syntax**

### Phase 3: Module Implementation (Week 3)

**Optimized Priority Order with Reduced Effort Estimates:**

#### Week 3: All Modules (5 working days)
**Day 1:**
1. **Asset Module** COMPLETED (0.5 day - completed reference implementation)
2. **Entry Module** IN_PROGRESS (0.5 day - 5,884 test lines, 50+ response calls)

**Day 2:**  
3. **Content Type Module** IN_PROGRESS (0.5 day - 2,273 test lines, 40+ field attributes)
4. **Stack Module** (0.5 day - 1,191 test lines, core client patterns)

**Day 3:**
5. **Global Fields Module** (0.5 day - 4,586 test lines, nested references)  
6. **Taxonomy Module** (0.5 day - 6,575 test lines, hierarchical data)

**Day 4:**
7. **Bulk Operations** (0.5 day - 4,344 test lines, 56 attributes, async operations)
8. **Workflows** (0.5 day - 3,722 test lines, 32+ attributes across models)

**Day 5:**
9. **Entry Variants** (0.5 day - 3,460 test lines, variant relationships)
10. **Variant Groups** (0.5 day - 3,122 test lines, group management)

#### Additional 4.5 Days (Remaining 9 Modules)
**Days 6-10:**
11. **Roles & Permissions** (0.5 day - 2,891 test lines, 24+ attributes)
12. **Delivery Tokens** (0.5 day - 2,708 test lines, 12 attributes)
13. **Environment** (0.5 day - 1,894 test lines, 7 attributes)
14. **Release Management** (0.5 day - 1,655 test lines, release workflows)
15. **Organisation** (0.5 day - 578 test lines, user management)
16. **Webhook** (0.5 day - event handling patterns)
17. **Users/Auth** (0.5 day - authentication flows)
18. **Locale** (0.5 day - localization services)
19. **Terms** (0.5 day - taxonomy integration)

### Phase 4: Integration Testing (Week 4 - Days 1-2)
- [ ] **Module-by-module integration test validation**
- [ ] **Cross-module integration verification**
- [ ] **Performance regression testing**

### Phase 5: Documentation & Release (Week 4 - Days 3-5)
- [ ] **Update all code examples in documentation**
- [ ] **Finalize migration guides for each module**
- [ ] **Prepare release notes and breaking change documentation**

---

## Risk Assessment & Mitigation

### Critical Risks

| Risk | Impact | Probability | Mitigation Strategy |
|------|---------|-------------|-------------------|
| **Mass integration test failures** | **Critical** | **High** | 51k+ lines need updates; systematic pattern-based approach |
| **Customer code breaks completely** | **Critical** | **High** | 500+ method calls break; comprehensive migration guide |
| **Field serialization errors** | **Critical** | **Medium** | 300+ JsonProperty attrs; automated validation scripts |
| **Performance regressions** | **High** | **Medium** | Benchmark critical paths; STJ performance monitoring |
| **Complex converter migration** | **High** | **High** | Field polymorphism critical; reference implementations |
| **Timeline overrun** | **Medium** | **Medium** | 8-9 week estimate; module-by-module validation |

### Module-Specific Risks

| Module | Primary Risk | Mitigation |
|--------|-------------|------------|
| **Entries** | Complex field relationships break | Comprehensive field testing, type validation |
| **Content Types** | Schema validation failures | Field polymorphism testing, converter validation |  
| **Taxonomy** | Hierarchical data integrity | Tree structure validation, relationship testing |
| **Bulk Operations** | Async operation tracking fails | Status polling tests, error handling validation |
| **Global Fields** | Cross-reference resolution breaks | Reference integrity tests, dependency validation |

### High-Risk Areas

#### 1. Polymorphic Field Deserialization
**Problem**: `FieldJsonConverter` complexity for content type schemas  
**Solution**: Careful STJ converter rewrite with extensive validation

#### 2. Scale of Model Attributes  
**Problem**: Hundreds of `[JsonProperty]` attributes across 39+ model files  
**Solution**: Automated scanning and replacement with validation scripts

#### 3. No SelectToken Equivalent
**Problem**: Complex JSON path operations break  
**Solution**: Convert to typed model deserialization or manual navigation

#### 4. Binary Incompatibility
**Problem**: Complete breaking change for all customers  
**Solution**: Major version bump with comprehensive migration support

### Mitigation Strategies

1. **Automated Testing**: Run full integration test suite after each module migration
2. **Beta Release Program**: Early access for key customers to validate migration
3. **Migration Tooling**: Scripts to help identify customer code that needs updates  
4. **Support Documentation**: Detailed FAQ and troubleshooting guide
5. **Rollback Plan**: Maintain parallel Newtonsoft version during transition period

---

## Complete Migration Checklist

### For SDK Development Team

#### Infrastructure Changes
- [ ] Update `ContentstackClient.SerializerSettings` → `SerializerOptions`
- [ ] Update `ContentstackResponse.OpenJObjectResponse()` → `OpenJsonObjectResponse()`
- [ ] Update `IResponse` interface method signatures
- [ ] Update constructor parameters throughout pipeline
- [ ] Update exception handling for `System.Text.Json.JsonException`

#### Model Changes (Per Module)
- [ ] Replace all `[JsonProperty(propertyName: "name")]` → `[JsonPropertyName("name")]`
- [ ] Update collection initializers with `= new()` syntax
- [ ] Update custom converter implementations
- [ ] Validate serialization output matches API requirements

#### Test Infrastructure
- [ ] Update mock response creation with `JsonSerializerOptions`
- [ ] Update test client configuration
- [ ] Update assertion patterns for JSON navigation
- [ ] Validate all unit tests pass

#### Integration Tests (Per Module)
- [ ] Update all `OpenJObjectResponse()` calls
- [ ] Update JSON field access from `.ToString()` → `.GetValue<T>()`
- [ ] Update array iteration patterns
- [ ] Update complex object deserialization
- [ ] Ensure null-conditional operators throughout

#### Documentation
- [ ] Update all code examples in API documentation
- [ ] Create module-specific migration guides
- [ ] Update README with migration information
- [ ] Add troubleshooting section

### For Customer Migration

#### Code Analysis
- [ ] Search codebase for `JObject`, `JToken`, `JArray`
- [ ] Find all `JsonConvert` usage
- [ ] Locate `JsonSerializerSettings` configurations
- [ ] Identify `[JsonProperty]` attributes in custom models
- [ ] Find custom Newtonsoft `JsonConverter` implementations

#### Client Configuration Updates
- [ ] Replace `SerializerSettings` with `SerializerOptions`
- [ ] Update null handling configuration
- [ ] Update date handling configuration
- [ ] Migrate custom converter registrations

#### Response Processing Updates
- [ ] Change `OpenJObjectResponse()` → `OpenJsonObjectResponse()`
- [ ] Update JSON navigation patterns
- [ ] Update type extraction methods
- [ ] Add null-conditional operators

#### Query Building Updates
- [ ] Replace `JObject` → `JsonObject` in query parameters
- [ ] Replace `JArray` → `JsonArray` in filter construction
- [ ] Update complex query object building

#### Testing Updates
- [ ] Update integration tests with new response patterns
- [ ] Update mock setups with new serializer configuration
- [ ] Update assertion patterns
- [ ] Run full test suite validation

#### Exception Handling
- [ ] Update catch blocks for `System.Text.Json.JsonException`
- [ ] Update error message parsing if applicable

---

## Troubleshooting Common Issues

### Issue 1: "Method not found: OpenJObjectResponse"

**Symptoms**: Compilation error when calling response methods  
**Cause**: Method renamed in migration  
**Solution**: Replace with `OpenJsonObjectResponse()`

```csharp
// Before (breaks)
var data = response.OpenJObjectResponse();

// After (works)
var data = response.OpenJsonObjectResponse();
```

### Issue 2: "Cannot convert JObject to JsonObject"

**Symptoms**: Type mismatch errors in query building  
**Cause**: Parameter type changes in AddQuery methods  
**Solution**: Use JsonObject instead of JObject

```csharp
// Before (breaks)
var filter = new JObject { ["key"] = "value" };

// After (works) 
var filter = new JsonObject { ["key"] = "value" };
```

### Issue 3: "Value<T>() method not found"

**Symptoms**: JSON navigation compilation errors  
**Cause**: Different value extraction API in System.Text.Json  
**Solution**: Use GetValue<T>() method

```csharp
// Before (breaks)
var uid = jsonObj["uid"]?.Value<string>();

// After (works)
var uid = jsonObj["uid"]?.GetValue<string>();
```

### Issue 4: "JsonProperty attribute not recognized"

**Symptoms**: Serialization using wrong property names  
**Cause**: Attribute name change  
**Solution**: Replace with JsonPropertyName

```csharp
// Before (breaks)
[JsonProperty(propertyName: "content_type")]

// After (works)
[JsonPropertyName("content_type")]
```

### Issue 5: Custom converter errors

**Symptoms**: Converter registration or execution failures  
**Cause**: Different converter base class and methods  
**Solution**: Rewrite converters for System.Text.Json

```csharp
// Before (Newtonsoft)
public class MyConverter : JsonConverter<MyType>
{
    public override MyType ReadJson(JsonReader reader, ...) { }
    public override void WriteJson(JsonWriter writer, ...) { }
}

// After (System.Text.Json)
public class MyConverter : JsonConverter<MyType>
{
    public override MyType Read(ref Utf8JsonReader reader, ...) { }
    public override void Write(Utf8JsonWriter writer, ...) { }
}
```

### Issue 6: Performance differences

**Symptoms**: Slower response processing or memory usage changes  
**Cause**: Different performance characteristics between libraries  
**Solution**: Profile and optimize critical paths

- Use JsonSerializerOptions caching
- Consider source generators for high-performance scenarios
- Monitor memory allocation patterns

### Issue 7: Null handling differences  

**Symptoms**: Unexpected null values or deserialization failures  
**Cause**: Stricter null handling in System.Text.Json  
**Solution**: Add proper null checking and configuration

```csharp
// Configure null handling
client.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

// Use null-conditional operators
var value = jsonObj["field"]?.GetValue<string>();
```

---

## Support Resources

### Migration Tools
- **Code Scanner**: Script to identify Newtonsoft usage patterns
- **Attribute Replacer**: Tool to update JsonProperty attributes
- **Test Validator**: Automated integration test verification

### Documentation Links
- [System.Text.Json Overview](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview)
- [Migrate from Newtonsoft.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/migrate-from-newtonsoft-how-to)
- [JsonConverter Migration Guide](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to)

### Team Contacts
- **SDK Team**: For core migration questions and technical issues
- **Developer Relations**: For customer migration support and feedback
- **Documentation Team**: For migration guide updates and improvements

---

## Migration Summary & Statistics

### Overall Scope
- **Total Modules**: 19 modules requiring migration
- **Completed Modules**: 1 (Asset COMPLETED)
- **Remaining Modules**: 18 modules
- **Estimated Timeline**: 2-3 weeks total effort

### Code Change Statistics  
- **Integration Test Files**: 18 files totaling 51,000+ lines
- **JsonProperty Attributes**: ~300+ attributes across 49+ files
- **Method Call Updates**: ~500+ OpenJObjectResponse() calls
- **Model Files**: 100+ model files requiring updates

### Effort Distribution
- **High Complexity** (3 modules): 1.5 days total (0.5 day each)
  - Entries, Content Types, Taxonomy
- **Medium Complexity** (8 modules): 4 days total (0.5 day each)  
  - Bulk Operations, Global Fields, Workflows, Variants, etc.
- **Standard Complexity** (8 modules): 4 days total (0.5 day each)
  - Stack, Organization, Webhooks, Environment, etc.

### Customer Impact Assessment
- **Breaking Changes**: 100% of SDK-facing code breaks
- **Migration Required**: All customers must update before upgrading
- **Support Period**: 6 months parallel support recommended
- **Beta Testing**: 2-month beta period for major customers

### Success Criteria
- [ ] All 51k+ integration test lines pass
- [ ] All 300+ JsonProperty attributes converted
- [ ] All 500+ method calls updated
- [ ] Zero API behavior changes
- [ ] Performance parity maintained
- [ ] Customer migration guide validated

---

## Document History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 2026-05-07 | Initial comprehensive migration guide with detailed module analysis | SDK Team |
| 1.1 | 2026-05-07 | Updated timeline to 2-3 days per module, removed emojis, revised to 8-9 week total | SDK Team |
| 1.2 | 2026-05-11 | Optimized timeline to 0.5 day per module, compressed total to 2-3 weeks | SDK Team |

---

This comprehensive migration guide provides detailed analysis of all 19 modules in the SDK migration. It will be updated as each module completes its migration, providing customers with module-specific guidance and the development team with implementation patterns and validation criteria.

**Next Update**: After Content Types and Entries modules complete migration (Week 3, Day 2)