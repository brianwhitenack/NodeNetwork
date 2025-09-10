using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.Views;

using ReactiveUI;

namespace PartCalculationApp.ViewModels
{
    public class PartsOutputNode : PartCalculationViewModel
    {
        static PartsOutputNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartsOutputNode>));
        }

        public InputViewModel<string> PartsInput { get; set; }

        public PartsOutputNode() : base(PartCalculationNodeType.Output)
        {
            Name = "Parts";

            PartsInput = new InputViewModel<string>(PortDataType.String);

            Inputs.Add(PartsInput);
        }
    }
}
