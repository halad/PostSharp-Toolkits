using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context
{
    public abstract class LoggingContextBuilder
    {
        protected readonly ModuleDeclaration module;
        private readonly ITypeSignature loggerType;

        protected LoggingContextBuilder(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;
        }

        public LoggingContext CreateContext()
        {
            LoggingContext context = new LoggingContext(this.loggerType)
            {
                LoggerInitializerMethod = GetLoggerInitializerMethod(),
                WriteTraceMethod = this.GetTraceMethod(),
                WriteTraceExceptionMethod = this.GetTraceExceptionMethod(),
                WriteInfoMethod = this.GetInfoMethod(),
                WriteInfoExceptionMethod = this.GetInfoExceptionMethod(),
                WriteWarningMethod = this.GetWarningMethod(),
                WriteWarningExceptionMethod = this.GetWarningExceptionMethod(),
                WriteErrorMethod = this.GetErrorMethod(),
                WriteErrorExceptionMethod = this.GetErrorExceptionMethod(),
                WriteFatalMethod = this.GetFatalMethod(),
                WriteFatalExceptionMethod = this.GetFatalExceptionMethod(),
                
            };

            return context;
        }

        protected abstract IMethod GetLoggerInitializerMethod();
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