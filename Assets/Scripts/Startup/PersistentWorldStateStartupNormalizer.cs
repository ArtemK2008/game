using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Startup
{
    public sealed class PersistentWorldStateStartupNormalizer
    {
        private readonly NextNodeUnlockService nextNodeUnlockService;
        private readonly WorldNodeStateResolver worldNodeStateResolver;

        public PersistentWorldStateStartupNormalizer(
            NextNodeUnlockService nextNodeUnlockService = null,
            WorldNodeStateResolver worldNodeStateResolver = null)
        {
            this.nextNodeUnlockService = nextNodeUnlockService ?? new NextNodeUnlockService();
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
        }

        public void Normalize(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            ApplyCompletedNodeUnlocks(worldGraph, worldState);
            worldState.ReplaceReachableNodes(BuildNormalizedReachableNodes(worldGraph, worldState));
        }

        private void ApplyCompletedNodeUnlocks(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            bool unlockedAnyNode;
            do
            {
                unlockedAnyNode = false;
                foreach (WorldNode node in worldGraph.Nodes)
                {
                    if (!IsClearedOrBetter(worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, node.NodeId)))
                    {
                        continue;
                    }

                    unlockedAnyNode |= nextNodeUnlockService.UnlockConnectedNodesWhenSourceClears(
                        worldGraph,
                        worldState,
                        node.NodeId) > 0;
                    unlockedAnyNode |= TryUnlockBossProgressionGate(worldGraph, worldState, node);
                }
            }
            while (unlockedAnyNode);
        }

        private IEnumerable<NodeId> BuildNormalizedReachableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            HashSet<NodeId> validNodeIds = CreateValidNodeIdSet(worldGraph);
            HashSet<NodeId> reachableNodeIds = new HashSet<NodeId>();

            if (worldState.HasCurrentNode)
            {
                AddNodeIdIfKnown(validNodeIds, reachableNodeIds, worldState.CurrentNodeId);
            }

            if (worldState.HasLastSafeNode)
            {
                AddNodeIdIfKnown(validNodeIds, reachableNodeIds, worldState.LastSafeNodeId);
            }

            foreach (string reachableNodeIdValue in worldState.ReachableNodeIdValues)
            {
                AddNodeIdIfKnown(validNodeIds, reachableNodeIds, new NodeId(reachableNodeIdValue));
            }

            foreach (PersistentNodeState nodeState in worldState.NodeStates)
            {
                if (!nodeState.IsCompleted)
                {
                    continue;
                }

                AddNodeIdIfKnown(validNodeIds, reachableNodeIds, nodeState.NodeId);
            }

            List<NodeId> expansionSourceNodeIds = new List<NodeId>(reachableNodeIds);
            for (int index = 0; index < expansionSourceNodeIds.Count; index++)
            {
                NodeId sourceNodeId = expansionSourceNodeIds[index];
                if (worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, sourceNodeId) == NodeState.Locked)
                {
                    continue;
                }

                foreach (WorldNodeConnection connection in worldGraph.GetOutboundConnections(sourceNodeId))
                {
                    if (worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, connection.TargetNodeId) == NodeState.Locked)
                    {
                        continue;
                    }

                    reachableNodeIds.Add(connection.TargetNodeId);
                }
            }

            return reachableNodeIds;
        }

        private bool TryUnlockBossProgressionGate(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            WorldNode completedBossNode)
        {
            if (completedBossNode.BossProgressionGate == null)
            {
                return false;
            }

            NodeId unlockedNodeId = completedBossNode.BossProgressionGate.UnlockedNodeId;
            if (worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, unlockedNodeId) != NodeState.Locked)
            {
                return false;
            }

            WorldNode unlockedNode = worldGraph.GetNode(unlockedNodeId);
            PersistentNodeState persistentNodeState = worldState.GetOrAddNodeState(
                unlockedNode.NodeId,
                TrackedNodeProgressRules.GetDefaultThreshold(unlockedNode.NodeType),
                NodeState.Locked);

            if (persistentNodeState.State != NodeState.Locked)
            {
                return false;
            }

            persistentNodeState.MarkAvailable();
            return true;
        }

        private static HashSet<NodeId> CreateValidNodeIdSet(WorldGraph worldGraph)
        {
            HashSet<NodeId> validNodeIds = new HashSet<NodeId>();
            foreach (WorldNode node in worldGraph.Nodes)
            {
                validNodeIds.Add(node.NodeId);
            }

            return validNodeIds;
        }

        private static void AddNodeIdIfKnown(
            HashSet<NodeId> validNodeIds,
            HashSet<NodeId> reachableNodeIds,
            NodeId nodeId)
        {
            if (validNodeIds.Contains(nodeId))
            {
                reachableNodeIds.Add(nodeId);
            }
        }

        private static bool IsClearedOrBetter(NodeState state)
        {
            return state == NodeState.Cleared || state == NodeState.Mastered;
        }
    }
}
