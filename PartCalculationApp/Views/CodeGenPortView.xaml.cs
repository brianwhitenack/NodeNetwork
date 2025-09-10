using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ExampleCodeGenApp.ViewModels;
using ReactiveUI;

namespace ExampleCodeGenApp.Views
{
    public partial class CodeGenPortView : IViewFor<PartCalculationPortViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(PartCalculationPortViewModel), typeof(CodeGenPortView), new PropertyMetadata(null));

        public PartCalculationPortViewModel ViewModel
        {
            get => (PartCalculationPortViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PartCalculationPortViewModel)value;
        }
        #endregion

        #region Template Resource Keys
        public const String ExecutionPortTemplateKey = "ExecutionPortTemplate";
        public const String IntegerPortTemplateKey = "IntegerPortTemplate";
        public const String StringPortTemplateKey = "StringPortTemplate"; 
        public const String MeasurementPortTemplateKey = "MeasurementPortTemplate"; 
        #endregion

        public CodeGenPortView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
	            this.WhenAnyValue(v => v.ViewModel).BindTo(this, v => v.PortView.ViewModel).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.PortType, v => v.PortView.Template, GetTemplateFromPortType)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsMirrored, v => v.PortView.RenderTransform,
                    isMirrored => new ScaleTransform(isMirrored ? -1.0 : 1.0, 1.0))
                    .DisposeWith(d);
            });
        }

        public ControlTemplate GetTemplateFromPortType(PortDataType type)
        {
            switch (type)
            {
                case PortDataType.Boolean: return (ControlTemplate) Resources[ExecutionPortTemplateKey];
                case PortDataType.Number: return (ControlTemplate) Resources[IntegerPortTemplateKey];
                case PortDataType.String: return (ControlTemplate) Resources[StringPortTemplateKey];
                case PortDataType.Measurement: return (ControlTemplate) Resources[MeasurementPortTemplateKey];
                default: throw new Exception("Unsupported port type");
            }
        }
    }
}
