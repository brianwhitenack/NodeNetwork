using System.Reactive.Disposables;
using System.Windows;

using ExampleCodeGenApp.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.Views
{
    public partial class MeasurementInputView : IViewFor<MeasurementInputObject>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(MeasurementInputObject), typeof(MeasurementInputView), new PropertyMetadata(null));

        public MeasurementInputObject ViewModel
        {
            get => (MeasurementInputObject)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MeasurementInputObject)value;
        }
        #endregion

        public MeasurementInputView()
        {
            InitializeComponent();

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel, vm => vm.MeasurementText, v => v.codeTextBlock.Text)
                .DisposeWith(d);
            });
        }
    }
}
