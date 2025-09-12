using System.Collections.Generic;

namespace ExampleCodeGenApp.Model
{
    public class Measurement
    {
        public string Type { get; set; }
        public double Length { get; set; }
        public double Area { get; set; }
        public int Count { get; set; }

        public Dictionary<string, object> Selections { get; set; } = new Dictionary<string, object>();
    }
}
