using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.State.Persistence;

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
                new NodeId("region_001_node_002"));
        }

        public static NodePlaceholderState CreateBossCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"));
        }

        public static NodePlaceholderState CreatePushCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"));
        }
    }
}
