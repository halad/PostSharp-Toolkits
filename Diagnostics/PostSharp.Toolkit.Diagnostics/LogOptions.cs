using System;

namespace PostSharp.Toolkit.Diagnostics
{
    [Flags]
    public enum LogOptions
    {
        None = 0,
        IncludeParameterType = 1,
        IncludeParameterName = 2,
        IncludeParameterValue = 4,
        NotLogged = 128
    }

    
}