using System;
using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.ValueNode;
using PartCalculationApp.Serialization;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class StringLiteralNode : PartCalculationViewModel
    {
        static StringLiteralNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<StringLiteralNode>));
        }

        public StringValueEditorViewModel ValueEditor { get; } = new StringValueEditorViewModel();

        public ValueNodeOutputViewModel<string> Output { get; }

        public StringLiteralNode() : base(NodeType.Literal)
        {
            this.Name = "String";

            Output = new OutputViewModel<string>(PortDataType.String)
            {
                Name = "Value",
                Editor = ValueEditor,
                Value = ValueEditor.ValueChanged.Select(v => v)
            };
            this.Outputs.Add(Output);
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedStringLiteralNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
