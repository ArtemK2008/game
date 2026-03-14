using System.Collections.Generic;

namespace Survivalon.Runtime
{
    internal static class WorldGraphTraversal
    {
        public static IReadOnlyList<WorldNode> GetReachableNodes(
            NodeId startNodeId,
            IReadOnlyDictionary<NodeId, WorldNode> nodesById,
            IReadOnlyDictionary<NodeId, List<WorldNodeConnection>> outboundConnectionsByNodeId)
        {
            WorldNode startNode = GetNode(startNodeId, nodesById);
            if (!IsTraversable(startNode.State))
            {
                return System.Array.Empty<WorldNode>();
            }

            Queue<NodeId> pendingNodeIds = new Queue<NodeId>();
            HashSet<NodeId> visitedNodeIds = new HashSet<NodeId> { startNodeId };
            List<WorldNode> reachableNodes = new List<WorldNode>();

            pendingNodeIds.Enqueue(startNodeId);

            while (pendingNodeIds.Count > 0)
            {
                NodeId currentNodeId = pendingNodeIds.Dequeue();

                foreach (WorldNodeConnection connection in outboundConnectionsByNodeId[currentNodeId])
                {
                    if (visitedNodeIds.Contains(connection.TargetNodeId))
                    {
                        continue;
                    }

                    WorldNode connectedNode = nodesById[connection.TargetNodeId];
                    if (!IsTraversable(connectedNode.State))
                    {
                        continue;
                    }

                    visitedNodeIds.Add(connection.TargetNodeId);
                    pendingNodeIds.Enqueue(connection.TargetNodeId);
                    reachableNodes.Add(connectedNode);
                }
            }

            return reachableNodes;
        }

        public static bool CanReach(
            NodeId startNodeId,
            NodeId targetNodeId,
            IReadOnlyDictionary<NodeId, WorldNode> nodesById,
            IReadOnlyDictionary<NodeId, List<WorldNodeConnection>> outboundConnectionsByNodeId)
        {
            if (startNodeId == targetNodeId)
            {
                return IsTraversable(GetNode(startNodeId, nodesById).State);
            }

            IReadOnlyList<WorldNode> reachableNodes = GetReachableNodes(
                startNodeId,
                nodesById,
                outboundConnectionsByNodeId);
            foreach (WorldNode reachableNode in reachableNodes)
            {
                if (reachableNode.NodeId == targetNodeId)
                {
                    return true;
                }
            }

            return false;
        }

        private static WorldNode GetNode(NodeId nodeId, IReadOnlyDictionary<NodeId, WorldNode> nodesById)
        {
            if (!nodesById.TryGetValue(nodeId, out WorldNode node))
            {
                throw new KeyNotFoundException($"World node '{nodeId}' was not found.");
            }

            return node;
        }

        private static bool IsTraversable(NodeState state)
        {
            return state != NodeState.Locked;
        }
    }
}
