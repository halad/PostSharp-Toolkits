using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using log4net;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetContext : ILoggingContext
    {
        private readonly ITypeSignature loggerType;
        private readonly IMethod loggerInitializerMethod;

        private readonly Predicate<MethodDefDeclaration> messageOverloadPredicate;
        private readonly Predicate<MethodDefDeclaration> exceptionOverloadPredicate;

        public Log4NetContext(ModuleDeclaration module)
        {
            this.loggerType = module.FindType(typeof(ILog));
            this.loggerInitializerMethod = module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger",
                method => method.Parameters.Count == 1 &&
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

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

        public ITypeSignature LoggerType
        {
            get { return this.loggerType; }
        }

        public void EmitInitialization(InstructionWriter writer)
        {
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.loggerInitializerMethod);
        }

        public void EmitTrace(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public void EmitInfo(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public void EmitWarning(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public void EmitError(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public void EmitFatal(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }
    }
}