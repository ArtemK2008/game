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
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            bool enteredActive = controller.TryEnterActiveState();

            Assert.That(enteredActive, Is.True);
            Assert.That(controller.HasCombatContext, Is.True);
            Assert.That(controller.HasCombatEncounterState, Is.True);
            Assert.That(controller.CombatContext.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(controller.CombatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Player Unit"));
            Assert.That(controller.CombatContext.PlayerEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(controller.CombatContext.PlayerEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.PlayerEntity.IsActive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(controller.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(controller.CombatContext.EnemyEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(4f));
            Assert.That(controller.CombatContext.EnemyEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.IsActive, Is.True);
            Assert.That(controller.CombatEncounterState.PlayerEntity.CurrentHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatEncounterState.EnemyEntity.CurrentHealth, Is.EqualTo(75f));
        }

        [Test]
        public void ShouldAdvanceCombatUntilCombatRunResolves()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 5 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceCombat(1f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.IsResolved, Is.True);
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(controller.CombatEncounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.EnemyEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActiveEnemy, Is.False);
            Assert.That(controller.CombatEncounterState.ActiveEnemyCount, Is.EqualTo(0));
            Assert.That(controller.CombatEncounterState.EnemyEntity.CurrentHealth, Is.EqualTo(0f));
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(5f).Within(0.001f));
        }

        [Test]
        public void ShouldResolveCombatRunAsFailedWhenPlayerIsDefeated()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateBossCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 12 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceCombat(1f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.IsResolved, Is.True);
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActivePlayer, Is.False);
            Assert.That(controller.CombatEncounterState.ActivePlayerCount, Is.EqualTo(0));
        }

        [Test]
        public void ShouldRejectManualCombatResolutionBeforeCombatOutcomeIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            bool resolved = controller.TryResolveRun(RunResolutionState.Succeeded);

            Assert.That(resolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
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
