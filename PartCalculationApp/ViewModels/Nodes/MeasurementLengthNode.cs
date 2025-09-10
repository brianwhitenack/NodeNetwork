using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class MeasurementLengthNode : PartCalculationViewModel
    {
        static MeasurementLengthNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<MeasurementLengthNode>));
        }

        public InputViewModel<Measurement> MeasurementInput { get; }

        public OutputViewModel<double?> MeasurementLengthOutput { get; }

        public MeasurementLengthNode() : base(NodeType.Function)
        {
            Name = "Length";

            MeasurementInput = new InputViewModel<Measurement>(PortDataType.Measurement)
            {
                Name = "Measurement",
            };

            Inputs.Add(MeasurementInput);

            System.IObservable<double?> selectedValue = this.WhenAnyValue(
                vm => vm.MeasurementInput.Value)
                .Select(_ => GetLength());

            MeasurementLengthOutput = new OutputViewModel<double?>(PortDataType.Number)
            {
                Name = "Length",
                Value = selectedValue
            };
            Outputs.Add(MeasurementLengthOutput);
        }

        private double? GetLength()
        {
            if (MeasurementInput.Value == null)
            {
                return null;
            }
            else
            {
                return MeasurementInput.Value.Length;
            }
        }
    }
}
