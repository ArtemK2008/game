using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;

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
