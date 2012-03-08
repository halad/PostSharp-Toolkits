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
            private readonly IMethod traceInfoString;
            private readonly IMethod traceInfoFormat;
            private readonly IMethod traceWarningString;
            private readonly IMethod traceWarningFormat;
            private readonly IMethod traceErrorString;
            private readonly IMethod traceErrorFormat;

            public TraceCategoryBuilder(ModuleDeclaration module)
            {
                this.module = module;

                ITypeSignature traceTypeSignature = module.Cache.GetType(typeof(System.Diagnostics.Trace));

                this.writeLineString = module.FindMethod(traceTypeSignature, "WriteLine",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

                this.traceInfoString = module.FindMethod(traceTypeSignature, "TraceInformation",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

                this.traceInfoFormat = module.FindMethod(traceTypeSignature, "TraceInformation",
                    method => method.Parameters.Count == 2 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              method.Parameters[1].ParameterType.BelongsToClassification(TypeClassifications.Array));

                this.traceWarningString = module.FindMethod(traceTypeSignature, "TraceWarning",
                    method => method.Parameters.Count == 1 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

                this.traceWarningFormat = module.FindMethod(traceTypeSignature, "TraceWarning",
                    method => method.Parameters.Count == 2 &&
                              IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                              method.Parameters[1].ParameterType.BelongsToClassification(TypeClassifications.Array));

                this.traceErrorString = module.FindMethod(traceTypeSignature, "TraceError",
                    method => method.Parameters.Count == 1 &&
                    IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

                this.traceErrorFormat = module.FindMethod(traceTypeSignature, "TraceError",
                    method => method.Parameters.Count == 2 &&
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
                bool useStringFormat = argumentsCount > 0;
                bool createArgsArray = useStringFormat;
                IMethod method;

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        method = this.writeLineString;
                        createArgsArray = false;
                        break;
                    case LogLevel.Info:
                        method = useStringFormat ? this.traceInfoFormat : this.traceInfoString;
                        break;
                    case LogLevel.Warning:
                        method = useStringFormat ? this.traceWarningFormat : this.traceWarningString;
                        break;
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        method = useStringFormat ? this.traceErrorFormat : this.traceErrorString;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel");
                }

                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);
                }

                writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);

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

                writer.EmitInstructionMethod(OpCodeNumber.Call, method);
            }
        }
    }
}