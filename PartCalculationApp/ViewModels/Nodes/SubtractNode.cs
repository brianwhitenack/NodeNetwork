using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;

using NodeNetwork.Views;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class SubtractNode : PartCalculationViewModel
    {
        static SubtractNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<SubtractNode>));
        }

        public InputViewModel<double?> Input1 { get; }
        public InputViewModel<double?> Input2 { get; }

        public OutputViewModel<double?> Output { get; }

        public SubtractNode() : base(NodeType.Function)
        {
            Name = "Subtract";

            Input1 = new InputViewModel<double?>(PortDataType.Number)
            {
                Name = "A",
                Editor = new DoubleValueEditorViewModel()
            };
            Inputs.Add(Input1);

            Input2 = new InputViewModel<double?>(PortDataType.Number)
            {
                Name = "B",
                Editor = new DoubleValueEditorViewModel()
            };
            Inputs.Add(Input2);

            var sum = this.WhenAnyValue(vm => vm.Input1.Value, vm => vm.Input2.Value)
                .Select(_ => Input1.Value != null && Input2.Value != null ? Input1.Value - Input2.Value : null);

            Output = new OutputViewModel<double?>(PortDataType.Number)
            {
                Name = "A - B",
                Value = sum
            };
            Outputs.Add(Output);
        }
    }
}
