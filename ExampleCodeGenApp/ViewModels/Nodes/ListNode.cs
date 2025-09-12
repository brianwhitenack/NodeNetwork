using DynamicData;

using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.Views;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class ListLiteralNode : CodeGenNodeViewModel
    {
        static ListLiteralNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ListLiteralNode>));
        }

        public IntegerValueEditorViewModel ValueEditor { get; } = new IntegerValueEditorViewModel();

        public ValueNodeOutputViewModel<ITypedExpression<int>> Output { get; }

        public ListLiteralNode() : base(NodeType.Literal)
        {
            this.Name = "Integer";

            Output = new CodeGenOutputViewModel<ITypedExpression<int>>(PortType.Integer)
            {
                Editor = ValueEditor,
                Value = ValueEditor.ValueChanged.Select(v => new IntLiteral { Value = v ?? 0 })
            };
            this.Outputs.Add(Output);
        }
    }
}
