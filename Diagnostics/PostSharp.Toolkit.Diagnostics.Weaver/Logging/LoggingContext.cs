using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public abstract class LoggingContext
    {
        private readonly ModuleDeclaration module;
        private readonly ITypeSignature loggerType;
        public IMethod TraceMethod { get; private set; }
        public IMethod TraceExceptionMethod { get; private set; }
        public IMethod InfoMethod { get; private set; }
        public IMethod InfoExceptionMethod { get; private set; }
        public IMethod WarningMethod { get; private set; }
        public IMethod WarningExceptionMethod { get; private set; }
        public IMethod ErrorMethod { get; private set; }
        public IMethod ErrorExceptionMethod { get; private set; }
        public IMethod FatalMethod { get; private set; }
        public IMethod FatalExceptionMethod { get; private set; }
        public IMethod IsTraceEnabledMethod { get; private set; }
        public IMethod IsInfoEnabledMethod { get; private set; }
        public IMethod IsWarningEnabledMethod { get; private set; }
        public IMethod IsErrorEnabledMethod { get; private set; }
        public IMethod IsFatalEnabledMethod { get; private set; }
        public IMethod InitializerMethod { get; private set; }

        public ITypeSignature LoggerType
        {
            get { return this.loggerType; }
        }

        public ModuleDeclaration Module
        {
            get { return this.module; }
        }

        protected LoggingContext(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;            
        }

        protected void InitializeContext()
        {
            InitializerMethod = this.GetInitializerMethod();
            TraceMethod = this.GetTraceMethod();
            TraceExceptionMethod = this.GetTraceExceptionMethod();
            InfoMethod = this.GetInfoMethod();
            InfoExceptionMethod = this.GetInfoExceptionMethod();
            WarningMethod = this.GetWarningMethod();
            WarningExceptionMethod = this.GetWarningExceptionMethod();
            ErrorMethod = this.GetErrorMethod();
            ErrorExceptionMethod = this.GetErrorExceptionMethod();
            FatalMethod = this.GetFatalMethod();
            FatalExceptionMethod = this.GetFatalExceptionMethod();
            IsTraceEnabledMethod = this.GetIsTraceEnabledMethod();
            IsInfoEnabledMethod = this.GetIsInfoEnabledMethod();
            IsWarningEnabledMethod = this.GetIsWarningEnabledMethod();
            IsErrorEnabledMethod = this.GetIsErrorEnabledMethod();
            IsFatalEnabledMethod = this.GetIsFatalEnabledMethod();
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