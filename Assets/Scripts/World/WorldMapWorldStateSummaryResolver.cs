using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldMapWorldStateSummaryResolver
    {
        private readonly WorldNodeStateResolver worldNodeStateResolver;

        public WorldMapWorldStateSummaryResolver(WorldNodeStateResolver worldNodeStateResolver = null)
        {
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
        }

        public WorldMapWorldStateSummary Resolve(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId currentContextNodeId,
            IReadOnlyCollection<NodeId> selectableNodeIds,
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

            if (forwardSelectableNodeIds == null)
            {
                throw new ArgumentNullException(nameof(forwardSelectableNodeIds));
            }

            WorldNode currentNode = worldGraph.GetNode(currentContextNodeId);
            WorldRegion currentRegion = worldGraph.GetRegion(currentNode.RegionId);
            HashSet<NodeId> forwardRouteSet = new HashSet<NodeId>(forwardSelectableNodeIds);

            List<NodeId> forwardRouteNodeIds = new List<NodeId>();
            List<NodeId> blockedLinkedNodeIds = new List<NodeId>();
            foreach (WorldNodeConnection connection in worldGraph.GetOutboundConnections(currentContextNodeId))
            {
                if (forwardRouteSet.Contains(connection.TargetNodeId))
                {
                    forwardRouteNodeIds.Add(connection.TargetNodeId);
                    continue;
                }

                blockedLinkedNodeIds.Add(connection.TargetNodeId);
            }

            List<NodeId> backtrackOrFarmNodeIds = new List<NodeId>();
            foreach (NodeId selectableNodeId in selectableNodeIds)
            {
                if (selectableNodeId == currentContextNodeId || forwardRouteSet.Contains(selectableNodeId))
                {
                    continue;
                }

                backtrackOrFarmNodeIds.Add(selectableNodeId);
            }

            backtrackOrFarmNodeIds.Sort(CompareNodeIds);

            return new WorldMapWorldStateSummary(
                currentRegion.LocationIdentity.DisplayName,
                currentRegion.RegionId,
                currentContextNodeId,
                worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, currentContextNodeId),
                selectableNodeIds.Count,
                forwardRouteNodeIds,
                backtrackOrFarmNodeIds,
                blockedLinkedNodeIds);
        }

        private static int CompareNodeIds(NodeId left, NodeId right)
        {
            return StringComparer.Ordinal.Compare(left.Value, right.Value);
        }
    }
}
