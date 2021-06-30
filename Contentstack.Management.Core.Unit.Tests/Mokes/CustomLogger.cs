using System;
using Contentstack.Management.Core.Log;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    public class CustomLogger : Logger 
    {
        public CustomLogger(Type type) : base(type){}

        public int DebugCount = 0;
        public int DebugFormatCount = 0;
        public int ErrorCount = 0;
        public int InfoFormatCount = 0;
        public int FlushCount = 0;

        public override void Debug(Exception exception, string messageFormat, params object[] args)
        {
            DebugCount++;
        }

        public override void DebugFormat(string message, params object[] arguments)
        {
            DebugFormatCount++;
        }

        public override void Error(Exception exception, string messageFormat, params object[] args)
        {
            ErrorCount++;
        }

        public override void Flush()
        {
            FlushCount++;
        }

        public override void InfoFormat(string message, params object[] arguments)
        {
            InfoFormatCount++;
        }
    }
}
