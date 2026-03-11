using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class WorldGraph
    {
        private readonly List<WorldRegion> regions;
        private readonly List<WorldNode> nodes;
        private readonly List<WorldNodeConnection> connections;
        private readonly Dictionary<RegionId, WorldRegion> regionsById;
        private readonly Dictionary<NodeId, WorldNode> nodesById;
        private readonly Dictionary<NodeId, List<WorldNodeConnection>> outboundConnectionsByNodeId;

        public WorldGraph(
            IEnumerable<WorldRegion> regions,
            IEnumerable<WorldNode> nodes,
            IEnumerable<WorldNodeConnection> connections)
        {
            if (regions == null)
            {
                throw new ArgumentNullException(nameof(regions));
            }

            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (connections == null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            this.regions = new List<WorldRegion>(regions);
            this.nodes = new List<WorldNode>(nodes);
            this.connections = new List<WorldNodeConnection>(connections);
            regionsById = CreateRegionLookup(this.regions);
            nodesById = CreateNodeLookup(this.nodes);
            outboundConnectionsByNodeId = CreateConnectionLookup(this.nodes, this.connections);

            ValidateRegionMembership();
        }

        public IReadOnlyList<WorldRegion> Regions => regions;

        public IReadOnlyList<WorldNode> Nodes => nodes;

        public IReadOnlyList<WorldNodeConnection> Connections => connections;

        public WorldNode GetNode(NodeId nodeId)
        {
            if (!nodesById.TryGetValue(nodeId, out WorldNode node))
            {
                throw new KeyNotFoundException($"World node '{nodeId}' was not found.");
            }

            return node;
        }

        public IReadOnlyList<WorldNodeConnection> GetOutboundConnections(NodeId sourceNodeId)
        {
            if (!nodesById.ContainsKey(sourceNodeId))
            {
                throw new KeyNotFoundException($"World node '{sourceNodeId}' was not found.");
            }

            return outboundConnectionsByNodeId[sourceNodeId];
        }

        public IReadOnlyList<WorldNode> GetReachableNodes(NodeId startNodeId)
        {
            WorldNode startNode = GetNode(startNodeId);
            if (!IsTraversable(startNode.State))
            {
                return Array.Empty<WorldNode>();
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

        public bool CanReach(NodeId startNodeId, NodeId targetNodeId)
        {
            if (startNodeId == targetNodeId)
            {
                return IsTraversable(GetNode(startNodeId).State);
            }

            IReadOnlyList<WorldNode> reachableNodes = GetReachableNodes(startNodeId);
            foreach (WorldNode reachableNode in reachableNodes)
            {
                if (reachableNode.NodeId == targetNodeId)
                {
                    return true;
                }
            }

            return false;
        }

        private static Dictionary<RegionId, WorldRegion> CreateRegionLookup(IReadOnlyList<WorldRegion> regions)
        {
            Dictionary<RegionId, WorldRegion> regionsById = new Dictionary<RegionId, WorldRegion>();

            foreach (WorldRegion region in regions)
            {
                if (!regionsById.TryAdd(region.RegionId, region))
                {
                    throw new ArgumentException($"Duplicate region id '{region.RegionId}' was provided.", nameof(regions));
                }
            }

            return regionsById;
        }

        private static Dictionary<NodeId, WorldNode> CreateNodeLookup(IReadOnlyList<WorldNode> nodes)
        {
            Dictionary<NodeId, WorldNode> nodesById = new Dictionary<NodeId, WorldNode>();

            foreach (WorldNode node in nodes)
            {
                if (!nodesById.TryAdd(node.NodeId, node))
                {
                    throw new ArgumentException($"Duplicate node id '{node.NodeId}' was provided.", nameof(nodes));
                }
            }

            return nodesById;
        }

        private static Dictionary<NodeId, List<WorldNodeConnection>> CreateConnectionLookup(
            IReadOnlyList<WorldNode> nodes,
            IReadOnlyList<WorldNodeConnection> connections)
        {
            Dictionary<NodeId, List<WorldNodeConnection>> connectionLookup = new Dictionary<NodeId, List<WorldNodeConnection>>();

            foreach (WorldNode node in nodes)
            {
                connectionLookup[node.NodeId] = new List<WorldNodeConnection>();
            }

            foreach (WorldNodeConnection connection in connections)
            {
                if (!connectionLookup.ContainsKey(connection.SourceNodeId))
                {
                    throw new ArgumentException($"Connection source node '{connection.SourceNodeId}' does not exist.", nameof(connections));
                }

                if (!connectionLookup.ContainsKey(connection.TargetNodeId))
                {
                    throw new ArgumentException($"Connection target node '{connection.TargetNodeId}' does not exist.", nameof(connections));
                }

                connectionLookup[connection.SourceNodeId].Add(connection);
            }

            return connectionLookup;
        }

        private static bool IsTraversable(NodeState state)
        {
            return state != NodeState.Locked;
        }

        private void ValidateRegionMembership()
        {
            foreach (WorldRegion region in regions)
            {
                foreach (NodeId nodeId in region.NodeIds)
                {
                    WorldNode node = GetNode(nodeId);
                    if (node.RegionId != region.RegionId)
                    {
                        throw new ArgumentException(
                            $"Node '{node.NodeId}' belongs to region '{node.RegionId}', but region '{region.RegionId}' references it.");
                    }
                }
            }

            foreach (WorldNode node in nodes)
            {
                if (!regionsById.TryGetValue(node.RegionId, out WorldRegion region))
                {
                    throw new ArgumentException($"Node '{node.NodeId}' references missing region '{node.RegionId}'.");
                }

                bool isDeclaredInRegion = false;
                foreach (NodeId nodeId in region.NodeIds)
                {
                    if (nodeId == node.NodeId)
                    {
                        isDeclaredInRegion = true;
                        break;
                    }
                }

                if (!isDeclaredInRegion)
                {
                    throw new ArgumentException(
                        $"Node '{node.NodeId}' is not listed in region '{region.RegionId}'.");
                }
            }
        }
    }
}
