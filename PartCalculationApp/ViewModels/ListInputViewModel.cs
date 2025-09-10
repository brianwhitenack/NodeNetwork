using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class ListInputViewModel<T> : ValueListNodeInputViewModel<T>
    {
        static ListInputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<ListInputViewModel<T>>));
        }

        public ListInputViewModel(PortDataType type)
        {
            this.Port = new PartCalculationPortViewModel { PortType = type };
        }
    }
}
