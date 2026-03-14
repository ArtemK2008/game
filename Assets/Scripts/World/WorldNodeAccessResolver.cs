using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class WorldNodeAccessResolver
    {
        private readonly NodeReachabilityResolver nodeReachabilityResolver;

        public WorldNodeAccessResolver(NodeReachabilityResolver nodeReachabilityResolver = null)
        {
            this.nodeReachabilityResolver = nodeReachabilityResolver ?? new NodeReachabilityResolver();
        }

        public IReadOnlyList<WorldNode> GetEnterableNodes(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            List<WorldNode> enterableNodes = new List<WorldNode>();
            HashSet<NodeId> addedNodeIds = new HashSet<NodeId>();

            AddReachableNodes(worldGraph, worldState, addedNodeIds, enterableNodes);
            AddFarmAccessibleClearedNodes(worldGraph, worldState, addedNodeIds, enterableNodes);

            return enterableNodes;
        }

        private void AddReachableNodes(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            HashSet<NodeId> addedNodeIds,
            List<WorldNode> enterableNodes)
        {
            foreach (WorldNode reachableNode in nodeReachabilityResolver.GetReachableNodes(worldGraph, worldState))
            {
                if (addedNodeIds.Add(reachableNode.NodeId))
                {
                    enterableNodes.Add(reachableNode);
                }
            }
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
    }
}
