using System.Reactive;
using System.Reactive.Linq;

using PartCalculationApp.Model;
using PartCalculationApp.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationOutput : ReactiveObject
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

        public PartCalculationOutput()
        {
            Calculate = ReactiveCommand.Create(() =>
                {
                    if (OutputNode?.PartsInput?.Value != null)
                    {
                        string selectedValue = OutputNode.PartsInput.Value;
                        if (selectedValue != null)
                        {
                            Print($"Selected Value: {selectedValue}");
                        }
                        else
                        {
                            Print("No value selected");
                        }
                    }
                    else
                    {
                        Print("Output node not connected");
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
