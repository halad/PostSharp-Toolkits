using System;
using System.Diagnostics;
using NUnit.Framework;
using TestAssembly;

namespace PostSharp.Toolkit.Tests.Trace
{
    [TestFixture]
    public class TraceTests : ConsoleTestsFixture
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(TextWriter));
        }

        [Test]
        public void Trace_Methods_LogsMethodEnterAndExit()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString();
            StringAssert.Contains("Entering: TestAssembly.SimpleClass.Method1()", output);
        }

        [Test]
        public void Trace_Properties_LogsPropertyGetter()
        {
            SimpleClass s = new SimpleClass();
            string value = s.Property1;

            string output = OutputString.ToString();
            StringAssert.Contains("Entering: TestAssembly.SimpleClass.get_Property1()", output);
        }

        [Test]
        public void Trace_Properties_LogsPropertySetter()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";

            string output = OutputString.ToString();
            StringAssert.Contains("Entering: TestAssembly.SimpleClass.set_Property1(string value = \"Test\")", output);
        }

        [Test]
        public void Trace_SimpleClassWithFields_LoggingNotAppliedToField()
        {
            SimpleClass s = new SimpleClass();
            s.Field1 = "Test";

            string output = OutputString.ToString();
            StringAssert.DoesNotContain("Field1", output);
        }

        [Test]
        public void Trace_OnException_PrintsException()
        {
            SimpleClass s = new SimpleClass();
            try
            {
                s.MethodThrowsException();
            }
            catch { }

            string output = OutputString.ToString();
            StringAssert.Contains("System.Exception: This is an exception", output);
        }

    }
}
