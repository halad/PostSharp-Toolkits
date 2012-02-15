using System;
using PostSharp.Extensibility;

namespace PostSharp.Toolkit.Diagnostics.Weaver.Logging
{
    public interface ILoggingBackendProvider : IService
    {
        ILoggingBackend GetBackend(string name);
    }
}