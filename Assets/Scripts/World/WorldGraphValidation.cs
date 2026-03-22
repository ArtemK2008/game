using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.World
{
    internal static class WorldGraphValidation
    {
        public static Dictionary<RegionId, WorldRegion> CreateRegionLookup(IReadOnlyList<WorldRegion> regions)
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

        public static Dictionary<NodeId, WorldNode> CreateNodeLookup(IReadOnlyList<WorldNode> nodes)
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

        public static Dictionary<NodeId, List<WorldNodeConnection>> CreateConnectionLookup(
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

        public static void ValidateRegionMembership(
            IReadOnlyList<WorldRegion> regions,
            IReadOnlyList<WorldNode> nodes,
            IReadOnlyDictionary<RegionId, WorldRegion> regionsById,
            IReadOnlyDictionary<NodeId, WorldNode> nodesById)
        {
            foreach (WorldRegion region in regions)
            {
                foreach (NodeId nodeId in region.NodeIds)
                {
                    WorldNode node = GetNode(nodeId, nodesById);
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

                if (node.RegionMaterialYieldContent != null &&
                    !RegionMaterialRewardSupportResolver.Supports(node.NodeType, region.ResourceCategory))
                {
                    throw new ArgumentException(
                        $"Node '{node.NodeId}' declares region material yield content, but region '{region.RegionId}' does not support region material rewards.");
                }
            }
        }

        private static WorldNode GetNode(NodeId nodeId, IReadOnlyDictionary<NodeId, WorldNode> nodesById)
        {
            if (!nodesById.TryGetValue(nodeId, out WorldNode node))
            {
                throw new KeyNotFoundException($"World node '{nodeId}' was not found.");
            }

            return node;
        }
    }
}

