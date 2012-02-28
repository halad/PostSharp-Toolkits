using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingBackendWriter : ILoggingBackendWriter
    {
        private readonly ModuleDeclaration module;
        private readonly ITypeSignature loggerType;
        
        protected LoggingBackendWriter(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;
        }

        public ITypeSignature LoggerType
        {
            get { return this.loggerType; }
        }

        public ModuleDeclaration Module
        {
            get { return this.module; }
        }

        protected IMethod FindMethod(string methodName, Predicate<MethodDefDeclaration> predicate = null)
        {
            return predicate != null ? this.module.FindMethod(this.loggerType, methodName, predicate)
                                     : this.module.FindMethod(this.loggerType, methodName);
        }

        public abstract void EmitInitialization(InstructionWriter writer, string category);
        public abstract void EmitTrace(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitInfo(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitWarning(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitError(InstructionWriter writer, string message, Exception exception = null);
        public abstract void EmitFatal(InstructionWriter writer, string message, Exception exception = null);
    }
}