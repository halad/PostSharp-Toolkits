using System;
using NUnit.Framework;
using PostSharp.Toolkit.Tests.Data;

namespace PostSharp.Toolkit.Tests
{
    [TestFixture]
    public class NLogTests : ConsoleTestsFixture
    {

        [Test]
        public void UnderTest_Scenario_ExpectedResult()
        {
            ClassForNLog classForNLog = new ClassForNLog();
            classForNLog.InvokeAction();

            Assert.IsNotNull(OutputString);
        }
    }
}