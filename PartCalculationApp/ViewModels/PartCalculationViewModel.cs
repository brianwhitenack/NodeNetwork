using ExampleCodeGenApp.Views;

using NodeNetwork.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationViewModel : NodeViewModel
    {
        public NodeType NodeType { get; }

        public PartCalculationViewModel(NodeType type)
        {
            NodeType = type;
        }

        static PartCalculationViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartCalculationViewModel>));
        }
    }
}
