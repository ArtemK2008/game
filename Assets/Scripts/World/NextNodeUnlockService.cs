using System;

namespace Survivalon.Runtime
{
    public sealed class NextNodeUnlockService
    {
        public int UnlockConnectedNodesWhenSourceClears(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId sourceNodeId)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (!IsClearedOrBetter(ResolveNodeState(worldGraph, worldState, sourceNodeId)))
            {
                return 0;
            }

            int unlockedNodeCount = 0;
            foreach (WorldNodeConnection connection in worldGraph.GetOutboundConnections(sourceNodeId))
            {
                WorldNode targetNode = worldGraph.GetNode(connection.TargetNodeId);
                if (ResolveNodeState(worldGraph, worldState, targetNode.NodeId) != NodeState.Locked)
                {
                    continue;
                }

                PersistentNodeState persistentNodeState = worldState.GetOrAddNodeState(
                    targetNode.NodeId,
                    ResolvePersistentThreshold(targetNode.NodeType),
                    NodeState.Locked);

                if (persistentNodeState.State != NodeState.Locked)
                {
                    continue;
                }

                persistentNodeState.MarkAvailable();
                unlockedNodeCount++;
            }

            return unlockedNodeCount;
        }

        private static NodeState ResolveNodeState(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId nodeId)
        {
            return worldState.TryGetNodeState(nodeId, out PersistentNodeState nodeState)
                ? nodeState.State
                : worldGraph.GetNode(nodeId).State;
        }

        private static bool IsClearedOrBetter(NodeState state)
        {
            return state == NodeState.Cleared || state == NodeState.Mastered;
        }

        private static int ResolvePersistentThreshold(NodeType nodeType)
        {
            int trackedThreshold = NodeProgressMeterService.GetDefaultThreshold(nodeType);
            return trackedThreshold > 0 ? trackedThreshold : 1;
        }
    }
}
