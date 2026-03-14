using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class CombatAutoAdvanceLoopTests
    {
        [Test]
        public void ShouldAdvanceCombatRunAutomaticallyUntilResolved()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());
            CombatAutoAdvanceLoop autoAdvanceLoop = new CombatAutoAdvanceLoop(0.25f);

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 24 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                autoAdvanceLoop.TryAdvance(controller, 0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
        }

        [Test]
        public void ShouldRemainDeterministicAcrossSmallRepeatedTicks()
        {
            RunLifecycleController incrementalController = new RunLifecycleController(CreateCombatNodeState());
            RunLifecycleController referenceController = new RunLifecycleController(CreateCombatNodeState());
            CombatAutoAdvanceLoop autoAdvanceLoop = new CombatAutoAdvanceLoop(0.1f);

            Assert.That(incrementalController.TryEnterActiveState(), Is.True);
            Assert.That(referenceController.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 10; index++)
            {
                autoAdvanceLoop.TryAdvance(incrementalController, 0.1f);
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
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());
            CombatAutoAdvanceLoop autoAdvanceLoop = new CombatAutoAdvanceLoop(0.25f);

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 24 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                autoAdvanceLoop.TryAdvance(controller, 0.25f);
            }

            float resolvedElapsedSeconds = controller.CombatEncounterState.ElapsedCombatSeconds;
            float playerHealthAfterResolution = controller.CombatEncounterState.PlayerEntity.CurrentHealth;

            bool advancedAfterResolution = autoAdvanceLoop.TryAdvance(controller, 1f);

            Assert.That(advancedAfterResolution, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(resolvedElapsedSeconds).Within(0.001f));
            Assert.That(controller.CombatEncounterState.PlayerEntity.CurrentHealth, Is.EqualTo(playerHealthAfterResolution).Within(0.001f));
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
    }
}
