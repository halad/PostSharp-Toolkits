using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceBackend : ILoggingBackend
    {
        private StringFormatWriter formatWriter;
        private ModuleDeclaration module;

        private IMethod writeLineString;
        private IMethod traceInfoString;
        private IMethod traceInfoFormat;
        private IMethod traceWarningString;
        private IMethod traceWarningFormat;
        private IMethod traceErrorString;
        private IMethod traceErrorFormat;

        public void Initialize(ModuleDeclaration module)
        {
            this.module = module;
            this.formatWriter = new StringFormatWriter(module);

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

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new TraceBackendInstance(this);
        }

        private class TraceBackendInstance : ILoggingBackendInstance
        {
            private readonly TraceBackend parent;

            public TraceBackendInstance(TraceBackend parent)
            {
                this.parent = parent;
            }

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return new TraceCategoryBuilder(this.parent);
            }
        }

        private class TraceCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly TraceBackend parent;

            public TraceCategoryBuilder(TraceBackend parent)
            {
                this.parent = parent;
            }

            public bool SupportsIsEnabled
            {
                get { return false; }
            }

            public void EmitGetIsEnabled(InstructionWriter writer, LogSeverity logSeverity)
            {
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string messageFormattingString,
                                  int argumentsCount, LogSeverity logSeverity, Action<InstructionWriter> getExceptionAction,
                                  Action<int, InstructionWriter> loadArgumentAction)
            {
                bool useStringFormat = false;
                bool createArgsArray = argumentsCount > 0;

                IMethod method;

                switch (logSeverity)
                {
                    case LogSeverity.Trace:
                        method = this.parent.writeLineString;
                        useStringFormat = createArgsArray;
                        break;
                    case LogSeverity.Info:
                        method = createArgsArray ? this.parent.traceInfoFormat : this.parent.traceInfoString;
                        break;
                    case LogSeverity.Warning:
                        method = createArgsArray ? this.parent.traceWarningFormat : this.parent.traceWarningString;
                        break;
                    case LogSeverity.Error:
                    case LogSeverity.Fatal:
                        method = createArgsArray ? this.parent.traceErrorFormat : this.parent.traceErrorString;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logSeverity");
                }

                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);
                }

                if (useStringFormat)
                {
                    this.parent.formatWriter.EmitFormatArguments(writer, messageFormattingString, argumentsCount, loadArgumentAction);
                }
                else
                {
                    writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);

                    if (createArgsArray)
                    {
                        writer.EmitInstructionInt32(OpCodeNumber.Ldc_I4, argumentsCount);
                        writer.EmitInstructionType(OpCodeNumber.Newarr,
                                                   this.parent.module.Cache.GetIntrinsicBoxedType(IntrinsicType.Object));
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
                }

                writer.EmitInstructionMethod(OpCodeNumber.Call, method);
            }
        }
    }
}