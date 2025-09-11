using System;

using NodeNetwork;
using NodeNetwork.ViewModels;

namespace ExampleCodeGenApp.ViewModels
{
    public static class ConnectionValidationHelper
    {
        /// <summary>
        /// Validates if a connection between two ports is valid, including implicit conversions
        /// from single items to collections.
        /// </summary>
        public static ConnectionValidationResult ValidateConnection(
            PartCalculationPortViewModel inputPort, 
            PartCalculationPortViewModel outputPort)
        {
            if (inputPort == null || outputPort == null)
            {
                return new ConnectionValidationResult(false, "Invalid port");
            }

            var inputType = inputPort.PortType;
            var outputType = outputPort.PortType;

            // Direct match - types are identical
            if (inputType == outputType)
            {
                return new ConnectionValidationResult(true, null);
            }

            // Check if input is a collection and output is a single item of the same base type
            if (inputType.IsCollection() && !outputType.IsCollection())
            {
                // Check if the base types match (e.g., StringCollection accepts String)
                if (CanConvertSingleToCollection(outputType, inputType))
                {
                    return new ConnectionValidationResult(true, 
                        $"Single {outputType} will be wrapped into {inputType}");
                }
            }

            return new ConnectionValidationResult(false, 
                $"Cannot connect {outputType} to {inputType}");
        }

        /// <summary>
        /// Checks if a single item type can be converted to a collection type.
        /// </summary>
        private static bool CanConvertSingleToCollection(PortDataType singleType, PortDataType collectionType)
        {
            switch (collectionType)
            {
                case PortDataType.StringCollection:
                    return singleType == PortDataType.String;
                case PortDataType.NumberCollection:
                    return singleType == PortDataType.Number;
                case PortDataType.BooleanCollection:
                    return singleType == PortDataType.Boolean;
                case PortDataType.MeasurementCollection:
                    return singleType == PortDataType.Measurement;
                case PortDataType.PartCollection:
                    return singleType == PortDataType.Part;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the base type of a collection type.
        /// </summary>
        public static PortDataType GetBaseType(PortDataType collectionType)
        {
            if (!collectionType.IsCollection())
            {
                return collectionType;
            }

            // Remove the Collection flag to get the base type
            return collectionType & ~PortDataType.Collection;
        }
    }
}