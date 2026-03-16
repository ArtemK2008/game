using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class PersistentPlayableCharacterInitializerTests
    {
        [Test]
        public void ShouldSeedDefaultPlayableCharacterIntoPersistentGameStateWhenMissing()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentPlayableCharacterInitializer initializer = new PersistentPlayableCharacterInitializer();

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.CharacterStates, Has.Count.EqualTo(1));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                Is.True);
            Assert.That(characterState.IsUnlocked, Is.True);
            Assert.That(characterState.IsSelectable, Is.True);
            Assert.That(characterState.IsActive, Is.True);
            Assert.That(characterState.SkillPackageId, Is.EqualTo("skill_package_vanguard_default"));
        }

        [Test]
        public void ShouldNotDuplicateDefaultPlayableCharacterWhenAlreadyPresent()
        {
            PersistentGameState gameState = new PersistentGameState();
            PersistentPlayableCharacterInitializer initializer = new PersistentPlayableCharacterInitializer();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: false,
                isSelectable: false,
                isActive: false));

            initializer.EnsureInitialized(gameState);

            Assert.That(gameState.CharacterStates, Has.Count.EqualTo(1));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                Is.True);
            Assert.That(characterState.IsUnlocked, Is.True);
            Assert.That(characterState.IsSelectable, Is.True);
            Assert.That(characterState.IsActive, Is.True);
        }
    }
}
