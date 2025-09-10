using System;

using DynamicData;

using ExampleCodeGenApp.Views;

using NodeNetwork.Toolkit.Group.AddEndpointDropPanel;
using NodeNetwork.ViewModels;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class GroupNodeViewModel : PartCalculationViewModel
    {
        static GroupNodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new GroupNodeView(), typeof(IViewFor<GroupNodeViewModel>));
        }

        public NetworkViewModel Subnet { get; }

        #region IOBinding
        public CodeNodeGroupIOBinding IOBinding
        {
            get => _ioBinding;
            set
            {
                if (_ioBinding != null)
                {
                    throw new InvalidOperationException("IOBinding is already set.");
                }
                _ioBinding = value;
                AddEndpointDropPanelVM = new AddEndpointDropPanelViewModel
                {
                    NodeGroupIOBinding = IOBinding
                };
            }
        }
        private CodeNodeGroupIOBinding _ioBinding;
        #endregion

        public AddEndpointDropPanelViewModel AddEndpointDropPanelVM { get; private set; }

        public GroupNodeViewModel(NetworkViewModel subnet) : base(PartCalculationNodeType.Group)
        {
            this.Name = "Group";
            this.Subnet = subnet;
        }
    }
}
