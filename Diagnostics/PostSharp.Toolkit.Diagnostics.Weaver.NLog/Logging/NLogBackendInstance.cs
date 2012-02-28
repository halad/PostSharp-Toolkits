using System;
using NLog;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    internal sealed class NLogBackendWriter : LoggingBackendWriter
    {
        public NLogBackendWriter(ModuleDeclaration module)
            : base(module, module.FindType(typeof(Logger)))
        {
            throw new NotImplementedException();
        }

        public override void EmitInitialization(InstructionWriter writer, string category)
        {
            throw new NotImplementedException();
        }

        public override void EmitTrace(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public override void EmitInfo(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public override void EmitWarning(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public override void EmitError(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public override void EmitFatal(InstructionWriter writer, string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }
    }
}