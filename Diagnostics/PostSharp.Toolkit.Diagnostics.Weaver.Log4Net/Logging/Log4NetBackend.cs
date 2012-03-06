using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackend : ILoggingBackend
    {
        public void Initialize(ModuleDeclaration module)
        {
            
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new Log4NetBackendInstance();
        }

        private class Log4NetBackendInstance : ILoggingBackendInstance
        {
            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return null;
            }
        }
    }
}