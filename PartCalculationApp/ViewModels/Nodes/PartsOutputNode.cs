using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class PartsOutputNode : PartCalculationViewModel
    {
        static PartsOutputNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartsOutputNode>));
        }

        public InputViewModel<Part> PartsInput { get; set; }

        public PartsOutputNode() : base(NodeType.Output)
        {
            Name = "Parts";

            PartsInput = new InputViewModel<Part>(PortDataType.Part);

            Inputs.Add(PartsInput);
        }
    }
}
