using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Combat;

namespace Survivalon.World
{
    public sealed class BootstrapWorldGraphBuilder
    {
        public WorldGraph Create()
        {
            List<WorldNode> nodes = new List<WorldNode>
            {
                new WorldNode(
                    BootstrapWorldScenario.ForestEntryNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.Cleared,
                    CombatEncounterCatalog.EnemyUnitEncounter),
                new WorldNode(
                    BootstrapWorldScenario.ForestPushNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.InProgress,
                    CombatEncounterCatalog.BulwarkRaiderEncounter),
                new WorldNode(
                    BootstrapWorldScenario.ForestGateNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.BossOrGate,
                    NodeState.Locked,
                    CombatEncounterCatalog.GatePlaceholderEncounter),
                new WorldNode(
                    BootstrapWorldScenario.ForestFarmNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    CombatEncounterCatalog.EnemyUnitEncounter),
                new WorldNode(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernRegionId, NodeType.ServiceOrProgression, NodeState.Available),
                new WorldNode(
                    BootstrapWorldScenario.CavernGateNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.BossOrGate,
                    NodeState.Locked,
                    CombatEncounterCatalog.GatePlaceholderEncounter),
            };

            List<WorldRegion> regions = new List<WorldRegion>
            {
                new WorldRegion(
                    BootstrapWorldScenario.ForestRegionId,
                    0,
                    BootstrapWorldScenario.ForestEntryNodeId,
                    new[]
                    {
                        BootstrapWorldScenario.ForestEntryNodeId,
                        BootstrapWorldScenario.ForestPushNodeId,
                        BootstrapWorldScenario.ForestGateNodeId,
                        BootstrapWorldScenario.ForestFarmNodeId,
                    },
                    ResourceCategory.RegionMaterial,
                    "frontier"),
                new WorldRegion(
                    BootstrapWorldScenario.CavernRegionId,
                    1,
                    BootstrapWorldScenario.CavernServiceNodeId,
                    new[]
                    {
                        BootstrapWorldScenario.CavernServiceNodeId,
                        BootstrapWorldScenario.CavernGateNodeId,
                    },
                    ResourceCategory.PersistentProgressionMaterial,
                    "depths"),
            };

            List<WorldNodeConnection> connections = new List<WorldNodeConnection>
            {
                new WorldNodeConnection(BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestPushNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestFarmNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.CavernServiceNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernGateNodeId),
            };

            return new WorldGraph(regions, nodes, connections);
        }
    }
}

