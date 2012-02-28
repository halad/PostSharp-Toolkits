using System;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingBackend : ILoggingBackend
    {
        protected ILoggingBackendWriter BackendWriter;

        public virtual void Initialize(ModuleDeclaration module)
        {
            this.BackendWriter = CreateBackendWriter(module);
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

            public void EmitWrite(InstructionWriter instructionWriter, string message, LogLevel logLevel)
            {
                this.EmitWriteException(instructionWriter, message, null, logLevel);
            }

            public void EmitWriteException(InstructionWriter instructionWriter, string message, Exception exception, LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Trace:
                        this.parent.BackendWriter.EmitTrace(instructionWriter, message, exception);
                        break;
                    case LogLevel.Info:
                        this.parent.BackendWriter.EmitInfo(instructionWriter, message, exception);
                        break;
                    case LogLevel.Warning:
                        this.parent.BackendWriter.EmitWarning(instructionWriter, message, exception);
                        break;
                    case LogLevel.Error:
                        this.parent.BackendWriter.EmitError(instructionWriter, message, exception);
                        break;
                    case LogLevel.Fatal:
                        this.parent.BackendWriter.EmitFatal(instructionWriter, message, exception);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel");
                }
            }
        }
    }
}