using System;

namespace PostSharp.Toolkit.Diagnostics
{
    [Flags]
    public enum LogParameters
    {
        None = 0,
        ParameterTypes = 1,
        ParameterNames = 2,
        ParameterValues = 4,
    }
}