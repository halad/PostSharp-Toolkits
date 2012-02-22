using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context
{
    public sealed class LoggingContext
    {
        public ITypeSignature LoggerType { get; private set; }
        public IMethod LoggerInitializerMethod { get; set; }

        public IMethod WriteTraceMethod { get; set; }
        public IMethod WriteTraceExceptionMethod { get; set; }
        public IMethod WriteInfoMethod { get; set; }
        public IMethod WriteInfoExceptionMethod { get; set; }
        public IMethod WriteWarningMethod { get; set; }
        public IMethod WriteWarningExceptionMethod { get; set; }
        public IMethod WriteErrorMethod { get; set; }
        public IMethod WriteErrorExceptionMethod { get; set; }
        public IMethod WriteFatalMethod { get; set; }
        public IMethod WriteFatalExceptionMethod { get; set; }
        
        public bool IsTraceEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsWarningEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsFatalEnabled { get; set; }

        public LoggingContext(ITypeSignature loggerType)
        {
            this.LoggerType = loggerType;
        }
    }
}