using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class PlayableCharacterSelectionServiceTests
    {
        [Test]
        public void ShouldBuildOnlySelectableKnownCharacterOptionsAndMarkCurrentSelection()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_unknown",
                isUnlocked: true,
                isSelectable: true,
                isActive: false));
            PlayableCharacterSelectionService service = new PlayableCharacterSelectionService();

            IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions = service.BuildSelectableOptions(gameState);

            Assert.That(selectionOptions, Has.Count.EqualTo(2));
            Assert.That(selectionOptions[0].CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(selectionOptions[0].DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(selectionOptions[0].IsSelected, Is.False);
            Assert.That(selectionOptions[1].CharacterId, Is.EqualTo("character_striker"));
            Assert.That(selectionOptions[1].DisplayName, Is.EqualTo("Striker"));
            Assert.That(selectionOptions[1].IsSelected, Is.True);
        }

        [Test]
        public void ShouldNormalizeSelectionToOnlySelectablePlayableCharacterWhenCurrentActiveStateIsInvalid()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_unknown",
                isUnlocked: true,
                isSelectable: true,
                isActive: true));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            PlayableCharacterSelectionService service = new PlayableCharacterSelectionService();

            service.EnsureValidSelection(gameState);

            Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
            Assert.That(vanguardState.IsActive, Is.True);
            Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
            Assert.That(strikerState.IsActive, Is.False);
            Assert.That(gameState.TryGetCharacterState("character_unknown", out PersistentCharacterState unknownState), Is.True);
            Assert.That(unknownState.IsActive, Is.False);
        }

        [Test]
        public void ShouldSelectOnlySelectablePlayableCharacterWhenRequested()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_unknown",
                isUnlocked: true,
                isSelectable: true,
                isActive: true));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            PlayableCharacterSelectionService service = new PlayableCharacterSelectionService();

            bool didSelect = service.TrySelectCharacter(gameState, "character_striker");

            Assert.That(didSelect, Is.True);
            Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
            Assert.That(vanguardState.IsActive, Is.False);
            Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
            Assert.That(strikerState.IsActive, Is.True);
            Assert.That(gameState.TryGetCharacterState("character_unknown", out PersistentCharacterState unknownState), Is.True);
            Assert.That(unknownState.IsActive, Is.False);
        }

        [Test]
        public void ShouldRejectSelectingUnknownPlayableCharacterId()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            PlayableCharacterSelectionService service = new PlayableCharacterSelectionService();

            bool didSelect = service.TrySelectCharacter(gameState, "character_unknown");

            Assert.That(didSelect, Is.False);
            Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
            Assert.That(vanguardState.IsActive, Is.True);
            Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
            Assert.That(strikerState.IsActive, Is.False);
        }
    }
}
