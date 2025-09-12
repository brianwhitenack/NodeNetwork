using ExampleCodeGenApp.Model.Compiler;

namespace ExampleCodeGenApp.Model
{
    public class IntLiteral : ITypedVariable<int>
    {
        public int Value { get; set; }

        public int Evaluate()
        {
            return Value;
        }

        public void SetValue(int value)
        {
            Value = value;
        }
    }
}
