using System;
using System.Linq;
using System.Reactive.Linq;

using NodeNetwork;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    /// <summary>
    /// A generic input that can accept any single value type and updates its appearance accordingly.
    /// </summary>
    public class GenericInputViewModel : NodeInputViewModel
    {
        static GenericInputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<GenericInputViewModel>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; private set; }

        private IObservable<object> _value;
        public IObservable<object> Value
        {
            get => _value;
            private set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public GenericInputViewModel()
        {
            PartCalculationPort = new PartCalculationPortViewModel 
            { 
                PortType = PortDataType.Unknown 
            };
            Port = PartCalculationPort;

            MaxConnections = 1;

            // Accept any single value type (not collections)
            ConnectionValidator = pendingConnection =>
            {
                if (pendingConnection.Output?.Port is PartCalculationPortViewModel outputPort)
                {
                    if (!outputPort.PortType.IsCollection())
                    {
                        return new ConnectionValidationResult(true, 
                            $"Will accept {outputPort.PortType} values");
                    }
                    return new ConnectionValidationResult(false, 
                        "This input accepts single values, not collections");
                }
                return new ConnectionValidationResult(false, "Invalid output port");
            };

            // Set up value observation from connections
            SetupValueObservation();
        }

        private void SetupValueObservation()
        {
            // Create an observable that tracks the connected output's value
            Value = Connections.Connect()
                .Select(_ => Connections.Items.FirstOrDefault())
                .Select(connection =>
                {
                    if (connection?.Output != null)
                    {
                        // Try to get the value from the output
                        var outputType = connection.Output.GetType();
                        var valueProperty = outputType.GetProperty("Value");
                        if (valueProperty != null)
                        {
                            var value = valueProperty.GetValue(connection.Output);
                            if (value is IObservable<object> observableValue)
                            {
                                return observableValue;
                            }
                            else if (value != null)
                            {
                                // If it's an observable of a specific type, we need to convert it
                                var observableType = value.GetType();
                                if (observableType.IsGenericType && 
                                    observableType.GetGenericTypeDefinition() == typeof(IObservable<>))
                                {
                                    // Use reflection to call Select and convert to object
                                    var elementType = observableType.GetGenericArguments()[0];
                                    var selectMethod = typeof(Observable).GetMethods()
                                        .Where(m => m.Name == "Select" && m.GetParameters().Length == 2)
                                        .First()
                                        .MakeGenericMethod(elementType, typeof(object));
                                    
                                    var convertFunc = CreateConvertToObjectFunc(elementType);
                                    var result = selectMethod.Invoke(null, new[] { value, convertFunc });
                                    return (IObservable<object>)result;
                                }
                            }
                        }
                    }
                    return Observable.Return<object>(null);
                })
                .Switch();
        }

        private static Delegate CreateConvertToObjectFunc(Type sourceType)
        {
            // Create a function that converts from sourceType to object
            var param = System.Linq.Expressions.Expression.Parameter(sourceType, "item");
            var convert = System.Linq.Expressions.Expression.Convert(param, typeof(object));
            var lambda = System.Linq.Expressions.Expression.Lambda(convert, param);
            return lambda.Compile();
        }

        public void UpdatePortType(PortDataType newType)
        {
            PartCalculationPort.PortType = newType;
        }

        /// <summary>
        /// Gets the current value synchronously.
        /// Returns null if no value is available.
        /// </summary>
        public object GetCurrentValue()
        {
            object currentValue = null;
            Value?.Take(1).Subscribe(v => currentValue = v);
            return currentValue;
        }
    }
}