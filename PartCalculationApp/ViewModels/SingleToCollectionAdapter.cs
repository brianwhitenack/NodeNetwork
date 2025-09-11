using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    /// <summary>
    /// Adapter that wraps single value outputs to be compatible with collection inputs.
    /// </summary>
    public class SingleToCollectionAdapter<T> : ReactiveObject
    {
        /// <summary>
        /// Creates an observable that converts a single value output to a list containing that value.
        /// </summary>
        public static IObservable<IList<T>> WrapSingleValue(IObservable<T> singleValue)
        {
            return singleValue.Select(value =>
            {
                if (value != null)
                {
                    return (IList<T>)new List<T> { value };
                }
                return new List<T>();
            });
        }

        /// <summary>
        /// Checks if a connection needs single-to-collection conversion and returns the appropriate observable.
        /// </summary>
        public static IObservable<IList<T>> GetAdaptedValues(ConnectionViewModel connection)
        {
            if (connection?.Output == null)
            {
                return Observable.Return(new List<T>());
            }

            // Check if the output is a single value that needs to be wrapped
            if (connection.Output is ValueNodeOutputViewModel<T> singleOutput)
            {
                var inputPort = connection.Input?.Port as PartCalculationPortViewModel;
                var outputPort = connection.Output?.Port as PartCalculationPortViewModel;

                if (inputPort != null && outputPort != null)
                {
                    // Check if this is a single-to-collection conversion
                    if (inputPort.PortType.IsCollection() && !outputPort.PortType.IsCollection())
                    {
                        return WrapSingleValue(singleOutput.Value);
                    }
                }
            }

            // If it's already a collection output, return it as-is
            if (connection.Output is ValueNodeOutputViewModel<IList<T>> listOutput)
            {
                return listOutput.Value;
            }

            return Observable.Return(new List<T>());
        }
    }
}