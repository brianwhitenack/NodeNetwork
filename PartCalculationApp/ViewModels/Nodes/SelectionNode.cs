using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class SelectionNode : PartCalculationViewModel
    {
        static SelectionNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<SelectionNode>));
        }

        public InputViewModel<string> SelectionNameInput { get; }
        public InputViewModel<Measurement> MeasurementInput { get; }

        public OutputViewModel<string> SelectionValueOutput { get; }

        public SelectionNode() : base(NodeType.Function)
        {
            Name = "Selection";

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

            System.IObservable<string> selectedValue = this.WhenAnyValue(
                vm => vm.SelectionNameInput.Value,
                vm => vm.MeasurementInput.Value)
                .Select(_ => GetSelectionValue());

            SelectionValueOutput = new OutputViewModel<string>(PortDataType.String)
            {
                Name = "Selection Value",
                Value = selectedValue
            };
            Outputs.Add(SelectionValueOutput);
        }

        private string GetSelectionValue()
        {
            if (SelectionNameInput.Value == null || MeasurementInput.Value == null)
            {
                return null;
            }

            if (MeasurementInput.Value.Selections.TryGetValue(SelectionNameInput.Value, out object value))
            {
                return value?.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
