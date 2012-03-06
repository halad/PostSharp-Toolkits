using System;
using System.Collections.Generic;
using System.Reflection;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeWeaver;
using PostSharp.Sdk.Collections;
using PostSharp.Sdk.Utilities;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public sealed class LoggingImplementationTypeBuilder
    {
        private readonly Dictionary<string,FieldDefDeclaration> categoryFields = new Dictionary<string, FieldDefDeclaration>();
        private readonly ModuleDeclaration module;
        private readonly TypeDefDeclaration containingType;
        private readonly WeavingHelper weavingHelper;
        private readonly InstructionWriter writer = new InstructionWriter();

        private InstructionSequence returnSequence;
        private InstructionBlock constructorBlock;

        public LoggingImplementationTypeBuilder(ModuleDeclaration module)
        {
            this.module = module;
            this.weavingHelper = new WeavingHelper(module);
            this.containingType = this.CreateContainingType();
        }

        public FieldDefDeclaration GetCategoryField(string category, ITypeSignature fieldType, Action<InstructionWriter> initializeFieldAction )
        {
            FieldDefDeclaration categoryField;
            if (!this.categoryFields.TryGetValue(category, out categoryField))
            {
                categoryField = this.CreateCategoryField(fieldType, initializeFieldAction);
                this.categoryFields[category] = categoryField;
            }

            return categoryField;
        }

        private TypeDefDeclaration CreateContainingType()
        {
            string uniqueName = this.module.Types.GetUniqueName(
                DebuggerSpecialNames.GetDeclarationSpecialName("LoggingImplementationDetails{0}"));

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

        private FieldDefDeclaration CreateCategoryField(ITypeSignature fieldType, Action<InstructionWriter> initializeFieldAction)
        {
            string fieldName = string.Format("l{0}", this.containingType.Fields.Count + 1);

            FieldDefDeclaration loggerFieldDef = new FieldDefDeclaration
            {
                Name = fieldName,
                Attributes = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly,
                FieldType = fieldType
            };
            this.containingType.Fields.Add(loggerFieldDef);

            InstructionSequence sequence = this.constructorBlock.AddInstructionSequence(null,
                                                                                        NodePosition.Before,
                                                                                        this.returnSequence);

            this.writer.AttachInstructionSequence(sequence);
            initializeFieldAction(this.writer);
            this.writer.EmitInstructionField(OpCodeNumber.Stsfld, loggerFieldDef);
            this.writer.DetachInstructionSequence();

            return loggerFieldDef;
        }
    }
}