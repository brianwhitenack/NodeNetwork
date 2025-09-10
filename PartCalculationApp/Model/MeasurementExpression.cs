using System.Collections.Generic;

using ExampleCodeGenApp.Model.Compiler;

namespace PartCalculationApp.Model
{
    public class MeasurementExpression : ITypedExpression<Measurement>
    {
        public string Compile(CompilerContext context)
        {
            return "Measurement";
        }
    }

    public class Measurement
    {
        public string Type { get; set; }
        public double Length { get; set; }
        public double Area { get; set; }
        public int Count { get; set; }

        public Dictionary<string, object> Selections { get; set; } = new Dictionary<string, object>();
    }
}
