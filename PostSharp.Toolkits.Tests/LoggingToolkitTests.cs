using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using PostSharp.Toolkit.Instrumentation;
using PostSharp.Toolkit.Tests.Data;

[assembly: Log(AttributeTargetTypes = "PostSharp.Toolkit.Tests.Data.*")]

namespace PostSharp.Toolkit.Tests
{
    [TestFixture]
    public class LoggingToolkitTests
    {
        private StringWriter textWriter;
        private StringBuilder outputString;

        [SetUp]
        public void SetUp()
        {
            this.outputString = new StringBuilder();
            this.textWriter = new StringWriter(this.outputString);
            Console.SetOut(this.textWriter);

            Console.WriteLine();
        }

        [TearDown]
        public void TearDown()
        {
            if (this.textWriter != null)
            {
                this.textWriter.Dispose();
            }
        }

        [Test]
        public void LoggingToolkit_AppliedToMethods_LogsEnteringAndExitingAMethod()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = this.outputString.ToString();
            StringAssert.Contains("Entering: PostSharp.Toolkit.Tests.Data.SimpleClass/Method1() : void (None)", output);
            StringAssert.Contains("Exiting: PostSharp.Toolkit.Tests.Data.SimpleClass/Method1() : void (None)", output);
        }

        [Test]
        public void LoggingToolkit_AppliedToProperties_LogsGettingAndSettingProperties()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";
            string value = s.Property1;
            
            string output = this.outputString.ToString();
            StringAssert.Contains("Entering: PostSharp.Toolkit.Tests.Data.SimpleClass/set_Property1(string value) : void (None)", output);
            StringAssert.Contains("Exiting: PostSharp.Toolkit.Tests.Data.SimpleClass/set_Property1(string value) : void (None)", output);
            StringAssert.Contains("Entering: PostSharp.Toolkit.Tests.Data.SimpleClass/get_Property1() : string (None)", output);
            StringAssert.Contains("Exiting: PostSharp.Toolkit.Tests.Data.SimpleClass/get_Property1() : string (None)", output);
        }

        [Test]
        public void LoggingToolkit_SimpleClassWithFields_LoggingNotAppliedToField()
        {
            SimpleClass s = new SimpleClass();
            s.Field1 = "Test";

            string output = this.outputString.ToString().Trim();
            Assert.IsEmpty(output);
        }

        [Test]
        public void LoggingToolkit_AppliedAtAssemblyLevel_AppliedOnTargetTypes()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = this.outputString.ToString().Trim();
            Assert.IsNotEmpty(output);
        }
    }
}