using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using DynamicData;

using ExampleCodeGenApp.Views;
using NodeNetwork.ViewModels;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public enum NodeType
    {
        EventNode, Function, FlowControl, Literal, Group
    }

    public class CodeGenNodeViewModel : NodeViewModel
    {
       

        static CodeGenNodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<CodeGenNodeViewModel>));
        }

        public ExecutionContextOutputViewModel FlowIn { get; protected set; }
        public ExecutionContextInputViewModel FlowOut { get; protected set; }

        public NodeType NodeType { get; }

        public CodeGenNodeViewModel(NodeType type)
        {
            NodeType = type;

            if (type == NodeType.Function)
            {
                FlowIn = new ExecutionContextOutputViewModel();
                Inputs.Add(FlowIn);
                FlowOut = new ExecutionContextInputViewModel();
                Outputs.Add(FlowOut);
            }
            //else if (type == NodeType.EventNode)
            //{
            //    FlowOut = new ExecutionContextInputViewModel();
            //    Outputs.Add(FlowOut);
            //}
        }
    }
}
