using System;

using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.Views;

using PartCalculationApp.Serialization;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class InputViewModel<T> : ValueNodeInputViewModel<T>, IInputOutputViewModel, ISerializableInputOutput
    {
        static InputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<InputViewModel<T>>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; }

        public InputViewModel(PortDataType type) : base(0)
        {
            PartCalculationPort = new PartCalculationPortViewModel
            {
                PortType = type
            };

            Port = PartCalculationPort;
        }

        public PortDataType GetPortDataType()
        {
            return PartCalculationPort.PortType;
        }

        public Guid GetId()
        {
            return Id;
        }

        public SerializedInputOutput Serialize()
        {
            return new SerializedInputOutput()
            {
                Id = Id,
                DataType = PartCalculationPort.PortType,
                EnteredValue = Editor?.GetValue(),
                Name = Name
            };
        }

        public void Deserialize(SerializedInputOutput data)
        {
            Id = data.Id;
            Editor?.SetValue(data.EnteredValue);
            Name = data.Name;
            // don't actually need to restore the type, as it baked in.
        }

        public string GetName()
        {
            return Name;
        }
    }
}
