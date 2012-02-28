using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendWriter
    {
        ITypeSignature LoggerType { get; }

        void EmitInitialization(InstructionWriter writer, string category);
        void EmitTrace(InstructionWriter writer, string message, Exception exception = null);
        void EmitInfo(InstructionWriter writer, string message, Exception exception = null);
        void EmitWarning(InstructionWriter writer, string message, Exception exception = null);
        void EmitError(InstructionWriter writer, string message, Exception exception = null);
        void EmitFatal(InstructionWriter writer, string message, Exception exception = null);
    }


    public interface ILoggingBackendInstance
    {
        void EmitWrite(InstructionWriter instructionWriter, string message, LogLevel logLevel);
        void EmitWriteException(InstructionWriter instructionWriter, string message, Exception exception, LogLevel logLevel);
    }

    public enum LogLevel
    {
        Trace,
        Info,
        Warning,
        Error,
        Fatal,
    }
}