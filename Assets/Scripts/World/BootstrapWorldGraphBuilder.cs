using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Towns;
using Survivalon.Data.World;

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
                    CombatStandardEncounterCatalog.EnemyUnitEncounter,
                    displayName: "Frontier Entry"),
                new WorldNode(
                    BootstrapWorldScenario.ForestPushNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.InProgress,
                    CombatStandardEncounterCatalog.BulwarkRaiderEncounter,
                    displayName: "Raider Trail"),
                new WorldNode(
                    BootstrapWorldScenario.ForestGateNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.BossOrGate,
                    NodeState.Locked,
                    CombatBossEncounterCatalog.GateBossEncounter,
                    new BossProgressionGateDefinition(BootstrapWorldScenario.CavernGateNodeId),
                    displayName: "Frontier Gate"),
                new WorldNode(
                    BootstrapWorldScenario.ForestFarmNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    CombatStandardEncounterCatalog.EnemyUnitEncounter,
                    regionMaterialYieldContent: new RegionMaterialYieldContentDefinition(1),
                    displayName: "Forest Farm"),
                new WorldNode(
                    BootstrapWorldScenario.CavernServiceNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.ServiceOrProgression,
                    NodeState.Available,
                    townServiceContext: TownServiceContextCatalog.CavernServiceHub,
                    displayName: "Cavern Service Hub"),
                new WorldNode(
                    BootstrapWorldScenario.CavernGateNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.BossOrGate,
                    NodeState.Locked,
                    CombatBossEncounterCatalog.GateBossEncounter,
                    bossRewardContent: new BossRewardContentDefinition(1),
                    displayName: "Cavern Gate"),
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
                    "frontier",
                    LocationIdentityCatalog.VerdantFrontier),
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
                    "depths",
                    LocationIdentityCatalog.EchoCaverns),
            };

            List<WorldNodeConnection> connections = new List<WorldNodeConnection>
            {
                new WorldNodeConnection(BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestPushNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestFarmNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.CavernServiceNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestGateNodeId, BootstrapWorldScenario.CavernGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernGateNodeId),
            };

            return new WorldGraph(regions, nodes, connections);
        }
    }
}

