using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.CodeModel;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;

namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging
{
    class LetsTryBackend : ILoggingBackend
    {
        public void Initialize(ModuleDeclaration module)
        {
            throw new NotImplementedException();
        }

        public ILoggingBackendInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
        {
            throw new NotImplementedException();
        }

        private class LetsTryBackendInstance : ILoggingBackendInstance
        {
            private ITypeSignature categoryType;
            private IMethod initializerMethod;

            public ILoggingCategoryBuilder GetCategoryBuilder(string categoryName)
            {
                LoggingImplementationTypeBuilder b;
                var f = b.CreateLoggerField(categoryName, this.categoryType, writer =>
                {
                    writer.EmitInstructionString(OpCodeNumber.Ldstr, categoryName);
                    writer.EmitInstructionMethod(OpCodeNumber.Call, initializerMethod);
                });

                return new LetsTryCategoryBuilder(f);
            }
        }

        private class LetsTryCategoryBuilder : ILoggingCategoryBuilder
        {
            private FieldDefDeclaration categoryField;

            public LetsTryCategoryBuilder(FieldDefDeclaration categoryField)
            {
                this.categoryField = categoryField;
            }

        

            public bool SupportsIsEnabled
            {
                get { return true; }
            }

            public void EmitGetIsEnabled(InstructionWriter writer, LogLevel level)
            {
                writer.EmitInstructionField(OpCodeNumber.Ldsfld, categoryField);
               
            }

          

            public void EmitWrite(InstructionWriter writer, InstructionBlock block, string messageFormattingString, int argumentsCount, LogLevel logLevel, Action<InstructionWriter> getExceptionAction, Action<int, InstructionWriter> loadArgumentAction)
            {
                IMethod method;
                bool createArgsArray;
                switch (logLevel)
                {
                    case LogLevel.Trace:
                        switch (argumentsCount)
                        {
                            case 0:
                                method = writeLine0;
                                break;

                            case 1:
                                method = writeLine1;
                                break;

                            default:
                                method = writeLineArray;
                                createArgsArray = true;
                                break;
                        }
                        break;
                    case LogLevel.Info:
                        break;
                    case LogLevel.Warning:
                        break;
                    case LogLevel.Error:
                        break;
                    case LogLevel.Fatal:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel");
                }

                if (createArgsArray)
                {
                    // newarr
                }

                // ldstr message
                for (int i = 0; i < argumentsCount; i++)
                {
                    if (createArgsArray)
                    {
                        // dup
                        // ldc.i4 i
                    }
                    loadArgumentAction(i,  writer);

                    if (createArgsArray)
                    {

                        // stelemem 
                    }

                }

            }
        }


    }


}
