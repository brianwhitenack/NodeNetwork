using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using PartCalculationApp.Model;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class MeasurementInputObject : ReactiveObject
    {
        public Measurement Measurement
        {
            get => _measurement;
            set => this.RaiseAndSetIfChanged(ref _measurement, value);
        }
        private Measurement _measurement;

        private readonly ObservableAsPropertyHelper<string> _measurementText;
        public string MeasurementText => _measurementText.Value;

        public MeasurementInputObject()
        {
            this.WhenAnyValue(vm => vm.Measurement).Where(c => c != null)
                .Select(c =>
                {
                    List<string> lines = new List<string>()
                    {
                        $"Area = {c.Area};",
                        $"Length = {c.Length};",
                        $"Count = {c.Count};",
                        $"Type = \"{c.Type}\";",
                        $"Selections:"
                    };
                    lines.AddRange(c.Selections.Select(kv => $"  {kv.Key} = {kv.Value};"));

                    return string.Join("\n", lines);
                })
                .ToProperty(this, vm => vm.MeasurementText, out _measurementText);
        }
    }
}
