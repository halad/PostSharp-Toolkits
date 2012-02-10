using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace PostSharp.Toolkit.Instrumentation
{
    [RequirePostSharp("PostSharp.Toolkit.Instrumentation.Weaver", "PostSharp.Toolkit.Instrumentation")]
    public interface ILogAspect : IAspect
    {
    }
}