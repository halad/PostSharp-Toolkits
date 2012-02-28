using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendInstance
    {
        void EmitWrite(InstructionWriter instructionWriter, string message, LogLevel logLevel);
        void EmitWriteException(InstructionWriter instructionWriter, string message, Exception exception, LogLevel logLevel);
    }
}