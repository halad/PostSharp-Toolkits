using System;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;
using PostSharp.Toolkit.Diagnostics.Weaver.Log4Net;
using PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.Logging;

[assembly: AssemblyProjectProvider(typeof(Log4NetProjectConfiguration))]

namespace PostSharp.Toolkit.Diagnostics.Weaver.Log4Net
{
    public class Log4NetProjectConfiguration : IProjectConfigurationProvider
    {
        public ProjectConfiguration GetProjectConfiguration()
        {
            ProjectConfiguration projectConfiguration = new ProjectConfiguration
            {
                Services = new ServiceConfigurationCollection
                {
                    new ServiceConfiguration(project => new Log4NetBackendProvider())
                },
            };

            return projectConfiguration;
        }
    }
}
