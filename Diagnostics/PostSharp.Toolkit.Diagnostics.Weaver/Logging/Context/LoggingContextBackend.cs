using System.Collections.Generic;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context
{
    public abstract class LoggingContextBackend : ILoggingBackend
    {
        private readonly Dictionary<string, FieldDefDeclaration> logFields = new Dictionary<string, FieldDefDeclaration>();
        private LoggingContexContainingTypeBuilder loggingContexContainingTypeBuilder;
        private LoggingContext loggingContext;

        public void Initialize(ModuleDeclaration module)
        {
            this.loggingContexContainingTypeBuilder = new LoggingContexContainingTypeBuilder(module);
            this.loggingContext = CreateContext(module);
        }

        protected abstract LoggingContext CreateContext(ModuleDeclaration module);

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new LoggingBackendInstance(this, aspectWeaverInstance);
        }

        private FieldDefDeclaration GetLoggerField(string category)
        {
            FieldDefDeclaration loggerField;
            if (!this.logFields.TryGetValue(category, out loggerField))
            {
                loggerField = this.loggingContexContainingTypeBuilder.CreateLoggerField(category, this.loggingContext);
                this.logFields[category] = loggerField;
            }

            return loggerField;
        }

        private sealed class LoggingBackendInstance : ILoggingBackendInstance
        {
            private readonly LoggingContextBackend parent;
            private readonly AspectWeaverInstance aspectWeaverInstance;

            public LoggingBackendInstance(LoggingContextBackend parent, AspectWeaverInstance aspectWeaverInstance)
            {
                this.parent = parent;
                this.aspectWeaverInstance = aspectWeaverInstance;
            }

            public void EmitWrite(string message, InstructionWriter instructionWriter)
            {
                MethodDefDeclaration targetMethod = this.aspectWeaverInstance.TargetElement as MethodDefDeclaration;
                if (targetMethod == null)
                {
                    return;
                }

                // TODO: nested types
                string category = targetMethod.DeclaringType.Name;
                FieldDefDeclaration loggerField = this.parent.GetLoggerField(category);
                instructionWriter.EmitInstructionField(OpCodeNumber.Ldsfld, loggerField);
                instructionWriter.EmitInstructionString(OpCodeNumber.Ldstr, message);


                // todo write the correct output
                instructionWriter.EmitInstructionMethod(OpCodeNumber.Call, this.parent.loggingContext.WriteInfoMethod);
            }
        }
    }
}