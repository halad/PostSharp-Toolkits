using System;
using System.Reflection;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeWeaver;
using PostSharp.Sdk.Collections;
using PostSharp.Sdk.Utilities;

namespace PostSharp.Toolkit.Instrumentation.Weaver.Logging
{
    public sealed class ContainingTypeBuilder
    {
        private readonly ModuleDeclaration module;
        private readonly TypeDefDeclaration containingType;
        private readonly WeavingHelper weavingHelper;
        private readonly InstructionWriter writer = new InstructionWriter();

        private InstructionSequence returnSequence;
        private InstructionBlock constructorBlock;

        public ContainingTypeBuilder(string name, ModuleDeclaration module)
        {
            this.module = module;
            this.weavingHelper = new WeavingHelper(module);
            this.containingType = this.CreateContainingType(name);
        }

        private TypeDefDeclaration CreateContainingType(string typeName)
        {
            string uniqueName = this.module.Types.GetUniqueName(
                DebuggerSpecialNames.GetDeclarationSpecialName(typeName + "{0}"));

            TypeDefDeclaration logCategoriesType = new TypeDefDeclaration
            {
                Name = uniqueName,
                Attributes = TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract,
                BaseType = ((IType)this.module.Cache.GetType("System.Object, mscorlib"))
            };
            this.module.Types.Add(this.containingType);

            // Add [CompilerGenerated] and [DebuggerNonUserCode] to the type
            this.weavingHelper.AddCompilerGeneratedAttribute(this.containingType.CustomAttributes);
            this.weavingHelper.AddDebuggerNonUserCodeAttribute(this.containingType.CustomAttributes);

            MethodDefDeclaration staticConstructor = new MethodDefDeclaration
            {
                Name = ".cctor",
                Attributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.RTSpecialName |
                             MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            };
            this.containingType.Methods.Add(staticConstructor);

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

        public FieldDefDeclaration CreateLoggerField(string category, ITypeSignature loggerType, Func<IMethod> loggerInitializer)
        {
            string fieldName = string.Format("l{0}", this.containingType.Fields.Count + 1);

            FieldDefDeclaration loggerFieldDef = new FieldDefDeclaration
            {
                Name = fieldName,
                Attributes = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly,
                FieldType = loggerType
            };
            this.containingType.Fields.Add(loggerFieldDef);

            InstructionSequence sequence = this.constructorBlock.AddInstructionSequence(null,
                                                                                        NodePosition.Before,
                                                                                        this.returnSequence);

            this.writer.AttachInstructionSequence(sequence);
            this.writer.EmitInstructionString(OpCodeNumber.Ldstr, category);
            this.writer.EmitInstructionMethod(OpCodeNumber.Call, loggerInitializer.Invoke());
            this.writer.EmitInstructionField(OpCodeNumber.Stsfld, loggerFieldDef);
            this.writer.DetachInstructionSequence();

            return loggerFieldDef;
        }
    }
}