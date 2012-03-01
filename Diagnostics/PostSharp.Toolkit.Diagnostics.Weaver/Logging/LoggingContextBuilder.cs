using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingContextBuilder
    {
        private readonly ModuleDeclaration module;
        private readonly ITypeSignature loggerType;

        protected ModuleDeclaration Module
        {
            get { return this.module; }
        }

        protected LoggingContextBuilder(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;
        }

        public LoggingContext CreateContext()
        {
            return new LoggingContext(this.module, this.loggerType)
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
                IsTraceEnabledMethod = this.GetIsTraceEnabledMethod(),
                IsInfoEnabledMethod = this.GetIsInfoEnabledMethod(),
                IsWarningEnabledMethod = this.GetIsWarningEnabledMethod(),
                IsErrorEnabledMethod = this.GetIsErrorEnabledMethod(),
                IsFatalEnabledMethod = this.GetIsFatalEnabledMethod()
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
        protected abstract IMethod GetIsTraceEnabledMethod();
        protected abstract IMethod GetIsInfoEnabledMethod();
        protected abstract IMethod GetIsWarningEnabledMethod();
        protected abstract IMethod GetIsErrorEnabledMethod();
        protected abstract IMethod GetIsFatalEnabledMethod();

        protected IMethod FindMethod(string methodName, Predicate<MethodDefDeclaration> predicate = null)
        {
            return predicate != null ? this.module.FindMethod(this.loggerType, methodName, predicate)
                       : this.module.FindMethod(this.loggerType, methodName);
        }
    }
}