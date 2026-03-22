using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public static class NodePlaceholderTestData
    {
        private static readonly BossProgressionGateDefinition ForestGateProgressionGate =
            new BossProgressionGateDefinition(
                BootstrapWorldScenario.CavernGateNodeId,
                "Cavern gate opened");

        public static NodePlaceholderState CreateServicePlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        public static NodePlaceholderState CreateCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"),
                CombatStandardEncounterCatalog.EnemyUnitEncounter);
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
                ForestGateProgressionGate);
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
                ForestGateProgressionGate);
        }

        public static NodePlaceholderState CreatePushCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"),
                CombatStandardEncounterCatalog.BulwarkRaiderEncounter);
        }
    }
}

