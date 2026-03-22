using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class WorldMapWorldStateSummary
    {
        public WorldMapWorldStateSummary(
            string currentLocationDisplayName,
            WorldMapNodeReferenceDisplayState currentNode,
            NodeState currentNodeState,
            int selectableDestinationCount,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> forwardRouteNodes,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> backtrackRouteNodes,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> replayableFarmNodes,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> blockedLinkedNodes)
        {
            if (string.IsNullOrWhiteSpace(currentLocationDisplayName))
            {
                throw new ArgumentException(
                    "Current location display name cannot be null or whitespace.",
                    nameof(currentLocationDisplayName));
            }

            if (currentNode == null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            if (forwardRouteNodes == null)
            {
                throw new ArgumentNullException(nameof(forwardRouteNodes));
            }

            if (backtrackRouteNodes == null)
            {
                throw new ArgumentNullException(nameof(backtrackRouteNodes));
            }

            if (replayableFarmNodes == null)
            {
                throw new ArgumentNullException(nameof(replayableFarmNodes));
            }

            if (blockedLinkedNodes == null)
            {
                throw new ArgumentNullException(nameof(blockedLinkedNodes));
            }

            if (selectableDestinationCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(selectableDestinationCount),
                    selectableDestinationCount,
                    "Selectable destination count cannot be negative.");
            }

            CurrentLocationDisplayName = currentLocationDisplayName;
            CurrentNode = currentNode;
            CurrentNodeState = currentNodeState;
            SelectableDestinationCount = selectableDestinationCount;
            ForwardRouteNodes = new List<WorldMapNodeReferenceDisplayState>(forwardRouteNodes).AsReadOnly();
            BacktrackRouteNodes = new List<WorldMapNodeReferenceDisplayState>(backtrackRouteNodes).AsReadOnly();
            ReplayableFarmNodes = new List<WorldMapNodeReferenceDisplayState>(replayableFarmNodes).AsReadOnly();
            BlockedLinkedNodes = new List<WorldMapNodeReferenceDisplayState>(blockedLinkedNodes).AsReadOnly();
        }

        public string CurrentLocationDisplayName { get; }

        public WorldMapNodeReferenceDisplayState CurrentNode { get; }

        public NodeState CurrentNodeState { get; }

        public int SelectableDestinationCount { get; }

        public IReadOnlyList<WorldMapNodeReferenceDisplayState> ForwardRouteNodes { get; }

        public IReadOnlyList<WorldMapNodeReferenceDisplayState> BacktrackRouteNodes { get; }

        public IReadOnlyList<WorldMapNodeReferenceDisplayState> ReplayableFarmNodes { get; }

        public IReadOnlyList<WorldMapNodeReferenceDisplayState> BlockedLinkedNodes { get; }
    }

    public sealed class WorldMapNodeReferenceDisplayState
    {
        public WorldMapNodeReferenceDisplayState(NodeId nodeId, string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            NodeId = nodeId;
            DisplayName = displayName;
        }

        public NodeId NodeId { get; }

        public string DisplayName { get; }
    }
}
