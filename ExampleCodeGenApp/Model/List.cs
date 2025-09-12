using System.Collections.Generic;

using ExampleCodeGenApp.Model.Compiler;

namespace ExampleCodeGenApp.Model
{
    public class List<T> : ITypedVariable<IEnumerable<T>>
    {
        public System.Collections.Generic.List<T> Values { get; set; } = new System.Collections.Generic.List<T>();

        public IEnumerable<T> Evaluate()
        {
            return Values;
        }

        public void SetValue(IEnumerable<T> value)
        {
            Values.Clear();
            if (value != null)
            {
                Values.AddRange(value);
            }
        }
    }
}
