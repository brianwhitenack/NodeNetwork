using NodeNetwork.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationPortViewModel : PortViewModel
    {
        static PartCalculationPortViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new Views.CodeGenPortView(), typeof(IViewFor<PartCalculationPortViewModel>));
        }

        #region PortType
        public PortDataType PortType
        {
            get => _portType;
            set => this.RaiseAndSetIfChanged(ref _portType, value);
        }
        private PortDataType _portType;
        #endregion
    }
}
