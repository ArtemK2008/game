using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunLifecycleControllerTests
    {
        [Test]
        public void ShouldStartInRunStartStateWithoutRunResult()
        {
            RunLifecycleController controller = CreateController();

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldAdvanceThroughRunLifecycleAndProduceRunResult()
        {
            RunLifecycleController controller = CreateController();

            bool enteredActive = controller.TryEnterActiveState();
            bool resolved = controller.TryResolveRun(RunResolutionState.Succeeded);
            bool enteredPostRun = controller.TryEnterPostRunState();

            Assert.That(enteredActive, Is.True);
            Assert.That(resolved, Is.True);
            Assert.That(enteredPostRun, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.NodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.RunResult.RewardPayload, Is.Not.Null);
            Assert.That(controller.RunResult.RewardPayload.HasRewards, Is.False);
            Assert.That(controller.RunResult.NextActionContext.CanReplayNode, Is.True);
            Assert.That(controller.RunResult.NextActionContext.CanChooseAnotherNode, Is.True);
            Assert.That(controller.RunResult.NextActionContext.CanStopSession, Is.True);
        }

        [Test]
        public void ShouldEnterPostRunStateOnlyAfterResolvedRun()
        {
            RunLifecycleController controller = CreateController();

            controller.TryEnterActiveState();
            controller.TryResolveRun(RunResolutionState.Succeeded);

            bool enteredPostRun = controller.TryEnterPostRunState();

            Assert.That(enteredPostRun, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
        }

        [Test]
        public void ShouldRejectOutOfOrderLifecycleTransitions()
        {
            RunLifecycleController controller = CreateController();

            bool resolvedBeforeActive = controller.TryResolveRun(RunResolutionState.Succeeded);
            bool enteredPostRunBeforeResolved = controller.TryEnterPostRunState();

            Assert.That(resolvedBeforeActive, Is.False);
            Assert.That(enteredPostRunBeforeResolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
        }

        private static RunLifecycleController CreateController()
        {
            return new RunLifecycleController(new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002")));
        }
    }
}
