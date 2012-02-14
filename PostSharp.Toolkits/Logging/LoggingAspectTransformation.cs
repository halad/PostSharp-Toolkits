using System;
using PostSharp.Sdk.AspectInfrastructure;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.AspectWeaver.Transformations;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Collections;

namespace PostSharp.Toolkit.Instrumentation.Weaver.Logging
{
    internal class LoggingAspectTransformation : MethodBodyTransformation
    {
        private readonly ILoggingBackend backend;

        public LoggingAspectTransformation(LoggingAspectWeaver aspectWeaver, ILoggingBackend backend)
            : base(aspectWeaver)
        {
            this.backend = backend;
        }

        public override string GetDisplayName(MethodSemantics semantic)
        {
            return "Logging Transformation";
        }

        public AspectWeaverTransformationInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new LoggingAspectTransformationInstance(this, aspectWeaverInstance);
        }

        public class LoggingAspectTransformationInstance : MethodBodyTransformationInstance
        {
            private readonly LoggingAspectTransformation parent;
            public LoggingAspectTransformationInstance(LoggingAspectTransformation parent, AspectWeaverInstance aspectWeaverInstance)
                : base(parent, aspectWeaverInstance)
            {
                this.parent = parent;
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
                private readonly string codeElementName;
                private ILoggingBackendInstance backendInstance;

                public Implementation(LoggingAspectTransformationInstance transformationInstance, MethodBodyTransformationContext context)
                    : base(transformationInstance.AspectWeaver.AspectInfrastructureTask, context)
                {
                    this.transformationInstance = transformationInstance;
                    this.codeElementName = string.Format("{0} ({1})", transformationInstance.AspectWeaverInstance.TargetElement,
                                                                      context.MethodSemantic);

                    this.backendInstance = this.transformationInstance.parent.backend.CreateInstance(transformationInstance.AspectWeaverInstance);
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
                    this.backendInstance.EmitWrite(message, writer);
                    writer.DetachInstructionSequence();
                }
            }
        }
    }
}