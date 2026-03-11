using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class BootstrapWorldMapFactory
    {
        private static readonly NodeId ForestEntryNodeId = new NodeId("region_001_node_001");
        private static readonly NodeId ForestPushNodeId = new NodeId("region_001_node_002");
        private static readonly NodeId ForestGateNodeId = new NodeId("region_001_node_003");
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

            return gameState;
        }
    }
}
