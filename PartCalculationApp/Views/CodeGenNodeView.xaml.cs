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
using NodeNetwork.Views;
using ReactiveUI;

namespace ExampleCodeGenApp.Views
{
    public partial class CodeGenNodeView : IViewFor<PartCalculationViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(PartCalculationViewModel), typeof(CodeGenNodeView), new PropertyMetadata(null));

        public PartCalculationViewModel ViewModel
        {
            get => (PartCalculationViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PartCalculationViewModel)value;
        }
        #endregion

        public CodeGenNodeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                NodeView.ViewModel = this.ViewModel;
                Disposable.Create(() => NodeView.ViewModel = null).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.NodeType, v => v.NodeView.Background, ConvertNodeTypeToBrush).DisposeWith(d);
            });
        }

        public static Brush ConvertNodeTypeToBrush(NodeType type)
        {
            switch (type)
            {
                case NodeType.Input: return new SolidColorBrush(Color.FromRgb(0x9b, 0x00, 0x00));
                case NodeType.Output: return new SolidColorBrush(Color.FromRgb(0x49, 0x49, 0x49));
                case NodeType.Function: return new SolidColorBrush(Color.FromRgb(0x00, 0x39, 0xcb));
                case NodeType.Loop: return new SolidColorBrush(Color.FromRgb(0x49, 0x49, 0x49));
                case NodeType.Literal: return new SolidColorBrush(Color.FromRgb(0x00, 0x60, 0x0f));
                //case NodeType.Group: return new SolidColorBrush(Color.FromRgb(0x7B, 0x1F, 0xA2));
                default: throw new Exception("Unsupported node type");
            }
        }
    }
}
