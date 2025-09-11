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
using PartCalculationApp.ViewModels.Nodes;
using PartCalculationApp.Serialization;

using ReactiveUI;
using Microsoft.Win32;

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

        public ReactiveCommand<Unit, Unit> SaveGraph { get; }
        public ReactiveCommand<Unit, Unit> LoadGraph { get; }

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

            DigitizerMeasurementsNode measurementInputNode = new DigitizerMeasurementsNode 
            {
                Position = new Point(50, 50),
                CanBeRemovedByUser = false
            };
            Network.Nodes.Add(measurementInputNode);

            PartsOutputNode partsOutputNode = new PartsOutputNode 
            {
                CanBeRemovedByUser = false,
                Position = new Point(1000, 50) 
            };
            Network.Nodes.Add(partsOutputNode);

            // variables
            NodeList.AddNodeType(() => new NumberLiteralNode());
            NodeList.AddNodeType(() => new StringLiteralNode());

            NodeList.AddNodeType(() => new StringSelectionNode());
            NodeList.AddNodeType(() => new NumberSelectionNode());
            NodeList.AddNodeType(() => new CreatePartNode());
            NodeList.AddNodeType(() => new MeasurementLengthNode());
            NodeList.AddNodeType(() => new ConcatenationNode());
            NodeList.AddNodeType(() => new ToStringNode());

            NodeList.AddNodeType(() => new DivideNode());
            NodeList.AddNodeType(() => new SubtractNode());
            NodeList.AddNodeType(() => new AddNode());
            NodeList.AddNodeType(() => new MultiplyNode());


            Measurement input = new Measurement()
            {
                Length = 50,
                Count = 1,
                Type = "Beam",
                Selections = new Dictionary<string, object>()
                 {
                    { "Thickness", 2 },
                    { "Width", 4 },
                    { "Material", "LVL" },
                    { "BeamType", "HEADER" },
                    { "Plies", 3 },
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

            // Initialize serialization
            var nodeFactory = new NodeFactory();
            RegisterNodeTypes(nodeFactory);
            var graphSerializer = new GraphSerializer(nodeFactory);

            // Save command
            SaveGraph = ReactiveCommand.Create(() =>
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = "graph.json"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    try
                    {
                        graphSerializer.SerializeToFile(Network, saveDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        // In a real app, show error message to user
                        Console.WriteLine($"Failed to save graph: {ex.Message}");
                    }
                }
            });

            // Load command
            LoadGraph = ReactiveCommand.Create(() =>
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json"
                };

                if (openDialog.ShowDialog() == true)
                {
                    try
                    {
                        var loadedNetwork = graphSerializer.DeserializeFromFile(openDialog.FileName, input);
                        
                        // Replace the current network with the loaded one
                        Network.Nodes.Clear();
                        Network.Connections.Clear();
                        
                        foreach (var node in loadedNetwork.Nodes.Items)
                        {
                            Network.Nodes.Add(node);
                        }
                        
                        foreach (var connection in loadedNetwork.Connections.Items)
                        {
                            Network.Connections.Add(connection);
                        }

                        // Re-wire special nodes if they exist
                        DigitizerMeasurementsNode digitizerNode = Network.Nodes.Items
                            .OfType<DigitizerMeasurementsNode>()
                            .FirstOrDefault();
                        PartsOutputNode partsNode = Network.Nodes.Items
                            .OfType<PartsOutputNode>()
                            .FirstOrDefault();

                        if (digitizerNode != null)
                        {
                            Output.InputNode = digitizerNode;
                            // Restore the measurement observable
                            IObservable<Measurement> measurementObservable = digitizerNode.MeasurementOutput.Value.Select(m => m);
                            measurementObservable.BindTo(this, vm => vm.MeasurementDisplay.Measurement);
                        }

                        if (partsNode != null)
                        {
                            Output.OutputNode = partsNode;
                        }
                    }
                    catch (Exception ex)
                    {
                        // In a real app, show error message to user
                        Console.WriteLine($"Failed to load graph: {ex.Message}");
                    }
                }
            });
        }

        private void RegisterNodeTypes(NodeFactory factory)
        {
            // Register all node types that implement ISerializableNode
            // These registrations should match the types in the NodeList
            factory.RegisterNode("StringLiteral", () => new StringLiteralNode());
            factory.RegisterNode("NumberLiteral", () => new NumberLiteralNode());
            factory.RegisterNode("StringSelection", () => new StringSelectionNode());
            factory.RegisterNode("NumberSelection", () => new NumberSelectionNode());
            factory.RegisterNode("MeasurementLength", () => new MeasurementLengthNode());
            factory.RegisterNode("CreatePart", () => new CreatePartNode());
            factory.RegisterNode("Concatenation", () => new ConcatenationNode());
            factory.RegisterNode("ToString", () => new ToStringNode());
            factory.RegisterNode("Add", () => new AddNode());
            factory.RegisterNode("Subtract", () => new SubtractNode());
            factory.RegisterNode("Multiply", () => new MultiplyNode());
            factory.RegisterNode("Divide", () => new DivideNode());
            factory.RegisterNode("DigitizerMeasurements", () => new DigitizerMeasurementsNode());
            factory.RegisterNode("PartsOutput", () => new PartsOutputNode());
        }
    }
}
