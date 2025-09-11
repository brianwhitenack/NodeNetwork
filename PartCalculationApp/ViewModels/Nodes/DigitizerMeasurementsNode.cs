using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;
using PartCalculationApp.Serialization;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class DigitizerMeasurementsNode : PartCalculationViewModel
    {
        static DigitizerMeasurementsNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<DigitizerMeasurementsNode>));
        }

        public OutputViewModel<Measurement> MeasurementOutput { get; set; }

        public DigitizerMeasurementsNode() : base(NodeType.Input)
        {
            Name = "Digitizer Measurement";

            MeasurementOutput = new OutputViewModel<Measurement>(PortDataType.Measurement);

            Outputs.Add(MeasurementOutput);
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedDigitizerMeasurementsNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
