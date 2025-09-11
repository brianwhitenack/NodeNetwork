using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

using NodeNetwork;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    public class ListInputViewModel<T> : ValueListNodeInputViewModel<T>
    {
        static ListInputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<ListInputViewModel<T>>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; }

        public ListInputViewModel(PortDataType type)
        {
            PartCalculationPort = new PartCalculationPortViewModel { PortType = type };
            this.Port = PartCalculationPort;

            // Set up connection validator to allow both single values and collections
            this.ConnectionValidator = pendingConnection =>
            {
                // First check if it's a ValueNode output (required by base class)
                bool isValueNode = pendingConnection.Output is ValueNodeOutputViewModel<T> ||
                                   pendingConnection.Output is ValueNodeOutputViewModel<IObservableList<T>>;
                
                if (!isValueNode)
                {
                    return new ConnectionValidationResult(false, "Output must be a value node");
                }

                // Then check our custom port type validation
                if (pendingConnection.Output?.Port is PartCalculationPortViewModel outputPort)
                {
                    return ConnectionValidationHelper.ValidateConnection(PartCalculationPort, outputPort);
                }
                
                // Default to allowing the connection if it's a value node
                return new ConnectionValidationResult(true, null);
            };
        }
    }
}
