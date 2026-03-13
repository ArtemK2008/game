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
        public void ShouldCreateCombatShellContextWhenCombatRunEntersActiveState()
        {
            RunLifecycleController controller = new RunLifecycleController(new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002")));

            bool enteredActive = controller.TryEnterActiveState();

            Assert.That(enteredActive, Is.True);
            Assert.That(controller.HasCombatContext, Is.True);
            Assert.That(controller.CombatContext.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(controller.CombatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Player Unit"));
            Assert.That(controller.CombatContext.PlayerEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(controller.CombatContext.PlayerEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.PlayerEntity.IsActive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(controller.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(controller.CombatContext.EnemyEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(controller.CombatContext.EnemyEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.IsActive, Is.True);
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
