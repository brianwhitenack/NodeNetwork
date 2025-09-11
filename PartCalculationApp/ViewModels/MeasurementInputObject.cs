using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using PartCalculationApp.Model;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class MeasurementInputObject : ReactiveObject
    {
        public List<Measurement> Measurements
        {
            get => _measurements;
            set => this.RaiseAndSetIfChanged(ref _measurements, value);
        }
        private List<Measurement> _measurements;

        private readonly ObservableAsPropertyHelper<string> _measurementText;
        public string MeasurementText => _measurementText.Value;

        public MeasurementInputObject()
        {
            this.WhenAnyValue(vm => vm.Measurements).Where(c => c != null)
                .Select(c =>
                {
                    List<string> lines = new List<string>();
                    foreach (var measurement in c)
                    {
                        lines.AddRange(new List<string>()
                        {
                            $"Area = {measurement.Area};",
                            $"Length = {measurement.Length};",
                            $"Count = {measurement.Count};",
                            $"Type = \"{measurement.Type}\";",
                            $"Selections:"
                        });
                        lines.AddRange(measurement.Selections.Select(kv => $"  {kv.Key} = {kv.Value};"));
                    }

                    return string.Join("\n", lines);
                })
                .ToProperty(this, vm => vm.MeasurementText, out _measurementText);
        }
    }
}
