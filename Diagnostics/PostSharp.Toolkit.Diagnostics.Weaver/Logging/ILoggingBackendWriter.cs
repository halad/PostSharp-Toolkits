using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendWriter
    {
        LoggingBackendMethods LoggingBackendMethods { get; }
        void EmitTrace(InstructionWriter writer, string message);
        void EmitTraceException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        void EmitInfo(InstructionWriter writer, string message);
        void EmitInfoException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        void EmitWarning(InstructionWriter writer, string message);
        void EmitWarningException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        void EmitError(InstructionWriter writer, string message);
        void EmitErrorException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        void EmitFatal(InstructionWriter writer, string message);
        void EmitFatalException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);

    }
}