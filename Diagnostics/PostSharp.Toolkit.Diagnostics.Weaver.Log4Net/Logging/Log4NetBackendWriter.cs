using System;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using log4net;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackendWriter : LoggingBackendWriter
    {
        private readonly Predicate<MethodDefDeclaration> messageOverloadPredicate;
        private readonly Predicate<MethodDefDeclaration> exceptionOverloadPredicate;

        private readonly IMethod debugMethod;
        private readonly IMethod debugExceptionMethod;
        private readonly IMethod infoMethod;
        private readonly IMethod infoExceptionMethod;
        private readonly IMethod warningMethod;
        private readonly IMethod warningExceptionMethod;
        private readonly IMethod errorMethod;
        private readonly IMethod errorExceptionMethod;
        private readonly IMethod fatalMethod;
        private readonly IMethod fatalExceptionMethod;
        private readonly IMethod isDebugEnabledMethod;
        private readonly IMethod isInfoEnabledMethod;
        private readonly IMethod isWarningEnabledMethod;
        private readonly IMethod isErrorEnabledMethod;
        private readonly IMethod isFatalEnabledMethod;
        private readonly IMethod initializerMethod;

        public Log4NetBackendWriter(ModuleDeclaration module)
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

            this.debugMethod = FindMethod("Debug", this.messageOverloadPredicate);
            this.debugExceptionMethod = FindMethod("Debug", this.exceptionOverloadPredicate);
            this.infoMethod = FindMethod("Info", this.messageOverloadPredicate);
            this.infoExceptionMethod = FindMethod("Info", this.exceptionOverloadPredicate);
            this.warningMethod = FindMethod("Warn", this.messageOverloadPredicate);
            this.warningExceptionMethod = FindMethod("Warn", this.exceptionOverloadPredicate);
            this.errorMethod = FindMethod("Error", this.messageOverloadPredicate);
            this.errorExceptionMethod = FindMethod("Error", this.exceptionOverloadPredicate);
            this.fatalMethod = FindMethod("Fatal", this.messageOverloadPredicate);
            this.fatalExceptionMethod = FindMethod("Fatal", this.exceptionOverloadPredicate);

            this.isDebugEnabledMethod = FindMethod("IsDebugEnabled");
            this.isInfoEnabledMethod = FindMethod("IsInfoEnabled");
            this.isWarningEnabledMethod = FindMethod("IsWarnEnabled");
            this.isErrorEnabledMethod = FindMethod("IsErrorEnabled");
            this.isFatalEnabledMethod = FindMethod("IsFatalEnabled");

            this.initializerMethod = Module.FindMethod(Module.FindType(typeof(LogManager)), "GetLogger",
                method => method.Parameters.Count == 1 && 
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        public override void EmitInitialization(InstructionWriter writer, string category)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, category);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.initializerMethod);
        }

        public override void EmitTrace(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.debugMethod);
        }

        public override void EmitInfo(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.infoMethod);
        }

        public override void EmitWarning(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.warningMethod);
        }

        public override void EmitError(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.errorMethod);
        }

        public override void EmitFatal(InstructionWriter writer, string message, Exception exception = null)
        {
            writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
            writer.EmitInstructionMethod(OpCodeNumber.Call, this.fatalMethod);
        }
    }
}