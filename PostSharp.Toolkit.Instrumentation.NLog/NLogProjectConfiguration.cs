using System;
using System.Collections.Generic;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;
using PostSharp.Toolkit.Instrumentation.Weaver;
using PostSharp.Toolkit.Instrumentation.Weaver.Logging;
using PostSharp.Toolkit.Instrumentation.Weaver.NLog;
using PostSharp.Toolkit.Instrumentation.Weaver.NLog.Logging;

[assembly: AssemblyProjectProvider(typeof(NLogProjectConfiguration))]
namespace PostSharp.Toolkit.Instrumentation.Weaver.NLog
{
    public class NLogProjectConfiguration : IProjectConfigurationProvider
    {
        public ProjectConfiguration GetProjectConfiguration()
        {
            ProjectConfiguration projectConfiguration = new ProjectConfiguration
            {
                Services = new ServiceConfigurationCollection
                {
                    new ServiceConfiguration(project => new NLogBackendProvider())
                },
            };

            return projectConfiguration;
        }
    }
}
