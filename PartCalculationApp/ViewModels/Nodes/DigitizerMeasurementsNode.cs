using System;
using System.Collections.Generic;
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
    public class DigitizerMeasurementsNode : PartCalculationViewModel
    {
        public static List<Measurement> TestAllMeasurement { get; private set; } = GetAllMeasurements();

        static DigitizerMeasurementsNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<DigitizerMeasurementsNode>));
        }

        public InputViewModel<string> MeasurementType { get; set; }

        public OutputViewModel<List<Measurement>> MeasurementOutput { get; set; }

        public DigitizerMeasurementsNode() : base(NodeType.Input)
        {
            Name = "Digitizer Measurement";

            MeasurementType = new InputViewModel<string>(PortDataType.String)
            {
                Name = "Measurement Type",
                Editor = new StringValueEditorViewModel()
            };
            Inputs.Add(MeasurementType);

            IObservable<List<Measurement>> measurements = this.WhenAnyValue(
               vm => vm.MeasurementType.Value
            ).Select(_ => FilterMeasurements());

            MeasurementOutput = new OutputViewModel<List<Measurement>>(PortDataType.MeasurementCollection)
            {
                Name = "Filtered Measurements",
                Value = measurements
            };
            Outputs.Add(MeasurementOutput);
        }

        private List<Measurement> FilterMeasurements()
        {
            if (string.IsNullOrWhiteSpace(MeasurementType.Value))
            {
                return new List<Measurement>();
            }
            else
            {
                return TestAllMeasurement.FindAll(m => m.Type == MeasurementType.Value);
            }
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedDigitizerMeasurementsNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }

        private static List<Measurement> GetAllMeasurements()
        {
            return new List<Measurement>
            {
                new Measurement()
                {
                    Length = 50,
                    Count = 1,
                    Type = "Beam",
                    Selections = new Dictionary<string, object>()
                     {
                        { "Thickness", 2 },
                        { "Width", 4 },
                        { "Material", "LVL" },
                        { "BeamType", "HEADER" },
                        { "Plies", 3 },
                     }
                }
            };
        }
    }
}
