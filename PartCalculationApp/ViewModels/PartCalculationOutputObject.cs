using System.Reactive;

using PartCalculationApp.Model;
using PartCalculationApp.ViewModels.Nodes;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationOutputObject : ReactiveObject
    {
        #region Code

        public Measurement MeasurementInput
        {
            get => _measurementInput;
            set => this.RaiseAndSetIfChanged(ref _measurementInput, value);
        }
        private Measurement _measurementInput;

        public DigitizerMeasurementsNode InputNode { get; set; }
        public PartsOutputNode OutputNode { get; set; }
        #endregion

        #region Output
        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }
        private string _output;
        #endregion

        public ReactiveCommand<Unit, Unit> Calculate { get; }
        public ReactiveCommand<Unit, Unit> ClearOutput { get; }

        public PartCalculationOutputObject()
        {
            Calculate = ReactiveCommand.Create(() =>
                {
                    if (OutputNode?.PartsInput?.Values != null)
                    {
                        foreach (Part generatedPart in OutputNode.PartsInput.Values.Items)
                        {
                            if (generatedPart != null)
                            {
                                Print($"Generated Part:");
                                Print($"SKU: {generatedPart.Sku}");
                                Print($"Desc: {generatedPart.Description}");
                                Print($"Package: {generatedPart.Package}");
                                Print($"Qty: {generatedPart.Quantity}");
                                Print($"UOM: {generatedPart.UnitOfMeasure}");
                            }
                            else
                            {
                                Print("No part created");
                            }
                        }
                    }
                    else
                    {
                        Print("No parts created");
                    }
                });

            ClearOutput = ReactiveCommand.Create(() => { Output = ""; });
        }

        public void Print(string msg)
        {
            Output += msg + "\n";
        }
    }
}
