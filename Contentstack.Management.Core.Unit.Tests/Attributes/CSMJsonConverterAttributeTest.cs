using System;
using System.Reflection;
using Contentstack.Management.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Attributes
{
    [TestClass]
    public class CSMJsonConverterAttributeTest
    {
        [TestMethod]
        public void Should_Autoload_Converters()
        {
            foreach (Type type in CsmJsonConverterAttribute.GetCustomAttribute(typeof(CsmJsonConverterAttribute)))
            {
                Assert.AreEqual("CustomJsonConverter", type.Name);
                Assert.AreNotEqual("CustomConverter", type.Name);
                foreach (var attr in type.GetCustomAttributes(typeof(CsmJsonConverterAttribute)))
                {
                    CsmJsonConverterAttribute ctdAttr = attr as CsmJsonConverterAttribute;
                    Assert.AreEqual("CustomAutoload", ctdAttr.Name);
                    Assert.AreNotEqual("CustomManualLoad", ctdAttr.Name);
                    Assert.AreEqual(true, ctdAttr.IsAutoloadEnable);

                }
            }
        }
    }
}
