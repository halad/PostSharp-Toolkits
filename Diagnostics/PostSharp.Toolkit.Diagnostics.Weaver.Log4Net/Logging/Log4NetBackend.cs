using System;
using System.Collections.Generic;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using log4net;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging
{
    internal sealed class Log4NetBackend : ILoggingBackend
    {
        private LoggingImplementationTypeBuilder loggingImplementation;
        private StringFormatWriter formatWriter;

        private IMethod writeDebugArrayMethod;
        private IMethod writeInfoArrayMethod;
        private IMethod writeWarningArrayMethod;
        private IMethod writeErrorArrayMethod;
        private IMethod writeFatalArrayMethod;
        
        private IMethod getIsDebugEnabledMethod;
        private IMethod getIsInfoEnabledMethod;
        private IMethod getIsWarnEnabledMethod;
        private IMethod getIsErrorEnabledMethod;
        private IMethod getIsFatalEnabledMethod;
        private IMethod categoryInitializerMethod;
        private ITypeSignature loggerType;

        private Predicate<MethodDefDeclaration> format1Predicate;
        private Predicate<MethodDefDeclaration> format2Predicate;
        private Predicate<MethodDefDeclaration> format3Predicate;
        private Predicate<MethodDefDeclaration> formatArrayPredicate;

        private readonly Dictionary<LogLevel, Dictionary<int, IMethod>> loggerMethods = new Dictionary<LogLevel, Dictionary<int, IMethod>>
        {
            { LogLevel.Trace, new Dictionary<int, IMethod>() },
            { LogLevel.Info, new Dictionary<int, IMethod>() },
            { LogLevel.Warning, new Dictionary<int, IMethod>() },
            { LogLevel.Error, new Dictionary<int, IMethod>() },
            { LogLevel.Fatal, new Dictionary<int, IMethod>() },
        };

        public void Initialize(ModuleDeclaration module)
        {
            this.loggingImplementation = new LoggingImplementationTypeBuilder(module);
            this.loggerType = module.FindType(typeof(ILog));

            this.categoryInitializerMethod = module.FindMethod(module.FindType(typeof(LogManager)), "GetLogger",
                method => method.Parameters.Count == 1 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String));

            this.format1Predicate = method => 
                method.Parameters.Count == 2 && 
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) && 
                IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object);

            this.format2Predicate = method => 
                method.Parameters.Count == 3 &&
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object);

            this.format3Predicate = method =>
                method.Parameters.Count == 4 &&
                IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                IntrinsicTypeSignature.Is(method.Parameters[1].ParameterType, IntrinsicType.Object) &&
                IntrinsicTypeSignature.Is(method.Parameters[2].ParameterType, IntrinsicType.Object) &&
                IntrinsicTypeSignature.Is(method.Parameters[3].ParameterType, IntrinsicType.Object);

            this.formatArrayPredicate = 
                method => method.Parameters.Count == 2 &&
                          IntrinsicTypeSignature.Is(method.Parameters[0].ParameterType, IntrinsicType.String) &&
                          method.Parameters[1].ParameterType.BelongsToClassification(TypeClassifications.Array);

            this.InitializeLoggerMethodsDictionary(module);

            this.getIsDebugEnabledMethod = module.FindMethod(this.loggerType, "get_IsDebugEnabled");
            this.getIsInfoEnabledMethod = module.FindMethod(this.loggerType, "get_IsInfoEnabled");
            this.getIsWarnEnabledMethod = module.FindMethod(this.loggerType, "get_IsWarnEnabled");
            this.getIsErrorEnabledMethod = module.FindMethod(this.loggerType, "get_IsErrorEnabled");
            this.getIsFatalEnabledMethod = module.FindMethod(this.loggerType, "get_IsFatalEnabled");
        }

        private void InitializeLoggerMethodsDictionary(ModuleDeclaration module)
        {
            this.loggerMethods[LogLevel.Trace][0] = module.FindMethod(this.loggerType, "Debug", 1);
            this.loggerMethods[LogLevel.Trace][1] = module.FindMethod(this.loggerType, "DebugFormat", this.format1Predicate);
            this.loggerMethods[LogLevel.Trace][2] = module.FindMethod(this.loggerType, "DebugFormat", this.format2Predicate);
            this.loggerMethods[LogLevel.Trace][3] = module.FindMethod(this.loggerType, "DebugFormat", this.format3Predicate);
            this.writeDebugArrayMethod = module.FindMethod(this.loggerType, "DebugFormat", this.formatArrayPredicate);

            this.loggerMethods[LogLevel.Info][0] = module.FindMethod(this.loggerType, "Info", 1);
            this.loggerMethods[LogLevel.Info][1] = module.FindMethod(this.loggerType, "InfoFormat", this.format1Predicate);
            this.loggerMethods[LogLevel.Info][2] = module.FindMethod(this.loggerType, "InfoFormat", this.format2Predicate);
            this.loggerMethods[LogLevel.Info][3] = module.FindMethod(this.loggerType, "InfoFormat", this.format3Predicate);
            this.writeInfoArrayMethod = module.FindMethod(this.loggerType, "InfoFormat", this.formatArrayPredicate);

            this.loggerMethods[LogLevel.Warning][0] = module.FindMethod(this.loggerType, "Warn", 1);
            this.loggerMethods[LogLevel.Warning][1] = module.FindMethod(this.loggerType, "WarnFormat", this.format1Predicate);
            this.loggerMethods[LogLevel.Warning][2] = module.FindMethod(this.loggerType, "WarnFormat", this.format2Predicate);
            this.loggerMethods[LogLevel.Warning][3] = module.FindMethod(this.loggerType, "WarnFormat", this.format3Predicate);
            this.writeWarningArrayMethod = module.FindMethod(this.loggerType, "WarnFormat", this.formatArrayPredicate);

            this.loggerMethods[LogLevel.Warning][0] = module.FindMethod(this.loggerType, "Warn", 1);
            this.loggerMethods[LogLevel.Warning][1] = module.FindMethod(this.loggerType, "WarnFormat", this.format1Predicate);
            this.loggerMethods[LogLevel.Warning][2] = module.FindMethod(this.loggerType, "WarnFormat", this.format2Predicate);
            this.loggerMethods[LogLevel.Warning][3] = module.FindMethod(this.loggerType, "WarnFormat", this.format3Predicate);
            this.writeErrorArrayMethod = module.FindMethod(this.loggerType, "ErrorFormat", this.formatArrayPredicate);

            this.loggerMethods[LogLevel.Error][0] = module.FindMethod(this.loggerType, "Error", 1);
            this.loggerMethods[LogLevel.Error][1] = module.FindMethod(this.loggerType, "ErrorFormat", this.format1Predicate);
            this.loggerMethods[LogLevel.Error][2] = module.FindMethod(this.loggerType, "ErrorFormat", this.format2Predicate);
            this.loggerMethods[LogLevel.Error][3] = module.FindMethod(this.loggerType, "ErrorFormat", this.format3Predicate);
            this.writeErrorArrayMethod = module.FindMethod(this.loggerType, "ErrorFormat", this.formatArrayPredicate);

            this.loggerMethods[LogLevel.Fatal][0] = module.FindMethod(this.loggerType, "Fatal", 1);
            this.loggerMethods[LogLevel.Fatal][1] = module.FindMethod(this.loggerType, "FatalFormat", this.format1Predicate);
            this.loggerMethods[LogLevel.Fatal][2] = module.FindMethod(this.loggerType, "FatalFormat", this.format2Predicate);
            this.loggerMethods[LogLevel.Fatal][3] = module.FindMethod(this.loggerType, "FatalFormat", this.format3Predicate);
            this.writeFatalArrayMethod = module.FindMethod(this.loggerType, "FatalFormat", this.formatArrayPredicate);
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            return new Log4NetBackendInstance(this, aspectWeaverInstance.AspectType.Module);
        }

        private class Log4NetBackendInstance : ILoggingBackendInstance
        {
            private readonly Log4NetBackend parent;
            private readonly ModuleDeclaration module;

            public Log4NetBackendInstance(Log4NetBackend parent, ModuleDeclaration module)
            {
                this.parent = parent;
                this.module = module;
            }

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                return new Log4NetCategoryBuilder(this.parent, this.module, categoryName);
            }
        }

        private class Log4NetCategoryBuilder : ILoggingCategoryBuilder
        {
            private readonly Log4NetBackend parent;
            private readonly ModuleDeclaration module;
            private readonly FieldDefDeclaration loggerField;

            public Log4NetCategoryBuilder(Log4NetBackend parent, ModuleDeclaration module, string categoryName)
            {
                this.parent = parent;
                this.module = module;

                this.loggerField = this.parent.loggingImplementation.GetCategoryField(categoryName, this.parent.loggerType, writer =>
                {
                    writer.EmitInstructionString(OpCodeNumber.Ldstr, categoryName);
                    writer.EmitInstructionMethod(OpCodeNumber.Call, this.parent.categoryInitializerMethod);
                });
            }

            public bool SupportsIsEnabled
            {
                get { return true; }
            }

            public void EmitGetIsEnabled(InstructionWriter writer, LogLevel level)
            {
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);

                switch (level)
                {
                    case LogLevel.Trace:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsDebugEnabledMethod);
                        break;
                    case LogLevel.Info:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsInfoEnabledMethod);
                        break;
                    case LogLevel.Warning:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsWarnEnabledMethod);
                        break;
                    case LogLevel.Error:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsErrorEnabledMethod);
                        break;
                    case LogLevel.Fatal:
                        writer.EmitInstructionMethod(OpCodeNumber.Callvirt, this.parent.getIsFatalEnabledMethod);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("level");
                }
            }

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string messageFormattingString,
                                  int argumentsCount, LogLevel logLevel, Action<InstructionWriter> getExceptionAction,
                                  Action<int, InstructionWriter> loadArgumentAction)
            {
                bool createArgsArray = false;

                IMethod method;
                if (!this.parent.loggerMethods[logLevel].TryGetValue(argumentsCount, out method))
                {
                    createArgsArray = true;
                    switch (logLevel)
                    {
                        case LogLevel.Trace:
                            method = this.parent.writeDebugArrayMethod;
                            break;
                        case LogLevel.Info:
                            method = this.parent.writeInfoArrayMethod;
                            break;
                        case LogLevel.Warning:
                            method = this.parent.writeWarningArrayMethod;
                            break;
                        case LogLevel.Error:
                            method = this.parent.writeErrorArrayMethod;
                            break;
                        case LogLevel.Fatal:
                            method = this.parent.writeFatalArrayMethod;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("logLevel");
                    }
                }

                if (getExceptionAction != null)
                {
                    getExceptionAction(writer);
                }

                writer.EmitInstructionField(OpCodeNumber.Ldsfld, this.loggerField);
                
                writer.EmitInstructionString(OpCodeNumber.Ldstr, messageFormattingString);

                if (createArgsArray)
                {
                    writer.EmitInstructionInt32(OpCodeNumber.Ldc_I4, argumentsCount);
                    writer.EmitInstructionType(OpCodeNumber.Newarr,
                                               this.module.Cache.GetIntrinsicBoxedType(IntrinsicType.Object));
                }

                for (int i = 0; i < argumentsCount; i++)
                {
                    if (createArgsArray)
                    {
                        writer.EmitInstruction(OpCodeNumber.Dup);
                        writer.EmitInstructionInt32(OpCodeNumber.Ldc_I4, i);
                    }

                    if (loadArgumentAction != null)
                    {
                        loadArgumentAction(i, writer);
                    }

                    if (createArgsArray)
                    {
                        writer.EmitInstruction(OpCodeNumber.Stelem_Ref);
                    }
                }

                writer.EmitInstructionMethod(OpCodeNumber.Callvirt, method);
            }
        }
    }
}