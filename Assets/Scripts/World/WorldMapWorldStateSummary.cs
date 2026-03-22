using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class WorldMapWorldStateSummary
    {
        public WorldMapWorldStateSummary(
            string currentLocationDisplayName,
            RegionId currentRegionId,
            NodeId currentNodeId,
            NodeState currentNodeState,
            int selectableDestinationCount,
            IReadOnlyList<NodeId> forwardRouteNodeIds,
            IReadOnlyList<NodeId> backtrackOrFarmNodeIds,
            IReadOnlyList<NodeId> blockedLinkedNodeIds)
        {
            if (string.IsNullOrWhiteSpace(currentLocationDisplayName))
            {
                throw new ArgumentException(
                    "Current location display name cannot be null or whitespace.",
                    nameof(currentLocationDisplayName));
            }

            if (forwardRouteNodeIds == null)
            {
                throw new ArgumentNullException(nameof(forwardRouteNodeIds));
            }

            if (backtrackOrFarmNodeIds == null)
            {
                throw new ArgumentNullException(nameof(backtrackOrFarmNodeIds));
            }

            if (blockedLinkedNodeIds == null)
            {
                throw new ArgumentNullException(nameof(blockedLinkedNodeIds));
            }

            if (selectableDestinationCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(selectableDestinationCount),
                    selectableDestinationCount,
                    "Selectable destination count cannot be negative.");
            }

            CurrentLocationDisplayName = currentLocationDisplayName;
            CurrentRegionId = currentRegionId;
            CurrentNodeId = currentNodeId;
            CurrentNodeState = currentNodeState;
            SelectableDestinationCount = selectableDestinationCount;
            ForwardRouteNodeIds = new List<NodeId>(forwardRouteNodeIds).AsReadOnly();
            BacktrackOrFarmNodeIds = new List<NodeId>(backtrackOrFarmNodeIds).AsReadOnly();
            BlockedLinkedNodeIds = new List<NodeId>(blockedLinkedNodeIds).AsReadOnly();
        }

        public string CurrentLocationDisplayName { get; }

        public RegionId CurrentRegionId { get; }

        public NodeId CurrentNodeId { get; }

        public NodeState CurrentNodeState { get; }

        public int SelectableDestinationCount { get; }

        public IReadOnlyList<NodeId> ForwardRouteNodeIds { get; }

        public IReadOnlyList<NodeId> BacktrackOrFarmNodeIds { get; }

        public IReadOnlyList<NodeId> BlockedLinkedNodeIds { get; }
    }
}
