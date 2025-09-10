using System.Linq;
using System.Reactive.Linq;

using PartCalculationApp.Model;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class MeasurementInputDisplayViewModel : ReactiveObject
    {
        public Measurement Measurement
        {
            get => _measurement;
            set => this.RaiseAndSetIfChanged(ref _measurement, value);
        }
        private Measurement _measurement;

        private readonly ObservableAsPropertyHelper<string> _measurementText;
        public string MeasurementText => _measurementText.Value;

        public MeasurementInputDisplayViewModel()
        {
            this.WhenAnyValue(vm => vm.Measurement).Where(c => c != null)
                .Select(c =>
                {
                    return "// Measurement Input Node\n" +
                           $"var measurement = new Measurement();\n" +
                           $"measurement.Area = {c.Area};\n" +
                           $"measurement.Length = {c.Length};\n" +
                           $"measurement.Count = {c.Count};\n" +
                           $"measurement.Type = \"{c.Type}\";\n" +
                           $"measurement.Selections = new Dictionary<string, object>() {{ {string.Join(", ", c.Selections.Select(s => $"{{ \"{s.Key}\", \"{s.Value}\" }}"))} }};\n";

                    //CompilerError = "";
                    //CompilerContext ctx = new CompilerContext();

                    //try
                    //{
                    //    return c.Compile(ctx);
                    //}
                    //catch (CompilerException e)
                    //{
                    //    string trace = string.Join("\n", ctx.VariablesScopesStack.Select(s => s.Identifier));
                    //    CompilerError = e.Message + "\nProblem is near:\n"+ trace;
                    //    return "";
                    //}
                })
                .ToProperty(this, vm => vm.MeasurementText, out _measurementText);
        }
    }
}
