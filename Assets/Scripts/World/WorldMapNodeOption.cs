using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class WorldMapNodeOption
    {
        public WorldMapNodeOption(
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState nodeState,
            bool isSelectable,
            bool isCurrentContext,
            bool isSelected)
        {
            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            NodeState = nodeState;
            IsSelectable = isSelectable;
            IsCurrentContext = isCurrentContext;
            IsSelected = isSelected;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState NodeState { get; }

        public bool IsSelectable { get; }

        public bool IsCurrentContext { get; }

        public bool IsSelected { get; }
    }
}

