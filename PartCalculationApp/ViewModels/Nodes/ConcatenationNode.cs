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
    public class ConcatenationNode : PartCalculationViewModel
    {
        static ConcatenationNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ConcatenationNode>));
        }

        public InputViewModel<string> Separator { get; }
        public InputViewModel<string> String1 { get; }
        public InputViewModel<string> String2 { get; }

        public OutputViewModel<string> Result { get; }

        public ConcatenationNode() : base(NodeType.Function)
        {
            Name = "Concatenate";

            Separator = new InputViewModel<string>(PortDataType.String)
            {
                Name = "Separator",
                Editor = new StringValueEditorViewModel()
                {
                    Value = " "
                }
            };

            String1 = new InputViewModel<string>(PortDataType.String)
            {
                Name = "String 1",
                Editor = new StringValueEditorViewModel()
            };

            String2 = new InputViewModel<string>(PortDataType.String)
            {
                Name = "String 2",
                Editor = new StringValueEditorViewModel()
            };

            Inputs.Add(Separator);
            Inputs.Add(String1);
            Inputs.Add(String2);

            IObservable<string> resultValue = this.WhenAnyValue(
                vm => vm.Separator.Value,
                vm => vm.String1.Value,
                vm => vm.String2.Value)
                .Select(_ => GetConcatenatedString());

            Result = new OutputViewModel<string>(PortDataType.String)
            {
                Name = "Result",
                Value = resultValue
            };
            Outputs.Add(Result);
        }

        private string GetConcatenatedString()
        {
            string sep = Separator.Value ?? " ";
            string str1 = String1.Value ?? "";
            string str2 = String2.Value ?? "";
            return $"{str1}{sep}{str2}";
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedConcatenationNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
