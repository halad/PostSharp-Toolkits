using NUnit.Framework;
using TestAssembly;
using log4net.Config;

namespace PostSharp.Toolkit.Tests.Log4Net
{
    [TestFixture]
    public class ToolkitForLog4NetTests : ConsoleTestsFixture
    {
        [SetUp]
        public override void SetUp()
        {
            BasicConfigurator.Configure();
            base.SetUp();
        }

        [Test]
        public void Log4Net_Methods_LogsMethodEnterAndExit()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString();
            StringAssert.Contains("DEBUG TestAssembly.SimpleClass (null) - Entering: TestAssembly.SimpleClass.Method1()", output);
        }

        [Test]
        public void Log4Net_Properties_LogsPropertyGetter()
        {
            SimpleClass s = new SimpleClass();
            string value = s.Property1;

            string output = OutputString.ToString();
            StringAssert.Contains("DEBUG TestAssembly.SimpleClass (null) - Entering: TestAssembly.SimpleClass.get_Property1()", output);
        }

        [Test]
        public void Log4Net_Properties_LogsPropertySetter()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";

            string output = OutputString.ToString();
            StringAssert.Contains("DEBUG TestAssembly.SimpleClass (null) - Entering: TestAssembly.SimpleClass.set_Property1(string value = \"Test\")", output);
        }

        [Test]
        public void Log4Net_OnException_PrintsException()
        {
            SimpleClass s = new SimpleClass();
            try
            {
                s.MethodThrowsException();
            }
            catch { }

            string output = OutputString.ToString();
            StringAssert.Contains("An exception occurred:\nSystem.Exception", output);
        }
    }
}