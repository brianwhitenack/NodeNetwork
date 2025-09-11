using System;

using DynamicData;

using ExampleCodeGenApp.Views;

using NodeNetwork.ViewModels;

using PartCalculationApp.Serialization;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public abstract class PartCalculationViewModel : NodeViewModel, ISerializableNode
    {
        public NodeType NodeType { get; }

        public PartCalculationViewModel(NodeType type)
        {
            NodeType = type;
        }

        static PartCalculationViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PartCalculationViewModel>));
        }

        public SerializedNode Serialize()
        {
            SerializedNode serializedNode = InternalSerialize();

            serializedNode.Id = Id;
            serializedNode.Name = Name;
            serializedNode.X = Position.X;
            serializedNode.Y = Position.Y;
            serializedNode.CanBeRemovedByUser = CanBeRemovedByUser;

            foreach (NodeInputViewModel input in Inputs.Items)
            {
                if (input is IInputOutputViewModel partCalculationInput)
                {
                    SerializedInputOutput serializedInput = partCalculationInput.Serialize();
                    serializedNode.Inputs.Add(serializedInput);
                }
                else
                {
                    throw new InvalidOperationException($"Input {input.Name} does not implement {nameof(IInputOutputViewModel)}");
                }
            }

            foreach (NodeOutputViewModel output in Outputs.Items)
            {
                if (output is IInputOutputViewModel partCalculationOutput)
                {
                    SerializedInputOutput serializedOutput = partCalculationOutput.Serialize();
                    serializedNode.Outputs.Add(serializedOutput);
                }
                else
                {
                    throw new InvalidOperationException($"Output {output.Name} does not implement {nameof(IInputOutputViewModel)}");
                }
            }

            return serializedNode;
        }

        protected abstract SerializedNode InternalSerialize();

        public void Deserialize(SerializedNode data)
        {
            InternalDeserialize(data);

            Id = data.Id;
            Name = data.Name;
            Position = new System.Windows.Point(data.X, data.Y);
            CanBeRemovedByUser = data.CanBeRemovedByUser;

            foreach (NodeInputViewModel input in Inputs.Items)
            {
                if (input is IInputOutputViewModel partCalculationInput)
                {
                    SerializedInputOutput serializedInput = data.Inputs.Find(i => i.Name == partCalculationInput.GetName());
                    if (serializedInput != null)
                    {
                        partCalculationInput.Deserialize(serializedInput);
                    }
                    else
                    {
                        throw new InvalidOperationException($"No serialized data found for input {partCalculationInput.GetName()}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Input {input.Name} does not implement {nameof(IInputOutputViewModel)}");
                }
            }

            foreach (NodeOutputViewModel output in Outputs.Items)
            {
                if (output is IInputOutputViewModel partCalculationOutput)
                {
                    SerializedInputOutput serializedOutput = data.Outputs.Find(o => o.Name == partCalculationOutput.GetName());
                    if (serializedOutput != null)
                    {
                        partCalculationOutput.Deserialize(serializedOutput);
                    }
                    else
                    {
                        throw new InvalidOperationException($"No serialized data found for output {partCalculationOutput.GetName()}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Output {output.Name} does not implement {nameof(IInputOutputViewModel)}");
                }
            }
        }

        protected abstract void InternalDeserialize(SerializedNode data);
    }
}
