using System;
using PostSharp.Aspects.Configuration;

namespace PostSharp.Toolkit.Diagnostics
{
    public class LogAspectConfiguration : AspectConfiguration
    {
        public int? Foo { get; set; }

        public LogLevels? OnEntryLevel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    [Flags]
    public enum LogLevels
    {
        None,
        MethodSignature,
        MethodParameters,
        ThisArgument,
        LocalVariables
    }

    //   [LogAspectConfiguration(Foo = 52)]
    public class MyLogAttribute : LogAttribute
    {
        public MyLogAttribute(LogLevels onEntryLevel)
        {
            OnEntryLevel = onEntryLevel;
        }

#if !SMALL

        private LogLevels? onEntryLevel;
        private LogLevels OnEntryLevel
        {
            get { return this.onEntryLevel.GetValueOrDefault(LogLevels.None); }
            set { this.onEntryLevel = value; }
        }

        protected override AspectConfiguration CreateAspectConfiguration()
        {
            return new LogAspectConfiguration();
        }
        protected override void SetAspectConfiguration(AspectConfiguration aspectConfiguration, System.Reflection.MethodBase targetMethod)
        {
            LogAspectConfiguration configuration = (LogAspectConfiguration)aspectConfiguration;
            configuration.Foo = targetMethod.GetParameters().Length > 0 ? 45 : 42;
            configuration.OnEntryLevel = this.onEntryLevel;
        }
#endif
    }


    public class LogAspectConfigurationAttribute : AspectConfigurationAttribute
    {
        private int? foo;

        public int Foo
        {
            get { return this.foo.GetValueOrDefault(); }
            set { this.foo = value; }
        }

        protected override void SetAspectConfiguration(AspectConfiguration aspectConfiguration)
        {
            base.SetAspectConfiguration(aspectConfiguration);

            LogAspectConfiguration configuration = (LogAspectConfiguration)aspectConfiguration;
            configuration.Foo = this.foo;
        }

        protected override AspectConfiguration CreateAspectConfiguration()
        {
            return new LogAspectConfiguration();
        }
    }
}
