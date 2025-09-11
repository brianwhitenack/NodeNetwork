using System;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;

using NodeNetwork.Views;

using PartCalculationApp.Serialization;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class AddNode : PartCalculationViewModel
    {
        static AddNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<AddNode>));
        }

        public InputViewModel<double?> Input1 { get; }
        public InputViewModel<double?> Input2 { get; }

        public OutputViewModel<double?> Output { get; }

        public AddNode() : base(NodeType.Function)
        {
            Name = "Add";

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
                .Select(_ => Input1.Value != null && Input2.Value != null ? Input1.Value + Input2.Value : null);

            Output = new OutputViewModel<double?>(PortDataType.Number)
            {
                Name = "A + B",
                Value = sum
            };
            Outputs.Add(Output);
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedAddNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
