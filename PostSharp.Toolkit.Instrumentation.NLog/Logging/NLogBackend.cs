using System.Collections.Generic;
using NLog;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Instrumentation.Weaver.Logging;

namespace PostSharp.Toolkit.Instrumentation.Weaver.NLog.Logging
{
    internal sealed class NLogBackend : ILoggingBackend
    {
        private ITypeSignature loggerType;
        private ITypeSignature logManagerType;
        private IMethod getMethodMethod;
        private IMethod writeInfoMethod;

        private ContainingTypeBuilder containingTypeBuilder;
           
        private readonly Dictionary<string, FieldDefDeclaration> logFields = new Dictionary<string, FieldDefDeclaration>();

        public void Initialize(ModuleDeclaration module)
        {
            this.containingTypeBuilder = new ContainingTypeBuilder("NLogCategories", module);

            this.loggerType = module.FindType(typeof(Logger));
            this.logManagerType = module.FindType(typeof(LogManager));
            this.getMethodMethod = module.FindMethod(this.logManagerType, "GetLogger", 
                method => method.Parameters.Count == 1 && IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

            this.writeInfoMethod = module.FindMethod(this.loggerType, "Info",
                method => method.Parameters.Count == 1 && IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new NLogBackendInstance(this, aspectWeaverInstance);
        }

        private FieldDefDeclaration GetLogField(string category)
        {
            FieldDefDeclaration loggerField;
            if (!this.logFields.TryGetValue(category, out loggerField))
            {
                loggerField = this.containingTypeBuilder.CreateLoggerField(category, this.loggerType, () => getMethodMethod);
                this.logFields[category] = loggerField;
            }

            return loggerField;
        }

        private sealed class NLogBackendInstance : ILoggingBackendInstance
        {
            private readonly NLogBackend parent;
            private readonly AspectWeaverInstance aspectWeaverInstance;

            public NLogBackendInstance(NLogBackend parent, AspectWeaverInstance aspectWeaverInstance)
            {
                this.parent = parent;
                this.aspectWeaverInstance = aspectWeaverInstance;
            }

            public void EmitWrite(string message, InstructionWriter instructionWriter)
            {
                MethodDefDeclaration targetMethod = this.aspectWeaverInstance.TargetElement as MethodDefDeclaration;
                if (targetMethod == null)
                {
                    return;
                }

                // TODO: nested types
                string category = targetMethod.DeclaringType.Name;
                FieldDefDeclaration loggerField = this.parent.GetLogField(category);
                instructionWriter.EmitInstructionField(OpCodeNumber.Ldsfld, loggerField);
                instructionWriter.EmitInstructionString(OpCodeNumber.Ldstr, message);
                instructionWriter.EmitInstructionMethod(OpCodeNumber.Call, this.parent.writeInfoMethod);
            }
        }
    }
}