using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Queryable
{
    [TestClass]
    public class ParameterCollectionTest
    {
        private JsonLoadSettings price_in_usd;

        [TestMethod]
        public void Initialize_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();

            Assert.IsNotNull(collection);
            Assert.IsNotNull(collection.GetSortedParametersList());
            Assert.AreEqual(0, collection.GetSortedParametersList().Count);

        }

        [TestMethod]
        public void Should_Add_Double_Value_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();

            collection.Add("limit", 10);

            Assert.AreEqual(1, collection.GetSortedParametersList().Count);
            Assert.IsTrue(collection.ContainsKey("limit"));
            Assert.IsInstanceOfType(collection["limit"], typeof(DoubleParameterValue));
            Assert.AreEqual(10, (collection["limit"] as DoubleParameterValue).Value);
        }

        [TestMethod]
        public void Should_Add_Bool_Value_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();

            collection.Add("include_count", true);

            Assert.AreEqual(1, collection.GetSortedParametersList().Count);
            Assert.IsTrue(collection.ContainsKey("include_count"));
            Assert.IsInstanceOfType(collection["include_count"], typeof(BoolParameterValue));
            Assert.AreEqual(true, (collection["include_count"] as BoolParameterValue).Value);
        }
        [TestMethod]
        public void Should_Add_String_Value_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();

            collection.Add("param1", "str_value");
            
            Assert.AreEqual(1, collection.GetSortedParametersList().Count);
            Assert.IsTrue(collection.ContainsKey("param1"));
            Assert.IsInstanceOfType(collection["param1"], typeof(StringParameterValue));
            Assert.AreEqual("str_value", (collection["param1"] as StringParameterValue).Value);
        }

        [TestMethod]
        public void Should_Add_String_List_Value_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();
            List<string> vs = new List<string>() { "1", "2", "3" };

            collection.Add("param1", vs);

            Assert.AreEqual(3, collection.GetSortedParametersList().Count);
            Assert.IsTrue(collection.ContainsKey("param1"));
            Assert.IsInstanceOfType(collection["param1"], typeof(StringListParameterValue));
            Assert.AreEqual(vs, (collection["param1"] as StringListParameterValue).Value);
        }

        [TestMethod]
        public void Should_Add_Double_List_Value_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();
            List<double> vs = new List<double>() { 1, 2, 3 };

            collection.Add("param1", vs);

            Assert.AreEqual(3, collection.GetSortedParametersList().Count);
            Assert.IsTrue(collection.ContainsKey("param1"));
            Assert.IsInstanceOfType(collection["param1"], typeof(DoubleListParameterValue));
            Assert.AreEqual(vs, (collection["param1"] as DoubleListParameterValue).Value);
        }

        [TestMethod]
        public void Should_Add_Query_JObject_In_Parameter_Collection()
        {
            ParameterCollection collection = new ParameterCollection();
            JObject queryObject = JObject.Parse("{ \"price_in_usd\": { \"$lt\": 600 } }");

            collection.AddQuery(queryObject);

            Assert.IsTrue(collection.ContainsKey("query"));
            Assert.IsInstanceOfType(collection["query"], typeof(StringParameterValue));
            Assert.AreEqual(Uri.EscapeDataString(queryObject.ToString()), (collection["query"] as StringParameterValue).Value);
        }
    }
}
