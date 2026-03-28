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
            bool isSelected,
            string locationDisplayName = null,
            string nodeDisplayName = null,
            WorldMapPathRole pathRole = WorldMapPathRole.BlockedPath,
            bool isFarmReady = false,
            string optionalChallengeDisplayName = null)
        {
            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            NodeState = nodeState;
            IsSelectable = isSelectable;
            IsCurrentContext = isCurrentContext;
            IsSelected = isSelected;
            LocationDisplayName = string.IsNullOrWhiteSpace(locationDisplayName)
                ? regionId.Value
                : locationDisplayName;
            NodeDisplayName = string.IsNullOrWhiteSpace(nodeDisplayName)
                ? nodeId.Value
                : nodeDisplayName;
            PathRole = pathRole;
            IsFarmReady = isFarmReady;
            OptionalChallengeDisplayName = optionalChallengeDisplayName ?? string.Empty;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState NodeState { get; }

        public bool IsSelectable { get; }

        public bool IsCurrentContext { get; }

        public bool IsSelected { get; }

        public string LocationDisplayName { get; }

        public string NodeDisplayName { get; }

        public WorldMapPathRole PathRole { get; }

        public bool IsFarmReady { get; }

        public string OptionalChallengeDisplayName { get; }
    }
}

