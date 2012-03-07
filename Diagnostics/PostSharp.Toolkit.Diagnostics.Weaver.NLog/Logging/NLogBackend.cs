using System;
using NLog;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using LogLevel = PostSharp.Toolkit.Diagnostics.Weaver.Logging.LogLevel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogBackend : ILoggingBackend
    {
        private LoggingImplementationTypeBuilder loggingImplementation;

        private IMethod writeDebugMethod;
        private IMethod writeDebugExceptionMethod;
        private IMethod writeInfoMethod;
        private IMethod writeInfoExceptionMethod;
        private IMethod writeWarnMethod;
        private IMethod writeWarnExceptionMethod;
        private IMethod writeErrorMethod;
        private IMethod writeErrorExceptionMethod;
        private IMethod writeFatalMethod;
        private IMethod writeFatalExceptionMethod;

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

            this.loggerType = module.FindType(typeof(Logger));
            Predicate<MethodDefDeclaration> singleMessagePredicate = 
                method => method.Parameters.Count == 1 && 
                    IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String);
            
            this.categoryInitializerMethod = module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger", singleMessagePredicate);

            this.writeDebugMethod = module.FindMethod(this.loggerType, "Trace", singleMessagePredicate);
            this.writeDebugExceptionMethod = module.FindMethod(this.loggerType, "TraceException");
            this.writeInfoMethod = module.FindMethod(this.loggerType, "Info", singleMessagePredicate);
            this.writeInfoExceptionMethod = module.FindMethod(this.loggerType, "InfoException");
            this.writeWarnMethod = module.FindMethod(this.loggerType, "Warn", singleMessagePredicate);
            this.writeWarnExceptionMethod = module.FindMethod(this.loggerType, "WarnException");
            this.writeErrorMethod = module.FindMethod(this.loggerType, "Error", singleMessagePredicate);
            this.writeErrorExceptionMethod = module.FindMethod(this.loggerType, "ErrorException");
            this.writeFatalMethod = module.FindMethod(this.loggerType, "Fatal", singleMessagePredicate);
            this.writeFatalExceptionMethod = module.FindMethod(this.loggerType, "FatalException");

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

                this.loggerField = this.parent.loggingImplementation.GetCategoryField(categoryName,
                                                                                      this.parent.loggerType, writer =>
                                                                                      {
                                                                                          writer.EmitInstructionString(
                                                                                              OpCodeNumber.Ldstr,
                                                                                              categoryName);
                                                                                          writer.EmitInstructionMethod(
                                                                                              OpCodeNumber.Call,
                                                                                              this.parent.
                                                                                                  categoryInitializerMethod);
                                                                                      });
            }

            public bool SupportsIsEnabled
            {
                get { return true; }
            }

            public void EmitGetIsEnabled(InstructionWriter writer, LogLevel level)
            {
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);

                switch (level)
                {
                    case LogLevel.Trace:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsTraceEnabledMethod);
                        break;
                    case LogLevel.Info:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsInfoEnabledMethod);
                        break;
                    case LogLevel.Warning:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsWarnEnabledMethod);
                        break;
                    case LogLevel.Error:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsErrorEnabledMethod);
                        break;
                    case LogLevel.Fatal:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsFatalEnabledMethod);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("level");
                }
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string messageFormattingString,
                                  int argumentsCount, LogLevel logLevel, Action<InstructionWriter> getExceptionAction,
                                  Action<int, InstructionWriter> loadArgumentAction)
            {
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);
                bool isException = getExceptionAction != null;

                IMethod method;
                switch (logLevel)
                {
                    case LogLevel.Trace:
                        method = isException ? this.parent.writeDebugExceptionMethod : this.parent.writeDebugMethod;
                        break;
                    case LogLevel.Info:
                        method = isException ? this.parent.writeInfoExceptionMethod : this.parent.writeInfoMethod;
                        break;
                    case LogLevel.Warning:
                        method = isException ? this.parent.writeWarnExceptionMethod : this.parent.writeWarnMethod;
                        break;
                    case LogLevel.Error:
                        method = isException ? this.parent.writeErrorExceptionMethod : this.parent.writeErrorMethod;
                        break;
                    case LogLevel.Fatal:
                        method = isException ? this.parent.writeFatalExceptionMethod : this.parent.writeFatalMethod;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel");
                }

                writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);

                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);
                }

                if (argumentsCount > 0)
                {
                    StringFormatHelper.WriteFormatArguments(writer, argumentsCount);
                }

                writer.EmitInstructionMethod(OpCodeNumber.Callvirt, method);
                ;
            }
        }
    }
}