using NUnit.Framework;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class PersistentGearStateInitializerTests
    {
        [Test]
        public void ShouldEnsureStarterGearOwnershipAndKeepOwnedGearIdsDistinct()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentGearStateInitializer initializer = new PersistentGearStateInitializer();

            gameState.EnsureOwnedGearId("gear_unknown_future");
            gameState.EnsureOwnedGearId("gear_unknown_future");

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.OwnedGearIds, Has.Count.EqualTo(2));
            Assert.That(gameState.OwnedGearIds, Does.Contain("gear_unknown_future"));
            Assert.That(gameState.OwnedGearIds, Does.Contain(GearIds.TrainingBlade));
        }

        [Test]
        public void ShouldNormalizeEquippedGearAgainstOwnershipAndCurrentShippedCategory()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentGearStateInitializer initializer = new PersistentGearStateInitializer();
            PersistentLoadoutState loadoutState = new PersistentLoadoutState(
                equippedGearStates: new EquippedGearState[]
                {
                    new EquippedGearState(GearIds.TrainingBlade, GearCategory.GeneralModifier),
                    new EquippedGearState("gear_unknown_future", GearCategory.PrimaryCombat),
                    new EquippedGearState(GearIds.TrainingBlade, GearCategory.PrimaryCombat),
                    new EquippedGearState(GearIds.TrainingBlade, GearCategory.PrimaryCombat),
                });

            gameState.EnsureOwnedGearId("gear_unknown_future");
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                loadoutState: loadoutState));

            initializer.EnsureInitialized(gameState);

            Assert.That(loadoutState.EquippedGearStates, Has.Count.EqualTo(1));
            Assert.That(loadoutState.EquippedGearStates[0].GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(loadoutState.EquippedGearStates[0].GearCategory, Is.EqualTo(GearCategory.PrimaryCombat));
        }
    }
}
