using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleCodeGenApp.Model.Compiler;

namespace ExampleCodeGenApp.Model
{
    public class ForLoop : IStatement
    {
        public IStatement LoopBody { get; set; }
        public IStatement LoopEnd { get; set; }

        public ITypedExpression<int> LowerBound { get; set; }
        public ITypedExpression<int> UpperBound { get; set; }

        public IntLiteral CurrentIndex { get; } = new IntLiteral();

        public void Execute()
        {
            for (int i = LowerBound.Evaluate(); i < UpperBound.Evaluate(); i++)
            {
                CurrentIndex.SetValue(i);
                LoopBody?.Execute();
            }

            LoopEnd?.Execute();
        }
    }
}
