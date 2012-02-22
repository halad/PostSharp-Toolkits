using System;
using NLog;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogContextBuilder : LoggingContextBuilder
    {
        private readonly Predicate<MethodDefDeclaration> oneStringArgumentPredicate;

        public NLogContextBuilder(ModuleDeclaration module)
            : base(module, module.FindType(typeof(Logger)))
        {
            this.oneStringArgumentPredicate = method => method.Parameters.Count == 1 &&
                                                        IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String);
        }

        protected override IMethod GetLoggerInitializerMethod()
        {
            return module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger", 
                method => method.Parameters.Count == 1 && 
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        protected override IMethod GetTraceMethod()
        {
            return FindMethod("Trace", this.oneStringArgumentPredicate);
        }

        protected override IMethod GetTraceExceptionMethod()
        {
            return FindMethod("TraceException");
        }

        protected override IMethod GetInfoMethod()
        {
            return FindMethod("Info", this.oneStringArgumentPredicate);
        }

        protected override IMethod GetInfoExceptionMethod()
        {
            return FindMethod("InfoException");
        }

        protected override IMethod GetWarningMethod()
        {
            return FindMethod("Warn", this.oneStringArgumentPredicate);
        }

        protected override IMethod GetWarningExceptionMethod()
        {
            return FindMethod("WarnException");
        }

        protected override IMethod GetErrorMethod()
        {
            return FindMethod("Error", this.oneStringArgumentPredicate);
        }

        protected override IMethod GetErrorExceptionMethod()
        {
            return FindMethod("ErrorException");
        }

        protected override IMethod GetFatalMethod()
        {
            return FindMethod("Fatal", this.oneStringArgumentPredicate);
        }

        protected override IMethod GetFatalExceptionMethod()
        {
            return FindMethod("FatalException");
        }
    }
}