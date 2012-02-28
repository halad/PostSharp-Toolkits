using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Writers;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console
{
    internal sealed class ConsoleBackend : LoggingBackend
    {
        protected override ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module)
        {
            return new WriteLineBackendWriter(module, typeof(System.Console));
        }
    }
}