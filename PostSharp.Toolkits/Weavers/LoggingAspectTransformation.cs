using System;
using PostSharp.Sdk.AspectInfrastructure;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.AspectWeaver.Transformations;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Collections;
using PostSharp.Toolkits.Logging;

namespace PostSharp.Toolkits.Weavers
{
    public class LoggingAspectTransformation : MethodBodyTransformation
    {
        private readonly Assets assets;

        public LoggingAspectTransformation(AspectWeaver aspectWeaver)
            : base(aspectWeaver)
        {
            this.assets = aspectWeaver.Module.Cache.GetItem(() => new Assets(aspectWeaver));
        }

        public override string GetDisplayName(MethodSemantics semantic)
        {
            return "Logging Transformation";
        }

        public AspectWeaverTransformationInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new LoggingAspectTransformationInstance(this, aspectWeaverInstance);
        }

        private sealed class Assets
        {
            public readonly GenericMethodReference LogWriteLineMethodImpl;
            public IMethod instance;

            public Assets(AspectWeaver aspectWeaver)
            {
                IMethod logWriteLineMethod = aspectWeaver.Module.FindMethod(
                    aspectWeaver.Module.FindType(typeof(ILoggingToolkit)), "WriteLine");

                this.LogWriteLineMethodImpl = logWriteLineMethod.GetMethodDefinition().FindOverride(aspectWeaver.AspectType);
                this.instance = this.LogWriteLineMethodImpl.GetInstance(aspectWeaver.AspectType);

            }
        }

        public class LoggingAspectTransformationInstance : MethodBodyTransformationInstance
        {
            public LoggingAspectTransformationInstance(MethodBodyTransformation parent, AspectWeaverInstance aspectWeaverInstance)
                : base(parent, aspectWeaverInstance)
            {
            }

            public override void Implement(MethodBodyTransformationContext context)
            {
                Implementation implementation = new Implementation(this, context);
                implementation.Implement();
            }

            public override MethodBodyTransformationOptions GetOptions(MetadataDeclaration originalTargetElement, MethodSemantics semantic)
            {
                return MethodBodyTransformationOptions.None;
            }

            private sealed class Implementation : MethodBodyWrappingImplementation
            {
                private readonly LoggingAspectTransformationInstance transformationInstance;
                private readonly Assets assets;
                private readonly string codeElementName;

                public Implementation(LoggingAspectTransformationInstance transformationInstance, MethodBodyTransformationContext context)
                    : base(transformationInstance.AspectWeaver.AspectInfrastructureTask, context)
                {
                    this.transformationInstance = transformationInstance;
                    this.assets = ((LoggingAspectTransformation)transformationInstance.Transformation).assets;
                    this.codeElementName = string.Format("{0} ({1})", transformationInstance.AspectWeaverInstance.TargetElement,
                                                                      context.MethodSemantic);
                }

                public void Implement()
                {
                    Implement(true, false, true, null);
                    this.Context.AddRedirection(this.Redirection);
                }

                protected override void ImplementOnException(InstructionBlock block, ITypeSignature exceptionType, InstructionWriter writer)
                {
                }

                protected override void ImplementOnExit(InstructionBlock block, InstructionWriter writer)
                {
                    this.EmitMessage(block, writer, "Exiting: " + this.codeElementName);
                }

                protected override void ImplementOnSuccess(InstructionBlock block, InstructionWriter writer)
                {
                }

                protected override void ImplementOnEntry(InstructionBlock block, InstructionWriter writer)
                {
                    this.EmitMessage(block, writer, "Entering: " + this.codeElementName);
                }

                private void EmitMessage(InstructionBlock block, InstructionWriter writer, string message)
                {
                    InstructionSequence sequence = block.AddInstructionSequence(null, NodePosition.After, null);

                    writer.AttachInstructionSequence(sequence);
                    this.transformationInstance.AspectWeaverInstance.AspectRuntimeInstanceField.EmitLoadField(writer,
                                                                                                              null);

                    writer.EmitInstructionString(OpCodeNumber.Ldstr, message);
                    writer.EmitInstructionMethod(
                        !this.assets.instance.IsVirtual ||
                        (this.assets.instance.IsSealed || this.assets.instance.DeclaringType.IsSealed)
                            ? OpCodeNumber.Call
                            : OpCodeNumber.Callvirt,
                        this.assets.instance.TranslateMethod(
                            this.transformationInstance.AspectWeaver.Module));


                    writer.DetachInstructionSequence();
                }
            }
        }
    }
}