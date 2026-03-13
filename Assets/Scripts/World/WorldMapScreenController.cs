using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class WorldMapScreenController
    {
        private readonly WorldGraph worldGraph;
        private readonly PersistentWorldState worldState;
        private readonly NodeReachabilityResolver nodeReachabilityResolver;
        private readonly SessionContextState sessionContext;
        private readonly Dictionary<RegionId, int> regionOrderById;
        private readonly HashSet<NodeId> forwardSelectableNodeIds;
        private readonly HashSet<NodeId> selectableNodeIds;
        private bool hasSelectedNode;
        private NodeId selectedNodeId;

        public WorldMapScreenController(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeReachabilityResolver nodeReachabilityResolver = null,
            SessionContextState sessionContext = null)
        {
            this.worldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            this.worldState = worldState ?? throw new ArgumentNullException(nameof(worldState));
            this.nodeReachabilityResolver = nodeReachabilityResolver ?? new NodeReachabilityResolver();
            this.sessionContext = sessionContext;
            regionOrderById = CreateRegionOrderLookup(worldGraph);
            forwardSelectableNodeIds = CreateSelectableNodeIdSet(
                this.nodeReachabilityResolver.GetForwardReachableNodes(worldGraph, worldState));
            selectableNodeIds = CreateSelectableNodeIdSet(
                this.nodeReachabilityResolver.GetReachableNodes(worldGraph, worldState));
            this.sessionContext?.SeedFromWorldState(worldState);
        }

        public bool HasSelection => hasSelectedNode;

        public bool HasForwardRouteChoice => forwardSelectableNodeIds.Count > 1;

        public int ForwardSelectableNodeCount => forwardSelectableNodeIds.Count;

        public SessionContextState SessionContext => sessionContext;

        public NodeId SelectedNodeId => hasSelectedNode
            ? selectedNodeId
            : throw new InvalidOperationException("No world map node is currently selected.");

        public IReadOnlyList<WorldMapNodeOption> BuildNodeOptions()
        {
            NodeId currentContextNodeId = ResolveCurrentContextNodeId();
            List<WorldNode> orderedNodes = new List<WorldNode>(worldGraph.Nodes);
            orderedNodes.Sort(CompareNodes);

            List<WorldMapNodeOption> nodeOptions = new List<WorldMapNodeOption>(orderedNodes.Count);
            foreach (WorldNode node in orderedNodes)
            {
                nodeOptions.Add(new WorldMapNodeOption(
                    node.NodeId,
                    node.RegionId,
                    node.NodeType,
                    node.State,
                    selectableNodeIds.Contains(node.NodeId),
                    node.NodeId == currentContextNodeId,
                    hasSelectedNode && node.NodeId == selectedNodeId));
            }

            return nodeOptions;
        }

        public bool TrySelectNode(NodeId nodeId)
        {
            if (!selectableNodeIds.Contains(nodeId))
            {
                return false;
            }

            selectedNodeId = nodeId;
            hasSelectedNode = true;
            sessionContext?.RecordSelection(nodeId, forwardSelectableNodeIds.Contains(nodeId));
            return true;
        }

        public bool TryGetSelectedNodeId(out NodeId nodeId)
        {
            if (hasSelectedNode)
            {
                nodeId = selectedNodeId;
                return true;
            }

            nodeId = default;
            return false;
        }

        public IReadOnlyList<NodeId> GetForwardSelectableNodeIds()
        {
            List<NodeId> nodeIds = new List<NodeId>(forwardSelectableNodeIds);
            nodeIds.Sort((left, right) => StringComparer.Ordinal.Compare(left.Value, right.Value));
            return nodeIds;
        }

        private static Dictionary<RegionId, int> CreateRegionOrderLookup(WorldGraph worldGraph)
        {
            Dictionary<RegionId, int> regionOrderById = new Dictionary<RegionId, int>();
            foreach (WorldRegion region in worldGraph.Regions)
            {
                regionOrderById[region.RegionId] = region.ProgressionOrder;
            }

            return regionOrderById;
        }

        private static HashSet<NodeId> CreateSelectableNodeIdSet(IEnumerable<WorldNode> reachableNodes)
        {
            if (reachableNodes == null)
            {
                throw new ArgumentNullException(nameof(reachableNodes));
            }

            HashSet<NodeId> selectableNodeIds = new HashSet<NodeId>();
            foreach (WorldNode reachableNode in reachableNodes)
            {
                selectableNodeIds.Add(reachableNode.NodeId);
            }

            return selectableNodeIds;
        }

        private int CompareNodes(WorldNode left, WorldNode right)
        {
            int regionOrderComparison = GetRegionOrder(left.RegionId).CompareTo(GetRegionOrder(right.RegionId));
            if (regionOrderComparison != 0)
            {
                return regionOrderComparison;
            }

            return StringComparer.Ordinal.Compare(left.NodeId.Value, right.NodeId.Value);
        }

        private int GetRegionOrder(RegionId regionId)
        {
            return regionOrderById.TryGetValue(regionId, out int regionOrder)
                ? regionOrder
                : int.MaxValue;
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

            throw new InvalidOperationException("World map requires a current node or last safe node.");
        }
    }
}
