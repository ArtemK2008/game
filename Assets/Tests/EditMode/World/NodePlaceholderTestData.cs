using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Towns;
using Survivalon.Data.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public static class NodePlaceholderTestData
    {
        private static readonly BossProgressionGateDefinition ForestGateProgressionGate =
            new BossProgressionGateDefinition(BootstrapWorldScenario.CavernGateNodeId);
        private static readonly BossRewardContentDefinition CavernGateBossRewardContent =
            new BossRewardContentDefinition(1);

        public static NodePlaceholderState CreateServicePlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002"),
                locationIdentity: LocationIdentityCatalog.EchoCaverns);
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
                locationIdentity: LocationIdentityCatalog.EchoCaverns);
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
                locationIdentity: LocationIdentityCatalog.VerdantFrontier);
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
                locationIdentity: LocationIdentityCatalog.VerdantFrontier);
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
                locationIdentity: LocationIdentityCatalog.VerdantFrontier);
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
                locationIdentity: LocationIdentityCatalog.EchoCaverns,
                bossRewardContent: CavernGateBossRewardContent);
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
                locationIdentity: LocationIdentityCatalog.VerdantFrontier);
        }
    }
}

