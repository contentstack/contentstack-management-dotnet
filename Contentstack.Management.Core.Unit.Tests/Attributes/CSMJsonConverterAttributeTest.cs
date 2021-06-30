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
            foreach (Type type in CSMJsonConverterAttribute.GetCustomAttribute(typeof(CSMJsonConverterAttribute)))
            {
                Assert.AreEqual(type.Name, "CustomJsonConverter");
                Assert.AreNotEqual(type.Name, "CustomConverter");
                foreach (var attr in type.GetCustomAttributes(typeof(CSMJsonConverterAttribute)))
                {
                    CSMJsonConverterAttribute ctdAttr = attr as CSMJsonConverterAttribute;
                    Assert.AreEqual(ctdAttr.Name, "CustomAutoload");
                    Assert.AreNotEqual(ctdAttr.Name, "CustomManualLoad");
                    Assert.AreEqual(ctdAttr.IsAutoloadEnable, true);

                }
            }
        }
    }
}
