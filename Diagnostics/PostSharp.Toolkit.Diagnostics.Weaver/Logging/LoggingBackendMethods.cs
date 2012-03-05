using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public sealed class LoggingBackendMethods
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
        public IMethod InitializerMethod { get; set; }

        public ITypeSignature LoggerType
        {
            get { return this.loggerType; }
        }

        public ModuleDeclaration Module
        {
            get { return this.module; }
        }

        internal LoggingBackendMethods(ModuleDeclaration module, ITypeSignature loggerType)
        {
            this.module = module;
            this.loggerType = loggerType;            
        }
    }
}