using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackend : LoggingBackend
    {
        protected override ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module)
        {
            LoggingContext traceContext = new TraceContextBuilder(module).CreateContext();

            return new TraceBackendWriter(traceContext);
        }
    }
}