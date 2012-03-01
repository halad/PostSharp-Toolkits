using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogBackend : LoggerBasedBackend
    {
        protected override ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module)
        {
            LoggingContext nlogContext = new NLogContextBuilder(module).CreateContext();

            return new LoggingContextBackendWriter(nlogContext);
        }
    }
}