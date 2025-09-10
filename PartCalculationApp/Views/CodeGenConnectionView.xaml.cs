using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExampleCodeGenApp.ViewModels;
using ReactiveUI;

namespace ExampleCodeGenApp.Views
{
    public partial class CodeGenConnectionView : IViewFor<PartCalculationConnectionViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(PartCalculationConnectionViewModel), typeof(CodeGenConnectionView), new PropertyMetadata(null));

        public PartCalculationConnectionViewModel ViewModel
        {
            get => (PartCalculationConnectionViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PartCalculationConnectionViewModel)value;
        }
        #endregion

        public CodeGenConnectionView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ConnectionView.ViewModel = this.ViewModel;
                d(Disposable.Create(() => ConnectionView.ViewModel = null));
            });
        }
    }
}
