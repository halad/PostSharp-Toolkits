using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Instrumentation.Weaver.Logging
{
    public interface ILoggingBackend
    {
        void Initialize(ModuleDeclaration module);
        ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance);
    }
    
    public interface ILoggingBackendInstance
    {
        void EmitWrite(string message, InstructionWriter instructionWriter);
    }
}