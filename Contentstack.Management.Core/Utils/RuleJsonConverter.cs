using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Polymorphic converter for <see cref="Rule"/> so that derived types
    /// (BranchRules, ContentTypeRules, etc.) are serialized with their own
    /// properties when stored in a <c>List&lt;Rule&gt;</c>.
    ///
    /// System.Text.Json only serializes the declared type by default, which
    /// drops subclass-only fields like "module" and "branches" from the payload,
    /// causing the Management API to return HTTP 422.
    ///
    /// Register this converter in JsonSerializerOptions.Converters (not via
    /// [JsonConverter] attribute) so that WithoutConverter&lt;T&gt;() can remove it
    /// during recursive serialization and prevent infinite recursion.
    /// </summary>
    internal class RuleJsonConverter : JsonConverter<Rule>
    {
        public override void Write(Utf8JsonWriter writer, Rule value, JsonSerializerOptions options)
        {
            var opts = options.WithoutConverter<RuleJsonConverter>();
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), opts);
        }

        public override Rule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var opts = options.WithoutConverter<RuleJsonConverter>();

            if (doc.RootElement.TryGetProperty("module", out var moduleProp))
            {
                return moduleProp.GetString() switch
                {
                    "branch"       => doc.RootElement.Deserialize<BranchRules>(opts),
                    "branch_alias" => doc.RootElement.Deserialize<BranchAliasRules>(opts),
                    "content_type" => doc.RootElement.Deserialize<ContentTypeRules>(opts),
                    "asset"        => doc.RootElement.Deserialize<AssetRules>(opts),
                    "folder"       => doc.RootElement.Deserialize<FolderRules>(opts),
                    "environment"  => doc.RootElement.Deserialize<EnvironmentRules>(opts),
                    "taxonomy"     => doc.RootElement.Deserialize<TaxonomyRules>(opts),
                    _              => doc.RootElement.Deserialize<Rule>(opts),
                };
            }

            return doc.RootElement.Deserialize<Rule>(opts);
        }
    }
}
