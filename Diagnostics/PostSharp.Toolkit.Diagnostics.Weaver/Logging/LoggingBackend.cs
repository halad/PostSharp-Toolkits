using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Utilities;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingBackend : ILoggingBackend
    {
        private ILoggingBackendWriter backendWriter;

        protected ILoggingBackendWriter BackendWriter 
        {
            get { return this.backendWriter; }
        }

        public virtual void Initialize(ModuleDeclaration module)
        {
            this.backendWriter = CreateBackendWriter(module);
        }

        protected abstract ILoggingBackendWriter CreateBackendWriter(ModuleDeclaration module);

        public virtual ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new ConsoleBasedLoggingBackendInstance(this, aspectWeaverInstance);
        }

        private class ConsoleBasedLoggingBackendInstance : ILoggingBackendInstance
        {
            private readonly LoggingBackend parent;
            private readonly AspectWeaverInstance aspectWeaverInstance;

            public ConsoleBasedLoggingBackendInstance(LoggingBackend parent, AspectWeaverInstance aspectWeaverInstance)
            {
                this.parent = parent;
                this.aspectWeaverInstance = aspectWeaverInstance;
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string category, string message, LogLevel logLevel)
            {
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