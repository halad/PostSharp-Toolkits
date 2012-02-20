using System;
using NUnit.Framework;
using PostSharp.Toolkit.Diagnostics;
using TestAssembly;

namespace PostSharp.Toolkit.Tests.NLog
{
    [TestFixture]
    public class LoggingToolkitForNLogTests : ConsoleTestsFixture
    {
        [Test]
        public void LoggingToolkit_Methods_LogsMethodEnterAndExit()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString();
            StringAssert.Contains("INFO|TestAssembly.SimpleClass|Entering: TestAssembly.SimpleClass/Method1()", output);
            StringAssert.Contains("INFO|TestAssembly.SimpleClass|Exiting: TestAssembly.SimpleClass/Method1()", output);
        }

        [Test]
        public void LoggingToolkit_Properties_LogsPropertyGetter()
        {
            SimpleClass s = new SimpleClass();
            string value = s.Property1;

            string output = OutputString.ToString();
            StringAssert.Contains("INFO|TestAssembly.SimpleClass|Entering: TestAssembly.SimpleClass/get_Property1()", output);
            StringAssert.Contains("INFO|TestAssembly.SimpleClass|Exiting: TestAssembly.SimpleClass/get_Property1()", output);
        }
        
        [Test]
        public void LoggingToolkit_Properties_LogsPropertySetter()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";

            string output = OutputString.ToString();
            StringAssert.Contains("INFO|TestAssembly.SimpleClass|Entering: TestAssembly.SimpleClass/set_Property1(string value)", output);
            StringAssert.Contains("INFO|TestAssembly.SimpleClass|Exiting: TestAssembly.SimpleClass/set_Property1(string value)", output);
        }
    }
}
