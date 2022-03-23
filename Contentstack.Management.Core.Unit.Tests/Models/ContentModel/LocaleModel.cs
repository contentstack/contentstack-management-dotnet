using System;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    public class LocaleModel : ILocale
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string FallbackLocale { get; set; }
    }
}
