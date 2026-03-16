using System;
using System.Collections.Generic;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.World
{
    public sealed class NodeReachabilityResolver
    {
        public IReadOnlyList<WorldNode> GetReachableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            ValidateInputs(worldGraph, worldState);

            NodeId anchorNodeId = ResolveAnchorNodeId(worldState);
            List<WorldNode> reachableNodes = new List<WorldNode>();
            HashSet<NodeId> addedNodeIds = new HashSet<NodeId>();

            AddForwardReachableNodes(worldGraph, anchorNodeId, addedNodeIds, reachableNodes);
            AddBacktrackReachableNodes(worldGraph, worldState, anchorNodeId, addedNodeIds, reachableNodes);

            return reachableNodes;
        }

        public IReadOnlyList<WorldNode> GetForwardReachableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            ValidateInputs(worldGraph, worldState);

            NodeId anchorNodeId = ResolveAnchorNodeId(worldState);
            List<WorldNode> reachableNodes = new List<WorldNode>();
            HashSet<NodeId> addedNodeIds = new HashSet<NodeId>();

            AddForwardReachableNodes(worldGraph, anchorNodeId, addedNodeIds, reachableNodes);

            return reachableNodes;
        }

        public IReadOnlyList<WorldNode> GetBacktrackReachableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            ValidateInputs(worldGraph, worldState);

            NodeId anchorNodeId = ResolveAnchorNodeId(worldState);
            List<WorldNode> reachableNodes = new List<WorldNode>();
            HashSet<NodeId> addedNodeIds = new HashSet<NodeId>();

            AddBacktrackReachableNodes(worldGraph, worldState, anchorNodeId, addedNodeIds, reachableNodes);

            return reachableNodes;
        }

        private static void AddForwardReachableNodes(
            WorldGraph worldGraph,
            NodeId anchorNodeId,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> reachableNodes)
        {
            foreach (WorldNodeConnection connection in worldGraph.GetOutboundConnections(anchorNodeId))
            {
                TryAddReachableNode(worldGraph, connection.TargetNodeId, anchorNodeId, addedNodeIds, reachableNodes);
            }
        }

        private static void AddBacktrackReachableNodes(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId anchorNodeId,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> reachableNodes)
        {
            if (worldState.HasLastSafeNode)
            {
                TryAddReachableNode(worldGraph, worldState.LastSafeNodeId, anchorNodeId, addedNodeIds, reachableNodes);
            }

            foreach (string reachableNodeIdValue in worldState.ReachableNodeIdValues)
            {
                TryAddReachableNode(worldGraph, new NodeId(reachableNodeIdValue), anchorNodeId, addedNodeIds, reachableNodes);
            }
        }

        private static void TryAddReachableNode(
            WorldGraph worldGraph,
            NodeId candidateNodeId,
            NodeId anchorNodeId,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> reachableNodes)
        {
            if (candidateNodeId == anchorNodeId || addedNodeIds.Contains(candidateNodeId))
            {
                return;
            }

            addedNodeIds.Add(candidateNodeId);
            reachableNodes.Add(worldGraph.GetNode(candidateNodeId));
        }

        private static NodeId ResolveAnchorNodeId(PersistentWorldState worldState)
        {
            if (worldState.HasCurrentNode)
            {
                return worldState.CurrentNodeId;
            }

            if (worldState.HasLastSafeNode)
            {
                return worldState.LastSafeNodeId;
            }

            throw new InvalidOperationException("Current world context requires a current node or last safe node.");
        }

        private static void ValidateInputs(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }
        }
    }
}
