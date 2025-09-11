using System;
using System.Collections.Generic;
using System.Windows;

using ExampleCodeGenApp.ViewModels;

using PartCalculationApp.ViewModels;

namespace PartCalculationApp.Serialization
{
    public class NodeFactory
    {
        private readonly Dictionary<string, Func<PartCalculationViewModel>> _nodeFactories;

        public NodeFactory()
        {
            _nodeFactories = new Dictionary<string, Func<PartCalculationViewModel>>();
        }

        /// <summary>
        /// Registers a node type with the factory.
        /// </summary>
        public void RegisterNode<TNode>(string typeName) where TNode : PartCalculationViewModel, ISerializableNode, new()
        {
            _nodeFactories[typeName] = () => new TNode();
        }

        /// <summary>
        /// Registers a node type with a custom factory function.
        /// </summary>
        public void RegisterNode(string typeName, Func<PartCalculationViewModel> factory)
        {
            _nodeFactories[typeName] = factory;
        }

        /// <summary>
        /// Creates a node instance from serialized data.
        /// </summary>
        public PartCalculationViewModel CreateNode(string nodeType)
        {
            if (!_nodeFactories.TryGetValue(nodeType, out var factory))
            {
                throw new NotSupportedException($"Node type '{nodeType}' is not registered. Please register it using RegisterNode.");
            }

            return factory();
        }

        /// <summary>
        /// Registers all standard node types.
        /// </summary>
        public void RegisterStandardNodes()
        {
            // These would be called during application startup
            // Each node type that implements ISerializableNode can be registered
            // Example:
            // RegisterNode<StringLiteralNode>("StringLiteral");
            // RegisterNode<NumberLiteralNode>("NumberLiteral");
            // etc.
        }
    }
}