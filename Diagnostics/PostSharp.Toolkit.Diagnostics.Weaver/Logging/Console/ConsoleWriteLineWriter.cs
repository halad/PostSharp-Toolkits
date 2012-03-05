using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console
{
    internal sealed class ConsoleWriteLineWriter : LoggingBackendWriter
    {
        private readonly IMethod writeLineMethod;

        public ConsoleWriteLineWriter(ModuleDeclaration module, Type backendType)
            : base(new LoggingBackendMethods(module, module.FindType(backendType)))
        {
            this.writeLineMethod = module.FindMethod(module.FindType(backendType), "WriteLine",
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        public override void EmitTrace(InstructionWriter writer, string message)
        {
            EmitWrite(writer, "TRACE: " + message, this.writeLineMethod);
        }

        public override void EmitTraceException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            EmitWriteException(writer, "TRACE: " + message, exceptionSymbol, this.writeLineMethod);
        }

        public override void EmitInfo(InstructionWriter writer, string message)
        {
            EmitWrite(writer, "INFO: " + message, this.writeLineMethod);
        }

        public override void EmitInfoException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            EmitWriteException(writer, "INFO: " + message, exceptionSymbol, this.writeLineMethod);
        }

        public override void EmitWarning(InstructionWriter writer, string message)
        {
            EmitWrite(writer, "WARN: " + message, this.writeLineMethod);
        }

        public override void EmitWarningException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            EmitWriteException(writer, "WARNING: " + message, exceptionSymbol, this.writeLineMethod);
        }

        public override void EmitError(InstructionWriter writer, string message)
        {
            EmitWrite(writer, "ERROR: " + message, this.writeLineMethod);
        }

        public override void EmitErrorException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            EmitWriteException(writer, "ERROR: " + message, exceptionSymbol, this.writeLineMethod);
        }

        public override void EmitFatal(InstructionWriter writer, string message)
        {
            EmitWrite(writer, "FATAL: " + message, this.writeLineMethod);
        }

        public override void EmitFatalException(InstructionWriter writer, string message, LocalVariableSymbol exceptionSymbol)
        {
            EmitWriteException(writer, "FATAL: " + message, exceptionSymbol, this.writeLineMethod);
        }
    }
}