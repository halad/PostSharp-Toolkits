using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendWriter
    {
        LoggingContext LoggingContext { get; }
        void EmitTrace(InstructionWriter writer, string message, Exception exception = null);
        void EmitInfo(InstructionWriter writer, string message, Exception exception = null);
        void EmitWarning(InstructionWriter writer, string message, Exception exception = null);
        void EmitError(InstructionWriter writer, string message, Exception exception = null);
        void EmitFatal(InstructionWriter writer, string message, Exception exception = null);
    }
}