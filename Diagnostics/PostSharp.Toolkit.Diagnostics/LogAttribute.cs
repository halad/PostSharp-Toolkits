using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
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
    //[LogAspectConfiguration(OnEntryLogOptions = LogOptions.IncludeParameterName | LogOptions.IncludeParameterType | LogOptions.IncludeParameterValue, OnExitLogOption = LogOptions.None)]
    public class LogAttribute : MethodLevelAspect, ILogAspect, ILogAspectBuildSemantics
    {
#if !SMALL
        private LogOptions? onEntryOptions;
        public LogOptions OnEntryLogOptions
        {
            get { return this.onEntryOptions.GetValueOrDefault(LogOptions.None); }
            set { this.onEntryOptions = value; }
        }

        private LogOptions? onSuccessOptions;
        public LogOptions OnSuccessLogOptions
        {
            get { return this.onSuccessOptions.GetValueOrDefault(LogOptions.None); }
            set { this.onSuccessOptions = value; }
        }

        private LogOptions? onExceptionOptions;
        public LogOptions OnExceptionLogOptions
        {
            get { return this.onSuccessOptions.GetValueOrDefault(LogOptions.None); }
            set { this.onSuccessOptions = value; }
        }

        protected override AspectConfiguration CreateAspectConfiguration()
        {
            return new LogAspectConfiguration();
        }

        protected override void SetAspectConfiguration(AspectConfiguration aspectConfiguration, System.Reflection.MethodBase targetMethod)
        {
            LogAspectConfiguration configuration = (LogAspectConfiguration)aspectConfiguration;
            configuration.OnEntryOptions = this.onEntryOptions;
            configuration.OnSuccessOptions = this.onSuccessOptions;
            configuration.OnExceptionOptions = this.onExceptionOptions;
        }
#endif
    }

  
}