using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using DynamicData;

using ExampleCodeGenApp.Model;
using ExampleCodeGenApp.ViewModels.Nodes;

using NodeNetwork.Toolkit.BreadcrumbBar;
using NodeNetwork.Toolkit.Group;
using NodeNetwork.Toolkit.Layout;
using NodeNetwork.Toolkit.Layout.ForceDirected;
using NodeNetwork.Toolkit.NodeList;
using NodeNetwork.Utilities;
using NodeNetwork.ViewModels;

using PartCalculationApp.Model;
using PartCalculationApp.ViewModels;
using PartCalculationApp.ViewModels.Nodes;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels
{
    class NetworkBreadcrumb : BreadcrumbViewModel
    {
        #region Network
        private NetworkViewModel _network;
        public NetworkViewModel Network
        {
            get => _network;
            set => this.RaiseAndSetIfChanged(ref _network, value);
        }
        #endregion
    }

    public class MainViewModel : ReactiveObject
    {
        #region Network
        private readonly ObservableAsPropertyHelper<NetworkViewModel> _network;
        public NetworkViewModel Network => _network.Value;
        #endregion

        public BreadcrumbBarViewModel NetworkBreadcrumbBar { get; } = new BreadcrumbBarViewModel();
        public NodeListViewModel NodeList { get; } = new NodeListViewModel();
        public PartCalculationOutputObject Output { get; } = new PartCalculationOutputObject();
        public MeasurementInputObject MeasurementDisplay { get; } = new MeasurementInputObject();

        public ReactiveCommand<Unit, Unit> AutoLayout { get; }
		public ReactiveCommand<Unit, Unit> StartAutoLayoutLive { get; }
		public ReactiveCommand<Unit, Unit> StopAutoLayoutLive { get; }

        public ReactiveCommand<Unit, Unit> GroupNodes { get; }
        public ReactiveCommand<Unit, Unit> UngroupNodes { get; }
        public ReactiveCommand<Unit, Unit> OpenGroup { get; }

        public MainViewModel()
        {
            this.WhenAnyValue(vm => vm.NetworkBreadcrumbBar.ActiveItem).Cast<NetworkBreadcrumb>()
                .Select(b => b?.Network)
                .ToProperty(this, vm => vm.Network, out _network);
            NetworkBreadcrumbBar.ActivePath.Add(new NetworkBreadcrumb
            {
                Name = "Main",
                Network = new NetworkViewModel()
            });

            DigitizerMeasurementsNode measurementInputNode = new DigitizerMeasurementsNode { CanBeRemovedByUser = false};
            Network.Nodes.Add(measurementInputNode);

            PartsOutputNode partsOutputNode = new PartsOutputNode { CanBeRemovedByUser = false, Position = new Point(300, 0) };
            Network.Nodes.Add(partsOutputNode);

            NodeList.AddNodeType(() => new IntegerLiteralNode());
            NodeList.AddNodeType(() => new StringLiteralNode());
            NodeList.AddNodeType(() => new DigitizerMeasurementsNode());
            NodeList.AddNodeType(() => new SelectionNode());
            NodeList.AddNodeType(() => new CreatePartNode());
            NodeList.AddNodeType(() => new MeasurementLengthNode());

            Measurement input = new Measurement()
            {
                Area = 10,
                Length = 5,
                Count = 1,
                Type = "Beam",
                Selections = new Dictionary<string, object>()
                 {
                     { "Material", "Metal" },
                 }
            };

            measurementInputNode.MeasurementOutput.Value = Observable.Return(input);
            Output.InputNode = measurementInputNode;
            Output.OutputNode = partsOutputNode;

            IObservable<Measurement> measurementObservable = measurementInputNode.MeasurementOutput.Value.Select(m => m);
            measurementObservable.BindTo(this, vm => vm.MeasurementDisplay.Measurement);

            ForceDirectedLayouter layouter = new ForceDirectedLayouter();
			AutoLayout = ReactiveCommand.Create(() => layouter.Layout(new Configuration { Network = Network }, 10000));
			StartAutoLayoutLive = ReactiveCommand.CreateFromObservable(() => 
				Observable.StartAsync(ct => layouter.LayoutAsync(new Configuration { Network = Network }, ct)).TakeUntil(StopAutoLayoutLive)
			);
			StopAutoLayoutLive = ReactiveCommand.Create(() => { }, StartAutoLayoutLive.IsExecuting);

            var grouper = new NodeGrouper
            {
                GroupNodeFactory = subnet => new GroupNodeViewModel(subnet),
                EntranceNodeFactory = () => new GroupSubnetIONodeViewModel(Network, true, false) { Name = "Group Input" },
                ExitNodeFactory = () => new GroupSubnetIONodeViewModel(Network, false, true) { Name = "Group Output" },
                SubNetworkFactory = () => new NetworkViewModel(),
                IOBindingFactory = (groupNode, entranceNode, exitNode) =>
                    new GroupIOBinding(groupNode, entranceNode, exitNode)
            };
            GroupNodes = ReactiveCommand.Create(() =>
            {
                var groupBinding = (GroupIOBinding) grouper.MergeIntoGroup(Network, Network.SelectedNodes.Items);
                ((GroupNodeViewModel)groupBinding.GroupNode).IOBinding = groupBinding;
                ((GroupSubnetIONodeViewModel)groupBinding.EntranceNode).IOBinding = groupBinding;
                ((GroupSubnetIONodeViewModel)groupBinding.ExitNode).IOBinding = groupBinding;
            }, this.WhenAnyObservable(vm => vm.Network.SelectedNodes.CountChanged).Select(c => c > 1));

            var isGroupNodeSelected = this.WhenAnyValue(vm => vm.Network)
                .Select(net => net.SelectedNodes.Connect())
                .Switch()
                .Select(_ => Network.SelectedNodes.Count == 1 && Network.SelectedNodes.Items.First() is GroupNodeViewModel);

            UngroupNodes = ReactiveCommand.Create(() =>
            {
                var selectedGroupNode = (GroupNodeViewModel)Network.SelectedNodes.Items.First();
                grouper.Ungroup(selectedGroupNode.IOBinding);
            }, isGroupNodeSelected);

            OpenGroup = ReactiveCommand.Create(() =>
            {
                var selectedGroupNode = (GroupNodeViewModel)Network.SelectedNodes.Items.First();
                NetworkBreadcrumbBar.ActivePath.Add(new NetworkBreadcrumb
                {
                    Network = selectedGroupNode.Subnet,
                    Name = selectedGroupNode.Name
                });
            }, isGroupNodeSelected);
        }
    }
}
