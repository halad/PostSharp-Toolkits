using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackendWriter : LoggingBackendWriter
    {
        public TraceBackendWriter(LoggingBackendMethods loggingBackendMethods)
            : base(loggingBackendMethods)
        {
        }

        public override void EmitTrace(InstructionWriter writer, string message)
        {
            this.EmitWrite(writer, message, LoggingBackendMethods.TraceMethod);
        }

        public override void EmitTraceException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            this.EmitWriteException(writer, message, exceptionSymbol, LoggingBackendMethods.TraceExceptionMethod);
        }

        public override void EmitInfo(InstructionWriter writer, string message)
        {
            this.EmitWrite(writer, message, LoggingBackendMethods.InfoMethod);
        }

        public override void EmitInfoException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            this.EmitWriteException(writer, message, exceptionSymbol, LoggingBackendMethods.InfoExceptionMethod);
        }

        public override void EmitWarning(InstructionWriter writer, string message)
        {
            this.EmitWrite(writer, message, LoggingBackendMethods.WarningMethod);
        }

        public override void EmitWarningException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            this.EmitWriteException(writer, message, exceptionSymbol, LoggingBackendMethods.WarningExceptionMethod);
        }

        public override void EmitError(InstructionWriter writer, string message)
        {
            this.EmitWrite(writer, message, LoggingBackendMethods.ErrorMethod);
        }

        public override void EmitErrorException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            this.EmitWriteException(writer, message, exceptionSymbol, LoggingBackendMethods.ErrorExceptionMethod);
        }

        public override void EmitFatal(InstructionWriter writer, string message)
        {
            this.EmitWrite(writer, message, LoggingBackendMethods.FatalMethod);
        }

        public override void EmitFatalException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            this.EmitWriteException(writer, message, exceptionSymbol, LoggingBackendMethods.FatalExceptionMethod);
        }
    }
}