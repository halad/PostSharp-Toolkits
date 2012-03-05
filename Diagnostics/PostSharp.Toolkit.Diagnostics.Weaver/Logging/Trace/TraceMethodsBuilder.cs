using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace
{
    internal sealed class TraceMethodsBuilder : LoggingMethodsBuilder
    {
        private readonly Predicate<MethodDefDeclaration> stringOverloadPredicate;
        private readonly Predicate<MethodDefDeclaration> objectOverloadPredicate;

        public TraceMethodsBuilder(ModuleDeclaration module)
            : base(module, module.FindType(typeof(System.Diagnostics.Trace)))
        {
            // matches Trace.Foo(string)
            this.stringOverloadPredicate =
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String);

            this.objectOverloadPredicate =
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.Object);
        }

        protected override IMethod GetInitializerMethod()
        {
            // no initialization required
            return null;
        }

        protected override IMethod GetTraceMethod()
        {
            return FindMethod("WriteLine", this.objectOverloadPredicate);
        }

        protected override IMethod GetTraceExceptionMethod()
        {
            return this.GetTraceMethod();
        }

        protected override IMethod GetInfoMethod()
        {
            return FindMethod("TraceInformation", this.stringOverloadPredicate);
        }

        protected override IMethod GetInfoExceptionMethod()
        {
            return this.GetInfoMethod();
        }

        protected override IMethod GetWarningMethod()
        {
            return FindMethod("TraceWarning", this.stringOverloadPredicate);
        }

        protected override IMethod GetWarningExceptionMethod()
        {
            return this.GetWarningMethod();
        }

        protected override IMethod GetErrorMethod()
        {
            return FindMethod("TraceError", this.stringOverloadPredicate);
        }

        protected override IMethod GetErrorExceptionMethod()
        {
            return this.GetErrorMethod();
        }

        protected override IMethod GetFatalMethod()
        {
            return FindMethod("Fail", this.stringOverloadPredicate);
        }

        protected override IMethod GetFatalExceptionMethod()
        {
            return this.GetFatalMethod();
        }
    }
}