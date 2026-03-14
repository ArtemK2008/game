using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunResultFactoryTests
    {
        [Test]
        public void ShouldCreateRunResultUsingProgressResolutionValues()
        {
            RunResult runResult = RunResultFactory.Create(
                CreateNodeState(),
                RunResolutionState.Succeeded,
                new RunProgressResolution(
                    2,
                    new NodeProgressUpdateResult(
                        isTracked: true,
                        currentProgress: 2,
                        progressThreshold: 3,
                        didReachClearThreshold: false,
                        nodeStateAfterUpdate: NodeState.InProgress),
                    didUnlockRoute: true));

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
                CreateNodeState(),
                RunResolutionState.Failed,
                new RunProgressResolution(
                    0,
                    NodeProgressUpdateResult.Untracked(NodeState.Available),
                    didUnlockRoute: false));

            Assert.That(runResult.RewardPayload, Is.SameAs(RunRewardPayload.Empty));
            Assert.That(runResult.RewardPayload.HasRewards, Is.False);
            Assert.That(runResult.NextActionContext.CanReplayNode, Is.True);
            Assert.That(runResult.NextActionContext.CanChooseAnotherNode, Is.True);
            Assert.That(runResult.NextActionContext.CanStopSession, Is.True);
        }

        private static NodePlaceholderState CreateNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }
    }
}
