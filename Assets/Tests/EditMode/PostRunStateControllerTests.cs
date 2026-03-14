using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class PostRunStateControllerTests
    {
        [Test]
        public void ShouldExposeReplayReturnAndStopAvailabilityFromRunResult()
        {
            PostRunStateController controller = CreateController();

            Assert.That(controller.CanReplayNode, Is.True);
            Assert.That(controller.CanReturnToWorld, Is.True);
            Assert.That(controller.CanStopSession, Is.True);
        }

        [Test]
        public void ShouldCreateFreshRunLifecycleControllerForReplay()
        {
            PostRunStateController controller = CreateController();

            RunLifecycleController replayController = controller.CreateReplayLifecycleController();

            Assert.That(replayController.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
            Assert.That(replayController.NodeContext.NodeId, Is.EqualTo(controller.NodeContext.NodeId));
            Assert.That(replayController.HasRunResult, Is.False);
        }

        private static PostRunStateController CreateController()
        {
            NodePlaceholderState placeholderState = new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002"));
            RunResult runResult = new RunResult(
                placeholderState.NodeId,
                RunResolutionState.Succeeded,
                RunRewardPayload.Empty,
                0,
                0,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));

            return new PostRunStateController(placeholderState, runResult);
        }
    }
}
