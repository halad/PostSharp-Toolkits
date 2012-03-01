using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public sealed class LoggingContext
    {
        private readonly ModuleDeclaration module;
        private readonly ITypeSignature loggerType;
        public IMethod TraceMethod { get; set; }
        public IMethod TraceExceptionMethod { get; set; }
        public IMethod InfoMethod { get; set; }
        public IMethod InfoExceptionMethod { get; set; }
        public IMethod WarningMethod { get; set; }
        public IMethod WarningExceptionMethod { get; set; }
        public IMethod ErrorMethod { get; set; }
        public IMethod ErrorExceptionMethod { get; set; }
        public IMethod FatalMethod { get; set; }
        public IMethod FatalExceptionMethod { get; set; }
        public IMethod IsTraceEnabledMethod { get; set; }
        public IMethod IsInfoEnabledMethod { get; set; }
        public IMethod IsWarningEnabledMethod { get; set; }
        public IMethod IsErrorEnabledMethod { get; set; }
        public IMethod IsFatalEnabledMethod { get; set; }
        public IMethod InitializerMethod { get; set; }

        public ITypeSignature LoggerType
        {
            get { return this.loggerType; }
        }

        protected ModuleDeclaration Module
        {
            get { return this.module; }
        }

        internal LoggingContext(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;            
        }
    }
}