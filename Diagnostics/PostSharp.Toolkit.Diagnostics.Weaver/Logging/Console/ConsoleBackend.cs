using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console
{
    internal sealed class ConsoleBackend : ILoggingBackend
    {
        public void Initialize(ModuleDeclaration module)
        {
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new ConsoleBackendInstance(aspectWeaverInstance.AspectType.Module);
        }

        private class ConsoleBackendInstance : ILoggingBackendInstance
        {
            private readonly ModuleDeclaration module;

            public ConsoleBackendInstance(ModuleDeclaration module)
            {
                this.module = module;
            }

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return new ConsoleCategoryBuilder(this.module);
            }
        }

        private class ConsoleCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly ModuleDeclaration module;

            private readonly IMethod writeLineMessage;
            private readonly IMethod writeLineFormat1;
            private readonly IMethod writeLineFormat2;
            private readonly IMethod writeLineFormat3;
            private readonly IMethod writeLineFormat4;
            private readonly IMethod writeLineFormatArray;


            public ConsoleCategoryBuilder(ModuleDeclaration module)
            {
                this.module = module;

                this.writeLineMessage = module.FindMethod(
                    module.Cache.GetType(typeof(System.Console)), "WriteLine",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

                this.writeLineFormat1 = module.FindMethod(
                    module.Cache.GetType(typeof(System.Console)), "WriteLine",
                    method => method.Parameters.Count == 2 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object));

                this.writeLineFormat2 = module.FindMethod(
                    module.Cache.GetType(typeof(System.Console)), "WriteLine",
                    method => method.Parameters.Count == 3 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object));

                this.writeLineFormat3 = module.FindMethod(
                    module.Cache.GetType(typeof(System.Console)), "WriteLine",
                    method => method.Parameters.Count == 4 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[3].ParameterType, IntrinsicType.Object));

                this.writeLineFormat4 = module.FindMethod(
                    module.Cache.GetType(typeof(System.Console)), "WriteLine",
                    method => method.Parameters.Count == 5 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[3].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[4].ParameterType, IntrinsicType.Object));

                this.writeLineFormatArray = module.FindMethod(
                    module.Cache.GetType(typeof(System.Console)), "WriteLine",
                    method => method.Parameters.Count >= 2 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              method.Parameters[1].ParameterType.BelongsToClassification(TypeClassifications.Array));
            }

            public bool SupportsIsEnabled
            {
                get { return false; }
            }

            public void EmitGetIsEnabled(InstructionWriter writer, LogLevel level)
            {
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string messageFormattingString,
                                  int argumentsCount, LogLevel logLevel, Action<InstructionWriter> getExceptionAction,
                                  Action<int, InstructionWriter> loadArgumentAction)
            {
                IMethod method;
                bool createArgsArray = false;

                switch (argumentsCount)
                {
                    case 0:
                        method = this.writeLineMessage;
                        break;
                    case 1:
                        method = this.writeLineFormat1;
                        break;
                    case 2:
                        method = this.writeLineFormat2;
                        break;
                    case 3:
                        method = this.writeLineFormat3;
                        break;
                    case 4:
                        method = this.writeLineFormat4;
                        break;
                    default:
                        method = this.writeLineFormatArray;
                        createArgsArray = true;
                        break;
                }

                writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);
                
                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);
                }

                if (createArgsArray)
                {
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

                writer.EmitInstructionMethod(OpCodeNumber.Call, method);
            }
        }
    }
}