using PostSharp.Aspects.Configuration;

namespace PostSharp.Toolkit.Diagnostics
{
    public class LogAspectConfigurationAttribute : AspectConfigurationAttribute
    {
        private LogParameters? onEntryParameter;
        public LogParameters OnEntryLogParameter
        {
            get { return this.onEntryParameter.GetValueOrDefault(LogParameters.None); }
            set { this.onEntryParameter = value; }
        }

        private LogParameters? onExitParameter;
        public LogParameters OnExitLogParameter
        {
            get { return this.onExitParameter.GetValueOrDefault(LogParameters.None); }
            set { this.onExitParameter = value; }
        }

        protected override void SetAspectConfiguration(AspectConfiguration aspectConfiguration)
        {
            base.SetAspectConfiguration(aspectConfiguration);

            LogAspectConfiguration configuration = (LogAspectConfiguration)aspectConfiguration;
            configuration.OnEntryParameter = this.onEntryParameter;
            configuration.OnEntryParameter = this.onExitParameter;
        }

        protected override AspectConfiguration CreateAspectConfiguration()
        {
            return new LogAspectConfiguration();
        }
    }
}