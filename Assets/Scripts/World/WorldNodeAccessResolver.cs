using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldNodeAccessResolver
    {
        private readonly NodeReachabilityResolver nodeReachabilityResolver;
        private readonly WorldNodeStateResolver worldNodeStateResolver;

        public WorldNodeAccessResolver(
            NodeReachabilityResolver nodeReachabilityResolver = null,
            WorldNodeStateResolver worldNodeStateResolver = null)
        {
            this.nodeReachabilityResolver = nodeReachabilityResolver ?? new NodeReachabilityResolver();
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
        }

        public IReadOnlyList<WorldNode> GetEnterableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            ValidateInputs(worldGraph, worldState);

            List<WorldNode> enterableNodes = new List<WorldNode>();
            HashSet<NodeId> addedNodeIds = new HashSet<NodeId>();

            AddPathEnterableNodes(worldGraph, worldState, addedNodeIds, enterableNodes);
            AddFarmAccessibleClearedNodes(worldGraph, worldState, addedNodeIds, enterableNodes);

            return enterableNodes;
        }

        public IReadOnlyList<WorldNode> GetForwardEnterableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            ValidateInputs(worldGraph, worldState);

            List<WorldNode> enterableNodes = new List<WorldNode>();
            HashSet<NodeId> addedNodeIds = new HashSet<NodeId>();

            foreach (WorldNode reachableNode in nodeReachabilityResolver.GetForwardReachableNodes(worldGraph, worldState))
            {
                TryAddPathEnterableNode(worldGraph, worldState, reachableNode, addedNodeIds, enterableNodes);
            }

            return enterableNodes;
        }

        public bool TryGetEnterableNode(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId nodeId,
            out WorldNode enterableNode)
        {
            ValidateInputs(worldGraph, worldState);

            foreach (WorldNode node in GetEnterableNodes(worldGraph, worldState))
            {
                if (node.NodeId == nodeId)
                {
                    enterableNode = node;
                    return true;
                }
            }

            enterableNode = null;
            return false;
        }

        private void AddPathEnterableNodes(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> enterableNodes)
        {
            foreach (WorldNode reachableNode in nodeReachabilityResolver.GetReachableNodes(worldGraph, worldState))
            {
                TryAddPathEnterableNode(worldGraph, worldState, reachableNode, addedNodeIds, enterableNodes);
            }
        }

        private void TryAddPathEnterableNode(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            WorldNode reachableNode,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> enterableNodes)
        {
            if (!addedNodeIds.Add(reachableNode.NodeId))
            {
                return;
            }

            if (worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, reachableNode.NodeId) == NodeState.Locked)
            {
                addedNodeIds.Remove(reachableNode.NodeId);
                return;
            }

            enterableNodes.Add(reachableNode);
        }

        private static void AddFarmAccessibleClearedNodes(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> enterableNodes)
        {
            foreach (PersistentNodeState persistentNodeState in worldState.NodeStates)
            {
                if (persistentNodeState.State != NodeState.Cleared)
                {
                    continue;
                }

                if (!addedNodeIds.Add(persistentNodeState.NodeId))
                {
                    continue;
                }

                enterableNodes.Add(worldGraph.GetNode(persistentNodeState.NodeId));
            }
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

