using System;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Internal
{
    [TestClass]
    public class LogManagerTest
    {
        LogManager LogManager = null;
        CustomLogger customLogger = new CustomLogger(typeof(LogManagerTest));

        [TestInitialize]
        public void TestInitialize()
        {
            LogManager = LogManager.GetLogManager(typeof(LogManagerTest));
            LogManager.AddLogger(customLogger);
        }

        [TestMethod]
        public void Empty_Log_Manager()
        {
            LogManager logManager = LogManager.EmptyLogger;
            Assert.IsNotNull(logManager);
        }

        [TestMethod]
        public void Logger_Log_Manager()
        {
            LogManager logManager = LogManager.GetLogManager(this.GetType());
            Assert.IsNotNull(logManager);
        }

        [TestMethod]
        public void Throw_On_Adding_Blank_Logger()
        {
            LogManager logManager = LogManager.GetLogManager(this.GetType());
            Assert.ThrowsException<ArgumentNullException>(() => logManager.AddLogger(null));
        }

        [TestMethod]
        public void Should_Add_Custom_Logger()
        {
            LogManager logManager = LogManager.GetLogManager(this.GetType());
            logManager.AddLogger(new CustomLogger(this.GetType()));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Should_Log_Debug_On_IsEnable()
        {
            LogManager.Debug(null, "TEST");
            LogManager.DebugFormat("TEST");
            LogManager.Error(null, "TEST");
            LogManager.InfoFormat("TEST");
            LogManager.Flush();

            Assert.AreEqual(customLogger.DebugCount, 1);
            Assert.AreEqual(customLogger.DebugFormatCount, 1);
            Assert.AreEqual(customLogger.ErrorCount, 1);
            Assert.AreEqual(customLogger.InfoFormatCount, 1);
            Assert.AreEqual(customLogger.FlushCount, 1);
        }

        [TestMethod]
        public void Should_Not_Log_Debug_On_IsEnable_False()
        {
            customLogger.IsEnabled = false;

            LogManager.Debug(null, "TEST");
            LogManager.DebugFormat("TEST");
            LogManager.Error(new ArgumentNullException("logger"), "TEST");
            LogManager.InfoFormat("TEST");
            LogManager.Flush();

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 1);
        }

        [TestMethod]
        public void Should_Log_Debug_On_IsDebug_Enable()
        {
            LogManager.Debug(null, "TEST");

            Assert.AreEqual(customLogger.DebugCount, 1);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Log_DebugFormat_On_IsDebug_Enable()
        {
            LogManager.DebugFormat("TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 1);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Log_Error_On_IsError_Enable()
        {
            LogManager.Error(new ArgumentNullException("logger"), "TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 1);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Log_Info_On_IsInfo_Enable()
        {
            LogManager.InfoFormat("TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 1);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Not_Log_Debug_On_IsDebug_Disable()
        {
            customLogger.IsDebugEnabled = false;
            LogManager.Debug(null, "TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Not_Log_DebugFormat_On_IsDebug_Disable()
        {
            customLogger.IsDebugEnabled = false;
            LogManager.DebugFormat("TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Not_Log_Error_On_IsError_Disable()
        {
            customLogger.IsErrorEnabled = false;
            LogManager.Error(new ArgumentNullException("logger"), "TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }

        [TestMethod]
        public void Should_Not_Log_Info_On_IsInfo_Disable()
        {
            customLogger.IsInfoEnabled = false;
            LogManager.InfoFormat("TEST");

            Assert.AreEqual(customLogger.DebugCount, 0);
            Assert.AreEqual(customLogger.DebugFormatCount, 0);
            Assert.AreEqual(customLogger.ErrorCount, 0);
            Assert.AreEqual(customLogger.InfoFormatCount, 0);
            Assert.AreEqual(customLogger.FlushCount, 0);
        }
    }
}
