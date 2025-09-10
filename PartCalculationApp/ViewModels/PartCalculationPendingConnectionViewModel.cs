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
    public class PartCalculationPendingConnectionViewModel : PendingConnectionViewModel
    {
        static PartCalculationPendingConnectionViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenPendingConnectionView(), typeof(IViewFor<PartCalculationPendingConnectionViewModel>));
        }

        public PartCalculationPendingConnectionViewModel(NetworkViewModel parent) : base(parent)
        {

        }
    }
}
