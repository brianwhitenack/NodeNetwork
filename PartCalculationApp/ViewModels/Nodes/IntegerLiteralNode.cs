using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.ValueNode;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class IntegerLiteralNode : PartCalculationViewModel
    {
        static IntegerLiteralNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<IntegerLiteralNode>));
        }

        public IntegerValueEditorViewModel ValueEditor { get; } = new IntegerValueEditorViewModel();

        public ValueNodeOutputViewModel<int> Output { get; }

        public IntegerLiteralNode() : base(PartCalculationNodeType.Literal)
        {
            Name = "Integer";

            Output = new OutputViewModel<int>(PortDataType.Number)
            {
                Editor = ValueEditor,
                Value = ValueEditor.ValueChanged.Select(v => v ?? 0)
            };
            Outputs.Add(Output);
        }
    }
}
