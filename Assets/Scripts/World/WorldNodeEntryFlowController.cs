using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldNodeEntryFlowController
    {
        private readonly WorldGraph worldGraph;
        private readonly PersistentWorldState worldState;
        private readonly WorldNodeAccessResolver worldNodeAccessResolver;
        private readonly WorldNodeStateResolver worldNodeStateResolver;

        public WorldNodeEntryFlowController(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeReachabilityResolver nodeReachabilityResolver = null,
            WorldNodeAccessResolver worldNodeAccessResolver = null,
            WorldNodeStateResolver worldNodeStateResolver = null)
        {
            this.worldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            this.worldState = worldState ?? throw new ArgumentNullException(nameof(worldState));
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
            this.worldNodeAccessResolver = worldNodeAccessResolver
                ?? new WorldNodeAccessResolver(nodeReachabilityResolver, this.worldNodeStateResolver);
        }

        public bool TryEnterNode(NodeId nodeId, out NodePlaceholderState placeholderState)
        {
            if (!worldNodeAccessResolver.TryGetEnterableNode(worldGraph, worldState, nodeId, out WorldNode enterableNode))
            {
                placeholderState = null;
                return false;
            }

            NodeId originNodeId = ResolveCurrentContextNodeId();
            WorldRegion enterableRegion = worldGraph.GetRegion(enterableNode.RegionId);
            worldState.SetCurrentNode(enterableNode.NodeId);
            worldState.SetLastSafeNode(originNodeId);
            worldState.ReplaceReachableNodes(BuildUpdatedReachableNodes(originNodeId));

            placeholderState = new NodePlaceholderState(
                enterableNode.NodeId,
                enterableNode.RegionId,
                enterableNode.NodeType,
                worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, enterableNode.NodeId),
                originNodeId,
                enterableNode.CombatEncounter,
                enterableNode.BossProgressionGate,
                enterableNode.TownServiceContext,
                enterableRegion.LocationIdentity,
                enterableNode.BossRewardContent,
                enterableNode.RegionMaterialYieldContent);
            return true;
        }

        private IEnumerable<NodeId> BuildUpdatedReachableNodes(NodeId originNodeId)
        {
            List<NodeId> updatedReachableNodeIds = new List<NodeId> { originNodeId };

            foreach (string reachableNodeIdValue in worldState.ReachableNodeIdValues)
            {
                updatedReachableNodeIds.Add(new NodeId(reachableNodeIdValue));
            }

            return updatedReachableNodeIds;
        }

        private NodeId ResolveCurrentContextNodeId()
        {
            if (worldState.HasCurrentNode)
            {
                return worldState.CurrentNodeId;
            }

            if (worldState.HasLastSafeNode)
            {
                return worldState.LastSafeNodeId;
            }

            throw new InvalidOperationException("World node entry requires a current node or last safe node.");
        }
    }
}

