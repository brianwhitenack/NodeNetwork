using ExampleCodeGenApp.Views;

using NodeNetwork.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationViewModel : NodeViewModel
    {
        public PartCalculationNodeType NodeType { get; }

        public PartCalculationViewModel(PartCalculationNodeType type)
        {
            NodeType = type;
        }

        static PartCalculationViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartCalculationViewModel>));
        }
    }
}
