using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingBackendWriter : ILoggingBackendWriter
    {
        private readonly LoggingBackendMethods loggingBackendMethods;

        protected LoggingBackendWriter(LoggingBackendMethods loggingBackendMethods)
        {
            this.loggingBackendMethods = loggingBackendMethods;
        }

        public LoggingBackendMethods LoggingBackendMethods
        {
            get { return this.loggingBackendMethods; }
        }

        public abstract void EmitTrace(InstructionWriter writer, string message);
        public abstract void EmitTraceException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        public abstract void EmitInfo(InstructionWriter writer, string message);
        public abstract void EmitInfoException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        public abstract void EmitWarning(InstructionWriter writer, string message);
        public abstract void EmitWarningException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        public abstract void EmitError(InstructionWriter writer, string message);
        public abstract void EmitErrorException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);
        public abstract void EmitFatal(InstructionWriter writer, string message);
        public abstract void EmitFatalException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol);

        protected void EmitWrite(InstructionWriter writer, string message, IMethod targetMethod)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            this.EmitCallHandler(targetMethod, writer);
        }

        protected virtual void EmitWriteException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol, IMethod targetMethod)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
                this.EmitCallHandler(targetMethod, writer);
            }

            writer.EmitInstructionLocalVariable(OpCodeNumber.Ldloc, exceptionSymbol);
            writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.loggingBackendMethods.Module.FindMethod(
                this.loggingBackendMethods.Module.Cache.GetType(typeof(object)), "ToString"));

            this.EmitCallHandler(targetMethod, writer);
        }

        private void EmitCallHandler(IMethod method, InstructionWriter writer)
        {
            writer.EmitInstructionMethod(
                !method.IsVirtual || (method.IsSealed || method.DeclaringType.IsSealed) ? OpCodeNumber.Call : OpCodeNumber.Callvirt,
                method.TranslateMethod(this.loggingBackendMethods.Module));
        }
    }
}