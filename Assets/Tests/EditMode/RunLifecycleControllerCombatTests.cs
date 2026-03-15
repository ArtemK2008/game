using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunLifecycleControllerCombatTests
    {
        [Test]
        public void ShouldCreateCombatShellContextWhenCombatRunEntersActiveState()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

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
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

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
        public void ShouldStartAutomaticCombatFlowWithoutAdvancingTime()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ShouldIgnoreZeroElapsedAutomaticTimeProgression()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            controller.TryStartAutomaticFlow();

            bool changed = controller.TryAdvanceAutomaticTime(0f);

            Assert.That(changed, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ShouldAdvanceAutomaticCombatFlowFromNodeEntryToPostRun()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentWorldState: worldState,
                resourceBalancesState: resourceBalances);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));

            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
        }

        [Test]
        public void ShouldResolveCombatRunAsFailedWhenPlayerIsDefeated()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateBossCombatNodeState());

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
        public void ShouldAdvanceAutomaticHostileCombatFlowToFailedPostRun()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentWorldState: worldState,
                resourceBalancesState: resourceBalances);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));

            for (int index = 0; index < 64 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.RunResult.RewardPayload, Is.SameAs(RunRewardPayload.Empty));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(0));
        }

        [Test]
        public void ShouldStopAutomaticCombatFlowAfterPostRunIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            controller.TryStartAutomaticFlow();
            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            float resolvedElapsedSeconds = controller.CombatEncounterState.ElapsedCombatSeconds;
            bool advancedAfterPostRun = controller.TryAdvanceAutomaticTime(1f);

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(advancedAfterPostRun, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(resolvedElapsedSeconds).Within(0.001f));
        }

        [Test]
        public void ShouldRejectManualCombatResolutionBeforeCombatOutcomeIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            bool resolved = controller.TryResolveRun(RunResolutionState.Succeeded);

            Assert.That(resolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
        }
    }
}
