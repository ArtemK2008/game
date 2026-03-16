using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.World
{
    public sealed class WorldNode
    {
        public WorldNode(NodeId nodeId, RegionId regionId, NodeType nodeType, NodeState state)
        {
            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            State = state;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState State { get; }
    }
}
