using System.Reactive.Disposables;
using System.Windows;

using ExampleCodeGenApp.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.Views
{
    public partial class MeasurementInputView : IViewFor<MeasurementInputDisplayViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(MeasurementInputDisplayViewModel), typeof(MeasurementInputView), new PropertyMetadata(null));

        public MeasurementInputDisplayViewModel ViewModel
        {
            get => (MeasurementInputDisplayViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MeasurementInputDisplayViewModel)value;
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
