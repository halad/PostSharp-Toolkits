using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackend : LoggingBackend
    {
        protected override ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module)
        {
            LoggingBackendMethods traceBackendMethods = new TraceMethodsBuilder(module).CreateContext();

            return new TraceBackendWriter(traceBackendMethods);
        }
    }
}