using NUnit.Framework;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunPersistentContextTests
    {
        [Test]
        public void ShouldCarrySelectedCharacterPersistentGearDataIntoCombatBaseline()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterGearAssignmentService gearAssignmentService =
                new PlayableCharacterGearAssignmentService();
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.TrainingBlade),
                Is.True);
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.GuardCharm),
                Is.True);

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(persistentContext.PlayableCharacterState.LoadoutState.EquippedGearStates, Has.Count.EqualTo(2));
            Assert.That(persistentContext.PlayableCharacterState.LoadoutState.EquippedGearStates, Has.Some.Matches<EquippedGearState>(state =>
                state.GearId == GearIds.TrainingBlade &&
                state.GearCategory == GearCategory.PrimaryCombat));
            Assert.That(persistentContext.PlayableCharacterState.LoadoutState.EquippedGearStates, Has.Some.Matches<EquippedGearState>(state =>
                state.GearId == GearIds.GuardCharm &&
                state.GearCategory == GearCategory.SecondarySupport));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(160f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(16f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
        }
    }
}
