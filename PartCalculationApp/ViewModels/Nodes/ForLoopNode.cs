//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using DynamicData;

//using ExampleCodeGenApp.Model;
//using ExampleCodeGenApp.Model.Compiler;
//using ExampleCodeGenApp.Views;

//using NodeNetwork.Toolkit.ValueNode;
//using NodeNetwork.ViewModels;

//using PartCalculationApp.ViewModels;

//using ReactiveUI;

//namespace ExampleCodeGenApp.ViewModels.Nodes
//{
//    public class ForLoopNode : PartCalculationViewModel
//    {
//        static ForLoopNode()
//        {
//            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ForLoopNode>));
//        }

//        public GenericListInputViewModel InputList { get; }
//        public GenericListOutputViewModel ResultList { get; }

//        public GenericOutputViewModel CurrentItem { get; }

//        public ValueListNodeInputViewModel<IStatement> LoopBodyFlow { get; }
//        public ValueListNodeInputViewModel<IStatement> LoopEndFlow { get; }

//        public ValueNodeInputViewModel<ITypedExpression<int>> FirstIndex { get; }
//        public ValueNodeInputViewModel<ITypedExpression<int>> LastIndex { get; }

//        public ValueNodeOutputViewModel<ITypedExpression<int>> CurrentIndex { get; }

//        public ForLoopNode() : base(NodeType.Loop)
//        {
//            EndpointGroup boundsGroup = new EndpointGroup("Bounds");

//            EndpointGroup controlFlowGroup = new EndpointGroup("Control Flow");

//            EndpointGroup controlFlowInputsGroup = new EndpointGroup(controlFlowGroup);

//            this.Name = "For Loop";

//            LoopBodyFlow = new ListInputViewModel<IStatement>(PortType.Execution)
//            {
//                Name = "Loop Body",
//                Group = controlFlowInputsGroup
//            };
//            this.Inputs.Add(LoopBodyFlow);

//            LoopEndFlow = new ListInputViewModel<IStatement>(PortType.Execution)
//            {
//                Name = "Loop End",
//                Group = controlFlowInputsGroup
//            };
//            this.Inputs.Add(LoopEndFlow);


//            FirstIndex = new InputViewModel<ITypedExpression<int>>(PortType.Number)
//            {
//                Name = "First Index",
//                Group = boundsGroup
//            };
//            this.Inputs.Add(FirstIndex);

//            LastIndex = new InputViewModel<ITypedExpression<int>>(PortType.Number)
//            {
//                Name = "Last Index",
//                Group = boundsGroup

//            };
//            this.Inputs.Add(LastIndex);

//            ForLoop value = new ForLoop();

//            var loopBodyChanged = LoopBodyFlow.Values.Connect().Select(_ => Unit.Default).StartWith(Unit.Default);
//            var loopEndChanged = LoopEndFlow.Values.Connect().Select(_ => Unit.Default).StartWith(Unit.Default);
//            InputList = new CodeGenOutputViewModel<IStatement>(PortType.Execution)
//            {
//                Name = "",
//                Value = Observable.CombineLatest(loopBodyChanged, loopEndChanged, FirstIndex.ValueChanged, LastIndex.ValueChanged,
//                        (bodyChange, endChange, firstI, lastI) => (BodyChange: bodyChange, EndChange: endChange, FirstI: firstI, LastI: lastI))
//                    .Select(v =>
//                    {
//                        value.LoopBody = new StatementSequence(LoopBodyFlow.Values.Items);
//                        value.LoopEnd = new StatementSequence(LoopEndFlow.Values.Items);
//                        value.LowerBound = v.FirstI ?? new IntLiteral { Value = 0 };
//                        value.UpperBound = v.LastI ?? new IntLiteral { Value = 1 };
//                        return value;
//                    }),
//                Group = controlFlowGroup
//            };
//            this.Outputs.Add(InputList);

//            CurrentIndex = new CodeGenOutputViewModel<ITypedExpression<int>>(PortType.Number)
//            {
//                Name = "Current Index",
//                Value = Observable.Return(new VariableReference<int> { LocalVariable = value.CurrentIndex })
//            };
//            this.Outputs.Add(CurrentIndex);
//        }
//    }
//}
