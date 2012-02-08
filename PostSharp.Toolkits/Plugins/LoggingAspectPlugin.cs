using PostSharp.Sdk.AspectWeaver;
using PostSharp.Toolkits.Logging;
using PostSharp.Toolkits.Weavers;

namespace PostSharp.Toolkits.Plugins
{
    public class LoggingAspectPlugIn : AspectWeaverPlugIn
    {
        public LoggingAspectPlugIn()
            : base(StandardPriorities.User)
        {
        }

        protected override void Initialize()
        {
            BindAspectWeaver<ILoggingToolkit, LoggingAspectWeaver>();
        }
    }
}