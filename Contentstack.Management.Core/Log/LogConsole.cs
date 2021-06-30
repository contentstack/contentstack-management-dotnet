using System;
namespace Contentstack.Management.Core.Log
{
    public class ConsoleLog : Logger
    {
        enum LogLevel
        {
            Verbose = 2,
            Debug = 3,
            Info = 4,
            Warn = 5,
            Error = 6,
            Assert = 7
        }

        public ConsoleLog (Type type):
            base(type)
        {

        }

        public override void Debug(Exception exception, string messageFormat, params object[] args)
        {
            this.Log(LogLevel.Debug, string.Format(messageFormat, args), exception);
        }

        public override void DebugFormat(string message, params object[] arguments)
        {
            this.Log(LogLevel.Debug, string.Format(message, arguments), null);
        }

        public override void Error(Exception exception, string messageFormat, params object[] args)
        {
            this.Log(LogLevel.Error, string.Format(messageFormat, args), exception);
        }

        public override void Flush()
        {
           
        }

        public override void InfoFormat(string message, params object[] arguments)
        {
            this.Log(LogLevel.Info, string.Format(message, arguments), null);
        }

        private void Log(LogLevel logLevel, string message, Exception ex)
        {
            string formatted = null;

            string loglevelString = logLevel.ToString().ToUpper();
            string dt = DateTime.UtcNow.ToLocalTime().ToString();
            if (ex != null)
                formatted = string.Format("{0}|{1}|{2} --> {3}", dt, loglevelString, message, ex.ToString());
            else
                formatted = string.Format("{0}|{1}|{2}", dt, loglevelString, message);

            Console.WriteLine(@"{0} {1}", DeclaringType.Name, formatted);
        }
    }
}
