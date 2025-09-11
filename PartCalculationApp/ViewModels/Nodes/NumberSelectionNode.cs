using System;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;
using PartCalculationApp.Serialization;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class NumberSelectionNode : PartCalculationViewModel
    {
        static NumberSelectionNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<NumberSelectionNode>));
        }

        public InputViewModel<string> SelectionNameInput { get; }
        public InputViewModel<Measurement> MeasurementInput { get; }

        public OutputViewModel<double?> SelectionValueOutput { get; }

        public NumberSelectionNode() : base(NodeType.Function)
        {
            Name = "Number Selection";

            MeasurementInput = new InputViewModel<Measurement>(PortDataType.Measurement)
            {
                Name = "Measurement",
            };

            SelectionNameInput = new InputViewModel<string>(PortDataType.String)
            {
                Name = "Selection Name",
                Editor = new StringValueEditorViewModel()
            };

            Inputs.Add(MeasurementInput);
            Inputs.Add(SelectionNameInput);

            System.IObservable<double?> selectedValue = this.WhenAnyValue(
                vm => vm.SelectionNameInput.Value,
                vm => vm.MeasurementInput.Value)
                .Select(_ => GetSelectionValue());

            SelectionValueOutput = new OutputViewModel<double?>(PortDataType.Number)
            {
                Name = "Selection Value",
                Value = selectedValue
            };
            Outputs.Add(SelectionValueOutput);
        }

        private double? GetSelectionValue()
        {
            if (SelectionNameInput.Value == null || MeasurementInput.Value == null)
            {
                return null;
            }

            if (MeasurementInput.Value.Selections.TryGetValue(SelectionNameInput.Value, out object value) && 
                double.TryParse(value?.ToString(), out double result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedNumberSelectionNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
