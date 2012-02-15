using System;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
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