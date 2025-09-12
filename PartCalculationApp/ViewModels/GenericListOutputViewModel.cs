using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    /// <summary>
    /// A generic output that can provide any collection type and updates its appearance accordingly.
    /// This is useful for nodes that need to dynamically adapt their output type based on inputs or configuration.
    /// </summary>
    public class GenericListOutputViewModel : NodeOutputViewModel
    {
        static GenericListOutputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<GenericListOutputViewModel>));
        }

        public PartCalculationPortViewModel PartCalculationPort { get; private set; }

        private IObservable<IObservableList<object>> _value;
        public IObservable<IObservableList<object>> Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private SourceList<object> _sourceList;

        public GenericListOutputViewModel()
        {
            PartCalculationPort = new PartCalculationPortViewModel 
            { 
                PortType = PortDataType.Collection 
            };
            Port = PartCalculationPort;

            // Initialize with an empty source list
            _sourceList = new SourceList<object>();
            Value = Observable.Return(_sourceList.AsObservableList());
        }

        /// <summary>
        /// Updates the port type to match the collection type being output.
        /// </summary>
        public void UpdatePortType(PortDataType newType)
        {
            PartCalculationPort.PortType = newType;
        }

        /// <summary>
        /// Sets the value as a static list of items.
        /// </summary>
        public void SetList<T>(IList<T> list)
        {
            _sourceList.Clear();
            if (list != null)
            {
                foreach (var item in list)
                {
                    _sourceList.Add(item);
                }
            }
        }

        /// <summary>
        /// Sets the value from an observable of lists.
        /// </summary>
        public void SetObservableList<T>(IObservable<IList<T>> observableList)
        {
            Value = observableList.Select(list =>
            {
                var sourceList = new SourceList<object>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        sourceList.Add(item);
                    }
                }
                return sourceList.AsObservableList();
            });
        }

        ///// <summary>
        ///// Sets the value from an existing IObservableList.
        ///// </summary>
        //public void SetObservableList<T>(IObservableList<T> observableList)
        //{
        //    // Convert the typed observable list to object observable list
        //    Value = Observable.Return(observableList.Transform(item => (object)item).AsObservableList());
        //}

        ///// <summary>
        ///// Sets the value from a SourceList that can be dynamically updated.
        ///// </summary>
        //public void SetSourceList<T>(SourceList<T> sourceList)
        //{
        //    // Transform the typed source list to object source list
        //    Value = Observable.Return(sourceList.Transform(item => (object)item).AsObservableList());
        //}

        /// <summary>
        /// Gets the internal source list for direct manipulation.
        /// Use with caution as type safety is not enforced.
        /// </summary>
        public SourceList<object> GetSourceList()
        {
            return _sourceList;
        }

        /// <summary>
        /// Clears all items from the output list.
        /// </summary>
        public void Clear()
        {
            _sourceList.Clear();
        }

        /// <summary>
        /// Adds a single item to the output list.
        /// </summary>
        public void AddItem(object item)
        {
            _sourceList.Add(item);
        }

        /// <summary>
        /// Adds multiple items to the output list.
        /// </summary>
        public void AddItems(IEnumerable<object> items)
        {
            _sourceList.AddRange(items);
        }

        /// <summary>
        /// Removes an item from the output list.
        /// </summary>
        public void RemoveItem(object item)
        {
            _sourceList.Remove(item);
        }
    }
}