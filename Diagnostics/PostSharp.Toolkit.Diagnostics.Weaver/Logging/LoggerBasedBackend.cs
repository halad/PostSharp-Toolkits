using System;
using System.Collections.Generic;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggerBasedBackend : LoggingBackend
    {
        private readonly Dictionary<object, FieldDefDeclaration> logFields = new Dictionary<object, FieldDefDeclaration>();

        private LoggingContextContainingTypeBuilder typeBuilder;

        public override void Initialize(ModuleDeclaration module)
        {
            base.Initialize(module);
            this.typeBuilder = new LoggingContextContainingTypeBuilder(module);
        }

        public override ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new LoggerBasedBackendInstance(this, aspectWeaverInstance);
        }

        public FieldDefDeclaration GetLoggerField(object category)
        {
            FieldDefDeclaration loggerField;
            if (!this.logFields.TryGetValue(category, out loggerField))
            {
                loggerField = this.typeBuilder.CreateLoggerField((string)category, BackendWriter);
                this.logFields[category] = loggerField;
            }

            return loggerField;
        }

        private class LoggerBasedBackendInstance : ILoggingBackendInstance
        {
            private readonly LoggerBasedBackend parent;
            private readonly AspectWeaverInstance aspectWeaverInstance;

            public LoggerBasedBackendInstance(LoggerBasedBackend parent, AspectWeaverInstance aspectWeaverInstance)
            {
                this.parent = parent;
                this.aspectWeaverInstance = aspectWeaverInstance;
            }

            public void EmitWrite(InstructionWriter instructionWriter, string message, LogLevel logLevel)
            {
                
            }

            public void EmitWriteException(InstructionWriter instructionWriter, string message, Exception exception, LogLevel logLevel)
            {
                
            }
        }
    }
}