using NUnit.Framework;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunResultFactoryTests
    {
        [Test]
        public void ShouldRejectMissingNodeContext()
        {
            Assert.That(
                () => RunResultFactory.Create(
                    null,
                    RunResolutionState.Succeeded,
                    CreateProgressResolution()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("nodeContext"));
        }

        [Test]
        public void ShouldCreateRunResultUsingProgressResolutionValues()
        {
            RunResult runResult = RunResultFactory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                CreateProgressResolution());

            Assert.That(runResult.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(runResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(runResult.NodeProgressDelta, Is.EqualTo(2));
            Assert.That(runResult.NodeProgressValue, Is.EqualTo(2));
            Assert.That(runResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(runResult.DidUnlockRoute, Is.True);
        }

        [Test]
        public void ShouldCreateRunResultWithDefaultRewardPayloadAndNextActions()
        {
            RunResult runResult = RunResultFactory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Failed,
                new RunProgressResolution(
                    0,
                    NodeProgressUpdateResult.Untracked(NodeState.Available),
                    didUnlockRoute: false));

            Assert.That(runResult.RewardPayload, Is.SameAs(RunRewardPayload.Empty));
            Assert.That(runResult.RewardPayload.CurrencyRewards, Is.Empty);
            Assert.That(runResult.RewardPayload.MaterialRewards, Is.Empty);
            Assert.That(runResult.RewardPayload.MilestoneCurrencyRewards, Is.Empty);
            Assert.That(runResult.RewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(runResult.RewardPayload.HasMilestoneRewards, Is.False);
            Assert.That(runResult.RewardPayload.HasRewards, Is.False);
            Assert.That(runResult.NextActionContext.CanReplayNode, Is.True);
            Assert.That(runResult.NextActionContext.CanChooseAnotherNode, Is.True);
            Assert.That(runResult.NextActionContext.CanStopSession, Is.True);
        }

        private static RunProgressResolution CreateProgressResolution()
        {
            return new RunProgressResolution(
                2,
                new NodeProgressUpdateResult(
                    isTracked: true,
                    currentProgress: 2,
                    progressThreshold: 3,
                    didReachClearThreshold: false,
                    nodeStateAfterUpdate: NodeState.InProgress),
                didUnlockRoute: true);
        }
    }
}
