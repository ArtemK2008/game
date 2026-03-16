using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class CombatAutoAdvanceLoopTests
    {
        [Test]
        public void ShouldAdvanceCombatRunAutomaticallyUntilResolved()
        {
            RunLifecycleController controller = new RunLifecycleController(NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 24 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
        }

        [Test]
        public void ShouldRemainDeterministicAcrossSmallRepeatedTicks()
        {
            RunLifecycleController incrementalController = new RunLifecycleController(NodePlaceholderTestData.CreateCombatPlaceholderState());
            RunLifecycleController referenceController = new RunLifecycleController(NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(incrementalController.TryEnterActiveState(), Is.True);
            Assert.That(referenceController.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 10; index++)
            {
                incrementalController.TryAdvanceTime(0.1f);
            }

            referenceController.TryAdvanceCombat(1f);

            Assert.That(
                incrementalController.CombatEncounterState.ElapsedCombatSeconds,
                Is.EqualTo(referenceController.CombatEncounterState.ElapsedCombatSeconds).Within(0.001f));
            Assert.That(
                incrementalController.CombatEncounterState.PlayerEntity.CurrentHealth,
                Is.EqualTo(referenceController.CombatEncounterState.PlayerEntity.CurrentHealth).Within(0.001f));
            Assert.That(
                incrementalController.CombatEncounterState.EnemyEntity.CurrentHealth,
                Is.EqualTo(referenceController.CombatEncounterState.EnemyEntity.CurrentHealth).Within(0.001f));
        }

        [Test]
        public void ShouldStopAutoAdvancingAfterCombatRunResolves()
        {
            RunLifecycleController controller = new RunLifecycleController(NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 24 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceTime(0.25f);
            }

            float resolvedElapsedSeconds = controller.CombatEncounterState.ElapsedCombatSeconds;
            float playerHealthAfterResolution = controller.CombatEncounterState.PlayerEntity.CurrentHealth;

            bool advancedAfterResolution = controller.TryAdvanceTime(1f);

            Assert.That(advancedAfterResolution, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(resolvedElapsedSeconds).Within(0.001f));
            Assert.That(controller.CombatEncounterState.PlayerEntity.CurrentHealth, Is.EqualTo(playerHealthAfterResolution).Within(0.001f));
        }

        [Test]
        public void ShouldResolveAutoAdvancedBossCombatAsFailedWhenHostileEnemyWins()
        {
            RunLifecycleController controller = new RunLifecycleController(NodePlaceholderTestData.CreateBossCombatPlaceholderState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 64 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActivePlayer, Is.False);
        }

    }
}
