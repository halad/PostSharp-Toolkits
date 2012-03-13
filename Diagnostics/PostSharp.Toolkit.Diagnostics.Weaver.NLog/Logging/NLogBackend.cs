using System;
using NLog;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogBackend : ILoggingBackend
    {
        private LoggingImplementationTypeBuilder loggingImplementation;
        private StringFormatWriter formatWriter;

        private IMethod writeDebugMethod;
        private IMethod writeInfoMethod;
        private IMethod writeWarnMethod;
        private IMethod writeErrorMethod;
        private IMethod writeFatalMethod;

        private IMethod getIsTraceEnabledMethod;
        private IMethod getIsInfoEnabledMethod;
        private IMethod getIsWarnEnabledMethod;
        private IMethod getIsErrorEnabledMethod;
        private IMethod getIsFatalEnabledMethod;
        private IMethod categoryInitializerMethod;
        private ITypeSignature loggerType;

        public void Initialize(ModuleDeclaration module)
        {
            this.loggingImplementation = new LoggingImplementationTypeBuilder(module);
            this.formatWriter = new StringFormatWriter(module);

            this.loggerType = module.FindType(typeof(Logger));

            Predicate<MethodDefDeclaration> singleMessagePredicate = 
                method => method.Parameters.Count == 1 && 
                    IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String);
            
            this.categoryInitializerMethod = module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger", singleMessagePredicate);

            this.writeDebugMethod = module.FindMethod(this.loggerType, "Trace", singleMessagePredicate);
            this.writeInfoMethod = module.FindMethod(this.loggerType, "Info", singleMessagePredicate);
            this.writeWarnMethod = module.FindMethod(this.loggerType, "Warn", singleMessagePredicate);
            this.writeErrorMethod = module.FindMethod(this.loggerType, "Error", singleMessagePredicate);
            this.writeFatalMethod = module.FindMethod(this.loggerType, "Fatal", singleMessagePredicate);

            this.getIsTraceEnabledMethod = module.FindMethod(this.loggerType, "get_IsTraceEnabled");
            this.getIsInfoEnabledMethod = module.FindMethod(this.loggerType, "get_IsInfoEnabled");
            this.getIsWarnEnabledMethod = module.FindMethod(this.loggerType, "get_IsWarnEnabled");
            this.getIsErrorEnabledMethod = module.FindMethod(this.loggerType, "get_IsErrorEnabled");
            this.getIsFatalEnabledMethod = module.FindMethod(this.loggerType, "get_IsFatalEnabled");
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new NLogBackendInstance(this);
        }

        private class NLogBackendInstance : ILoggingBackendInstance
        {
            private readonly NLogBackend parent;

            public NLogBackendInstance(NLogBackend parent)
            {
                this.parent = parent;
            }

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return new NLogCategoryBuilder(this.parent, categoryName);
            }
        }

        private class NLogCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly NLogBackend parent;
            private readonly FieldDefDeclaration loggerField;

            public NLogCategoryBuilder(NLogBackend parent, string categoryName)
            {
                this.parent = parent;

                this.loggerField = this.parent.loggingImplementation.GetCategoryField(categoryName, this.parent.loggerType, writer =>
                {
                    writer.EmitInstructionString(OpCodeNumber.Ldstr, categoryName);
                    writer.EmitInstructionMethod(OpCodeNumber.Call, this.parent.categoryInitializerMethod);
                });
            }

            public bool SupportsIsEnabled
            {
                get { return true; }
            }

            public void EmitGetIsEnabled(InstructionWriter writer, LogSeverity logSeverity)
            {
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);

                switch (logSeverity)
                {
                    case LogSeverity.Trace:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsTraceEnabledMethod);
                        break;
                    case LogSeverity.Info:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsInfoEnabledMethod);
                        break;
                    case LogSeverity.Warning:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsWarnEnabledMethod);
                        break;
                    case LogSeverity.Error:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsErrorEnabledMethod);
                        break;
                    case LogSeverity.Fatal:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsFatalEnabledMethod);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logSeverity");
                }
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string messageFormattingString,
                                  int argumentsCount, LogSeverity logSeverity, Action<InstructionWriter> getExceptionAction,
                                  Action<int, InstructionWriter> loadArgumentAction)
            {
                IMethod method;
                switch (logSeverity)
                {
                    case LogSeverity.Trace:
                        method = this.parent.writeDebugMethod;
                        break;
                    case LogSeverity.Info:
                        method = this.parent.writeInfoMethod;
                        break;
                    case LogSeverity.Warning:
                        method = this.parent.writeWarnMethod;
                        break;
                    case LogSeverity.Error:
                        method = this.parent.writeErrorMethod;
                        break;
                    case LogSeverity.Fatal:
                        method = this.parent.writeFatalMethod;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logSeverity");
                }

                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);
                }

                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);

                if (argumentsCount > 0)
                {
                    this.parent.formatWriter.EmitFormatArguments(writer, messageFormattingString, argumentsCount, loadArgumentAction);
                }
                else
                {
                    writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);
                }

                writer.EmitInstructionMethod(OpCodeNumber.Callvirt, method);
            }
        }
    }
}