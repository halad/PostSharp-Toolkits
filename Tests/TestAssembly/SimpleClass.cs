using System;

namespace TestAssembly
{
    public class SimpleClass
    {
        public string Field1;

        public string Property1 { get; set; }

        public void Method1() { }

        public void MethodThrowsInvalidOperationException()
        {
            throw new InvalidOperationException();
        }
    }
}