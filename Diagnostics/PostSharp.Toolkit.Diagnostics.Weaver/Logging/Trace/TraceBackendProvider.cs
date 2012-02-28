using System;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackendProvider : ILoggingBackendProvider
    {
        public ILoggingBackend GetBackend(string name)
        {
            if (name.Equals("trace", StringComparison.OrdinalIgnoreCase))
                return new TraceBackend();

            return null;
        }
    }
}