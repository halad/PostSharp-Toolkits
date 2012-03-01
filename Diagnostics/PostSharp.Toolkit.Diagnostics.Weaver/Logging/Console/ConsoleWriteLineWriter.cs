using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console
{
    internal sealed class ConsoleWriteLineWriter : LoggingBackendWriter
    {
        private readonly IMethod writeLineMethod;

        public ConsoleWriteLineWriter(ModuleDeclaration module, Type backendType)
            : base(null)
        {
            this.writeLineMethod = module.FindMethod(module.FindType(backendType), "WriteLine",
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        public override void EmitTrace(InstructionWriter writer, string message, Exception exception = null)
        {
            this.EmitWrite(writer, "TRACE: " + message);
        }

        public override void EmitInfo(InstructionWriter writer, string message, Exception exception = null)
        {
            this.EmitWrite(writer, "INFO: " + message);
        }

        public override void EmitWarning(InstructionWriter writer, string message, Exception exception = null)
        {
            this.EmitWrite(writer, "WARN: " + message);
        }

        public override void EmitError(InstructionWriter writer, string message, Exception exception = null)
        {
            this.EmitWrite(writer, "ERROR: " + message);
        }

        public override void EmitFatal(InstructionWriter writer, string message, Exception exception = null)
        {
            this.EmitWrite(writer, "FATAL: " + message);
        }

        private void EmitWrite(InstructionWriter writer, string message)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.writeLineMethod);
        }
    }
}