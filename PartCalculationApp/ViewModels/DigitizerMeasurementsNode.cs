using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.ValueNode;

using PartCalculationApp.Model;

using ReactiveUI;

namespace PartCalculationApp.ViewModels
{
    public class DigitizerMeasurementsNode : CodeGenNodeViewModel
    {
        static DigitizerMeasurementsNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<DigitizerMeasurementsNode>));
        }

        public CodeGenOutputViewModel<MeasurementExpression> MeasurementOutput { get; set; }

        public DigitizerMeasurementsNode() : base(NodeType.Literal)
        {
            Name = "Digitizer Measurement";

            MeasurementOutput = new CodeGenOutputViewModel<MeasurementExpression>(PortType.Measurement);

            Outputs.Add(MeasurementOutput);
        }
    }

    public class SelectionNode : CodeGenNodeViewModel
    {
        static SelectionNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<SelectionNode>));
        }

        public CodeGenInputViewModel<string> SelectionNameInput { get; }
        public CodeGenInputViewModel<Measurement> MeasurementInput { get; }

        public CodeGenOutputViewModel<string> SelectionValueOutput { get; }

        public SelectionNode() : base(NodeType.Function)
        {
            Name = "Selection";

            MeasurementInput = new CodeGenInputViewModel<Measurement>(PortType.Measurement)
            {
                Name = "Measurement",
            };

            SelectionNameInput = new CodeGenInputViewModel<string>(PortType.String)
            {
                Name = "Selection Name",
                Editor = new StringValueEditorViewModel()
            };

            Inputs.Add(MeasurementInput);
            Inputs.Add(SelectionNameInput);

            System.IObservable<string> selectedValue = this.WhenAnyValue(
                vm => vm.SelectionNameInput.Value,
                vm => vm.MeasurementInput.Value)
                .Select(GetSelectionValue);

            SelectionValueOutput = new CodeGenOutputViewModel<string>(PortType.String)
            {
                Name = "Selection Value",
                Value = selectedValue
            };
            Outputs.Add(SelectionValueOutput);
        }

        private string GetSelectionValue((string selectionName, Measurement measurement) _)
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
