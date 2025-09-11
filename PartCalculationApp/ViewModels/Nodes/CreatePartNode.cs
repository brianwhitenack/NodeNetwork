using System.Reactive.Linq;

using DynamicData;

using ExampleCodeGenApp.ViewModels;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;

using PartCalculationApp.Model;
using PartCalculationApp.Serialization;

using ReactiveUI;

namespace PartCalculationApp.ViewModels.Nodes
{
    public class CreatePartNode : PartCalculationViewModel
    {
        static CreatePartNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<CreatePartNode>));
        }

        public InputViewModel<string> SkuInput { get; }
        public InputViewModel<string> DescriptionInput { get; }
        public InputViewModel<string> PackageInput { get; }
        public InputViewModel<double?> QuantityInput { get; set; }
        public InputViewModel<string> UnitOfMeasurementInput { get; }

        public OutputViewModel<Part> PartOutput { get; }

        public CreatePartNode() : base(NodeType.Function)
        {
            Name = "Create Part";

            SkuInput = new InputViewModel<string>(PortDataType.String)
            {
                Name = "SKU",
                Editor = new StringValueEditorViewModel()
            };

            DescriptionInput = new InputViewModel<string>(PortDataType.String)
            {
                Name = "Description",
                Editor = new StringValueEditorViewModel()
            };

            PackageInput = new InputViewModel<string>(PortDataType.String)
            {
                Name = "Package",
                Editor = new StringValueEditorViewModel()
            };

            QuantityInput = new InputViewModel<double?>(PortDataType.Number)
            {
                Name = "Quantity",
                Editor = new DoubleValueEditorViewModel()
            };

            UnitOfMeasurementInput = new InputViewModel<string>(PortDataType.String)
            {
                Name = "Unit of Measurement",
                Editor = new StringValueEditorViewModel()
            };

            Inputs.Add(SkuInput);
            Inputs.Add(DescriptionInput);
            Inputs.Add(PackageInput);
            Inputs.Add(QuantityInput);
            Inputs.Add(UnitOfMeasurementInput);

            System.IObservable<Part> newPart = this.WhenAnyValue(
                vm => vm.SkuInput.Value,
                vm => vm.DescriptionInput.Value,
                vm => vm.PackageInput.Value,
                vm => vm.QuantityInput.Value,
                vm => vm.UnitOfMeasurementInput.Value)
                .Select(_ => CreatePart());

            PartOutput = new OutputViewModel<Part>(PortDataType.Part)
            {
                Name = "Created Part",
                Value = newPart
            };
            Outputs.Add(PartOutput);
        }

        private Part CreatePart()
        {
            if (SkuInput.Value == null || DescriptionInput.Value == null || PackageInput.Value == null || QuantityInput.Value == null || QuantityInput.Value <= 0 || UnitOfMeasurementInput.Value == null)
            {
                return null;
            }

            return new Part
            {
                Sku = SkuInput.Value,
                Description = DescriptionInput.Value,
                Package = PackageInput.Value,
                Quantity = QuantityInput.Value.Value,
                UnitOfMeasure = UnitOfMeasurementInput.Value
            };
        }

        protected override SerializedNode InternalSerialize()
        {
            return new SerializedCreatePartNode();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {

        }
    }
}
