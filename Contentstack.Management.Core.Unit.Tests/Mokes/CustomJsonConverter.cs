using Contentstack.Management.Core.Attributes;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Marker types for <see cref="CsmJsonConverterAttribute"/> tests (not used as real JSON converters).
    /// </summary>
    [CsmJsonConverter("CustomAutoload")]
    public class CustomJsonConverter
    {
    }

    [CsmJsonConverter("CustomManualLoad", false)]
    public class CustomConverter
    {
    }
}
