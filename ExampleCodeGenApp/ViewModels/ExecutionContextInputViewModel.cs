using System.Reactive.Linq;

using ExampleCodeGenApp.Model;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.ViewModels.Nodes;

using NodeNetwork.Views;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class ExecutionContextInputViewModel : CodeGenOutputViewModel<IStatement>
    {
        static ExecutionContextInputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<ExecutionContextInputViewModel>));
        }

        public ExecutionContextInputViewModel() : base(PortType.Execution)
        {
            Name = "In Flow";

            Value = Observable.Return(new StubStatement());
        }
    }

    public class ExecutionContextOutputViewModel : CodeGenInputViewModel<IStatement>
    {
        static ExecutionContextOutputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<ExecutionContextOutputViewModel>));
        }

        public ExecutionContextOutputViewModel() : base(PortType.Execution)
        {
            Name = "Out Flow";
        }
    }
}
