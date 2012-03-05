using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackend : LoggerBasedBackend
    {
        protected override ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module)
        {
            LoggingBackendMethods log4NetBackendMethods = new Log4NetMethodsBuilder(module).CreateContext();

            return new LoggingContextBackendWriter(log4NetBackendMethods);
        }
    }
}