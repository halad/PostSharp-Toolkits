using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Context;
using log4net;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetContextBuilder : LoggingContextBuilder
    {
        private readonly Predicate<MethodDefDeclaration> messageOverloadPredicate;
        private readonly Predicate<MethodDefDeclaration> exceptionOverloadPredicate;

        public Log4NetContextBuilder(ModuleDeclaration module)
            : base(module, module.FindType(typeof(ILog)))
        {
            // matches ILog.Foo(object) overload
            this.messageOverloadPredicate = 
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.Object);

            ITypeSignature exceptionType = module.Cache.GetType(typeof(Exception));
            
            // matches ILog.Foo(object, Exception) overload
            this.exceptionOverloadPredicate =
                method => method.Parameters.Count == 2 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.Object) &&
                          method.Parameters[1].ParameterType.Equals(exceptionType);
        }

        protected override IMethod GetLoggerInitializerMethod()
        {
            return Module.FindMethod(Module.FindType(typeof(LogManager)), "GetLogger",
                method => method.Parameters.Count == 1 &&
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        protected override IMethod GetTraceMethod()
        {
            return FindMethod("Debug", this.messageOverloadPredicate);
        }

        protected override IMethod GetTraceExceptionMethod()
        {
            return FindMethod("Debug", this.exceptionOverloadPredicate);
        }

        protected override IMethod GetInfoMethod()
        {
            return FindMethod("Info", this.messageOverloadPredicate);
        }

        protected override IMethod GetInfoExceptionMethod()
        {
            return FindMethod("Info", this.exceptionOverloadPredicate);
        }

        protected override IMethod GetWarningMethod()
        {
            return FindMethod("Warn", this.messageOverloadPredicate);
        }

        protected override IMethod GetWarningExceptionMethod()
        {
            return FindMethod("Warn", this.exceptionOverloadPredicate);
        }

        protected override IMethod GetErrorMethod()
        {
            return FindMethod("Error", this.messageOverloadPredicate);
        }

        protected override IMethod GetErrorExceptionMethod()
        {
            return FindMethod("Error", this.exceptionOverloadPredicate);
        }

        protected override IMethod GetFatalMethod()
        {
            return FindMethod("Fatal", this.messageOverloadPredicate);
        }

        protected override IMethod GetFatalExceptionMethod()
        {
            return FindMethod("Fatal", this.exceptionOverloadPredicate);
        }
    }
}