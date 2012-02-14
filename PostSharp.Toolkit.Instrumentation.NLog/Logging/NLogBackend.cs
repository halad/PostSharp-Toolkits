using System.Collections.Generic;
using System.Reflection;
using NLog;
using PostSharp.Hosting;
using PostSharp.Reflection;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Sdk.CodeWeaver;
using PostSharp.Sdk.Collections;
using PostSharp.Sdk.Utilities;
using PostSharp.Toolkit.Instrumentation.Weaver.Logging;

namespace PostSharp.Toolkit.Instrumentation.Weaver.NLog.Logging
{
    internal sealed class NLogBackend : ILoggingBackend
    {
        private ITypeSignature loggerType;
        private ITypeSignature logManagerType;
        private IMethod getMethodMethod;
        private IMethod writeInfoMethod;
        private WeavingHelper weavingHelper;

        private TypeDefDeclaration loggerContainingType;
        private InstructionSequence returnSequence;
        private InstructionBlock constructorBlock;
        InstructionWriter writer = new InstructionWriter();
            
        private readonly Dictionary<string, FieldDefDeclaration> logFields = new Dictionary<string, FieldDefDeclaration>();

        public void Initialize(ModuleDeclaration module)
        {
            this.weavingHelper = new WeavingHelper(module);
            this.loggerContainingType = this.CreateLoggerContainingType(module);

            this.loggerType = module.FindType(typeof(Logger));
            this.logManagerType = module.FindType(typeof(LogManager));
            this.getMethodMethod = module.FindMethod(this.logManagerType, "GetLogger", 
                                                method => method.Parameters.Count == 1 && 
                                                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

            this.writeInfoMethod = module.FindMethod(this.loggerType, "Info",
                                                     method => method.Parameters.Count == 1 &&
                                                               IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new NLogBackendInstance(this, aspectWeaverInstance);
        }

        public FieldDefDeclaration GetLogField(string category)
        {
            FieldDefDeclaration loggerField;
            if (!this.logFields.TryGetValue(category, out loggerField))
            {
                loggerField = this.CreateLoggerCategoryField(category, this.loggerContainingType);
                this.logFields[category] = loggerField;
            }

            return loggerField;
        }

        private FieldDefDeclaration CreateLoggerCategoryField(string category, TypeDefDeclaration containingType)
        {
            string fieldName = string.Format("l{0}", containingType.Fields.Count + 1);
            
            FieldDefDeclaration loggerFieldDef = new FieldDefDeclaration
            {
                Name = fieldName,
                Attributes = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly,
                FieldType = this.loggerType
            };
            containingType.Fields.Add(loggerFieldDef);

            InstructionSequence sequence = this.constructorBlock.AddInstructionSequence(null,
                                                                                        NodePosition.Before,
                                                                                        this.returnSequence);

            this.writer.AttachInstructionSequence(sequence);
            this.writer.EmitInstructionString(OpCodeNumber.Ldstr, category);
            this.writer.EmitInstructionMethod(OpCodeNumber.Call, this.getMethodMethod);
            this.writer.EmitInstructionField(OpCodeNumber.Stsfld, loggerFieldDef);
            this.writer.DetachInstructionSequence();

            return loggerFieldDef;
        }

        private TypeDefDeclaration CreateLoggerContainingType(ModuleDeclaration module)
        {
            string uniqueName = module.Types.GetUniqueName(
                DebuggerSpecialNames.GetDeclarationSpecialName("NLogCategories{0}"));

            TypeDefDeclaration containingType = new TypeDefDeclaration
            {
                Name = uniqueName,
                Attributes = TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract,
                BaseType = ((IType)module.Cache.GetType("System.Object, mscorlib"))
            };
            module.Types.Add(containingType);

            // Add [CompilerGenerated] and [DebuggerNonUserCode] to the type
            this.weavingHelper.AddCompilerGeneratedAttribute(containingType.CustomAttributes);
            this.weavingHelper.AddDebuggerNonUserCodeAttribute(containingType.CustomAttributes);

            MethodDefDeclaration staticConstructor = new MethodDefDeclaration
            {
                Name = ".cctor",
                Attributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.RTSpecialName |
                             MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            };
            containingType.Methods.Add(staticConstructor);
            
            staticConstructor.ReturnParameter = new ParameterDeclaration
            {
                Attributes = ParameterAttributes.Retval,
                ParameterType = module.Cache.GetIntrinsic(IntrinsicType.Void)
            };

            this.constructorBlock = staticConstructor.MethodBody.RootInstructionBlock = staticConstructor.MethodBody.CreateInstructionBlock();
            this.returnSequence = staticConstructor.MethodBody.RootInstructionBlock.AddInstructionSequence(null, NodePosition.After, null);

            this.writer.AttachInstructionSequence(this.returnSequence);
            this.writer.EmitInstruction(OpCodeNumber.Ret);
            this.writer.DetachInstructionSequence();

            return containingType;
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