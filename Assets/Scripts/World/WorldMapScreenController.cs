using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldMapScreenController
    {
        private readonly WorldGraph worldGraph;
        private readonly PersistentWorldState worldState;
        private readonly NodeReachabilityResolver nodeReachabilityResolver;
        private readonly WorldNodeAccessResolver worldNodeAccessResolver;
        private readonly WorldNodeStateResolver worldNodeStateResolver;
        private readonly WorldMapWorldStateSummaryResolver worldStateSummaryResolver;
        private readonly WorldNodeDisplayNameResolver worldNodeDisplayNameResolver;
        private readonly SessionContextState sessionContext;
        private readonly Dictionary<RegionId, int> regionOrderById;
        private bool hasSelectedNode;
        private NodeId selectedNodeId;

        public WorldMapScreenController(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeReachabilityResolver nodeReachabilityResolver = null,
            SessionContextState sessionContext = null,
            WorldNodeAccessResolver worldNodeAccessResolver = null,
            WorldNodeStateResolver worldNodeStateResolver = null)
        {
            this.worldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            this.worldState = worldState ?? throw new ArgumentNullException(nameof(worldState));
            this.nodeReachabilityResolver = nodeReachabilityResolver ?? new NodeReachabilityResolver();
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
            this.worldNodeAccessResolver = worldNodeAccessResolver
                ?? new WorldNodeAccessResolver(this.nodeReachabilityResolver, this.worldNodeStateResolver);
            worldNodeDisplayNameResolver = new WorldNodeDisplayNameResolver();
            worldStateSummaryResolver = new WorldMapWorldStateSummaryResolver(
                this.worldNodeStateResolver,
                worldNodeDisplayNameResolver);
            this.sessionContext = sessionContext;
            regionOrderById = CreateRegionOrderLookup(worldGraph);
            this.sessionContext?.SeedFromWorldState(worldState);
        }

        public bool HasSelection => hasSelectedNode;

        public bool HasForwardRouteChoice => BuildAccessState().ForwardSelectableNodeIds.Count > 1;

        public int ForwardSelectableNodeCount => BuildAccessState().ForwardSelectableNodeIds.Count;

        public SessionContextState SessionContext => sessionContext;

        public NodeId SelectedNodeId => hasSelectedNode
            ? selectedNodeId
            : throw new InvalidOperationException("No world map node is currently selected.");

        public IReadOnlyList<WorldMapNodeOption> BuildNodeOptions()
        {
            NodeId currentContextNodeId = ResolveCurrentContextNodeId();
            WorldMapAccessState accessState = BuildAccessState();
            List<WorldNode> orderedNodes = new List<WorldNode>(worldGraph.Nodes);
            orderedNodes.Sort(CompareNodes);

            List<WorldMapNodeOption> nodeOptions = new List<WorldMapNodeOption>(orderedNodes.Count);
            foreach (WorldNode node in orderedNodes)
            {
                WorldRegion region = worldGraph.GetRegion(node.RegionId);
                nodeOptions.Add(new WorldMapNodeOption(
                    node.NodeId,
                    node.RegionId,
                    node.NodeType,
                    worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, node.NodeId),
                    accessState.SelectableNodeIds.Contains(node.NodeId),
                    node.NodeId == currentContextNodeId,
                    hasSelectedNode && node.NodeId == selectedNodeId,
                    region.LocationIdentity.DisplayName,
                    worldNodeDisplayNameResolver.Resolve(node),
                    ResolvePathRole(node.NodeId, currentContextNodeId, accessState)));
            }

            return nodeOptions;
        }

        public WorldMapWorldStateSummary BuildWorldStateSummary()
        {
            NodeId currentContextNodeId = ResolveCurrentContextNodeId();
            WorldMapAccessState accessState = BuildAccessState();
            return worldStateSummaryResolver.Resolve(
                worldGraph,
                worldState,
                currentContextNodeId,
                accessState.SelectableNodeIds,
                accessState.PathSelectableNodeIds,
                accessState.ForwardSelectableNodeIds);
        }

        public bool TrySelectNode(NodeId nodeId)
        {
            WorldMapAccessState accessState = BuildAccessState();
            if (!accessState.SelectableNodeIds.Contains(nodeId))
            {
                return false;
            }

            selectedNodeId = nodeId;
            hasSelectedNode = true;
            sessionContext?.RecordSelection(nodeId, accessState.ForwardSelectableNodeIds.Contains(nodeId));
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
            List<NodeId> nodeIds = new List<NodeId>(BuildAccessState().ForwardSelectableNodeIds);
            nodeIds.Sort((left, right) => StringComparer.Ordinal.Compare(left.Value, right.Value));
            return nodeIds;
        }

        public string ResolveSelectedNodeDisplayName()
        {
            if (!hasSelectedNode)
            {
                return null;
            }

            return worldNodeDisplayNameResolver.Resolve(worldGraph, selectedNodeId);
        }

        private WorldMapAccessState BuildAccessState()
        {
            return new WorldMapAccessState(
                CreateSelectableNodeIdSet(worldNodeAccessResolver.GetEnterableNodes(worldGraph, worldState)),
                CreateSelectableNodeIdSet(worldNodeAccessResolver.GetPathEnterableNodes(worldGraph, worldState)),
                CreateSelectableNodeIdSet(worldNodeAccessResolver.GetForwardEnterableNodes(worldGraph, worldState)));
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

        private static WorldMapPathRole ResolvePathRole(
            NodeId nodeId,
            NodeId currentContextNodeId,
            WorldMapAccessState accessState)
        {
            if (nodeId == currentContextNodeId)
            {
                return WorldMapPathRole.CurrentContext;
            }

            if (accessState.ForwardSelectableNodeIds.Contains(nodeId))
            {
                return WorldMapPathRole.ForwardRoute;
            }

            if (accessState.SelectableNodeIds.Contains(nodeId))
            {
                return accessState.PathSelectableNodeIds.Contains(nodeId)
                    ? WorldMapPathRole.BacktrackRoute
                    : WorldMapPathRole.ReplayableFarmNode;
            }

            return WorldMapPathRole.BlockedPath;
        }

        private sealed class WorldMapAccessState
        {
            public WorldMapAccessState(
                HashSet<NodeId> selectableNodeIds,
                HashSet<NodeId> pathSelectableNodeIds,
                HashSet<NodeId> forwardSelectableNodeIds)
            {
                SelectableNodeIds = selectableNodeIds ?? throw new ArgumentNullException(nameof(selectableNodeIds));
                PathSelectableNodeIds = pathSelectableNodeIds ?? throw new ArgumentNullException(nameof(pathSelectableNodeIds));
                ForwardSelectableNodeIds = forwardSelectableNodeIds ?? throw new ArgumentNullException(nameof(forwardSelectableNodeIds));
            }

            public HashSet<NodeId> SelectableNodeIds { get; }

            public HashSet<NodeId> PathSelectableNodeIds { get; }

            public HashSet<NodeId> ForwardSelectableNodeIds { get; }
        }
    }
}

