using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    internal sealed class DiagnosticsBackendProvider : ILoggingBackendProvider
    {
        public ILoggingBackend GetBackend(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "console":
                    return new ConsoleBackend();
                case "trace":
                    return new TraceBackend();
                default:
                    return null;
            }
        }
    }
}