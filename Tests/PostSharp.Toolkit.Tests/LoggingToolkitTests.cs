using System;
using NUnit.Framework;
using TestAssembly;

namespace PostSharp.Toolkit.Tests
{
    [TestFixture]
    public class LoggingToolkitTests : ConsoleTestsFixture
    {
        [Test]
        public void LoggingToolkit_Methods_LogsMethodEnterAndExit()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString();
            StringAssert.Contains("Entering: TestAssembly.SimpleClass/Method1() : void (None)", output);
            StringAssert.Contains("Exiting: TestAssembly.SimpleClass/Method1() : void (None)", output);
        }

        [Test]
        public void LoggingToolkit_Properties_LogsPropertyGetter()
        {
            SimpleClass s = new SimpleClass();
            string value = s.Property1;
            
            string output = OutputString.ToString();
            StringAssert.Contains("Entering: TestAssembly.SimpleClass/get_Property1() : string (None)", output);
            StringAssert.Contains("Exiting: TestAssembly.SimpleClass/get_Property1() : string (None)", output);
        }

        [Test]
        public void LoggingToolkit_Properties_LogsPropertySetter()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";
            
            string output = OutputString.ToString();
            StringAssert.Contains("Entering: TestAssembly.SimpleClass/set_Property1(string value) : void (None)", output);
            StringAssert.Contains("Exiting: TestAssembly.SimpleClass/set_Property1(string value) : void (None)", output);
        }

        [Test]
        public void LoggingToolkit_SimpleClassWithFields_LoggingNotAppliedToField()
        {
            SimpleClass s = new SimpleClass();
            s.Field1 = "Test";

            string output = OutputString.ToString();
            StringAssert.DoesNotContain("Field1", output);
        }

        [Test]
        public void LoggingToolkit_OnException_PrintsException()
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

        [Test]
        public void LoggingToolkit_MethodArguments_LogsMethodArgumentNames()
        {
            SimpleClass s = new SimpleClass();
            s.MethodWithArguments("TEST", 12345);

            string output = OutputString.ToString();
            Console.WriteLine(output);
        }
    }
}