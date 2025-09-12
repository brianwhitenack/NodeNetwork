using System.Collections.Generic;

using ExampleCodeGenApp.Model.Compiler;

namespace ExampleCodeGenApp.Model
{
    public class StatementSequence : IStatement
    {
        public List<IStatement> Statements { get; } = new List<IStatement>();

        public StatementSequence(params IStatement[] statements)
        {
            if (statements != null && statements.Length > 0)
            {
                Statements.AddRange(statements);
            }
        }

        public StatementSequence(IEnumerable<IStatement> statements)
        {
            Statements.AddRange(statements);
        }

        public void Execute()
        {
            foreach (IStatement statement in Statements)
            {
                statement.Execute();
            }
        }
    }
}
