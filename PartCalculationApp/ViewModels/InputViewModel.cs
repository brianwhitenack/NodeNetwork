using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.Views;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class InputViewModel<T> : ValueNodeInputViewModel<T>
    {
        static InputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<InputViewModel<T>>));
        }

        public InputViewModel(PortDataType type)
        {
            this.Port = new PartCalculationPortViewModel { PortType = type };
        }
    }
}
