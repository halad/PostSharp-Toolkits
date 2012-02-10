using System;
using System.Collections.Generic;
using PostSharp.Extensibility;

namespace PostSharp.Toolkit.Instrumentation.Weaver.Logging
{
    public interface ILoggingBackendProvider : IService
    {
        ILoggingBackend GetBackend(string name);
    }
}