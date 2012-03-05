using System;
using NLog;
using NUnit.Framework;
using PostSharp.Toolkit.Diagnostics;
using TestAssembly;

namespace PostSharp.Toolkit.Tests.NLog
{
    [TestFixture]
    public class LoggingToolkitForNLogTests : ConsoleTestsFixture
    {
        [Test]
        public void NLog_Methods_LogsMethodEnterAndExit()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString();
            StringAssert.Contains("TRACE|TestAssembly.SimpleClass|Entering: TestAssembly.SimpleClass/Method1()", output);
            StringAssert.Contains("TRACE|TestAssembly.SimpleClass|Exiting: TestAssembly.SimpleClass/Method1()", output);
        }

        [Test]
        public void NLog_Properties_LogsPropertyGetter()
        {
            SimpleClass s = new SimpleClass();
            string value = s.Property1;

            string output = OutputString.ToString();
            StringAssert.Contains("TRACE|TestAssembly.SimpleClass|Entering: TestAssembly.SimpleClass/get_Property1()",
                                  output);
            StringAssert.Contains("TRACE|TestAssembly.SimpleClass|Exiting: TestAssembly.SimpleClass/get_Property1()",
                                  output);
        }

        [Test]
        public void NLog_Properties_LogsPropertySetter()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";

            string output = OutputString.ToString();
            StringAssert.Contains(
                "TRACE|TestAssembly.SimpleClass|Entering: TestAssembly.SimpleClass/set_Property1(string value)", output);
            StringAssert.Contains(
                "TRACE|TestAssembly.SimpleClass|Exiting: TestAssembly.SimpleClass/set_Property1(string value)", output);
        }

        [Test, Ignore("Depends on the logger layout")]
        public void NLog_OnException_PrintsException()
        {
            SimpleClass s = new SimpleClass();
            try
            {
                s.MethodThrowsException();
            }
            catch
            {
            }

            string output = OutputString.ToString();
            StringAssert.Contains("System.Exception", output);
        }
    }
}
