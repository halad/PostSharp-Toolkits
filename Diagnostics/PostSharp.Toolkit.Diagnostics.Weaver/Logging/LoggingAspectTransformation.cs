using System;
using System.Text;
using PostSharp.Sdk.AspectInfrastructure;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.AspectWeaver.Transformations;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Collections;
using PostSharp.Sdk.Utilities;

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
                private readonly ILoggingBackendInstance backendInstance;
                private readonly LogParameters onEntryParameter;
                private readonly LogParameters onExitParameter;

                public Implementation(LoggingAspectTransformationInstance transformationInstance, MethodBodyTransformationContext context)
                    : base(transformationInstance.AspectWeaver.AspectInfrastructureTask, context)
                {
                    this.transformationInstance = transformationInstance;
                    this.backendInstance = this.transformationInstance.parent.backend.CreateInstance(transformationInstance.AspectWeaverInstance);

                    // todo fix configuration
                    //this.onEntryParameter = this.transformationInstance.AspectWeaverInstance.GetConfigurationValue<LogAspectConfiguration, LogParameters>(c => c.OnEntryParameter);
                    //this.onExitParameter = this.transformationInstance.AspectWeaverInstance.GetConfigurationValue<LogAspectConfiguration, LogParameters>(c => c.OnExitParameter);

                    this.onEntryParameter = LogParameters.ParameterTypes | LogParameters.ParameterNames |
                                            LogParameters.ParameterValues;
                }

                public void Implement()
                {
                    ITypeSignature exceptionSignature = this.transformationInstance.AspectWeaver.Module.Cache.GetType(typeof(Exception));

                    Implement(true, false, true, new[] { exceptionSignature });
                    this.Context.AddRedirection(this.Redirection);
                }

                protected override void ImplementOnException(InstructionBlock block, ITypeSignature exceptionType, InstructionWriter writer)
                {
                    MethodDefDeclaration targetMethod = this.transformationInstance.AspectWeaverInstance.TargetElement as MethodDefDeclaration;
                    if (targetMethod == null)
                    {
                        return;
                    }

                    // TODO: nested types
                    string category = targetMethod.DeclaringType.Name;
                    ILoggingCategoryBuilder builder = this.backendInstance.GetCategoryBuilder(category);
                    InstructionSequence sequence = block.AddInstructionSequence(null, NodePosition.After, null);
                    writer.AttachInstructionSequence(sequence);

                    LocalVariableSymbol exceptionLocal = block.MethodBody.RootInstructionBlock.DefineLocalVariable(
                        exceptionType, DebuggerSpecialNames.GetVariableSpecialName("ex"));

                    LogLevel logLevel = LogLevel.Warning;
                    if (builder.SupportsIsEnabled)
                    {
                        builder.EmitGetIsEnabled(writer, logLevel);
                        InstructionSequence branchSequence = block.AddInstructionSequence(null, NodePosition.After, sequence);
                        writer.EmitBranchingInstruction(OpCodeNumber.Brfalse_S, branchSequence);
                    }

                    builder.EmitWrite(writer, block, "An exception occurred:\n{0}", 1, logLevel,
                                      w => w.EmitInstructionLocalVariable(OpCodeNumber.Stloc, exceptionLocal),
                                      (i, w) => w.EmitInstructionLocalVariable(OpCodeNumber.Ldloc, exceptionLocal));

                    writer.EmitInstruction(OpCodeNumber.Rethrow);
                    writer.DetachInstructionSequence();
                }

                protected override void ImplementOnExit(InstructionBlock block, InstructionWriter writer)
                {
                    MethodDefDeclaration targetMethod = Context.TargetElement as MethodDefDeclaration;
                    if (targetMethod == null)
                    {
                        return;
                    }

                    string messageFormatString = this.CreateMessageFormatString(this.onExitParameter, targetMethod);

                    this.EmitMessage(block, writer, targetMethod, "Exiting: " + messageFormatString);
                }

                private string CreateMessageFormatString(LogParameters logParameter, MethodDefDeclaration targetMethod)
                {
                    StringBuilder formatBuilder = new StringBuilder();

                    formatBuilder.AppendFormat("{0}.{1}", targetMethod.DeclaringType, targetMethod.Name);
                    formatBuilder.Append("(");

                    int parameterCount = Context.MethodMapping.MethodSignature.ParameterCount;
                    for (int i = 0; i < parameterCount; i++)
                    {
                        if (i > 0)
                        {
                            formatBuilder.Append(", ");
                        }

                        if ((logParameter & LogParameters.ParameterTypes) != 0)
                        {
                            formatBuilder.Append(Context.MethodMapping.MethodSignature.GetParameterType(i).GetDisplayName());
                            formatBuilder.Append(' ');
                        }
                        if ((logParameter & LogParameters.ParameterNames) != 0)
                        {
                            formatBuilder.Append(Context.MethodMapping.MethodMappingInformation.GetParameterName(i));
                            formatBuilder.Append(' ');
                        }
                        if ((logParameter & LogParameters.ParameterValues) != 0)
                        {
                            formatBuilder.AppendFormat(" = {{{0}}}", i);
                        }
                    }

                    formatBuilder.Append(")");

                    return formatBuilder.ToString();
                }

                protected override void ImplementOnSuccess(InstructionBlock block, InstructionWriter writer)
                {
                }

                protected override void ImplementOnEntry(InstructionBlock block, InstructionWriter writer)
                {
                    MethodDefDeclaration targetMethod = Context.TargetElement as MethodDefDeclaration;
                    if (targetMethod == null)
                    {
                        return;
                    }

                    string messageFormatString = this.CreateMessageFormatString(this.onEntryParameter, targetMethod);

                    this.EmitMessage(block, writer, targetMethod, "Entering: " + messageFormatString);
                }

                private void EmitMessage(InstructionBlock block, InstructionWriter writer, MethodDefDeclaration targetMethod, string messageFormatString)
                {
                    // TODO: nested types
                    string category = targetMethod.DeclaringType.Name;
                    ILoggingCategoryBuilder builder = this.backendInstance.GetCategoryBuilder(category);

                    InstructionSequence sequence = block.AddInstructionSequence(null, NodePosition.After, null);
                    writer.AttachInstructionSequence(sequence);
                    
                    if (builder.SupportsIsEnabled)
                    {
                        builder.EmitGetIsEnabled(writer, LogLevel.Trace);
                        InstructionSequence branchSequence = block.AddInstructionSequence(null, NodePosition.After, sequence);
                        writer.EmitBranchingInstruction(OpCodeNumber.Brfalse_S, branchSequence);
                    }

                    int parameterCount = Context.MethodMapping.MethodSignature.ParameterCount;

                    builder.EmitWrite(writer, block, messageFormatString, parameterCount, LogLevel.Trace, null,
                                      (i, instructionWriter) =>
                                      {
                                          instructionWriter.EmitInstructionInt16(OpCodeNumber.Ldarg, (short)i);
                                          instructionWriter.EmitConvertToObject(
                                              this.Context.MethodMapping.MethodSignature.GetParameterType(i));
                                      });
                    
                    writer.DetachInstructionSequence();
                }
            }
        }
    }
}