using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Writers;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackend : LoggerBasedBackend
    {
        protected override ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module)
        {
            LoggingContext log4NetContext = new Log4NetContext(module);

            return new LoggingContextBackendWriter(log4NetContext);
        }
    }
}