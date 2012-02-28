using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackend
    {
        void Initialize(ModuleDeclaration module);
        ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance);
    }
}