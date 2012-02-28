using System;
using System.Reflection;
using NLog;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogContext : ILoggingContext
    {
        private readonly ITypeSignature loggerType;
        private readonly IMethod loggerInitializerMethod;

        public NLogContext(ModuleDeclaration module)
        {
            this.loggerType = module.FindType(typeof(Logger));

            this.loggerInitializerMethod =  module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger", 
                method => method.Parameters.Count == 1 && 
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
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