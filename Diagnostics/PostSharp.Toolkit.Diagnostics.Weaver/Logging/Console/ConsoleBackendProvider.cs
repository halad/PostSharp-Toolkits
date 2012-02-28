using System;
using PostSharp.Sdk.AspectWeaver;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console
{
    internal sealed class ConsoleBackendProvider : ILoggingBackendProvider
    {
        public ILoggingBackend GetBackend(string name)
        {
            if (name.Equals("console", StringComparison.OrdinalIgnoreCase))
                return new ConsoleBackend();

            return null;
        }
    }
}