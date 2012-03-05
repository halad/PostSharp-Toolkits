using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingMethodsBuilder
    {
        private readonly ModuleDeclaration module;
        private readonly ITypeSignature loggerType;

        protected ModuleDeclaration Module
        {
            get { return this.module; }
        }

        protected LoggingMethodsBuilder(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;
        }

        public LoggingBackendMethods CreateContext()
        {
            return new LoggingBackendMethods(this.module, this.loggerType)
            {
                InitializerMethod = this.GetInitializerMethod(),
                TraceMethod = this.GetTraceMethod(),
                TraceExceptionMethod = this.GetTraceExceptionMethod(),
                InfoMethod = this.GetInfoMethod(),
                InfoExceptionMethod = this.GetInfoExceptionMethod(),
                WarningMethod = this.GetWarningMethod(),
                WarningExceptionMethod = this.GetWarningExceptionMethod(),
                ErrorMethod = this.GetErrorMethod(),
                ErrorExceptionMethod = this.GetErrorExceptionMethod(),
                FatalMethod = this.GetFatalMethod(),
                FatalExceptionMethod = this.GetFatalExceptionMethod(),
            };
        }

        protected abstract IMethod GetInitializerMethod();
        protected abstract IMethod GetTraceMethod();
        protected abstract IMethod GetTraceExceptionMethod();
        protected abstract IMethod GetInfoMethod();
        protected abstract IMethod GetInfoExceptionMethod();
        protected abstract IMethod GetWarningMethod();
        protected abstract IMethod GetWarningExceptionMethod();
        protected abstract IMethod GetErrorMethod();
        protected abstract IMethod GetErrorExceptionMethod();
        protected abstract IMethod GetFatalMethod();
        protected abstract IMethod GetFatalExceptionMethod();

        protected IMethod FindMethod(string methodName, Predicate<MethodDefDeclaration> predicate = null)
        {
            return predicate != null ? this.module.FindMethod(this.loggerType, methodName, predicate)
                       : this.module.FindMethod(this.loggerType, methodName);
        }
    }
}