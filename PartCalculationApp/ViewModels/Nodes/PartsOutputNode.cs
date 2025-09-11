using System.Collections.Generic;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;
using PartCalculationApp.Serialization;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class PartsOutputNode : PartCalculationViewModel
    {
        static PartsOutputNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartsOutputNode>));
        }

        public ListInputViewModel<Part> PartsInput { get; set; }

        public PartsOutputNode() : base(NodeType.Output)
        {
            Name = "Parts";

            PartsInput = new ListInputViewModel<Part>(PortDataType.PartCollection)
            {
                Name = "Parts"
            };

            Inputs.Add(PartsInput);
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedPartsOutputNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
