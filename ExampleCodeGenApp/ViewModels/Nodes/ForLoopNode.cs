﻿using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.Model;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class ForLoopNode : CodeGenNodeViewModel
    {
        static ForLoopNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ForLoopNode>));
        }

        public ValueNodeOutputViewModel<IStatement> LoopIn { get; }

        public ValueListNodeInputViewModel<IStatement> LoopBodyFlow { get; }
        public ValueListNodeInputViewModel<IStatement> LoopEndFlow { get; }
        
        public ValueNodeInputViewModel<ITypedExpression<int>> FirstIndex { get; }
        public ValueNodeInputViewModel<ITypedExpression<int>> LastIndex { get; }

        public ValueNodeOutputViewModel<ITypedExpression<int>> CurrentIndex { get; }

        public ForLoopNode() : base(NodeType.FlowControl)
        {
            var boundsGroup = new EndpointGroup("Bounds");

            var controlFlowGroup = new EndpointGroup("Control Flow");

            var controlFlowInputsGroup = new EndpointGroup(controlFlowGroup);

            this.Name = "For Loop";
            
            LoopBodyFlow = new CodeGenListInputViewModel<IStatement>(PortType.Execution)
            {
                Name = "Loop Body",
                Group = controlFlowInputsGroup
            };
            this.Inputs.Add(LoopBodyFlow);

            LoopEndFlow = new CodeGenListInputViewModel<IStatement>(PortType.Execution)
            {
                Name = "Loop End",
                Group = controlFlowInputsGroup
            };
            this.Inputs.Add(LoopEndFlow);


            FirstIndex = new CodeGenInputViewModel<ITypedExpression<int>>(PortType.Integer)
            {
                Name = "First Index",
                Group = boundsGroup
            };
            this.Inputs.Add(FirstIndex);

            LastIndex = new CodeGenInputViewModel<ITypedExpression<int>>(PortType.Integer)
            {
                Name = "Last Index",
                Group = boundsGroup

            };
            this.Inputs.Add(LastIndex);

            ForLoop value = new ForLoop();

            var loopBodyChanged = LoopBodyFlow.Values.Connect().Select(_ => Unit.Default).StartWith(Unit.Default);
            var loopEndChanged = LoopEndFlow.Values.Connect().Select(_ => Unit.Default).StartWith(Unit.Default);
            LoopIn = new CodeGenOutputViewModel<IStatement>(PortType.Execution)
            {
                Name = "Flow In",
                Value = Observable.CombineLatest(loopBodyChanged, loopEndChanged, FirstIndex.ValueChanged, LastIndex.ValueChanged,
                        (bodyChange, endChange, firstI, lastI) => (BodyChange: bodyChange, EndChange: endChange, FirstI: firstI, LastI: lastI))
                    .Select(v => {
                        value.LoopBody = new StatementSequence(LoopBodyFlow.Values.Items);
                        value.LoopEnd = new StatementSequence(LoopEndFlow.Values.Items);
                        value.LowerBound = v.FirstI ?? new IntLiteral {Value = 0};
                        value.UpperBound = v.LastI ?? new IntLiteral {Value = 1};
                        return value; 
                    }),
                Group = controlFlowGroup
            };
            this.Outputs.Add(LoopIn);

            CurrentIndex = new CodeGenOutputViewModel<ITypedExpression<int>>(PortType.Integer)
            {
                Name = "Current Index",
                Value = Observable.Return(value.CurrentIndex)
            };
            this.Outputs.Add(CurrentIndex);
        }
    }
}
