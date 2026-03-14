using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class BootstrapWorldMapFactory
    {
        private static readonly NodeId ForestEntryNodeId = new NodeId("region_001_node_001");
        private static readonly NodeId ForestPushNodeId = new NodeId("region_001_node_002");
        private static readonly NodeId ForestGateNodeId = new NodeId("region_001_node_003");
        private static readonly NodeId ForestFarmNodeId = new NodeId("region_001_node_004");
        private static readonly NodeId CavernServiceNodeId = new NodeId("region_002_node_001");
        private static readonly NodeId CavernGateNodeId = new NodeId("region_002_node_002");
        private static readonly RegionId ForestRegionId = new RegionId("region_001");
        private static readonly RegionId CavernRegionId = new RegionId("region_002");

        public WorldGraph CreateWorldGraph()
        {
            List<WorldNode> nodes = new List<WorldNode>
            {
                new WorldNode(ForestEntryNodeId, ForestRegionId, NodeType.Combat, NodeState.Cleared),
                new WorldNode(ForestPushNodeId, ForestRegionId, NodeType.Combat, NodeState.InProgress),
                new WorldNode(ForestGateNodeId, ForestRegionId, NodeType.BossOrGate, NodeState.Locked),
                new WorldNode(ForestFarmNodeId, ForestRegionId, NodeType.Combat, NodeState.Available),
                new WorldNode(CavernServiceNodeId, CavernRegionId, NodeType.ServiceOrProgression, NodeState.Available),
                new WorldNode(CavernGateNodeId, CavernRegionId, NodeType.BossOrGate, NodeState.Locked),
            };

            List<WorldRegion> regions = new List<WorldRegion>
            {
                new WorldRegion(
                    ForestRegionId,
                    0,
                    ForestEntryNodeId,
                    new[]
                    {
                        ForestEntryNodeId,
                        ForestPushNodeId,
                        ForestGateNodeId,
                        ForestFarmNodeId,
                    },
                    ResourceCategory.RegionMaterial,
                    "frontier"),
                new WorldRegion(
                    CavernRegionId,
                    1,
                    CavernServiceNodeId,
                    new[]
                    {
                        CavernServiceNodeId,
                        CavernGateNodeId,
                    },
                    ResourceCategory.PersistentProgressionMaterial,
                    "depths"),
            };

            List<WorldNodeConnection> connections = new List<WorldNodeConnection>
            {
                new WorldNodeConnection(ForestEntryNodeId, ForestPushNodeId),
                new WorldNodeConnection(ForestPushNodeId, ForestGateNodeId),
                new WorldNodeConnection(ForestPushNodeId, ForestFarmNodeId),
                new WorldNodeConnection(ForestPushNodeId, CavernServiceNodeId),
                new WorldNodeConnection(CavernServiceNodeId, CavernGateNodeId),
            };

            return new WorldGraph(regions, nodes, connections);
        }

        public PersistentGameState CreateGameState()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(ForestPushNodeId);
            gameState.WorldState.SetLastSafeNode(ForestEntryNodeId);
            gameState.WorldState.ReplaceReachableNodes(new[] { ForestEntryNodeId });
            gameState.WorldState.ReplaceNodeStates(CreatePersistentNodeStates());

            return gameState;
        }

        private static IEnumerable<PersistentNodeState> CreatePersistentNodeStates()
        {
            int combatThreshold = NodeProgressMeterService.GetDefaultThreshold(NodeType.Combat);
            int bossThreshold = NodeProgressMeterService.GetDefaultThreshold(NodeType.BossOrGate);

            return new[]
            {
                CreateNodeState(ForestEntryNodeId, combatThreshold, NodeState.Cleared, combatThreshold),
                CreateNodeState(ForestPushNodeId, combatThreshold, NodeState.InProgress, 1),
                CreateNodeState(ForestGateNodeId, bossThreshold, NodeState.Locked, 0),
                CreateNodeState(ForestFarmNodeId, combatThreshold, NodeState.Available, 0),
                CreateNodeState(CavernGateNodeId, bossThreshold, NodeState.Locked, 0),
            };
        }

        private static PersistentNodeState CreateNodeState(
            NodeId nodeId,
            int unlockThreshold,
            NodeState nodeState,
            int unlockProgress)
        {
            NodeState initialState = nodeState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
            PersistentNodeState persistentNodeState = new PersistentNodeState(nodeId, unlockThreshold, initialState);

            if (initialState != NodeState.Locked && unlockProgress > 0)
            {
                persistentNodeState.ApplyUnlockProgress(unlockProgress);
            }

            if (nodeState == NodeState.Mastered)
            {
                persistentNodeState.MarkMastered();
            }

            return persistentNodeState;
        }
    }
}
