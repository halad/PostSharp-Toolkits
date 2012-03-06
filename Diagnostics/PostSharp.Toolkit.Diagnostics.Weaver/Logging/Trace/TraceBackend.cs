using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackend : ILoggingBackend
    {
        public void Initialize(ModuleDeclaration module)
        {
            
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new TraceBackendInstance(aspectWeaverInstance.AspectType.Module);
        }

        private class TraceBackendInstance : ILoggingBackendInstance
        {
            private readonly ModuleDeclaration module;

            public TraceBackendInstance(ModuleDeclaration module)
            {
                this.module = module;
            }

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return new TraceCategoryBuilder(this.module);
            }
        }

        private class TraceCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly ModuleDeclaration module;

            private readonly IMethod writeLineString;
            private readonly IMethod writeLineObject;
            private readonly IMethod writeLineStringCategory;
            private readonly IMethod writeLineObjectCategory;

            public TraceCategoryBuilder(ModuleDeclaration module)
            {
                this.module = module;

                this.writeLineString = module.FindMethod(
                    module.Cache.GetType(typeof(System.Diagnostics.Trace)), "WriteLine",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

                this.writeLineObject = module.FindMethod(
                    module.Cache.GetType(typeof(System.Diagnostics.Trace)), "WriteLine",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.Object));

                this.writeLineStringCategory = module.FindMethod(
                    module.Cache.GetType(typeof(System.Diagnostics.Trace)), "WriteLine",
                    method => method.Parameters.Count == 2 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.String));

                this.writeLineObjectCategory = module.FindMethod(
                    module.Cache.GetType(typeof(System.Diagnostics.Trace)), "WriteLine",
                    method => method.Parameters.Count == 2 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.Object) &&
                              IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.String));
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
                bool createArgsArray = argumentsCount > 3;

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

                if (argumentsCount > 0)
                {
                    IMethod stringFormatMethod;

                    if (createArgsArray)
                    {
                        stringFormatMethod = this.module.FindMethod(
                            this.module.FindType(typeof(string)), "Format",
                            method => method.Parameters.Count > 1 &&
                                      IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                                      method.Parameters[0].Name == "format" &&
                                      method.Parameters[1].ParameterType.BelongsToClassification(TypeClassifications.Array));
                    }
                    else
                    {
                        stringFormatMethod = this.module.FindMethod(
                            this.module.FindType(typeof(string)), "Format",
                            method => method.Parameters.Count > 1 &&
                                      IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                                      method.Parameters[0].Name == "format" &&
                                      method.Parameters.Count - 1 == argumentsCount);
                    } 
                    writer.EmitInstructionMethod(OpCodeNumber.Call, stringFormatMethod);
                }
                writer.EmitInstructionMethod(OpCodeNumber.Call, writeLineString);
            }
        }
    }
}