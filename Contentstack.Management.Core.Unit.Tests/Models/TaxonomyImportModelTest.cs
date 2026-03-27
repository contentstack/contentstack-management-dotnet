using System;
using System.IO;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class TaxonomyImportModelTest
    {
        [TestMethod]
        public void Throws_When_FilePath_Is_Null()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new TaxonomyImportModel((string)null));
            Assert.AreEqual("filePath", ex.ParamName);
        }

        [TestMethod]
        public void Throws_When_FilePath_Is_Empty()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new TaxonomyImportModel(""));
            Assert.AreEqual("filePath", ex.ParamName);
        }

        [TestMethod]
        public void Throws_When_Stream_Is_Null()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new TaxonomyImportModel((Stream)null, "taxonomy.json"));
            Assert.AreEqual("stream", ex.ParamName);
        }
    }
}
