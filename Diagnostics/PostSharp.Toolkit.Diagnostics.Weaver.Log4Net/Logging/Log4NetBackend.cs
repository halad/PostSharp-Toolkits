using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackend : LoggingContextBackend
    {
        protected override LoggingContext CreateContext(ModuleDeclaration module)
        {
            return new Log4NetContextBuilder(module).CreateContext();
        }
    }
}