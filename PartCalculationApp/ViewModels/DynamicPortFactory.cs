using System;
using System.Reactive.Linq;
using DynamicData;
using ExampleCodeGenApp.ViewModels;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using PartCalculationApp.Model;

namespace PartCalculationApp.ViewModels
{
    /// <summary>
    /// Factory for creating typed ports at runtime based on detected types.
    /// </summary>
    public static class DynamicPortFactory
    {
        /// <summary>
        /// Creates a properly typed output based on the port data type.
        /// </summary>
        public static NodeOutputViewModel CreateTypedOutput(PortDataType portType, string name = "Output")
        {
            switch (portType)
            {
                case PortDataType.String:
                    return new OutputViewModel<string>(portType) { Name = name };
                    
                case PortDataType.Number:
                    return new OutputViewModel<double?>(portType) { Name = name };
                    
                case PortDataType.Boolean:
                    return new OutputViewModel<bool>(portType) { Name = name };
                    
                case PortDataType.Measurement:
                    return new OutputViewModel<Measurement>(portType) { Name = name };
                    
                case PortDataType.Part:
                    return new OutputViewModel<Part>(portType) { Name = name };
                    
                case PortDataType.StringCollection:
                    return new ListOutputViewModel<string>(portType) { Name = name };
                    
                case PortDataType.NumberCollection:
                    return new ListOutputViewModel<double?>(portType) { Name = name };
                    
                case PortDataType.BooleanCollection:
                    return new ListOutputViewModel<bool>(portType) { Name = name };
                    
                case PortDataType.MeasurementCollection:
                    return new ListOutputViewModel<Measurement>(portType) { Name = name };
                    
                case PortDataType.PartCollection:
                    return new ListOutputViewModel<Part>(portType) { Name = name };
                    
                default:
                    // Return a generic output for unknown types
                    return new GenericOutputViewModel 
                    { 
                        Name = name,
                        Value = Observable.Return<object>(null)
                    };
            }
        }

        /// <summary>
        /// Creates a properly typed input based on the port data type.
        /// </summary>
        public static NodeInputViewModel CreateTypedInput(PortDataType portType, string name = "Input")
        {
            switch (portType)
            {
                case PortDataType.String:
                    return new InputViewModel<string>(portType) { Name = name };
                    
                case PortDataType.Number:
                    return new InputViewModel<double?>(portType) { Name = name };
                    
                case PortDataType.Boolean:
                    return new InputViewModel<bool>(portType) { Name = name };
                    
                case PortDataType.Measurement:
                    return new InputViewModel<Measurement>(portType) { Name = name };
                    
                case PortDataType.Part:
                    return new InputViewModel<Part>(portType) { Name = name };
                    
                case PortDataType.StringCollection:
                    return new ListInputViewModel<string>(portType) { Name = name };
                    
                case PortDataType.NumberCollection:
                    return new ListInputViewModel<double?>(portType) { Name = name };
                    
                case PortDataType.BooleanCollection:
                    return new ListInputViewModel<bool>(portType) { Name = name };
                    
                case PortDataType.MeasurementCollection:
                    return new ListInputViewModel<Measurement>(portType) { Name = name };
                    
                case PortDataType.PartCollection:
                    return new ListInputViewModel<Part>(portType) { Name = name };
                    
                default:
                    // Return a generic input for unknown types
                    return new GenericInputViewModel { Name = name };
            }
        }

        /// <summary>
        /// Updates an output's value using reflection based on its actual type.
        /// </summary>
        public static void SetOutputValue(NodeOutputViewModel output, object value)
        {
            if (output == null) return;

            var outputType = output.GetType();
            
            // Try to find the Value property
            var valueProperty = outputType.GetProperty("Value");
            if (valueProperty != null)
            {
                // If the value is already an observable, set it directly
                if (value != null && valueProperty.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    valueProperty.SetValue(output, value);
                }
                // Otherwise, wrap it in an observable
                else
                {
                    var elementType = valueProperty.PropertyType.GetGenericArguments()[0];
                    var returnMethod = typeof(Observable).GetMethod("Return").MakeGenericMethod(elementType);
                    var observableValue = returnMethod.Invoke(null, new[] { value });
                    valueProperty.SetValue(output, observableValue);
                }
            }
        }
    }
}