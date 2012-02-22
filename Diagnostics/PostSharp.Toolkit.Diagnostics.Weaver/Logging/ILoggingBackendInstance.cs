using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendInstance
    {
        void EmitWrite(string message, InstructionWriter instructionWriter);
    }
}