using System;
using System.Text;
using PostSharp.Sdk.AspectInfrastructure;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.AspectWeaver.Transformations;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
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
                private readonly LogOptions onEntryOptions;
                private readonly LogOptions onSuccessOptions;

                public Implementation(LoggingAspectTransformationInstance transformationInstance, MethodBodyTransformationContext context)
                    : base(transformationInstance.AspectWeaver.AspectInfrastructureTask, context)
                {
                    this.transformationInstance = transformationInstance;
                    this.backendInstance = this.transformationInstance.parent.backend.CreateInstance(transformationInstance.AspectWeaverInstance);

                    // todo fix configuration
                    this.onEntryOptions = this.transformationInstance.AspectWeaverInstance.GetConfigurationValue<LogAspectConfiguration, LogOptions>(c => c.OnEntryOptions);
                    this.onSuccessOptions = this.transformationInstance.AspectWeaverInstance.GetConfigurationValue<LogAspectConfiguration, LogOptions>(c => c.OnSuccessOptions);
                }

                public void Implement()
                {
                    ITypeSignature exceptionSignature = this.transformationInstance.AspectWeaver.Module.Cache.GetType(typeof(Exception));

                    bool hasOnEntry = (this.onEntryOptions != LogOptions.None &&
                                       (this.onEntryOptions & LogOptions.NotLogged) != 0);

                    bool hasOnSuccess = (this.onSuccessOptions != LogOptions.None &&
                                         (this.onSuccessOptions & LogOptions.NotLogged) != 0);

                    Implement(hasOnEntry, hasOnSuccess, false, new[] { exceptionSignature });
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
                }

                protected override void ImplementOnEntry(InstructionBlock block, InstructionWriter writer)
                {
                    MethodDefDeclaration targetMethod = Context.TargetElement as MethodDefDeclaration;
                    if (targetMethod == null)
                    {
                        return;
                    }

                    string messageFormatString = this.CreateMessageFormatString(this.onEntryOptions, targetMethod);

                    this.EmitMessage(block, writer, targetMethod, "Entering: " + messageFormatString);
                }

                protected override void ImplementOnSuccess(InstructionBlock block, InstructionWriter writer)
                {
                    MethodDefDeclaration targetMethod = Context.TargetElement as MethodDefDeclaration;
                    if (targetMethod == null)
                    {
                        return;
                    }

                    string messageFormatString = this.CreateMessageFormatString(this.onSuccessOptions, targetMethod);

                    this.EmitMessage(block, writer, targetMethod, "Leaving: " + messageFormatString);
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
                    bool hasThis = Context.MethodMapping.MethodSignature.CallingConvention == CallingConvention.HasThis;

                    builder.EmitWrite(writer, block, messageFormatString, parameterCount, LogLevel.Trace, null,
                                      (i, instructionWriter) =>
                                      {
                                          instructionWriter.EmitInstructionInt16(OpCodeNumber.Ldarg, (short)(hasThis ? i + 1 : i));
                                          instructionWriter.EmitConvertToObject(
                                              this.Context.MethodMapping.MethodSignature.GetParameterType(i));
                                      });
                    
                    writer.DetachInstructionSequence();
                }

                private string CreateMessageFormatString(LogOptions logOption, MethodDefDeclaration targetMethod)
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

                        ITypeSignature parameterType = Context.MethodMapping.MethodSignature.GetParameterType(i);
                        if ((logOption & LogOptions.IncludeParameterType) != 0)
                        {
                            formatBuilder.Append(parameterType.ToString());
                            formatBuilder.Append(' ');
                        }

                        if ((logOption & LogOptions.IncludeParameterName) != 0)
                        {
                            formatBuilder.Append(Context.MethodMapping.MethodMappingInformation.GetParameterName(i));
                            formatBuilder.Append(' ');
                        }

                        if ((logOption & LogOptions.IncludeParameterValue) != 0)
                        {
                            formatBuilder.AppendFormat("= ");

                            if (IntrinsicTypeSignature.Is(parameterType, IntrinsicType.String))
                            {
                                formatBuilder.AppendFormat("\"" + "{{{0}}}" + "\"", i);
                            }
                            else
                            {
                                formatBuilder.AppendFormat("{{{0}}}", i);
                            }
                        }
                    }

                    formatBuilder.Append(")");

                    return formatBuilder.ToString();
                }
            }
        }
    }
}