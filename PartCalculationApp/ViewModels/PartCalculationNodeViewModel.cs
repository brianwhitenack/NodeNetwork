using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExampleCodeGenApp.Views;
using NodeNetwork.ViewModels;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class PartCalculationNodeViewModel : NodeViewModel
    {
        static PartCalculationNodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartCalculationNodeViewModel>));
        }

        public NodeType NodeType { get; }

        public PartCalculationNodeViewModel(NodeType type)
        {
            NodeType = type;
        }
    }
}
