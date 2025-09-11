using System;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Serialization;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class ToStringNode : PartCalculationViewModel
    {
        static ToStringNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ToStringNode>));
        }

        public InputViewModel<double?> Input { get; }

        public OutputViewModel<string> Output { get; }

        public ToStringNode() : base(NodeType.Function)
        {
            Name = "To String";

            Input = new InputViewModel<double?>(PortDataType.Number)
            {
                Name = "Input",
                Editor = new DoubleValueEditorViewModel()
            };
            Inputs.Add(Input);

            IObservable<string> resultValue = this.WhenAnyValue(
               vm => vm.Input.Value
            ).Select(_ => GetToString());

            Output = new OutputViewModel<string>(PortDataType.String)
            {
                Name = "Input as Text",
                Value = resultValue
            };
           
            Outputs.Add(Output);
        }

        private string GetToString()
        {
            if (Input.Value == null)
            {
                return string.Empty;
            }
            else
            {
                return Input.Value.ToString();
            }
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedToStringNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
