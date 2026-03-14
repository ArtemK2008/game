using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class NodeProgressMeterServiceTests
    {
        [Test]
        public void ShouldSeedPersistentCombatNodeProgressAtExpectedDefault()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;

            bool foundNodeState = worldState.TryGetNodeState(
                new NodeId("region_001_node_004"),
                out PersistentNodeState nodeState);

            Assert.That(foundNodeState, Is.True);
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(0));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
            Assert.That(nodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldApplyEnemyDefeatProgressToCombatNode()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            NodeProgressUpdateResult updateResult = service.ApplyRunProgress(
                worldState,
                CreateCombatNodeState(),
                progressDelta: 1);

            Assert.That(updateResult.IsTracked, Is.True);
            Assert.That(updateResult.CurrentProgress, Is.EqualTo(1));
            Assert.That(updateResult.ProgressThreshold, Is.EqualTo(3));
            Assert.That(updateResult.DidReachClearThreshold, Is.False);
            Assert.That(updateResult.NodeStateAfterUpdate, Is.EqualTo(NodeState.InProgress));
        }

        [Test]
        public void ShouldAccumulateRepeatedSuccessfulRunProgressAcrossCombatRuns()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            service.ApplyRunProgress(worldState, CreateCombatNodeState(), progressDelta: 1);
            NodeProgressUpdateResult secondUpdate = service.ApplyRunProgress(
                worldState,
                CreateCombatNodeState(),
                progressDelta: 1);

            Assert.That(secondUpdate.CurrentProgress, Is.EqualTo(2));
            Assert.That(secondUpdate.ProgressThreshold, Is.EqualTo(3));
            Assert.That(secondUpdate.NodeStateAfterUpdate, Is.EqualTo(NodeState.InProgress));
        }

        [Test]
        public void ShouldNotGrantProgressForFailedCombatEquivalentDelta()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            NodeProgressUpdateResult updateResult = service.ApplyRunProgress(
                worldState,
                CreateBossCombatNodeState(),
                progressDelta: 0);

            Assert.That(updateResult.IsTracked, Is.True);
            Assert.That(updateResult.CurrentProgress, Is.EqualTo(0));
            Assert.That(updateResult.ProgressThreshold, Is.EqualTo(3));
            Assert.That(updateResult.DidReachClearThreshold, Is.False);
            Assert.That(updateResult.NodeStateAfterUpdate, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldReachClearThresholdAfterEnoughSuccessfulRuns()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            service.ApplyRunProgress(worldState, CreateCombatNodeState(), progressDelta: 1);
            service.ApplyRunProgress(worldState, CreateCombatNodeState(), progressDelta: 1);
            NodeProgressUpdateResult finalUpdate = service.ApplyRunProgress(
                worldState,
                CreateCombatNodeState(),
                progressDelta: 1);

            Assert.That(finalUpdate.CurrentProgress, Is.EqualTo(3));
            Assert.That(finalUpdate.ProgressThreshold, Is.EqualTo(3));
            Assert.That(finalUpdate.DidReachClearThreshold, Is.True);
            Assert.That(finalUpdate.NodeStateAfterUpdate, Is.EqualTo(NodeState.Cleared));
        }

        [Test]
        public void ShouldKeepTrackedNodeClearedAndCappedAfterThresholdIsReached()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            service.ApplyRunProgress(worldState, CreateCombatNodeState(), progressDelta: 1);
            service.ApplyRunProgress(worldState, CreateCombatNodeState(), progressDelta: 1);
            service.ApplyRunProgress(worldState, CreateCombatNodeState(), progressDelta: 1);
            NodeProgressUpdateResult postClearUpdate = service.ApplyRunProgress(
                worldState,
                CreateCombatNodeState(),
                progressDelta: 1);

            Assert.That(postClearUpdate.CurrentProgress, Is.EqualTo(3));
            Assert.That(postClearUpdate.ProgressThreshold, Is.EqualTo(3));
            Assert.That(postClearUpdate.DidReachClearThreshold, Is.False);
            Assert.That(postClearUpdate.NodeStateAfterUpdate, Is.EqualTo(NodeState.Cleared));
        }

        private static NodePlaceholderState CreateCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        private static NodePlaceholderState CreateBossCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"));
        }
    }
}
