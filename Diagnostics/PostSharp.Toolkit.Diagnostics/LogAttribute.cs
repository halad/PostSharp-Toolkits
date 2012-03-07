using System;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace PostSharp.Toolkit.Diagnostics
{
    [Serializable]
    [AttributeUsage(
      AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Struct,
      AllowMultiple = true,
      Inherited = false)]
    [MulticastAttributeUsage(
    MulticastTargets.InstanceConstructor | MulticastTargets.StaticConstructor | MulticastTargets.Method,
      AllowMultiple = true)]
    [Metric("UsedFeatures", "Toolkit.Diagnostics.Logging")]
    //[AspectConfigurationAttributeType(typeof(LogAspectConfigurationAttribute))]
    public class LogAttribute : MethodLevelAspect, ILogAspect, ILogAspectBuildSemantics
    {
    }
}