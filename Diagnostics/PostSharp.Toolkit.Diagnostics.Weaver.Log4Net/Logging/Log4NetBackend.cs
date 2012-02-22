using System;
using System.Collections.Generic;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context;
using log4net.Repository.Hierarchy;

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