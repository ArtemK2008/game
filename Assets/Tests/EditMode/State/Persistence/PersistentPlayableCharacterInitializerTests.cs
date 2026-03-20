using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class PersistentPlayableCharacterInitializerTests
    {
        [Test]
        public void ShouldSeedAllShippedPlayableCharactersIntoPersistentGameStateWhenMissing()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentPlayableCharacterInitializer initializer = new PersistentPlayableCharacterInitializer();

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.CharacterStates, Has.Count.EqualTo(2));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(vanguardState.IsUnlocked, Is.True);
            Assert.That(vanguardState.IsSelectable, Is.True);
            Assert.That(vanguardState.IsActive, Is.True);
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardDefault));
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(strikerState.IsUnlocked, Is.True);
            Assert.That(strikerState.IsSelectable, Is.True);
            Assert.That(strikerState.IsActive, Is.False);
            Assert.That(strikerState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
        }

        [Test]
        public void ShouldEnsureAllShippedPlayableCharactersExistWithoutOverridingExistingValidSelection()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentPlayableCharacterInitializer initializer = new PersistentPlayableCharacterInitializer();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: false,
                isSelectable: false,
                isActive: false));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.CharacterStates, Has.Count.EqualTo(2));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(vanguardState.IsUnlocked, Is.True);
            Assert.That(vanguardState.IsSelectable, Is.True);
            Assert.That(vanguardState.IsActive, Is.False);
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardDefault));
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(strikerState.IsUnlocked, Is.True);
            Assert.That(strikerState.IsSelectable, Is.True);
            Assert.That(strikerState.IsActive, Is.True);
            Assert.That(strikerState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
        }

        [Test]
        public void ShouldNormalizeInvalidPersistedSkillPackageAssignmentsDuringInitialization()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentPlayableCharacterInitializer initializer = new PersistentPlayableCharacterInitializer();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardBurstDrill));

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardDefault));
            Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
            Assert.That(strikerState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
        }
    }
}

