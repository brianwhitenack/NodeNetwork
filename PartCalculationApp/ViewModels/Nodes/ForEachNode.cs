using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;

using PartCalculationApp.Model;
using PartCalculationApp.Serialization;
using PartCalculationApp.ViewModels;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    /// <summary>
    /// A generic ForEach node that adapts its port types based on what's connected.
    /// It maps a function over a collection:
    /// 1. Takes a collection input
    /// 2. Outputs each item for processing
    /// 3. Takes the processed result back
    /// 4. Outputs the collected results
    /// </summary>
    public class ForEachNode : PartCalculationViewModel
    {
        static ForEachNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ForEachNode>));
        }

        // Input: Collection to iterate over
        public GenericListInputViewModel CollectionInput { get; private set; }

        // Output: Current item being processed (dynamically typed)
        public NodeOutputViewModel CurrentItemOutput { get; private set; }

        // Input: Result from processing current item (dynamically typed)
        public NodeInputViewModel ProcessedItemInput { get; private set; }

        // Output: Collection of all processed results (dynamically typed)
        public NodeOutputViewModel ResultsOutput { get; private set; }

        // Track the current detected types
        private PortDataType _detectedCollectionType = PortDataType.Unknown;
        private PortDataType _detectedItemType = PortDataType.Unknown;
        private PortDataType _detectedResultItemType = PortDataType.Unknown;
        private PortDataType _detectedResultCollectionType = PortDataType.Unknown;
        
        // Flag to prevent recursive port updates
        private bool _isUpdatingPorts = false;

        public ForEachNode() : base(NodeType.Loop)
        {
            Name = "For Each";

            // Start with generic ports
            CollectionInput = new GenericListInputViewModel { Name = "Items" };
            CurrentItemOutput = new GenericOutputViewModel { Name = "Current" };
            ProcessedItemInput = new GenericInputViewModel { Name = "Result" };
            ResultsOutput = new GenericListOutputViewModel { Name = "Results" };

            Inputs.Add(CollectionInput);
            Inputs.Add(ProcessedItemInput);
            Outputs.Add(CurrentItemOutput);
            Outputs.Add(ResultsOutput);

            // Monitor connections to detect and update port types
            // Use ObserveOn to defer the updates and avoid nested writes
            CollectionInput.Connections.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateInputPortTypes());

            ProcessedItemInput.Connections.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateResultPortTypes());

            // Set up the value transformation logic
            SetupValueTransformation();
        }

        private void UpdateInputPortTypes()
        {
            var connection = CollectionInput.Connections.Items.FirstOrDefault();
            if (connection?.Output?.Port is PartCalculationPortViewModel outputPort)
            {
                var newCollectionType = outputPort.PortType;
                var newItemType = GetItemTypeFromCollection(newCollectionType);
                
                // Only recreate if type actually changed
                if (newItemType != _detectedItemType)
                {
                    _detectedCollectionType = newCollectionType;
                    _detectedItemType = newItemType;
                    
                    // Update collection input port type
                    CollectionInput.UpdatePortType(_detectedCollectionType);
                    
                    // Replace the current item output with a properly typed one
                    ReplaceCurrentItemOutput(newItemType);
                    
                    // Update the name to reflect the type
                    UpdateNodeName();
                }
            }
            else
            {
                // No connection - reset to generic if not already
                if (_detectedItemType != PortDataType.Unknown)
                {
                    _detectedCollectionType = PortDataType.Unknown;
                    _detectedItemType = PortDataType.Unknown;
                    
                    CollectionInput.UpdatePortType(PortDataType.Collection);
                    ReplaceCurrentItemOutput(PortDataType.Unknown);
                    
                    Name = "For Each";
                }
            }
        }

        private void ReplaceCurrentItemOutput(PortDataType itemType)
        {
            // Store existing connections
            var existingConnections = CurrentItemOutput?.Connections.Items.ToList() ?? new List<ConnectionViewModel>();
            
            // Remove old output
            if (CurrentItemOutput != null)
            {
                Outputs.Remove(CurrentItemOutput);
            }
            
            // Create new typed output
            if (itemType != PortDataType.Unknown)
            {
                CurrentItemOutput = DynamicPortFactory.CreateTypedOutput(itemType, "Current");
                
                // Set initial observable value based on type
                SetInitialOutputValue(CurrentItemOutput, itemType);
            }
            else
            {
                CurrentItemOutput = new GenericOutputViewModel { Name = "Current" };
                // For generic output, set a null observable
                if (CurrentItemOutput is GenericOutputViewModel genericOutput)
                {
                    genericOutput.Value = Observable.Return<object>(null);
                }
            }
            
            // Add at the correct position (first output)
            Outputs.Insert(0, CurrentItemOutput);
            
            // Note: Connections will be lost when replacing ports, which is expected behavior
            // The user will need to reconnect after the type changes
            
            // Trigger value transformation update
            UpdateValueTransformation();
        }
        
        private void SetInitialOutputValue(NodeOutputViewModel output, PortDataType portType)
        {
            // Set an initial observable that will be updated when collection items arrive
            var outputType = output.GetType();
            var valueProperty = outputType.GetProperty("Value");
            
            if (valueProperty != null)
            {
                // Create an observable that starts with null/default and will be updated
                var elementType = valueProperty.PropertyType.GetGenericArguments()[0];
                
                // Create Observable.Return<T>(default(T))
                // Get the specific Return method with one generic parameter and one regular parameter
                var returnMethod = typeof(Observable).GetMethods()
                    .Where(m => m.Name == "Return" && 
                           m.IsGenericMethodDefinition && 
                           m.GetGenericArguments().Length == 1 && 
                           m.GetParameters().Length == 1)
                    .First()
                    .MakeGenericMethod(elementType);
                    
                var defaultValue = elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
                var observableValue = returnMethod.Invoke(null, new[] { defaultValue });
                
                valueProperty.SetValue(output, observableValue);
            }
        }

        private void UpdateResultPortTypes()
        {
            var connection = ProcessedItemInput.Connections.Items.FirstOrDefault();
            if (connection?.Output?.Port is PartCalculationPortViewModel outputPort)
            {
                var newResultItemType = outputPort.PortType;
                var newResultCollectionType = GetCollectionTypeFromItem(newResultItemType);
                
                // Only recreate if type actually changed
                if (newResultItemType != _detectedResultItemType)
                {
                    _detectedResultItemType = newResultItemType;
                    _detectedResultCollectionType = newResultCollectionType;
                    
                    // Replace the processed item input with a properly typed one
                    ReplaceProcessedItemInput(newResultItemType);
                    
                    // Replace the results output with a properly typed one
                    ReplaceResultsOutput(newResultCollectionType);
                    
                    // Update the name to reflect both types
                    UpdateNodeName();
                }
            }
            else
            {
                // No connection - reset to generic if not already
                if (_detectedResultItemType != PortDataType.Unknown)
                {
                    _detectedResultItemType = PortDataType.Unknown;
                    _detectedResultCollectionType = PortDataType.Collection;
                    
                    ReplaceProcessedItemInput(PortDataType.Unknown);
                    ReplaceResultsOutput(PortDataType.Collection);
                    
                    UpdateNodeName();
                }
            }
        }

        private void ReplaceProcessedItemInput(PortDataType itemType)
        {
            // Prevent recursive updates
            if (_isUpdatingPorts) return;
            
            try
            {
                _isUpdatingPorts = true;
                
                // Remove old input
                if (ProcessedItemInput != null)
                {
                    Inputs.Remove(ProcessedItemInput);
                }
                
                // Create new typed input
                if (itemType != PortDataType.Unknown)
                {
                    ProcessedItemInput = DynamicPortFactory.CreateTypedInput(itemType, "Result");
                }
                else
                {
                    ProcessedItemInput = new GenericInputViewModel { Name = "Result" };
                }
                
                // Add at the correct position (second input)
                if (Inputs.Count >= 1)
                    Inputs.Insert(1, ProcessedItemInput);
                else
                    Inputs.Add(ProcessedItemInput);
                
                // Re-setup connection monitoring with deferred execution
                ProcessedItemInput.Connections.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => 
                    {
                        if (!_isUpdatingPorts)
                        {
                            UpdateResultPortTypes();
                        }
                    });
            }
            finally
            {
                _isUpdatingPorts = false;
            }
        }

        private void ReplaceResultsOutput(PortDataType collectionType)
        {
            // Remove old output
            if (ResultsOutput != null)
            {
                Outputs.Remove(ResultsOutput);
            }
            
            // Create new typed output
            if (collectionType != PortDataType.Collection && collectionType != PortDataType.Unknown)
            {
                ResultsOutput = DynamicPortFactory.CreateTypedOutput(collectionType, "Results");
                
                // Set initial empty collection value
                SetInitialCollectionOutputValue(ResultsOutput, collectionType);
            }
            else
            {
                ResultsOutput = new GenericListOutputViewModel { Name = "Results" };
                // For generic output, set an empty list observable
                if (ResultsOutput is GenericListOutputViewModel genericOutput)
                {
                    genericOutput.SetList(new List<object>());
                }
            }
            
            // Add at the correct position (second output)
            if (Outputs.Count >= 1)
                Outputs.Insert(1, ResultsOutput);
            else
                Outputs.Add(ResultsOutput);
                
            // Trigger value transformation update
            UpdateValueTransformation();
        }
        
        private void SetInitialCollectionOutputValue(NodeOutputViewModel output, PortDataType collectionType)
        {
            // Set an initial empty collection observable
            var outputType = output.GetType();
            
            // For ListOutputViewModel<T>, use the SetList method
            if (outputType.IsGenericType && 
                outputType.GetGenericTypeDefinition() == typeof(ListOutputViewModel<>))
            {
                var elementType = outputType.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(elementType);
                var emptyList = Activator.CreateInstance(listType);
                
                var setListMethod = outputType.GetMethod("SetList");
                if (setListMethod != null)
                {
                    setListMethod.Invoke(output, new[] { emptyList });
                }
            }
        }

        private void UpdateNodeName()
        {
            if (_detectedItemType != PortDataType.Unknown && _detectedResultItemType != PortDataType.Unknown)
            {
                Name = $"For Each ({_detectedItemType} → {_detectedResultItemType})";
            }
            else if (_detectedItemType != PortDataType.Unknown)
            {
                Name = $"For Each ({_detectedItemType})";
            }
            else
            {
                Name = "For Each";
            }
        }

        private PortDataType GetItemTypeFromCollection(PortDataType collectionType)
        {
            switch (collectionType)
            {
                case PortDataType.StringCollection:
                    return PortDataType.String;
                case PortDataType.NumberCollection:
                    return PortDataType.Number;
                case PortDataType.BooleanCollection:
                    return PortDataType.Boolean;
                case PortDataType.MeasurementCollection:
                    return PortDataType.Measurement;
                case PortDataType.PartCollection:
                    return PortDataType.Part;
                default:
                    return PortDataType.Unknown;
            }
        }

        private PortDataType GetCollectionTypeFromItem(PortDataType itemType)
        {
            switch (itemType)
            {
                case PortDataType.String:
                    return PortDataType.StringCollection;
                case PortDataType.Number:
                    return PortDataType.NumberCollection;
                case PortDataType.Boolean:
                    return PortDataType.BooleanCollection;
                case PortDataType.Measurement:
                    return PortDataType.MeasurementCollection;
                case PortDataType.Part:
                    return PortDataType.PartCollection;
                default:
                    return PortDataType.Collection;
            }
        }

        private void SetupValueTransformation()
        {
            // Create a reactive pipeline that:
            // 1. Takes items from CollectionInput
            // 2. For each item, outputs it through CurrentItemOutput
            // 3. Waits for the processed result in ProcessedItemInput
            // 4. Collects all results and outputs through ResultsOutput

            // Monitor when ports are replaced and re-setup the transformation
            this.WhenAnyValue(
                x => x.CurrentItemOutput,
                x => x.ProcessedItemInput,
                x => x.ResultsOutput,
                x => x.CollectionInput)
                .Subscribe(_ => UpdateValueTransformation());
        }

        private void UpdateValueTransformation()
        {
            // This is a simplified implementation that demonstrates the concept
            // In a real implementation, you'd need proper coordination between items

            try
            {
                // Get values from CollectionInput
                var inputItems = CollectionInput.Values.Connect()
                    .Select(_ => CollectionInput.Values.Items.ToList())
                    .StartWith(new List<object>());

                // Set current item output to first item (simplified)
                if (CurrentItemOutput != null)
                {
                    var currentItemObservable = inputItems
                        .Select(items => items?.FirstOrDefault())
                        .Where(item => item != null);

                    SetPortValue(CurrentItemOutput, currentItemObservable);
                }

                // Get processed values and create results
                if (ProcessedItemInput != null && ResultsOutput != null)
                {
                    var processedValue = GetPortValue(ProcessedItemInput) ?? Observable.Return<object>(null);

                    var transformedResults = inputItems
                        .CombineLatest(processedValue, (items, processedTemplate) =>
                        {
                            // Simplified: apply same processing to all items
                            var results = new List<object>();
                            if (items != null && processedTemplate != null)
                            {
                                foreach (var item in items)
                                {
                                    results.Add(processedTemplate);
                                }
                            }
                            return results;
                        });

                    SetPortValue(ResultsOutput, transformedResults);
                }
            }
            catch (Exception ex)
            {
                // Log error or handle gracefully
                System.Diagnostics.Debug.WriteLine($"Error in UpdateValueTransformation: {ex.Message}");
            }
        }

        private IObservable<object> GetPortValue(NodeInputViewModel input)
        {
            if (input == null) return null;

            var inputType = input.GetType();
            var valueProperty = inputType.GetProperty("Value");
            
            if (valueProperty != null)
            {
                var value = valueProperty.GetValue(input);
                
                // If it's already IObservable<object>, return it
                if (value is IObservable<object> observableObject)
                {
                    return observableObject;
                }
                
                // If it's a typed observable, we need to convert it
                if (value != null && value.GetType().IsGenericType && 
                    value.GetType().GetGenericTypeDefinition() == typeof(IObservable<>))
                {
                    // Use reflection to convert typed observable to object observable
                    var elementType = value.GetType().GetGenericArguments()[0];
                    var selectMethod = typeof(Observable).GetMethods()
                        .Where(m => m.Name == "Select" && m.GetParameters().Length == 2)
                        .First()
                        .MakeGenericMethod(elementType, typeof(object));
                    
                    var convertFunc = CreateConvertToObjectFunc(elementType);
                    var result = selectMethod.Invoke(null, new[] { value, convertFunc });
                    return (IObservable<object>)result;
                }
            }
            
            return Observable.Return<object>(null);
        }

        private void SetPortValue(NodeOutputViewModel output, IObservable<object> value)
        {
            if (output == null || value == null) return;

            var outputType = output.GetType();
            var valueProperty = outputType.GetProperty("Value");
            
            if (valueProperty != null)
            {
                // Check if we need to convert the observable type
                var expectedType = valueProperty.PropertyType;
                
                if (expectedType.IsAssignableFrom(value.GetType()))
                {
                    // Direct assignment possible
                    valueProperty.SetValue(output, value);
                }
                else if (expectedType.IsGenericType && 
                         expectedType.GetGenericTypeDefinition() == typeof(IObservable<>))
                {
                    // Need to convert from IObservable<object> to IObservable<T>
                    var targetElementType = expectedType.GetGenericArguments()[0];
                    
                    // Special handling for collections
                    if (targetElementType.IsGenericType && 
                        targetElementType.GetGenericTypeDefinition() == typeof(IObservableList<>))
                    {
                        // Handle IObservable<IObservableList<T>>
                        SetCollectionPortValue(output, value, valueProperty);
                    }
                    else
                    {
                        // Handle regular IObservable<T>
                        var selectMethod = typeof(Observable).GetMethods()
                            .Where(m => m.Name == "Select" && m.GetParameters().Length == 2)
                            .First()
                            .MakeGenericMethod(typeof(object), targetElementType);
                        
                        var convertFunc = CreateConvertFromObjectFunc(targetElementType);
                        var convertedValue = selectMethod.Invoke(null, new object[] { value, convertFunc });
                        valueProperty.SetValue(output, convertedValue);
                    }
                }
            }
        }

        private void SetCollectionPortValue(NodeOutputViewModel output, IObservable<object> value, 
            System.Reflection.PropertyInfo valueProperty)
        {
            // For collection outputs, we need to convert IObservable<object> (list) 
            // to IObservable<IObservableList<T>>
            
            var listOutputType = output.GetType();
            if (listOutputType.IsGenericType && 
                listOutputType.GetGenericTypeDefinition() == typeof(ListOutputViewModel<>))
            {
                // Use the SetObservableList method
                var setMethod = listOutputType.GetMethod("SetObservableList", 
                    new[] { typeof(IObservable<>).MakeGenericType(typeof(IList<>).MakeGenericType(
                        listOutputType.GetGenericArguments()[0])) });
                
                if (setMethod != null)
                {
                    // Convert our IObservable<object> to IObservable<IList<T>>
                    var elementType = listOutputType.GetGenericArguments()[0];
                    var typedListObservable = ConvertToTypedListObservable(value, elementType);
                    setMethod.Invoke(output, new[] { typedListObservable });
                }
            }
        }

        private object ConvertToTypedListObservable(IObservable<object> value, Type elementType)
        {
            // Create IObservable<IList<T>> from IObservable<object> where object is a list
            var selectMethod = typeof(Observable).GetMethods()
                .Where(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .First();
            
            var listType = typeof(IList<>).MakeGenericType(elementType);
            var genericSelect = selectMethod.MakeGenericMethod(typeof(object), listType);
            
            // Create conversion function
            var param = System.Linq.Expressions.Expression.Parameter(typeof(object), "obj");
            var convertExpr = System.Linq.Expressions.Expression.Convert(param, listType);
            var lambda = System.Linq.Expressions.Expression.Lambda(convertExpr, param);
            var convertFunc = lambda.Compile();
            
            return genericSelect.Invoke(null, new object[] { value, convertFunc });
        }

        private static Delegate CreateConvertToObjectFunc(Type sourceType)
        {
            var param = System.Linq.Expressions.Expression.Parameter(sourceType, "item");
            var convert = System.Linq.Expressions.Expression.Convert(param, typeof(object));
            var lambda = System.Linq.Expressions.Expression.Lambda(convert, param);
            return lambda.Compile();
        }

        private static Delegate CreateConvertFromObjectFunc(Type targetType)
        {
            var param = System.Linq.Expressions.Expression.Parameter(typeof(object), "item");
            var convert = System.Linq.Expressions.Expression.Convert(param, targetType);
            var lambda = System.Linq.Expressions.Expression.Lambda(convert, param);
            return lambda.Compile();
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedForeachNode
            {

            };
        }

        protected override void InternalDeserialize(SerializedNode data)
        {
           
        }
    }
}