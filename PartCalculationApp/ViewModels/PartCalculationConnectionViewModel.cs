using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleCodeGenApp.Views;
using NodeNetwork.ViewModels;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationConnectionViewModel : ConnectionViewModel
    {
        static PartCalculationConnectionViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenConnectionView(), typeof(IViewFor<PartCalculationConnectionViewModel>));
        }

        public PartCalculationConnectionViewModel(NetworkViewModel parent, NodeInputViewModel input, NodeOutputViewModel output) : base(parent, input, output)
        {

        }
    }
}
