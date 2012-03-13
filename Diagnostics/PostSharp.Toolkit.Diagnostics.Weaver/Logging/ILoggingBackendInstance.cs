using System;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendInstance
    {
        ILoggingCategoryBuilder GetCategoryBuilder(string categoryName);
        //void EmitWrite(InstructionWriter writer, InstructionBlock block, ILoggingCategoryBuilder category, string message, int argumentsCount, LogSeverity logLevel, Action<InstructionWriter> getExceptionAction, Action<int,InstructionWriter> loadArgumentAction );
        //void EmitWriteException(InstructionWriter writer, InstructionBlock block, string category, string message, ITypeSignature exceptionType, LogSeverity logLevel);
    }
}