using System;
using PostSharp.Aspects.Configuration;

namespace PostSharp.Toolkit.Diagnostics
{
    public class LogAspectConfiguration : AspectConfiguration
    {
        public LogParameters? OnEntryParameter { get; set; }
        public LogParameters? OnExitParameter { get; set; }
    }

    public class MyLogAttribute : LogAttribute
    {
        public MyLogAttribute(LogParameters onEntryParameters = LogParameters.None, LogParameters onExitParameters = LogParameters.None)
        {
            this.OnEntryParameter = onEntryParameters;
            this.OnExitParameter = onExitParameters;
        }
        
#if !SMALL
        private LogParameters? onEntryParameter;
        private LogParameters OnEntryParameter
        {
            get { return this.onEntryParameter.GetValueOrDefault(LogParameters.None); }
            set { this.onEntryParameter = value; }
        }

        private LogParameters? onExitParameter;
        private LogParameters OnExitParameter
        {
            get { return this.onExitParameter.GetValueOrDefault(LogParameters.None); }
            set { this.onExitParameter = value; }
        }

        protected override AspectConfiguration CreateAspectConfiguration()
        {
            return new LogAspectConfiguration();
        }

        protected override void SetAspectConfiguration(AspectConfiguration aspectConfiguration, System.Reflection.MethodBase targetMethod)
        {
            LogAspectConfiguration configuration = (LogAspectConfiguration)aspectConfiguration;
            configuration.OnEntryParameter = this.onEntryParameter;
            configuration.OnExitParameter = this.onExitParameter;
        }
#endif
    }
}
