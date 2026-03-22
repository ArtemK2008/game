using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldMapWorldStateSummaryResolver
    {
        private readonly WorldNodeStateResolver worldNodeStateResolver;
        private readonly WorldNodeDisplayNameResolver worldNodeDisplayNameResolver;

        public WorldMapWorldStateSummaryResolver(
            WorldNodeStateResolver worldNodeStateResolver = null,
            WorldNodeDisplayNameResolver worldNodeDisplayNameResolver = null)
        {
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
            this.worldNodeDisplayNameResolver = worldNodeDisplayNameResolver ?? new WorldNodeDisplayNameResolver();
        }

        public WorldMapWorldStateSummary Resolve(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId currentContextNodeId,
            IReadOnlyCollection<NodeId> selectableNodeIds,
            IReadOnlyCollection<NodeId> pathSelectableNodeIds,
            IReadOnlyCollection<NodeId> forwardSelectableNodeIds)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (selectableNodeIds == null)
            {
                throw new ArgumentNullException(nameof(selectableNodeIds));
            }

            if (pathSelectableNodeIds == null)
            {
                throw new ArgumentNullException(nameof(pathSelectableNodeIds));
            }

            if (forwardSelectableNodeIds == null)
            {
                throw new ArgumentNullException(nameof(forwardSelectableNodeIds));
            }

            WorldNode currentNode = worldGraph.GetNode(currentContextNodeId);
            WorldRegion currentRegion = worldGraph.GetRegion(currentNode.RegionId);
            HashSet<NodeId> forwardRouteSet = new HashSet<NodeId>(forwardSelectableNodeIds);
            HashSet<NodeId> pathRouteSet = new HashSet<NodeId>(pathSelectableNodeIds);

            List<WorldMapNodeReferenceDisplayState> forwardRouteNodes = new List<WorldMapNodeReferenceDisplayState>();
            List<WorldMapNodeReferenceDisplayState> blockedLinkedNodes = new List<WorldMapNodeReferenceDisplayState>();
            foreach (WorldNodeConnection connection in worldGraph.GetOutboundConnections(currentContextNodeId))
            {
                if (forwardRouteSet.Contains(connection.TargetNodeId))
                {
                    forwardRouteNodes.Add(CreateNodeReference(worldGraph, connection.TargetNodeId));
                    continue;
                }

                blockedLinkedNodes.Add(CreateNodeReference(worldGraph, connection.TargetNodeId));
            }

            List<WorldMapNodeReferenceDisplayState> backtrackRouteNodes = new List<WorldMapNodeReferenceDisplayState>();
            List<WorldMapNodeReferenceDisplayState> replayableFarmNodes = new List<WorldMapNodeReferenceDisplayState>();
            foreach (NodeId selectableNodeId in selectableNodeIds)
            {
                if (selectableNodeId == currentContextNodeId || forwardRouteSet.Contains(selectableNodeId))
                {
                    continue;
                }

                if (pathRouteSet.Contains(selectableNodeId))
                {
                    backtrackRouteNodes.Add(CreateNodeReference(worldGraph, selectableNodeId));
                    continue;
                }

                replayableFarmNodes.Add(CreateNodeReference(worldGraph, selectableNodeId));
            }

            forwardRouteNodes.Sort(CompareNodeReferences);
            backtrackRouteNodes.Sort(CompareNodeReferences);
            replayableFarmNodes.Sort(CompareNodeReferences);
            blockedLinkedNodes.Sort(CompareNodeReferences);

            return new WorldMapWorldStateSummary(
                currentRegion.LocationIdentity.DisplayName,
                CreateNodeReference(worldGraph, currentContextNodeId),
                worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, currentContextNodeId),
                selectableNodeIds.Count,
                forwardRouteNodes,
                backtrackRouteNodes,
                replayableFarmNodes,
                blockedLinkedNodes);
        }

        private WorldMapNodeReferenceDisplayState CreateNodeReference(WorldGraph worldGraph, NodeId nodeId)
        {
            return new WorldMapNodeReferenceDisplayState(
                nodeId,
                worldNodeDisplayNameResolver.Resolve(worldGraph, nodeId));
        }

        private static int CompareNodeReferences(
            WorldMapNodeReferenceDisplayState left,
            WorldMapNodeReferenceDisplayState right)
        {
            int displayNameComparison = StringComparer.Ordinal.Compare(left.DisplayName, right.DisplayName);
            if (displayNameComparison != 0)
            {
                return displayNameComparison;
            }

            return StringComparer.Ordinal.Compare(left.NodeId.Value, right.NodeId.Value);
        }
    }
}
