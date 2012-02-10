using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Instrumentation.Weaver.Logging
{
    public interface ILoggingBackend
    {
        void Initialize(ModuleDeclaration module);
        void EmitWrite(string message, InstructionWriter instructionWriter);
    }
}