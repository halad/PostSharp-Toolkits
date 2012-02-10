using System;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace PostSharp.Toolkit.Instrumentation
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Assembly)]
    public sealed class LogAttribute : MethodLevelAspect, ILogAspect
    {
    }
}