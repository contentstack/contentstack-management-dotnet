using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Log;

namespace Contentstack.Management.Core.Internal
{
    public class LogManager : ILogManager
    {
        internal List<Logger> loggers;
        private static LogManager emptyLogger = new LogManager();

        private LogManager()
        {
            loggers = new List<Logger>();
        }

        public void Debug(Exception exception, string messageFormat, params object[] args)
        {
            foreach (Logger logger in loggers)
            {
                if (logger.IsEnabled && logger.IsDebugEnabled)
                {
                    logger.Debug(exception, messageFormat, args);
                }
            }
        }

        public void DebugFormat(string messageFormat, params object[] args)
        {
            foreach (Logger logger in loggers)
            {
                if (logger.IsEnabled && logger.IsDebugEnabled)
                {
                    logger.DebugFormat(messageFormat, args);
                }
            }
        }

        public void Error(Exception exception, string messageFormat, params object[] args)
        {
            foreach (Logger logger in loggers)
            {
                if (logger.IsEnabled && logger.IsErrorEnabled)
                {
                    logger.Error(exception, messageFormat, args);
                }
            }
        }

        public void Flush()
        {
            foreach (Logger logger in loggers)
            {
                logger.Flush();
            }
        }

        public void InfoFormat(string messageFormat, params object[] args)
        {
            foreach (Logger logger in loggers)
            {
                if (logger.IsEnabled && logger.IsInfoEnabled)
                {
                    logger.InfoFormat(messageFormat, args);
                }
            }
        }

        public void AddLogger(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            loggers.Add(logger);
        }

        public static LogManager EmptyLogger { get { return emptyLogger; } }

        public static LogManager GetLogManager(Type type)
        {
            var logManager = new LogManager();

            var consoleLog = new ConsoleLog(type);
            logManager.AddLogger(consoleLog);

            return logManager;
        }
    }
}
