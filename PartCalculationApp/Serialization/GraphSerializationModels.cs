using System;
using System.Collections.Generic;

using ExampleCodeGenApp.ViewModels;

using JsonSubTypes;
using Newtonsoft.Json;

namespace PartCalculationApp.Serialization
{
    public class SerializedGraph
    {
        public string Version { get; set; } = "1.0";
        public List<SerializedNode> Nodes { get; set; } = new List<SerializedNode>();
        public List<SerializedConnection> Connections { get; set; } = new List<SerializedConnection>();
        public GraphMetadata Metadata { get; set; } = new GraphMetadata();
    }

    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(SerializedStringLiteralNode), "StringLiteral")]
    [JsonSubtypes.KnownSubType(typeof(SerializedNumberLiteralNode), "NumberLiteral")]
    [JsonSubtypes.KnownSubType(typeof(SerializedStringSelectionNode), "StringSelection")]
    [JsonSubtypes.KnownSubType(typeof(SerializedNumberSelectionNode), "NumberSelection")]
    [JsonSubtypes.KnownSubType(typeof(SerializedMeasurementLengthNode), "MeasurementLength")]
    [JsonSubtypes.KnownSubType(typeof(SerializedCreatePartNode), "CreatePart")]
    [JsonSubtypes.KnownSubType(typeof(SerializedConcatenationNode), "Concatenation")]
    [JsonSubtypes.KnownSubType(typeof(SerializedToStringNode), "ToString")]
    [JsonSubtypes.KnownSubType(typeof(SerializedAddNode), "Add")]
    [JsonSubtypes.KnownSubType(typeof(SerializedSubtractNode), "Subtract")]
    [JsonSubtypes.KnownSubType(typeof(SerializedMultiplyNode), "Multiply")]
    [JsonSubtypes.KnownSubType(typeof(SerializedDivideNode), "Divide")]
    [JsonSubtypes.KnownSubType(typeof(SerializedDigitizerMeasurementsNode), "DigitizerMeasurements")]
    [JsonSubtypes.KnownSubType(typeof(SerializedPartsOutputNode), "PartsOutput")]
    [JsonSubtypes.KnownSubType(typeof(SerializedForeachNode), "ForEach")]
    public abstract class SerializedNode
    {
        public Guid Id { get; set; }
        public abstract string Type { get; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool CanBeRemovedByUser { get; set; } = true;
        public List<SerializedInputOutput> Inputs { get; set; } = new List<SerializedInputOutput>();
        public List<SerializedInputOutput> Outputs { get; set; } = new List<SerializedInputOutput>();
    }

    public class SerializedInputOutput
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public PortDataType DataType { get; set; }
        public object EnteredValue { get; set; }
        public int Index { get; set; }
    }

    public class SerializedConnection
    {
        public Guid OutputNodeId { get; set; }
        public Guid OutputPortId { get; set; }
        public Guid InputNodeId { get; set; }
        public Guid InputPortId { get; set; }
    }

    public class GraphMetadata
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public string Description { get; set; }
    }

    // Specific node type serialization classes
    public class SerializedStringLiteralNode : SerializedNode
    {
        public override string Type => "StringLiteral";
        public string Value { get; set; }
    }

    public class SerializedNumberLiteralNode : SerializedNode
    {
        public override string Type => "NumberLiteral";
        public double Value { get; set; }
    }

    public class SerializedStringSelectionNode : SerializedNode
    {
        public override string Type => "StringSelection";
        public string SelectionName { get; set; }
    }

    public class SerializedNumberSelectionNode : SerializedNode
    {
        public override string Type => "NumberSelection";
        public string SelectionName { get; set; }
    }

    public class SerializedMeasurementLengthNode : SerializedNode
    {
        public override string Type => "MeasurementLength";
    }

    public class SerializedCreatePartNode : SerializedNode
    {
        public override string Type => "CreatePart";
        public string PartTemplate { get; set; }
    }

    public class SerializedConcatenationNode : SerializedNode
    {
        public override string Type => "Concatenation";
    }

    public class SerializedToStringNode : SerializedNode
    {
        public override string Type => "ToString";
    }

    public class SerializedAddNode : SerializedNode
    {
        public override string Type => "Add";
    }

    public class SerializedSubtractNode : SerializedNode
    {
        public override string Type => "Subtract";
    }

    public class SerializedMultiplyNode : SerializedNode
    {
        public override string Type => "Multiply";
    }

    public class SerializedDivideNode : SerializedNode
    {
        public override string Type => "Divide";
    }

    public class SerializedDigitizerMeasurementsNode : SerializedNode
    {
        public override string Type => "DigitizerMeasurements";
        public Dictionary<string, object> MeasurementData { get; set; }
    }

    public class SerializedPartsOutputNode : SerializedNode
    {
        public override string Type => "PartsOutput";
    }

    public class SerializedForeachNode : SerializedNode
    {
        public override string Type => "ForEach";
    }
}