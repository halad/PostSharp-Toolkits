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
        private IMethod writeDebugExceptionMethod;
        private IMethod writeInfoMethod;
        private IMethod writeInfoExceptionMethod;
        private IMethod writeWarnMethod;
        private IMethod writeWarnExceptionMethod;
        private IMethod writeErrorMethod;
        private IMethod writeErrorExceptionMethod;
        private IMethod writeFatalMethod;
        private IMethod writeFatalExceptionMethod;

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
            this.writeDebugExceptionMethod = module.FindMethod(this.loggerType, "Debug", 2);
            this.writeInfoMethod = module.FindMethod(this.loggerType, "Info", 1);
            this.writeInfoExceptionMethod = module.FindMethod(this.loggerType, "Info", 2);
            this.writeWarnMethod = module.FindMethod(this.loggerType, "Warn", 1);
            this.writeWarnExceptionMethod = module.FindMethod(this.loggerType, "Warn", 2);
            this.writeErrorMethod = module.FindMethod(this.loggerType, "Error", 1);
            this.writeErrorExceptionMethod = module.FindMethod(this.loggerType, "Error", 2);
            this.writeFatalMethod = module.FindMethod(this.loggerType, "Fatal", 1);
            this.writeFatalExceptionMethod = module.FindMethod(this.loggerType, "Fatal", 2);

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
                return new Log4NetCategoryBuilder(this.parent, categoryName);
            }
        }

        private class Log4NetCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly Log4NetBackend parent;
            private readonly FieldDefDeclaration loggerField;

            public Log4NetCategoryBuilder(Log4NetBackend parent, string categoryName)
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

            public void EmitGetIsEnabled(InstructionWriter writer, LogLevel level)
            {
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);

                switch (level)
                {
                    case LogLevel.Trace:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsDebugEnabledMethod);
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
                    this.parent.formatWriter.EmitFormatArguments(writer, messageFormattingString, argumentsCount);
                }

                writer.EmitInstructionMethod(OpCodeNumber.Callvirt, method);
            }
        }
    }
}