using NUnit.Framework;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
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

        [Test]
        public void ShouldPassPersistentWorldStateIntoReplayLifecycleController()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            PostRunStateController controller = CreateCombatController();

            RunLifecycleController replayController = controller.CreateReplayLifecycleController(
                persistentContext: new RunPersistentContext(persistentWorldState: worldState));

            Assert.That(replayController.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < 24 && replayController.CurrentState != RunLifecycleState.PostRun; index++)
            {
                replayController.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(replayController.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
        }

        private static PostRunStateController CreateController()
        {
            NodePlaceholderState placeholderState = NodePlaceholderTestData.CreateServicePlaceholderState();
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

        private static PostRunStateController CreateCombatController()
        {
            NodePlaceholderState placeholderState = NodePlaceholderTestData.CreateCombatPlaceholderState();
            RunResult runResult = new RunResult(
                placeholderState.NodeId,
                RunResolutionState.Succeeded,
                RunRewardPayload.Empty,
                1,
                1,
                3,
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
