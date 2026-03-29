using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Gear;
using Survivalon.Data.Towns;
using Survivalon.Data.World;

namespace Survivalon.World
{
    public sealed class BootstrapWorldGraphBuilder
    {
        private static readonly OptionalChallengeContentDefinition ForestEliteChallengeContent =
            new OptionalChallengeContentDefinition("Elite challenge");
        private static readonly RegionMaterialYieldContentDefinition SunscorchFarmYieldContent =
            new RegionMaterialYieldContentDefinition(1);

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
                    bossRewardContent: new BossRewardContentDefinition(
                        persistentProgressionMaterialBonus: 0,
                        gearRewardId: GearIds.GatebreakerBlade),
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
                    BootstrapWorldScenario.ForestEliteNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    CombatStandardEncounterCatalog.BulwarkRaiderEncounter,
                    regionMaterialYieldContent: new RegionMaterialYieldContentDefinition(2),
                    optionalChallengeContent: ForestEliteChallengeContent,
                    displayName: "Raider Holdout"),
                new WorldNode(
                    BootstrapWorldScenario.CavernServiceNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.ServiceOrProgression,
                    NodeState.Available,
                    townServiceContext: TownServiceContextCatalog.CavernServiceHub,
                    displayName: "Cavern Service Hub"),
                new WorldNode(
                    BootstrapWorldScenario.CavernPushNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    CombatStandardEncounterCatalog.EnemyUnitEncounter,
                    displayName: "Echo Approach"),
                new WorldNode(
                    BootstrapWorldScenario.CavernFarmNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    CombatStandardEncounterCatalog.EnemyUnitEncounter,
                    displayName: "Relic Cache"),
                new WorldNode(
                    BootstrapWorldScenario.CavernApproachNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    CombatStandardEncounterCatalog.BulwarkRaiderEncounter,
                    displayName: "Gate Antechamber"),
                new WorldNode(
                    BootstrapWorldScenario.CavernGateNodeId,
                    BootstrapWorldScenario.CavernRegionId,
                    NodeType.BossOrGate,
                    NodeState.Locked,
                    CombatBossEncounterCatalog.GateBossEncounter,
                    new BossProgressionGateDefinition(BootstrapWorldScenario.SunscorchEntryNodeId),
                    bossRewardContent: new BossRewardContentDefinition(1),
                    displayName: "Cavern Gate"),
                new WorldNode(
                    BootstrapWorldScenario.SunscorchEntryNodeId,
                    BootstrapWorldScenario.SunscorchRegionId,
                    NodeType.Combat,
                    NodeState.Locked,
                    CombatStandardEncounterCatalog.RuinSentinelEncounter,
                    displayName: "Scorched Approach"),
                new WorldNode(
                    BootstrapWorldScenario.SunscorchPushNodeId,
                    BootstrapWorldScenario.SunscorchRegionId,
                    NodeType.Combat,
                    NodeState.Locked,
                    CombatStandardEncounterCatalog.RuinSentinelEncounter,
                    displayName: "Ruin Span"),
                new WorldNode(
                    BootstrapWorldScenario.SunscorchFarmNodeId,
                    BootstrapWorldScenario.SunscorchRegionId,
                    NodeType.Combat,
                    NodeState.Locked,
                    CombatStandardEncounterCatalog.RuinSentinelEncounter,
                    regionMaterialYieldContent: SunscorchFarmYieldContent,
                    displayName: "Ash Cache"),
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
                        BootstrapWorldScenario.ForestEliteNodeId,
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
                        BootstrapWorldScenario.CavernPushNodeId,
                        BootstrapWorldScenario.CavernFarmNodeId,
                        BootstrapWorldScenario.CavernApproachNodeId,
                        BootstrapWorldScenario.CavernGateNodeId,
                    },
                    ResourceCategory.PersistentProgressionMaterial,
                    "depths",
                    LocationIdentityCatalog.EchoCaverns),
                new WorldRegion(
                    BootstrapWorldScenario.SunscorchRegionId,
                    2,
                    BootstrapWorldScenario.SunscorchEntryNodeId,
                    new[]
                    {
                        BootstrapWorldScenario.SunscorchEntryNodeId,
                        BootstrapWorldScenario.SunscorchPushNodeId,
                        BootstrapWorldScenario.SunscorchFarmNodeId,
                    },
                    ResourceCategory.RegionMaterial,
                    "sunscorch",
                    LocationIdentityCatalog.SunscorchRuins),
            };

            List<WorldNodeConnection> connections = new List<WorldNodeConnection>
            {
                new WorldNodeConnection(BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestPushNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestFarmNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestEliteNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.CavernServiceNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestGateNodeId, BootstrapWorldScenario.CavernGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernPushNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernFarmNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernPushNodeId, BootstrapWorldScenario.CavernApproachNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernApproachNodeId, BootstrapWorldScenario.CavernGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernGateNodeId, BootstrapWorldScenario.SunscorchEntryNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.SunscorchEntryNodeId, BootstrapWorldScenario.SunscorchPushNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.SunscorchEntryNodeId, BootstrapWorldScenario.SunscorchFarmNodeId),
            };

            return new WorldGraph(regions, nodes, connections);
        }
    }
}

