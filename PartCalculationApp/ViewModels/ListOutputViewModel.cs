using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.Views;
using PartCalculationApp.Serialization;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    /// <summary>
    /// Output view model that produces a list of values.
    /// Can be connected to both single value inputs (which will receive each item) 
    /// and collection inputs (which will receive the entire list).
    /// </summary>
    public class ListOutputViewModel<T> : ValueNodeOutputViewModel<IObservableList<T>>, IInputOutputViewModel
    {
        static ListOutputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<ListOutputViewModel<T>>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; }

        public ListOutputViewModel(PortDataType type) : base()
        {
            PartCalculationPort = new PartCalculationPortViewModel 
            {
                PortType = type
            };

            Port = PartCalculationPort;
        }

        /// <summary>
        /// Sets the value as a static list.
        /// </summary>
        public void SetList(IList<T> list)
        {
            var observableList = new SourceList<T>();
            observableList.AddRange(list);
            Value = Observable.Return(observableList.AsObservableList());
        }

        /// <summary>
        /// Sets the value from an observable of lists.
        /// </summary>
        public void SetObservableList(IObservable<IList<T>> observableList)
        {
            Value = observableList.Select(list =>
            {
                var sourceList = new SourceList<T>();
                if (list != null)
                {
                    sourceList.AddRange(list);
                }
                return sourceList.AsObservableList();
            });
        }

        /// <summary>
        /// Sets the value from a SourceList that can be dynamically updated.
        /// </summary>
        public void SetSourceList(SourceList<T> sourceList)
        {
            Value = Observable.Return(sourceList.AsObservableList());
        }

        public PortDataType GetPortDataType()
        {
            return PartCalculationPort.PortType;
        }

        public Guid GetId()
        {
            return Id;
        }

        public SerializedInputOutput Serialize()
        {
            return new SerializedInputOutput()
            {
                Id = Id,
                DataType = PartCalculationPort.PortType,
                Name = Name
            };
        }

        public void Deserialize(SerializedInputOutput data)
        {
            Id = data.Id;
            Name = data.Name;
        }

        public string GetName()
        {
            return Name;
        }
    }
}