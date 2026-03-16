using System;
using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class NodeProgressMeterServiceTests
    {
        [Test]
        public void ShouldRejectMissingWorldState()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();

            Assert.That(
                () => service.ApplyRunProgress(null, NodePlaceholderTestData.CreateCombatPlaceholderState(), 1),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldState"));
        }

        [Test]
        public void ShouldRejectMissingNodeContext()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();

            Assert.That(
                () => service.ApplyRunProgress(new PersistentWorldState(), null, 1),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("nodeContext"));
        }

        [Test]
        public void ShouldRejectNegativeProgressDelta()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();

            Assert.That(
                () => service.ApplyRunProgress(new PersistentWorldState(), NodePlaceholderTestData.CreateCombatPlaceholderState(), -1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("progressDelta"));
        }

        [Test]
        public void ShouldSeedPersistentCombatNodeProgressAtExpectedDefault()
        {
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();

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
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                progressDelta: 1);

            Assert.That(updateResult.IsTracked, Is.True);
            Assert.That(updateResult.CurrentProgress, Is.EqualTo(1));
            Assert.That(updateResult.ProgressThreshold, Is.EqualTo(3));
            Assert.That(updateResult.DidReachClearThreshold, Is.False);
            Assert.That(updateResult.NodeStateAfterUpdate, Is.EqualTo(NodeState.InProgress));
        }

        [Test]
        public void ShouldReturnUntrackedProgressResultWithoutPersistingServiceNodeState()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            NodeProgressUpdateResult updateResult = service.ApplyRunProgress(
                worldState,
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                progressDelta: 1);

            Assert.That(updateResult.IsTracked, Is.False);
            Assert.That(updateResult.CurrentProgress, Is.EqualTo(0));
            Assert.That(updateResult.ProgressThreshold, Is.EqualTo(0));
            Assert.That(updateResult.NodeStateAfterUpdate, Is.EqualTo(NodeState.Available));
            Assert.That(worldState.TryGetNodeState(NodePlaceholderTestData.CreateServicePlaceholderState().NodeId, out _), Is.False);
        }

        [Test]
        public void ShouldAccumulateRepeatedSuccessfulRunProgressAcrossCombatRuns()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            NodeProgressUpdateResult secondUpdate = service.ApplyRunProgress(
                worldState,
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
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
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
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

            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            NodeProgressUpdateResult finalUpdate = service.ApplyRunProgress(
                worldState,
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
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

            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            NodeProgressUpdateResult postClearUpdate = service.ApplyRunProgress(
                worldState,
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                progressDelta: 1);

            Assert.That(postClearUpdate.CurrentProgress, Is.EqualTo(3));
            Assert.That(postClearUpdate.ProgressThreshold, Is.EqualTo(3));
            Assert.That(postClearUpdate.DidReachClearThreshold, Is.False);
            Assert.That(postClearUpdate.NodeStateAfterUpdate, Is.EqualTo(NodeState.Cleared));
        }

        [Test]
        public void ShouldKeepTrackedNodeProgressUnchangedWhenZeroDeltaIsApplied()
        {
            NodeProgressMeterService service = new NodeProgressMeterService();
            PersistentWorldState worldState = new PersistentWorldState();

            service.ApplyRunProgress(worldState, NodePlaceholderTestData.CreateCombatPlaceholderState(), progressDelta: 1);
            NodeProgressUpdateResult updateResult = service.ApplyRunProgress(
                worldState,
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                progressDelta: 0);

            Assert.That(updateResult.IsTracked, Is.True);
            Assert.That(updateResult.CurrentProgress, Is.EqualTo(1));
            Assert.That(updateResult.ProgressThreshold, Is.EqualTo(3));
            Assert.That(updateResult.DidReachClearThreshold, Is.False);
            Assert.That(updateResult.NodeStateAfterUpdate, Is.EqualTo(NodeState.InProgress));
        }
    }
}
