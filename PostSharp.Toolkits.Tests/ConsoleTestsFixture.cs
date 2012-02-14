using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace PostSharp.Toolkit.Tests
{
    public class ConsoleTestsFixture
    {
        private StringWriter textWriter;
        private StringBuilder outputString;

        public StringWriter TextWriter
        {
            get { return this.textWriter; }
        }

        public StringBuilder OutputString
        {
            get { return this.outputString; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.outputString = new StringBuilder();
            this.textWriter = new StringWriter(this.OutputString);
            Console.SetOut(this.TextWriter);
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (this.TextWriter != null)
            {
                this.TextWriter.Dispose();
            }
        }
    }
}