using System;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackendProvider : ILoggingBackendProvider
    {
        public ILoggingBackend GetBackend(string name)
        {
            if (name.Equals("log4net", StringComparison.OrdinalIgnoreCase))
                return new Log4NetBackend();

            return null;
        }
    }
}