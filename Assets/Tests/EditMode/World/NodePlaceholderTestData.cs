using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public static class NodePlaceholderTestData
    {
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
                CombatEncounterCatalog.EnemyUnitEncounter);
        }

        public static NodePlaceholderState CreateBossCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"),
                CombatEncounterCatalog.GatePlaceholderEncounter);
        }

        public static NodePlaceholderState CreatePushCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"),
                CombatEncounterCatalog.BulwarkRaiderEncounter);
        }
    }
}

