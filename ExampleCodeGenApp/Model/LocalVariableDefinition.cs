using System;

namespace ExampleCodeGenApp.Model
{
    public class LocalVariableDefinition<T> : ITypedVariableDefinition<T>
    {
        public string VariableName { get; private set; }
        public string Value { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
