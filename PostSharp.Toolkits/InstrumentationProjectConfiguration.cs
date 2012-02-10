using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;
using PostSharp.Toolkit.Instrumentation.Weaver;
using PostSharp.Toolkit.Instrumentation.Weaver.Logging;
[assembly: AssemblyProjectProvider(typeof(InstrumentationProjectConfiguration))]
namespace PostSharp.Toolkit.Instrumentation.Weaver
{
    public class InstrumentationProjectConfiguration : IProjectConfigurationProvider
    {
        public ProjectConfiguration GetProjectConfiguration()
        {
            ProjectConfiguration projectConfiguration = new ProjectConfiguration
            {
                Properties = new PropertyConfigurationCollection
                {
                    new PropertyConfiguration("LoggingBackend", "Console") { Overwrite = false }
                },
                TaskTypes = new TaskTypeConfigurationCollection
                {
                    new TaskTypeConfiguration(InstrumentationPlugIn.Name, project => new InstrumentationPlugIn())
                    {
                        AutoInclude = true,
                        Dependencies = new DependencyConfigurationCollection
                        {
                            new DependencyConfiguration("AspectWeaver")
                        }
                    }
                },
                Services = new ServiceConfigurationCollection
                {
                    new ServiceConfiguration(project => new ConsoleLoggingBackendProvider())
                },
                TaskFactories = new Dictionary<string, CreateTaskDelegate>
                {
                    { InstrumentationPlugIn.Name, project => new InstrumentationPlugIn() }
                }
            };


            return projectConfiguration;
        }
    }
}
