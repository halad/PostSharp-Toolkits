using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using log4net;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackend : ILoggingBackend
    {
        private LoggingImplementationTypeBuilder loggingImplementation;
        private StringFormatWriter formatWriter;

        private IMethod writeDebugMethod;
        private IMethod writeInfoMethod;
        private IMethod writeWarningMethod;
        private IMethod writeErrorMethod;
        private IMethod writeFatalMethod;
        
        private IMethod getIsDebugEnabledMethod;
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
            this.loggerType = module.FindType(typeof(ILog));

            this.categoryInitializerMethod = module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger",
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

            this.writeDebugMethod = module.FindMethod(this.loggerType, "Debug", 1);
            this.writeInfoMethod = module.FindMethod(this.loggerType, "Info", 1);
            this.writeWarningMethod = module.FindMethod(this.loggerType, "Warn", 1);
            this.writeErrorMethod = module.FindMethod(this.loggerType, "Error", 1);
            this.writeFatalMethod = module.FindMethod(this.loggerType, "Fatal", 1);

            this.getIsDebugEnabledMethod = module.FindMethod(this.loggerType, "get_IsDebugEnabled");
            this.getIsInfoEnabledMethod = module.FindMethod(this.loggerType, "get_IsInfoEnabled");
            this.getIsWarnEnabledMethod = module.FindMethod(this.loggerType, "get_IsWarnEnabled");
            this.getIsErrorEnabledMethod = module.FindMethod(this.loggerType, "get_IsErrorEnabled");
            this.getIsFatalEnabledMethod = module.FindMethod(this.loggerType, "get_IsFatalEnabled");
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new Log4NetBackendInstance(this, aspectWeaverInstance.AspectType.Module);
        }

        private class Log4NetBackendInstance : ILoggingBackendInstance
        {
            private readonly Log4NetBackend parent;
            private readonly ModuleDeclaration module;

            public Log4NetBackendInstance(Log4NetBackend parent, ModuleDeclaration module)
            {
                this.parent = parent;
                this.module = module;
            }

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return new Log4NetCategoryBuilder(this.parent, this.module, categoryName);
            }
        }

        private class Log4NetCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly Log4NetBackend parent;
            private readonly ModuleDeclaration module;
            private readonly FieldDefDeclaration loggerField;

            public Log4NetCategoryBuilder(Log4NetBackend parent, ModuleDeclaration module, string categoryName)
            {
                this.parent = parent;
                this.module = module;

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
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsDebugEnabledMethod);
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
                        method = this.parent.writeWarningMethod;
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