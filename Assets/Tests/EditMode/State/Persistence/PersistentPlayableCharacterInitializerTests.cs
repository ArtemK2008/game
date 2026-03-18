using NUnit.Framework;
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
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo("skill_package_vanguard_default"));
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(strikerState.IsUnlocked, Is.True);
            Assert.That(strikerState.IsSelectable, Is.True);
            Assert.That(strikerState.IsActive, Is.False);
            Assert.That(strikerState.SkillPackageId, Is.EqualTo("skill_package_striker_default"));
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
                skillPackageId: "skill_package_striker_default"));

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.CharacterStates, Has.Count.EqualTo(2));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(vanguardState.IsUnlocked, Is.True);
            Assert.That(vanguardState.IsSelectable, Is.True);
            Assert.That(vanguardState.IsActive, Is.False);
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(strikerState.IsUnlocked, Is.True);
            Assert.That(strikerState.IsSelectable, Is.True);
            Assert.That(strikerState.IsActive, Is.True);
        }
    }
}

