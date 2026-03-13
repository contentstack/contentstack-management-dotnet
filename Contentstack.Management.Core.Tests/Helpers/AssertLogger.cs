using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.Helpers
{
    public static class AssertLogger
    {
        public static void IsNotNull(object value, string name = "")
        {
            bool passed = value != null;
            TestOutputLogger.LogAssertion($"IsNotNull({name})", "NotNull", value?.ToString() ?? "null", passed);
            Assert.IsNotNull(value);
        }

        public static void IsNull(object value, string name = "")
        {
            bool passed = value == null;
            TestOutputLogger.LogAssertion($"IsNull({name})", "null", value?.ToString() ?? "null", passed);
            Assert.IsNull(value);
        }

        public static void AreEqual<T>(T expected, T actual, string name = "")
        {
            bool passed = Equals(expected, actual);
            TestOutputLogger.LogAssertion($"AreEqual({name})", expected?.ToString() ?? "null", actual?.ToString() ?? "null", passed);
            Assert.AreEqual(expected, actual);
        }

        public static void AreEqual<T>(T expected, T actual, string message, string name)
        {
            bool passed = Equals(expected, actual);
            TestOutputLogger.LogAssertion($"AreEqual({name})", expected?.ToString() ?? "null", actual?.ToString() ?? "null", passed);
            Assert.AreEqual(expected, actual, message);
        }

        public static void IsTrue(bool condition, string name = "")
        {
            TestOutputLogger.LogAssertion($"IsTrue({name})", "True", condition.ToString(), condition);
            Assert.IsTrue(condition);
        }

        public static void IsTrue(bool condition, string message, string name)
        {
            TestOutputLogger.LogAssertion($"IsTrue({name})", "True", condition.ToString(), condition);
            Assert.IsTrue(condition, message);
        }

        public static void IsFalse(bool condition, string name = "")
        {
            TestOutputLogger.LogAssertion($"IsFalse({name})", "False", condition.ToString(), !condition);
            Assert.IsFalse(condition);
        }

        public static void IsFalse(bool condition, string message, string name)
        {
            TestOutputLogger.LogAssertion($"IsFalse({name})", "False", condition.ToString(), !condition);
            Assert.IsFalse(condition, message);
        }

        public static void IsInstanceOfType(object value, Type expectedType, string name = "")
        {
            bool passed = value != null && expectedType.IsInstanceOfType(value);
            TestOutputLogger.LogAssertion(
                $"IsInstanceOfType({name})",
                expectedType?.Name ?? "null",
                value?.GetType()?.Name ?? "null",
                passed);
            Assert.IsInstanceOfType(value, expectedType);
        }

        public static T ThrowsException<T>(Action action, string name = "") where T : Exception
        {
            try
            {
                action();
                TestOutputLogger.LogAssertion($"ThrowsException<{typeof(T).Name}>({name})", typeof(T).Name, "NoException", false);
                throw new AssertFailedException($"Expected exception {typeof(T).Name} was not thrown.");
            }
            catch (T ex)
            {
                TestOutputLogger.LogAssertion($"ThrowsException<{typeof(T).Name}>({name})", typeof(T).Name, typeof(T).Name, true);
                return ex;
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                TestOutputLogger.LogAssertion($"ThrowsException<{typeof(T).Name}>({name})", typeof(T).Name, ex.GetType().Name, false);
                throw new AssertFailedException(
                    $"Expected exception {typeof(T).Name} but got {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        public static void Fail(string message)
        {
            TestOutputLogger.LogAssertion("Fail", "N/A", message ?? "", false);
            Assert.Fail(message);
        }

        public static void Fail(string message, params object[] parameters)
        {
            TestOutputLogger.LogAssertion("Fail", "N/A", message ?? "", false);
            Assert.Fail(message, parameters);
        }

        public static void Inconclusive(string message)
        {
            TestOutputLogger.LogAssertion("Inconclusive", "N/A", message ?? "", false);
            Assert.Inconclusive(message);
        }
    }
}
