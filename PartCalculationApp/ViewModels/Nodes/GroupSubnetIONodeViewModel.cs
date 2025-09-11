using System;
using System.Collections.Generic;
using System.Text;
using ExampleCodeGenApp.Views;
using NodeNetwork.Toolkit.Group.AddEndpointDropPanel;
using NodeNetwork.ViewModels;

using PartCalculationApp.Serialization;

using ReactiveUI;

namespace ExampleCodeGenApp.ViewModels.Nodes
{
    public class GroupSubnetIONodeViewModel : PartCalculationViewModel
    {
        static GroupSubnetIONodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new GroupSubnetIONodeView(), typeof(IViewFor<GroupSubnetIONodeViewModel>));
        }

        public NetworkViewModel Subnet { get; }

        #region IOBinding
        public GroupIOBinding IOBinding
        {
            get => _ioBinding;
            set
            {
                if (_ioBinding != null)
                {
                    throw new InvalidOperationException("IOBinding is already set.");
                }
                _ioBinding = value;
                AddEndpointDropPanelVM = new AddEndpointDropPanelViewModel(_isEntranceNode, _isExitNode)
                {
                    NodeGroupIOBinding = IOBinding
                };
            }
        }
        private GroupIOBinding _ioBinding;
        #endregion

        public AddEndpointDropPanelViewModel AddEndpointDropPanelVM { get; set; }

        private readonly bool _isEntranceNode, _isExitNode;

        public GroupSubnetIONodeViewModel(NetworkViewModel subnet, bool isEntranceNode, bool isExitNode) : base(NodeType.Group)
        {
            this.Subnet = subnet;
            _isEntranceNode = isEntranceNode;
            _isExitNode = isExitNode;
        }

        protected override SerializedNode InternalSerialize()
        {
            throw new NotImplementedException();
        }

        protected override void InternalDeserialize(SerializedNode data)
        {
            throw new NotImplementedException();
        }
    }
}
