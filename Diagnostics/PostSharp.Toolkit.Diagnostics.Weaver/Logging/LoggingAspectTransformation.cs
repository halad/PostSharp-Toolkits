using System;
using PostSharp.Sdk.AspectInfrastructure;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.AspectWeaver.Transformations;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Collections;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
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
                private readonly ILoggingBackendInstance backendInstance;

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
                    ITypeSignature exceptionSignature = this.transformationInstance.AspectWeaver.Module.Cache.GetType(typeof(Exception));


                    Implement(true, false, true, new[] { exceptionSignature });
                    this.Context.AddRedirection(this.Redirection);
                }

                protected override void ImplementOnException(InstructionBlock block, ITypeSignature exceptionType, InstructionWriter writer)
                {
                    backendInstance.GetCategoryBuilder("xboo").EmitWrite(writer, block, "Got an exception( {0} )", 1, LogLevel.Warning, 
                        null /* ldloc */,  (i, instructionWriter) => writer.EmitInstructionInt16(OpCodeNumber.Ldarg, i) );
                    this.EmitExceptionMessage(block, writer, exceptionType, "An exception occurred");
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
                    MethodDefDeclaration targetMethod = this.transformationInstance.AspectWeaverInstance.TargetElement as MethodDefDeclaration;
                    if (targetMethod == null)
                    {
                        return;
                    }

                    // TODO: nested types
                    string category = targetMethod.DeclaringType.Name;

                    InstructionSequence sequence = block.AddInstructionSequence(null, NodePosition.After, null);

                    writer.AttachInstructionSequence(sequence);
                    this.backendInstance.EmitWrite(writer, block, category, message, LogLevel.Trace);
                    writer.DetachInstructionSequence();
                }

                private void EmitExceptionMessage(InstructionBlock block, InstructionWriter writer, ITypeSignature exceptionType, string message)
                {
                    MethodDefDeclaration targetMethod = (MethodDefDeclaration)this.transformationInstance.AspectWeaverInstance.TargetElement;

                    // TODO: nested types
                    string category = targetMethod.DeclaringType.Name;

                    InstructionSequence sequence = block.AddInstructionSequence(null, NodePosition.After, null);
                    writer.AttachInstructionSequence(sequence);

                    this.backendInstance.EmitWriteException(writer, block, category, message, exceptionType, LogLevel.Info);
                    writer.EmitInstruction(OpCodeNumber.Rethrow);
                    writer.DetachInstructionSequence();
                }
            }
        }
    }
}