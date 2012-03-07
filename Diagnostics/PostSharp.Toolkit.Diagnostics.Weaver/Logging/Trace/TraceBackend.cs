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

            public TraceCategoryBuilder(ModuleDeclaration module)
            {
                this.module = module;

                this.writeLineString = module.FindMethod(
                    module.Cache.GetType(typeof(System.Diagnostics.Trace)), "WriteLine",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
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

                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);

                    writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.module.FindMethod(
                        this.module.Cache.GetType(typeof(object)), "ToString"));
                }
                else
                {
                    writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);
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

                writer.EmitInstructionMethod(OpCodeNumber.Call, writeLineString);
            }
        }
    }
}