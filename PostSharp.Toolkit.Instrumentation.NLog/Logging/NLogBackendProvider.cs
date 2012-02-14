using System;
using PostSharp.Toolkit.Instrumentation.Weaver.Logging;

namespace PostSharp.Toolkit.Instrumentation.Weaver.NLog.Logging
{
    public sealed class NLogBackendProvider : ILoggingBackendProvider
    {
        public ILoggingBackend GetBackend(string name)
        {
            if (name.Equals("nlog", StringComparison.OrdinalIgnoreCase))
                return new NLogBackend();

            return null;
        }
    }
}