using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Gear;
using Survivalon.Data.Towns;
using Survivalon.Data.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public static class NodePlaceholderTestData
    {
        private static readonly BossProgressionGateDefinition ForestGateProgressionGate =
            new BossProgressionGateDefinition(BootstrapWorldScenario.CavernGateNodeId);
        private static readonly BossProgressionGateDefinition CavernGateProgressionGate =
            new BossProgressionGateDefinition(BootstrapWorldScenario.SunscorchEntryNodeId);
        private static readonly BossRewardContentDefinition ForestGateBossRewardContent =
            new BossRewardContentDefinition(0, GearIds.GatebreakerBlade);
        private static readonly BossRewardContentDefinition CavernGateBossRewardContent =
            new BossRewardContentDefinition(1);
        private static readonly RegionMaterialYieldContentDefinition FrontierFarmYieldContent =
            new RegionMaterialYieldContentDefinition(1);
        private static readonly RegionMaterialYieldContentDefinition ForestEliteYieldContent =
            new RegionMaterialYieldContentDefinition(2);
        private static readonly OptionalChallengeContentDefinition ForestEliteChallengeContent =
            new OptionalChallengeContentDefinition("Elite challenge");

        public static NodePlaceholderState CreateServicePlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002"),
                locationIdentity: LocationIdentityCatalog.EchoCaverns,
                nodeDisplayName: "Cavern Service Hub");
        }

        public static NodePlaceholderState CreateTownServicePlaceholderState()
        {
            return new NodePlaceholderState(
                BootstrapWorldScenario.CavernServiceNodeId,
                BootstrapWorldScenario.CavernRegionId,
                NodeType.ServiceOrProgression,
                NodeState.Available,
                BootstrapWorldScenario.ForestPushNodeId,
                townServiceContext: TownServiceContextCatalog.CavernServiceHub,
                locationIdentity: LocationIdentityCatalog.EchoCaverns,
                nodeDisplayName: "Cavern Service Hub");
        }

        public static NodePlaceholderState CreateCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"),
                CombatStandardEncounterCatalog.EnemyUnitEncounter,
                locationIdentity: LocationIdentityCatalog.VerdantFrontier,
                supportsRegionMaterialRewards: true,
                nodeDisplayName: "Forest Farm");
        }

        public static NodePlaceholderState CreateFrontierFarmPlaceholderState()
        {
            return new NodePlaceholderState(
                BootstrapWorldScenario.ForestFarmNodeId,
                BootstrapWorldScenario.ForestRegionId,
                NodeType.Combat,
                NodeState.Available,
                BootstrapWorldScenario.ForestPushNodeId,
                CombatStandardEncounterCatalog.EnemyUnitEncounter,
                locationIdentity: LocationIdentityCatalog.VerdantFrontier,
                regionMaterialYieldContent: FrontierFarmYieldContent,
                supportsRegionMaterialRewards: true,
                nodeDisplayName: "Forest Farm");
        }

        public static NodePlaceholderState CreateBossCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"),
                CombatBossEncounterCatalog.GateBossEncounter,
                ForestGateProgressionGate,
                locationIdentity: LocationIdentityCatalog.VerdantFrontier,
                bossRewardContent: ForestGateBossRewardContent,
                nodeDisplayName: "Frontier Gate");
        }

        public static NodePlaceholderState CreateEliteChallengePlaceholderState()
        {
            return new NodePlaceholderState(
                BootstrapWorldScenario.ForestEliteNodeId,
                BootstrapWorldScenario.ForestRegionId,
                NodeType.Combat,
                NodeState.Available,
                BootstrapWorldScenario.ForestPushNodeId,
                CombatStandardEncounterCatalog.BulwarkRaiderEncounter,
                locationIdentity: LocationIdentityCatalog.VerdantFrontier,
                regionMaterialYieldContent: ForestEliteYieldContent,
                optionalChallengeContent: ForestEliteChallengeContent,
                supportsRegionMaterialRewards: true,
                nodeDisplayName: "Raider Holdout");
        }

        public static NodePlaceholderState CreateForestGateBossPlaceholderState()
        {
            return new NodePlaceholderState(
                BootstrapWorldScenario.ForestGateNodeId,
                BootstrapWorldScenario.ForestRegionId,
                NodeType.BossOrGate,
                NodeState.Available,
                BootstrapWorldScenario.ForestPushNodeId,
                CombatBossEncounterCatalog.GateBossEncounter,
                ForestGateProgressionGate,
                locationIdentity: LocationIdentityCatalog.VerdantFrontier,
                bossRewardContent: ForestGateBossRewardContent,
                nodeDisplayName: "Frontier Gate");
        }

        public static NodePlaceholderState CreateCavernGateBossPlaceholderState()
        {
            return new NodePlaceholderState(
                BootstrapWorldScenario.CavernGateNodeId,
                BootstrapWorldScenario.CavernRegionId,
                NodeType.BossOrGate,
                NodeState.Available,
                BootstrapWorldScenario.CavernServiceNodeId,
                CombatBossEncounterCatalog.GateBossEncounter,
                CavernGateProgressionGate,
                locationIdentity: LocationIdentityCatalog.EchoCaverns,
                bossRewardContent: CavernGateBossRewardContent,
                nodeDisplayName: "Cavern Gate");
        }

        public static NodePlaceholderState CreateSunscorchCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                BootstrapWorldScenario.SunscorchEntryNodeId,
                BootstrapWorldScenario.SunscorchRegionId,
                NodeType.Combat,
                NodeState.Available,
                BootstrapWorldScenario.CavernGateNodeId,
                CombatStandardEncounterCatalog.RuinSentinelEncounter,
                locationIdentity: LocationIdentityCatalog.SunscorchRuins,
                supportsRegionMaterialRewards: true,
                nodeDisplayName: "Scorched Approach");
        }

        public static NodePlaceholderState CreatePushCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"),
                CombatStandardEncounterCatalog.BulwarkRaiderEncounter,
                locationIdentity: LocationIdentityCatalog.VerdantFrontier,
                supportsRegionMaterialRewards: true,
                nodeDisplayName: "Raider Trail");
        }
    }
}

