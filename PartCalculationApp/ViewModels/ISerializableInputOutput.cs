using PartCalculationApp.Serialization;

namespace ExampleCodeGenApp.ViewModels
{
    public interface ISerializableInputOutput
    {
        public SerializedInputOutput Serialize();

        public void Deserialize(SerializedInputOutput data);
    }
}
