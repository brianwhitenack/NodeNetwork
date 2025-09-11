namespace PartCalculationApp.Serialization
{
    public interface ISerializableNode
    {
        /// <summary>
        /// Serializes the node's specific properties to a SerializedNode object.
        /// Base properties (Name, Position, etc.) are handled by the serializer.
        /// </summary>
        SerializedNode Serialize();

        /// <summary>
        /// Deserializes the node's specific properties from a SerializedNode object.
        /// Base properties (Name, Position, etc.) are handled by the deserializer.
        /// </summary>
        void Deserialize(SerializedNode data);
    }
}