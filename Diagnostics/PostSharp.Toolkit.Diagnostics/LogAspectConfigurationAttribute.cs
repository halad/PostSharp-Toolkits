using PostSharp.Aspects.Configuration;

namespace PostSharp.Toolkit.Diagnostics
{
    public class LogAspectConfigurationAttribute : AspectConfigurationAttribute
    {
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
            get { return this.onExceptionOptions.GetValueOrDefault(LogOptions.None); }
            set { this.onExceptionOptions = value; }
        }

        protected override void SetAspectConfiguration(AspectConfiguration aspectConfiguration)
        {
            base.SetAspectConfiguration(aspectConfiguration);

            LogAspectConfiguration configuration = (LogAspectConfiguration)aspectConfiguration;
            configuration.OnEntryOptions = this.onEntryOptions;
            configuration.OnSuccessOptions = this.onSuccessOptions;
            configuration.OnExceptionOptions = this.onExceptionOptions;
        }

        protected override AspectConfiguration CreateAspectConfiguration()
        {
            return new LogAspectConfiguration();
        }
    }
}