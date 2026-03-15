using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunLifecycleControllerTests
    {
        [Test]
        public void ShouldStartInRunStartStateWithoutRunResult()
        {
            RunLifecycleController controller = RunLifecycleControllerTestData.CreateController();

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldAdvanceThroughRunLifecycleAndProduceRunResult()
        {
            RunLifecycleController controller = RunLifecycleControllerTestData.CreateController();

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
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.MaterialRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.HasRewards, Is.False);
            Assert.That(controller.RunResult.NextActionContext.CanReplayNode, Is.True);
            Assert.That(controller.RunResult.NextActionContext.CanChooseAnotherNode, Is.True);
            Assert.That(controller.RunResult.NextActionContext.CanStopSession, Is.True);
        }

        [Test]
        public void ShouldIgnoreElapsedTimeForNonCombatRun()
        {
            RunLifecycleController controller = RunLifecycleControllerTestData.CreateController();

            Assert.That(controller.TryEnterActiveState(), Is.True);

            bool advanced = controller.TryAdvanceTime(1f);

            Assert.That(advanced, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldEnterPostRunStateOnlyAfterResolvedRun()
        {
            RunLifecycleController controller = RunLifecycleControllerTestData.CreateController();

            controller.TryEnterActiveState();
            controller.TryResolveRun(RunResolutionState.Succeeded);

            bool enteredPostRun = controller.TryEnterPostRunState();

            Assert.That(enteredPostRun, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
        }

        [Test]
        public void ShouldRejectOutOfOrderLifecycleTransitions()
        {
            RunLifecycleController controller = RunLifecycleControllerTestData.CreateController();

            bool resolvedBeforeActive = controller.TryResolveRun(RunResolutionState.Succeeded);
            bool enteredPostRunBeforeResolved = controller.TryEnterPostRunState();

            Assert.That(resolvedBeforeActive, Is.False);
            Assert.That(enteredPostRunBeforeResolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
        }
    }
}
