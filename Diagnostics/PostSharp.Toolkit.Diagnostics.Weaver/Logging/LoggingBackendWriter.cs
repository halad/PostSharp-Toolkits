using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingBackendWriter : ILoggingBackendWriter
    {
        private readonly LoggingContext loggingContext;

        protected LoggingBackendWriter(LoggingContext loggingContext)
        {
            this.loggingContext = loggingContext;
        }

        public LoggingContext LoggingContext
        {
            get { return this.loggingContext; }
        }

        public abstract void EmitTrace(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitInfo(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitWarning(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitError(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitFatal(InstructionWriter writer, string message, Exception exception = null);
    }
}