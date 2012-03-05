using System;
using System.Collections.Generic;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Utilities;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggerBasedBackend : LoggingBackend
    {
        private readonly Dictionary<object, FieldDefDeclaration> logFields = new Dictionary<object, FieldDefDeclaration>();

        private LoggingImplementationTypeBuilder typeBuilder;

        public override void Initialize(ModuleDeclaration module)
        {
            base.Initialize(module);
            this.typeBuilder = new LoggingImplementationTypeBuilder(module);
        }

        public override ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new LoggerBasedBackendInstance(this);
        }

        public FieldDefDeclaration GetLoggerField(object category)
        {
            FieldDefDeclaration loggerField;
            if (!this.logFields.TryGetValue(category, out loggerField))
            {
                loggerField = this.typeBuilder.CreateLoggerField((string)category, BackendWriter.LoggingBackendMethods);
                this.logFields[category] = loggerField;
            }

            return loggerField;
        }

        private class LoggerBasedBackendInstance : ILoggingBackendInstance
        {
            private readonly LoggerBasedBackend parent;

            public LoggerBasedBackendInstance(LoggerBasedBackend parent)
            {
                this.parent = parent;
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string category, string message, LogLevel logLevel)
            {
                FieldDefDeclaration loggerField = this.parent.GetLoggerField(category);
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, loggerField);

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        this.parent.BackendWriter.EmitTrace(writer, message);
                        break;
                    case LogLevel.Info:
                        this.parent.BackendWriter.EmitInfo(writer, message);
                        break;
                    case LogLevel.Warning:
                        this.parent.BackendWriter.EmitWarning(writer, message);
                        break;
                    case LogLevel.Error:
                        this.parent.BackendWriter.EmitError(writer, message);
                        break;
                    case LogLevel.Fatal:
                        this.parent.BackendWriter.EmitFatal(writer, message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel");
                }
            }

            public void EmitWriteException(InstructionWriter writer, InstructionBlock block, string category, string message, ITypeSignature exceptionType, LogLevel logLevel)
            {
                LocalVariableSymbol exceptionLocal = block.MethodBody.RootInstructionBlock.DefineLocalVariable(
                exceptionType, DebuggerSpecialNames.GetVariableSpecialName("ex"));
                writer.EmitInstructionLocalVariable(OpCodeNumber.Stloc, exceptionLocal);

                FieldDefDeclaration loggerField = this.parent.GetLoggerField(category);
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, loggerField);

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        this.parent.BackendWriter.EmitTraceException(writer, message, exceptionLocal);
                        break;
                    case LogLevel.Info:
                        this.parent.BackendWriter.EmitInfoException(writer, message, exceptionLocal);
                        break;
                    case LogLevel.Warning:
                        this.parent.BackendWriter.EmitWarningException(writer, message, exceptionLocal);
                        break;
                    case LogLevel.Error:
                        this.parent.BackendWriter.EmitErrorException(writer, message, exceptionLocal);
                        break;
                    case LogLevel.Fatal:
                        this.parent.BackendWriter.EmitFatalException(writer, message, exceptionLocal);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel");
                }
            }
        }
    }
}