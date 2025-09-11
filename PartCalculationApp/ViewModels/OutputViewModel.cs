using System;

using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.Views;

using PartCalculationApp.Serialization;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class OutputViewModel<T> : ValueNodeOutputViewModel<T>, IInputOutputViewModel
    {
        static OutputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<OutputViewModel<T>>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; }

        public OutputViewModel(PortDataType type) : base()
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
