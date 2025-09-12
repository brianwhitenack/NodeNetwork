using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleCodeGenApp.Model.Compiler;

namespace ExampleCodeGenApp.Model
{
    public class StringLiteral : ITypedVariable<string>
    {
        public string Value { get; set; }

        public string Evaluate()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Value = value;
        }
    }
}
