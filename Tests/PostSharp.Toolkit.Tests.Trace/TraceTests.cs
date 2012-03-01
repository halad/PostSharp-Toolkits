using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public void UnderTest_Scenario_ExpectedResult()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();

            string output = OutputString.ToString().Trim();
        }
    }
}
