using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class WorldNodeEntryFlowController
    {
        private readonly WorldGraph worldGraph;
        private readonly PersistentWorldState worldState;
        private readonly WorldNodeAccessResolver worldNodeAccessResolver;

        public WorldNodeEntryFlowController(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeReachabilityResolver nodeReachabilityResolver = null,
            WorldNodeAccessResolver worldNodeAccessResolver = null)
        {
            this.worldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            this.worldState = worldState ?? throw new ArgumentNullException(nameof(worldState));
            this.worldNodeAccessResolver = worldNodeAccessResolver ?? new WorldNodeAccessResolver(nodeReachabilityResolver);
        }

        public bool TryEnterNode(NodeId nodeId, out NodePlaceholderState placeholderState)
        {
            foreach (WorldNode reachableNode in worldNodeAccessResolver.GetEnterableNodes(worldGraph, worldState))
            {
                if (reachableNode.NodeId != nodeId)
                {
                    continue;
                }

                NodeId originNodeId = ResolveCurrentContextNodeId();
                worldState.SetCurrentNode(reachableNode.NodeId);
                worldState.SetLastSafeNode(originNodeId);
                worldState.ReplaceReachableNodes(BuildUpdatedReachableNodes(originNodeId));
                NodeState placeholderNodeState = worldState.TryGetNodeState(reachableNode.NodeId, out PersistentNodeState persistentNodeState)
                    ? persistentNodeState.State
                    : reachableNode.State;

                placeholderState = new NodePlaceholderState(
                    reachableNode.NodeId,
                    reachableNode.RegionId,
                    reachableNode.NodeType,
                    placeholderNodeState,
                    originNodeId);
                return true;
            }

            placeholderState = null;
            return false;
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
