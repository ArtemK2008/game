using NUnit.Framework;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunPersistentContextTests
    {
        [Test]
        public void ShouldCarrySelectedCharacterPersistentGearDataWithoutChangingCombatBaselineYet()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            vanguardState.LoadoutState.ReplaceEquippedGearStates(new[]
            {
                new EquippedGearState(GearIds.TrainingBlade, GearCategory.PrimaryCombat),
            });

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(persistentContext.PlayableCharacterState.LoadoutState.EquippedGearStates, Has.Count.EqualTo(1));
            Assert.That(
                persistentContext.PlayableCharacterState.LoadoutState.EquippedGearStates[0].GearId,
                Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
        }
    }
}
