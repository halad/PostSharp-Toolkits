using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver
{
    public sealed class StringFormatWriter
    {
        private readonly ModuleDeclaration module;
        private readonly IMethod format1Method;
        private readonly IMethod format2Method;
        private readonly IMethod format3Method;
        private readonly IMethod formatArrayMethod;

        public StringFormatWriter(ModuleDeclaration module)
        {
            this.module = module;
            ITypeSignature stringType = module.Cache.GetType(typeof(string));

            this.format1Method = module.FindMethod(stringType, "Format",
                method => method.Parameters.Count == 2 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                          IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object));

            this.format2Method = module.FindMethod(stringType, "Format",
                method => method.Parameters.Count == 3 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                          IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                          IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object));

            this.format3Method = module.FindMethod(stringType, "Format",
                method => method.Parameters.Count == 4 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                          IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                          IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object) &&
                          IntrinsicTypeSignature.Is(method.Parameters[3].ParameterType, IntrinsicType.Object));

            this.formatArrayMethod = module.FindMethod(stringType, "Format",
                method => method.Parameters.Count == 2 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                          method.Parameters[1].ParameterType.BelongsToClassification(TypeClassifications.Array));

        }

        public void EmitFormatArguments(InstructionWriter writer, string format, int argumentsCount, Action<int, InstructionWriter> loadArgumentAction)
        {
            if (argumentsCount == 0) throw new ArgumentOutOfRangeException("argumentsCount");

            IMethod formatMethod;
            bool createArgsArray = false;

            switch (argumentsCount)
            {
                case 1:
                    formatMethod = this.format1Method;
                    break;
                case 2:
                    formatMethod = this.format2Method;
                    break;
                case 3:
                    formatMethod = this.format3Method;
                    break;
                default:
                    formatMethod = this.formatArrayMethod;
                    createArgsArray = true;
                    break;
            }

            writer.EmitInstructionString(OpCodeNumber.Ldstr, format);

            if (createArgsArray)
            {
                writer.EmitInstructionInt32(OpCodeNumber.Ldc_I4, argumentsCount);
                writer.EmitInstructionType(OpCodeNumber.Newarr,
                                           this.module.Cache.GetIntrinsicBoxedType(IntrinsicType.Object));
            }

            for (int i = 0; i < argumentsCount; i++)
            {
                if (createArgsArray)
                {
                    writer.EmitInstruction(OpCodeNumber.Dup);
                    writer.EmitInstructionInt32(OpCodeNumber.Ldc_I4, i);
                }

                if (loadArgumentAction != null)
                {
                    loadArgumentAction(i, writer);
                }

                if (createArgsArray)
                {
                    writer.EmitInstruction(OpCodeNumber.Stelem_Ref);
                }
            }

            writer.EmitInstructionMethod(OpCodeNumber.Call, formatMethod);
        }
    }
}