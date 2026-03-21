using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class PlayableCharacterGearAssignmentServiceTests
    {
        [Test]
        public void ShouldBuildOwnedPrimaryCombatGearOptionForSelectedCharacter()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearOptions =
                service.BuildOptionsForSelectedCharacter(gameState, GearCategory.PrimaryCombat);

            Assert.That(gearOptions, Has.Count.EqualTo(1));
            Assert.That(gearOptions[0].CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(gearOptions[0].GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(gearOptions[0].DisplayName, Is.EqualTo("Training Blade"));
            Assert.That(gearOptions[0].GearCategory, Is.EqualTo(GearCategory.PrimaryCombat));
            Assert.That(gearOptions[0].IsEquipped, Is.False);
        }

        [Test]
        public void ShouldBuildOwnedSecondarySupportGearOptionForSelectedCharacter()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearOptions =
                service.BuildOptionsForSelectedCharacter(gameState, GearCategory.SecondarySupport);

            Assert.That(gearOptions, Has.Count.EqualTo(1));
            Assert.That(gearOptions[0].CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(gearOptions[0].GearId, Is.EqualTo(GearIds.GuardCharm));
            Assert.That(gearOptions[0].DisplayName, Is.EqualTo("Guard Charm"));
            Assert.That(gearOptions[0].GearCategory, Is.EqualTo(GearCategory.SecondarySupport));
            Assert.That(gearOptions[0].IsEquipped, Is.False);
        }

        [Test]
        public void ShouldAssignOwnedPrimaryCombatGearToSelectedCharacter()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            bool didAssign = service.TryAssignSelectedCharacterGear(gameState, GearIds.TrainingBlade);

            Assert.That(didAssign, Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState equippedGearState),
                Is.True);
            Assert.That(equippedGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
        }

        [Test]
        public void ShouldAssignOwnedSecondarySupportGearToSelectedCharacter()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            bool didAssign = service.TryAssignSelectedCharacterGear(gameState, GearIds.GuardCharm);

            Assert.That(didAssign, Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.SecondarySupport,
                    out EquippedGearState equippedGearState),
                Is.True);
            Assert.That(equippedGearState.GearId, Is.EqualTo(GearIds.GuardCharm));
        }

        [Test]
        public void ShouldClearAssignedPrimaryCombatGearForSelectedCharacter()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: false);
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            strikerState.LoadoutState.SetEquippedGearState(
                new EquippedGearState(GearIds.TrainingBlade, GearCategory.PrimaryCombat));
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            bool didClear = service.TryClearSelectedCharacterGear(gameState, GearCategory.PrimaryCombat);

            Assert.That(didClear, Is.True);
            Assert.That(
                strikerState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState _),
                Is.False);
        }

        [Test]
        public void ShouldRejectAssigningUnownedPrimaryCombatGear()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            gameState.ReplaceOwnedGearIds(System.Array.Empty<string>());
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            bool didAssign = service.TryAssignSelectedCharacterGear(gameState, GearIds.TrainingBlade);

            Assert.That(didAssign, Is.False);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState _),
                Is.False);
        }

        [Test]
        public void ShouldKeepPrimaryCombatGearAssignmentCharacterSpecificAcrossSelectionChanges()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            PlayableCharacterGearAssignmentService gearAssignmentService = new PlayableCharacterGearAssignmentService();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();

            bool didAssign = gearAssignmentService.TryAssignSelectedCharacterGear(
                gameState,
                GearIds.TrainingBlade);

            Assert.That(didAssign, Is.True);
            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(
                strikerState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState _),
                Is.False);

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_vanguard"), Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState equippedGearState),
                Is.True);
            Assert.That(equippedGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
        }

        [Test]
        public void ShouldIgnoreUnknownOwnedGearIdsWhenBuildingPrimaryCombatOptions()
        {
            PersistentGameState gameState = CreateGameState(vanguardIsActive: true);
            gameState.EnsureOwnedGearId("gear_unknown_test");
            PlayableCharacterGearAssignmentService service = new PlayableCharacterGearAssignmentService();

            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearOptions =
                service.BuildOptionsForSelectedCharacter(gameState, GearCategory.PrimaryCombat);

            Assert.That(gearOptions, Has.Count.EqualTo(1));
            Assert.That(gearOptions[0].GearId, Is.EqualTo(GearIds.TrainingBlade));
        }

        private static PersistentGameState CreateGameState(bool vanguardIsActive)
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.EnsureOwnedGearId(GearIds.TrainingBlade);
            gameState.EnsureOwnedGearId(GearIds.GuardCharm);
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: vanguardIsActive,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: !vanguardIsActive,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            return gameState;
        }
    }
}
