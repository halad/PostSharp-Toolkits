using System;
using NLog;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogMethodsBuilder : LoggingMethodsBuilder
    {
        private readonly Predicate<MethodDefDeclaration> messageOverloadPredicate;

        public NLogMethodsBuilder(ModuleDeclaration module)
            : base(module, module.FindType(typeof(Logger)))
        {
            // matches Logger.Foo(string)
            this.messageOverloadPredicate = 
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String);
        }

        protected override IMethod GetInitializerMethod()
        {
            return Module.FindMethod(Module.FindType(typeof(LogManager)), "GetLogger",
                method => method.Parameters.Count == 1 &&
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        protected override IMethod GetTraceMethod()
        {
            return FindMethod("Trace", this.messageOverloadPredicate);
        }

        protected override IMethod GetTraceExceptionMethod()
        {
            return FindMethod("TraceException");
        }

        protected override IMethod GetInfoMethod()
        {
            return FindMethod("Info", this.messageOverloadPredicate);
        }

        protected override IMethod GetInfoExceptionMethod()
        {
            return FindMethod("InfoException");
        }

        protected override IMethod GetWarningMethod()
        {
            return FindMethod("Warn", this.messageOverloadPredicate);
        }

        protected override IMethod GetWarningExceptionMethod()
        {
            return FindMethod("WarnException");
        }

        protected override IMethod GetErrorMethod()
        {
            return FindMethod("Error", this.messageOverloadPredicate);
        }

        protected override IMethod GetErrorExceptionMethod()
        {
            return FindMethod("ErrorException");
        }

        protected override IMethod GetFatalMethod()
        {
            return FindMethod("Fatal", this.messageOverloadPredicate);
        }

        protected override IMethod GetFatalExceptionMethod()
        {
            return FindMethod("FatalException");
        }
    }
}