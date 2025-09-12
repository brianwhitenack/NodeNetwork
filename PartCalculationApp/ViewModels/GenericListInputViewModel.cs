using System;
using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;

using NodeNetwork;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;

using ReactiveUI;

namespace PartCalculationApp.ViewModels
{
    // Helper class to handle typed observable subscriptions
    internal class ListItemObserver<T> : IObserver<T>
    {
        private readonly SourceList<object> _sourceList;

        public ListItemObserver(SourceList<object> sourceList)
        {
            _sourceList = sourceList;
        }

        public void OnNext(T value)
        {
            _sourceList.Clear();
            if (value != null)
            {
                // Check if it's an IObservableList<T>
                var itemsProperty = value.GetType().GetProperty("Items");
                if (itemsProperty != null)
                {
                    var items = itemsProperty.GetValue(value);
                    if (items is System.Collections.IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            _sourceList.Add(item);
                        }
                    }
                }
                // Check if it's directly enumerable (IList<T>, array, etc.)
                else if (value is System.Collections.IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        _sourceList.Add(item);
                    }
                }
                // Single item
                else
                {
                    _sourceList.Add(value);
                }
            }
        }

        public void OnError(Exception error)
        {
            // Handle error if needed
        }

        public void OnCompleted()
        {
            // Handle completion if needed
        }
    }

    /// <summary>
    /// A generic input that can accept any collection type and updates its appearance accordingly.
    /// </summary>
    public class GenericListInputViewModel : NodeInputViewModel
    {
        static GenericListInputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<GenericListInputViewModel>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; private set; }

        public IObservableList<object> Values { get; private set; }

        public GenericListInputViewModel()
        {
            PartCalculationPort = new PartCalculationPortViewModel 
            { 
                PortType = PortDataType.Collection 
            };
            Port = PartCalculationPort;

            MaxConnections = 1;

            // Accept any collection type
            ConnectionValidator = pendingConnection =>
            {
                if (pendingConnection.Output?.Port is PartCalculationPortViewModel outputPort)
                {
                    if (outputPort.PortType.IsCollection())
                    {
                        return new ConnectionValidationResult(true, 
                            $"Will process each item from {outputPort.PortType}");
                    }
                    return new ConnectionValidationResult(false, 
                        "ForEach requires a collection input");
                }
                return new ConnectionValidationResult(false, "Invalid output port");
            };

            // Set up value observation from connections
            SetupValueObservation();
        }

        private void SetupValueObservation()
        {
            // Create a source list to hold our values
            var sourceList = new SourceList<object>();
            Values = sourceList.AsObservableList();

            // Monitor connections and extract values from the connected output
            Connections.Connect()
                .Subscribe(changes =>
                {
                    var connection = Connections.Items.FirstOrDefault();
                    if (connection?.Output != null)
                    {
                        // Use reflection to get the Value property
                        var outputType = connection.Output.GetType();
                        var valueProperty = outputType.GetProperty("Value");
                        
                        if (valueProperty != null)
                        {
                            var value = valueProperty.GetValue(connection.Output);
                            
                            // Handle different observable types
                            if (value is IObservable<IObservableList<object>> observableList)
                            {
                                // Direct match for our generic type
                                observableList.Subscribe(list =>
                                {
                                    sourceList.Clear();
                                    if (list != null)
                                    {
                                        sourceList.AddRange(list.Items);
                                    }
                                });
                            }
                            else if (value != null)
                            {
                                // Try to handle typed observables using reflection
                                var valueType = value.GetType();
                                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(IObservable<>))
                                {
                                    var innerType = valueType.GetGenericArguments()[0];
                                    
                                    // Create a subscription using reflection
                                    var subscribeMethod = valueType.GetMethod("Subscribe", new[] { typeof(IObserver<>).MakeGenericType(innerType) });
                                    if (subscribeMethod != null)
                                    {
                                        // Create an observer that handles the items
                                        var observerType = typeof(ListItemObserver<>).MakeGenericType(innerType);
                                        var observer = Activator.CreateInstance(observerType, sourceList);
                                        subscribeMethod.Invoke(value, new[] { observer });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // No connection - clear values
                        sourceList.Clear();
                    }
                });
        }

        public void UpdatePortType(PortDataType newType)
        {
            PartCalculationPort.PortType = newType;
        }
    }
}