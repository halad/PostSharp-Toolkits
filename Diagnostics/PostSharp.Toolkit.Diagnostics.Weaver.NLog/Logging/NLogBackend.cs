using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogBackend : ILoggingBackend
    {
        public void Initialize(ModuleDeclaration module)
        {
            
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new NLogBackendInstance();
        }

        private class NLogBackendInstance : ILoggingBackendInstance
        {
            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return null;
            }
        }
    }
}