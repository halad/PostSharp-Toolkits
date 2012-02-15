using System;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;
using PostSharp.Toolkit.Diagnostics.Weaver.NLog;
using PostSharp.Toolkit.Diagnostics.Weaver.NLog.Logging;

[assembly: AssemblyProjectProvider(typeof(NLogProjectConfiguration))]
namespace PostSharp.Toolkit.Diagnostics.Weaver.NLog
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
