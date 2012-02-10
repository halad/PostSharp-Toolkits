using System;
using PostSharp.Extensibility;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Toolkit.Instrumentation.Weaver.Logging;

namespace PostSharp.Toolkit.Instrumentation.Weaver
{
    internal class InstrumentationPlugIn : AspectWeaverPlugIn
    {
        public const string Name = "PostSharp.Toolkit.Instrumentation";

        private ILoggingBackend backend;

        public InstrumentationPlugIn()
            : base(StandardPriorities.User)
        {
        }

        public ILoggingBackend Backend
        {
            get
            {
                if (this.backend == null)
                {
                    this.InitializeBackend();
                }
                return this.backend;
            }
        }

        private void InitializeBackend()
        {
            string loggingBackendName = this.Project.Evaluate("{$LoggingBackend}", true);

            if (loggingBackendName == null)
            {
                return;
            }

            this.backend = this.GetBackend(loggingBackendName);

            if (this.backend == null)
            {
                InstrumentationMessageSource.Instance.Write(MessageLocation.Unknown, SeverityType.Fatal, "IN0001",
                                                            loggingBackendName);
                return;
            }

            this.backend.Initialize(this.Project.Module);
        }

        protected override void Initialize()
        {
            BindAspectWeaver<ILogAspect, LoggingAspectWeaver>();
        }

        private ILoggingBackend GetBackend(string loggingBackendName)
        {
            foreach (ILoggingBackendProvider provider in this.Project.GetServices<ILoggingBackendProvider>())
            {
                ILoggingBackend backend = provider.GetBackend(loggingBackendName);
                if (backend != null)
                {
                    return backend;
                }
            }

            return null;
        }
    }
}