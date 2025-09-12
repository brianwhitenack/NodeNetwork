using System;
using System.Reactive.Linq;

using ExampleCodeGenApp.ViewModels;

using NodeNetwork.ViewModels;
using NodeNetwork.Views;

using ReactiveUI;

namespace PartCalculationApp.ViewModels
{
    /// <summary>
    /// A generic output that can provide any single item type and updates its appearance accordingly.
    /// </summary>
    public class GenericOutputViewModel : NodeOutputViewModel
    {
        static GenericOutputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<GenericOutputViewModel>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; private set; }

        private IObservable<object> _value;
        public IObservable<object> Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public GenericOutputViewModel()
        {
            PartCalculationPort = new PartCalculationPortViewModel 
            { 
                PortType = PortDataType.Unknown 
            };
            Port = PartCalculationPort;

            Value = Observable.Return<object>(null);
        }

        public void UpdatePortType(PortDataType newType)
        {
            PartCalculationPort.PortType = newType;
        }
    }
}