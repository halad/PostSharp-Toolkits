using System;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace PostSharp.Toolkits.Logging
{
    [RequirePostSharp("LoggingAspectPlugin", "PostSharp.Toolkits")]
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class LoggingToolkit : MethodLevelAspect, ILoggingToolkit
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}