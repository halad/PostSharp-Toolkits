using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceContextBuilder : LoggingContextBuilder
    {
        private readonly Predicate<MethodDefDeclaration> messageOverloadPredicate;

        public TraceContextBuilder(ModuleDeclaration module)
            : base(module, module.FindType(typeof(System.Diagnostics.Trace)))
        {
            // matches Trace.Foo(string)
            this.messageOverloadPredicate =
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String);
        }

        protected override IMethod GetInitializerMethod()
        {
            // no initialization required
            return null;
        }

        protected override IMethod GetTraceMethod()
        {
            // System.Diagnostics.Trace.WriteLine(string)
            return FindMethod("WriteLine", this.messageOverloadPredicate);
        }

        protected override IMethod GetTraceExceptionMethod()
        {
            return null;
        }

        protected override IMethod GetInfoMethod()
        {
            // System.Diagnostics.Trace.TraceInformation(string)
            return FindMethod("TraceInformation", this.messageOverloadPredicate);
        }

        protected override IMethod GetInfoExceptionMethod()
        {
            return null;
        }

        protected override IMethod GetWarningMethod()
        {
            // System.Diagnostics.Trace.TraceWarning(string)
            return FindMethod("TraceWarning", this.messageOverloadPredicate);
        }

        protected override IMethod GetWarningExceptionMethod()
        {
            return null;
        }

        protected override IMethod GetErrorMethod()
        {
            // System.Diagnostics.Trace.TraceError(string)
            return FindMethod("TraceError", this.messageOverloadPredicate);
        }

        protected override IMethod GetErrorExceptionMethod()
        {
            return null;
        }

        protected override IMethod GetFatalMethod()
        {
            // System.Diagnostics.Trace.Fail(string)
            return FindMethod("Fail", this.messageOverloadPredicate);
        }

        protected override IMethod GetFatalExceptionMethod()
        {
            return null;
        }

        protected override IMethod GetIsTraceEnabledMethod()
        {
            return null;
        }

        protected override IMethod GetIsInfoEnabledMethod()
        {
            return null;
        }

        protected override IMethod GetIsWarningEnabledMethod()
        {
            return null;
        }

        protected override IMethod GetIsErrorEnabledMethod()
        {
            return null;
        }

        protected override IMethod GetIsFatalEnabledMethod()
        {
            return null;
        }
    }
}