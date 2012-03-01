using System;
using System.Collections.Generic;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;
using PostSharp.Toolkit.Diagnostics.Weaver;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Console;
using PostSharp.Toolkit.Diagnostics.Weaver.Logging.Trace;

[assembly: AssemblyProjectProvider(typeof(InstrumentationProjectConfiguration))]

namespace PostSharp.Toolkit.Diagnostics.Weaver
{
    public class InstrumentationProjectConfiguration : IProjectConfigurationProvider
    {
        public ProjectConfiguration GetProjectConfiguration()
        {
            ProjectConfiguration projectConfiguration = new ProjectConfiguration
            {
                Properties = new PropertyConfigurationCollection
                {
                    new PropertyConfiguration("LoggingBackend", "trace") { Overwrite = false },
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
                    new ServiceConfiguration(project => new DiagnosticsBackendProvider()),
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
