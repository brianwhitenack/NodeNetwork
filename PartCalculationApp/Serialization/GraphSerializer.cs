using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;

using Newtonsoft.Json;

using NodeNetwork.ViewModels;

using PartCalculationApp.Model;
using PartCalculationApp.ViewModels.Nodes;

namespace PartCalculationApp.Serialization
{
    public class GraphSerializer
    {
        private readonly NodeFactory _nodeFactory;
        private readonly JsonSerializerSettings _jsonSettings;

        public GraphSerializer(NodeFactory nodeFactory)
        {
            _nodeFactory = nodeFactory;
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Serializes a network to JSON string.
        /// </summary>
        public string SerializeToJson(NetworkViewModel network)
        {
            var graph = SerializeGraph(network);
            return JsonConvert.SerializeObject(graph, _jsonSettings);
        }

        /// <summary>
        /// Serializes a network to a file.
        /// </summary>
        public void SerializeToFile(NetworkViewModel network, string filePath)
        {
            var json = SerializeToJson(network);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Deserializes a network from JSON string.
        /// </summary>
        public NetworkViewModel DeserializeFromJson(string json)
        {
            var graph = JsonConvert.DeserializeObject<SerializedGraph>(json, _jsonSettings);
            return DeserializeGraph(graph);
        }

        /// <summary>
        /// Deserializes a network from a file.
        /// </summary>
        public NetworkViewModel DeserializeFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return DeserializeFromJson(json);
        }

        private SerializedGraph SerializeGraph(NetworkViewModel network)
        {
            SerializedGraph graph = new SerializedGraph();

            // Serialize nodes
            foreach (NodeViewModel node in network.Nodes.Items)
            {
                if (node is PartCalculationViewModel calcNode)
                {
                    SerializedNode serializedNode = calcNode.Serialize();

                    graph.Nodes.Add(serializedNode);
                }
            }

            // Serialize connections
            foreach (ConnectionViewModel connection in network.Connections.Items)
            {
                NodeOutputViewModel output = connection.Output;
                NodeInputViewModel input = connection.Input;
                NodeViewModel outputNode = output.Parent;
                NodeViewModel inputNode = input.Parent;

                graph.Connections.Add(new SerializedConnection
                {
                    OutputNodeId = outputNode.Id,
                    OutputPortId = output.Id,
                    InputNodeId = inputNode.Id,
                    InputPortId = input.Id
                });
            }

            return graph;
        }

        private NetworkViewModel DeserializeGraph(SerializedGraph graph)
        {
            NetworkViewModel network = new NetworkViewModel();

            // Phase 1: Create all nodes
            foreach (SerializedNode serializedNode in graph.Nodes)
            {
                PartCalculationViewModel node = _nodeFactory.CreateNode(serializedNode.Type);
                node.Deserialize(serializedNode);
                network.Nodes.Add(node);
            }

            //DigitizerMeasurementsNode measurementInputNode = network.Nodes.Items.OfType<DigitizerMeasurementsNode>().First();
            //measurementInputNode.MeasurementOutput.Value = Observable.Return(measurementInput);

            // Phase 2: Restore connections
            foreach (SerializedConnection serializedConnection in graph.Connections)
            {
                NodeViewModel outputNode = network.Nodes.Items.Single(node => node.Id == serializedConnection.OutputNodeId);
                NodeViewModel inputNode = network.Nodes.Items.Single(node => node.Id == serializedConnection.InputNodeId);
                NodeInputViewModel input = inputNode.Inputs.Items.Single(input => input.Id == serializedConnection.InputPortId);
                NodeOutputViewModel output = outputNode.Outputs.Items.Single(output => output.Id == serializedConnection.OutputPortId);

                ConnectionViewModel connection = network.ConnectionFactory(input, output);
                network.Connections.Add(connection);
            }

            return network;
        }
    }
}