using System;

namespace ExampleCodeGenApp.ViewModels
{
    public interface IInputOutputViewModel : ISerializableInputOutput
    {
        public Guid GetId();

        public PortDataType GetPortDataType();

        public string GetName();
    }
}
