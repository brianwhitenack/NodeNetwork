using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;

using ReactiveUI;

namespace PartCalculationApp.ViewModels
{
    public class DigitizerMeasurementsNode : PartCalculationViewModel
    {
        static DigitizerMeasurementsNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<DigitizerMeasurementsNode>));
        }

        public OutputViewModel<Measurement> MeasurementOutput { get; set; }

        public DigitizerMeasurementsNode() : base(PartCalculationNodeType.Input)
        {
            Name = "Digitizer Measurement";

            MeasurementOutput = new OutputViewModel<Measurement>(PortDataType.Measurement);

            Outputs.Add(MeasurementOutput);
        }
    }
}
