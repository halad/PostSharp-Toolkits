using System;
using NUnit.Framework;
using PostSharp.Toolkits.Logging;

namespace PostSharp.Toolkits.Tests
{
    public class SimpleClass
    {
        private int age;
        public int Age
        {
            get { return this.age; }
            [LoggingToolkit]
            set { this.age = value; }
        }


        public void Method1()
        {
            for (int i = 0; i < 10; i++)
            {
                Method2(i);
            }
        }

        private void Method2(int i)
        {
            Age = i;
            // do something with i
        }
    }


    [TestFixture]
    public class LoggingAspectTests
    {
        [Test]
        public void UnderTest_Scenario_ExpectedResult()
        {
            SimpleClass s = new SimpleClass();
            s.Method1();
        }
    }
}