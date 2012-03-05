using System;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Aspects.Configuration;

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
        public virtual bool ShouldIncludeParameterValue(ParameterInfo parameter)
        {
            return true;
        }
    }
}