using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using PostSharp.Toolkit.Diagnostics;
using PostSharp.Toolkit.Tests.Data;

[assembly: Log(AttributeTargetTypes = "PostSharp.Toolkit.Tests.Data.SimpleClass")]

namespace PostSharp.Toolkit.Tests
{
    [TestFixture]
    public class LoggingToolkitTests : ConsoleTestsFixture
    {
        [Test]
        public void LoggingToolkit_AppliedToMethods_LogsEnteringAndExitingAMethod()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString();
            StringAssert.Contains("Entering: PostSharp.Toolkit.Tests.Data.SimpleClass/Method1() : void (None)", output);
            StringAssert.Contains("Exiting: PostSharp.Toolkit.Tests.Data.SimpleClass/Method1() : void (None)", output);
        }

        [Test]
        public void LoggingToolkit_AppliedToProperties_LogsGettingAndSettingProperties()
        {
            SimpleClass s = new SimpleClass();
            s.Property1 = "Test";
            string value = s.Property1;
            
            string output = OutputString.ToString();
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

            string output = OutputString.ToString().Trim();
            Assert.IsEmpty(output);
        }

        [Test]
        public void LoggingToolkit_AppliedAtAssemblyLevel_AppliedOnTargetTypes()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString().Trim();
            Assert.IsNotEmpty(output);
        }
    }
}