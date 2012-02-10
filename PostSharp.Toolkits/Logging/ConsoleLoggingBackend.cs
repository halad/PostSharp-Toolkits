using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Instrumentation.Weaver.Logging
{
    internal sealed class ConsoleLoggingBackend : ILoggingBackend
    {
        private IMethod writeLineMethod;

        public void Initialize(ModuleDeclaration module)
        {
            writeLineMethod = module.FindMethod(module.FindType(typeof(Console)), "WriteLine",
                                                method =>
                                                method.Parameters.Count == 1 &&
                                                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType,
                                                                          IntrinsicType.String));
        }

        public void EmitWrite(string message, InstructionWriter instructionWriter)
        {
            instructionWriter.EmitInstructionString(OpCodeNumber.Ldstr, message);
            instructionWriter.EmitInstructionMethod(OpCodeNumber.Call, this.writeLineMethod);
        }
    }
}