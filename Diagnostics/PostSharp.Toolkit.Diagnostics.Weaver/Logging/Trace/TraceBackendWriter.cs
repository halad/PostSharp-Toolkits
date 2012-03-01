using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackendWriter : LoggingBackendWriter
    {
        public TraceBackendWriter(LoggingContext loggingContext)
            : base(loggingContext)
        {
        }

        public override void EmitTrace(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, LoggingContext.TraceMethod);
        }

        public override void EmitInfo(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, LoggingContext.InfoMethod);
        }

        public override void EmitWarning(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, LoggingContext.WarningMethod);
        }

        public override void EmitError(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, LoggingContext.ErrorMethod);
        }

        public override void EmitFatal(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, LoggingContext.FatalMethod);
        }
    }
}