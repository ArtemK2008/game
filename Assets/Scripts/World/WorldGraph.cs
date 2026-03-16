using System;
using System.Collections.Generic;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.World
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
            regionsById = WorldGraphValidation.CreateRegionLookup(this.regions);
            nodesById = WorldGraphValidation.CreateNodeLookup(this.nodes);
            outboundConnectionsByNodeId = WorldGraphValidation.CreateConnectionLookup(this.nodes, this.connections);

            WorldGraphValidation.ValidateRegionMembership(this.regions, this.nodes, regionsById, nodesById);
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
            return WorldGraphTraversal.GetReachableNodes(startNodeId, nodesById, outboundConnectionsByNodeId);
        }

        public bool CanReach(NodeId startNodeId, NodeId targetNodeId)
        {
            return WorldGraphTraversal.CanReach(startNodeId, targetNodeId, nodesById, outboundConnectionsByNodeId);
        }
    }
}
