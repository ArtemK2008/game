namespace Survivalon.Runtime
{
    public sealed class NodePlaceholderState
    {
        public NodePlaceholderState(
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState nodeState,
            NodeId originNodeId)
        {
            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            NodeState = nodeState;
            OriginNodeId = originNodeId;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState NodeState { get; }

        public NodeId OriginNodeId { get; }

        public bool UsesCombatShell => NodeType == NodeType.Combat || NodeType == NodeType.BossOrGate;
    }
}
