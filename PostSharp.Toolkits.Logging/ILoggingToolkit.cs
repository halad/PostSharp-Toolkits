using PostSharp.Aspects;

namespace PostSharp.Toolkits.Logging
{
    public interface ILoggingToolkit : IAspect
    {
        void WriteLine(string message);
    }
}