using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.Views;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class OutputViewModel<T> : ValueNodeOutputViewModel<T>
    {
        static OutputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<OutputViewModel<T>>));
        }

        public OutputViewModel(PortDataType type)
        {
            this.Port = new PartCalculationPortViewModel { PortType = type };
        }
    }
}
