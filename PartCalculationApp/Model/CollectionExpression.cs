using System.Collections.Generic;

using ExampleCodeGenApp.Model.Compiler;

namespace PartCalculationApp.Model
{
    public class CollectionExpression<T> : ITypedExpression<IEnumerable<T>>
    {
        public string Compile(CompilerContext context)
        {
            return "Collection";
        }
    }
}
