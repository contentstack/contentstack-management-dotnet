using System;
using Contentstack.Management.Core.Log;

namespace Contentstack.Management.Core.Internal
{
    public interface ILogManager
    {
        void AddLogger(Logger logger);
        void InfoFormat(string messageFormat, params object[] args);
        void Debug(Exception exception, string messageFormat, params object[] args);
        void DebugFormat(string messageFormat, params object[] args);
        void Error(Exception exception, string messageFormat, params object[] args);
        void Flush();
    }
}
