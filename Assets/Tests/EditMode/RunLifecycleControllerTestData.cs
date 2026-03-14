using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public static class RunLifecycleControllerTestData
    {
        public static RunLifecycleController CreateController()
        {
            return new RunLifecycleController(new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002")));
        }

        public static NodePlaceholderState CreateCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        public static NodePlaceholderState CreateBossCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"));
        }

        public static NodePlaceholderState CreatePushCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"));
        }

        public static void RunToPostRun(RunLifecycleController controller, int maxStepCount = 128)
        {
            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < maxStepCount && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
        }
    }
}
