using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.ValueNode;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class NumberLiteralNode : PartCalculationViewModel
    {
        static NumberLiteralNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<NumberLiteralNode>));
        }

        public DoubleValueEditorViewModel ValueEditor { get; } = new DoubleValueEditorViewModel();

        public ValueNodeOutputViewModel<double?> Output { get; }

        public NumberLiteralNode() : base(NodeType.Literal)
        {
            Name = "Number";

            ValueEditor = new DoubleValueEditorViewModel();

            Output = new OutputViewModel<double?>(PortDataType.Number)
            {
                Name = "Value",
                Editor = ValueEditor,
                Value = ValueEditor.ValueChanged.Select(v => v)
            };
            Outputs.Add(Output);
        }
    }
}
