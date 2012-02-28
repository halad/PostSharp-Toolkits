using System;
using System.Reflection;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeWeaver;
using PostSharp.Sdk.Collections;
using PostSharp.Sdk.Utilities;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public sealed class LoggingContextContainingTypeBuilder
    {
        private readonly ModuleDeclaration module;
        private readonly TypeDefDeclaration containingType;
        private readonly WeavingHelper weavingHelper;
        private readonly InstructionWriter writer = new InstructionWriter();

        private InstructionSequence returnSequence;
        private InstructionBlock constructorBlock;

        public LoggingContextContainingTypeBuilder(ModuleDeclaration module)
        {
            this.module = module;
            this.weavingHelper = new WeavingHelper(module);
            this.containingType = this.CreateContainingType();
        }

        private TypeDefDeclaration CreateContainingType()
        {
            string uniqueName = this.module.Types.GetUniqueName(
                DebuggerSpecialNames.GetDeclarationSpecialName("LoggingContext{0}"));

            TypeDefDeclaration logCategoriesType = new TypeDefDeclaration
            {
                Name = uniqueName,
                Attributes = TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract,
                BaseType = ((IType)this.module.Cache.GetType("System.Object, mscorlib"))
            };
            this.module.Types.Add(logCategoriesType);

            // Add [CompilerGenerated] and [DebuggerNonUserCode] to the type
            this.weavingHelper.AddCompilerGeneratedAttribute(logCategoriesType.CustomAttributes);
            this.weavingHelper.AddDebuggerNonUserCodeAttribute(logCategoriesType.CustomAttributes);

            MethodDefDeclaration staticConstructor = new MethodDefDeclaration
            {
                Name = ".cctor",
                Attributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.RTSpecialName |
                             MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            };
            logCategoriesType.Methods.Add(staticConstructor);

            staticConstructor.ReturnParameter = new ParameterDeclaration
            {
                Attributes = ParameterAttributes.Retval,
                ParameterType = this.module.Cache.GetIntrinsic(IntrinsicType.Void)
            };

            this.constructorBlock = staticConstructor.MethodBody.RootInstructionBlock = staticConstructor.MethodBody.CreateInstructionBlock();
            this.returnSequence = staticConstructor.MethodBody.RootInstructionBlock.AddInstructionSequence(null, NodePosition.After, null);

            this.writer.AttachInstructionSequence(this.returnSequence);
            this.writer.EmitInstruction(OpCodeNumber.Ret);
            this.writer.DetachInstructionSequence();

            return logCategoriesType;
        }

        public FieldDefDeclaration CreateLoggerField(string category, ILoggingBackendWriter backendWriter)
        {
            string fieldName = string.Format("l{0}", this.containingType.Fields.Count + 1);

            FieldDefDeclaration loggerFieldDef = new FieldDefDeclaration
            {
                Name = fieldName,
                Attributes = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly,
                FieldType = backendWriter.LoggerType
            };
            this.containingType.Fields.Add(loggerFieldDef);

            InstructionSequence sequence = this.constructorBlock.AddInstructionSequence(null,
                                                                                        NodePosition.Before,
                                                                                        this.returnSequence);

            this.writer.AttachInstructionSequence(sequence);
            backendWriter.EmitInitialization(writer, category);
            this.writer.EmitInstructionField(OpCodeNumber.Stsfld, loggerFieldDef);
            this.writer.DetachInstructionSequence();

            return loggerFieldDef;
        }
    }
}