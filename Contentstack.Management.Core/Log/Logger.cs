using System;
namespace Contentstack.Management.Core.Log
{
    public abstract class Logger
    {
        public Type DeclaringType { get; set; }

        /// <summary>
        /// Enable/Disable the logging.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Enable/Disable the Error logs.
        /// </summary>
        public bool IsErrorEnabled { get; set; } = true;

        /// <summary>
        /// Enable/Disable the Debug logs.
        /// </summary>
        public bool IsDebugEnabled { get; set; } = true;

        /// <summary>
        /// Enable/Disable the InfoFormat logs.
        /// </summary>
        /// 
        public bool IsInfoEnabled { get; set; } = true;

        #region Constructor
        public Logger(Type declaringType)
        {
            DeclaringType = declaringType;
            IsEnabled = true;
        }
        #endregion

        #region Logging methods
        /// <summary>
        /// Flushes the logger contents.
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// Simple wrapper around the Error method.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public abstract void Error(Exception exception, string messageFormat, params object[] args);

        /// <summary>
        /// Simple wrapper around the Debug method.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public abstract void Debug(Exception exception, string messageFormat, params object[] args);

        /// <summary>
        /// Simple wrapper around the DebugFormat method.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        public abstract void DebugFormat(string message, params object[] arguments);

        /// <summary>
        /// Simple wrapper around the InfoFormat method.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        public abstract void InfoFormat(string message, params object[] arguments);

        #endregion
    }
}
