using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Queryable
{
    [TestClass]
    public class QueryParamValueTest
    {
        [TestMethod]
        public void Initialize_Double_Parameter()
        {
            DoubleParameterValue value = new DoubleParameterValue(10);

            Assert.AreEqual(10, value.Value);
        }

        [TestMethod]
        public void Initialize_String_Parameter()
        {
            StringParameterValue value = new StringParameterValue("value");

            Assert.AreEqual("value", value.Value);
        }

        [TestMethod]
        public void Initialize_String_List_Parameter()
        {
            List<string> vs = new List<string>() { "1", "2", "3" };
            StringListParameterValue value = new StringListParameterValue(vs);

            Assert.AreEqual(vs, value.Value);
        }

        [TestMethod]
        public void Initialize_Double_List_Parameter()
        {
            List<double> vs = new List<double>() { 1, 2, 3 };
            DoubleListParameterValue value = new DoubleListParameterValue(vs);

            Assert.AreEqual(vs, value.Value);
        }
    }
}
