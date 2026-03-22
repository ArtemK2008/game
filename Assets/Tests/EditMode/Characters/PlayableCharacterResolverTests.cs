using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Characters
{
    /// <summary>
    /// Проверяет runtime-разрешение текущего персонажа из persistent state.
    /// </summary>
    public sealed class PlayableCharacterResolverTests
    {
        [Test]
        public void ShouldResolveCurrentPlayableCharacterFromActivePersistentCharacterState()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            PlayableCharacterResolver resolver = new PlayableCharacterResolver();

            PlayableCharacterProfile character = resolver.ResolveCurrent(gameState);

            Assert.That(character.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(character.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(character.CombatEntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(character.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(character.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(character.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(character.BaseStats.Defense, Is.EqualTo(12f));
        }

        [Test]
        public void ShouldResolveSelectedSecondPlayableCharacterFromActivePersistentCharacterState()
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
            PlayableCharacterResolver resolver = new PlayableCharacterResolver();

            PlayableCharacterProfile character = resolver.ResolveCurrent(gameState);

            Assert.That(character.CharacterId, Is.EqualTo("character_striker"));
            Assert.That(character.DisplayName, Is.EqualTo("Striker"));
            Assert.That(character.CombatEntityId, Is.EqualTo(new CombatEntityId("player_striker")));
            Assert.That(character.BaseStats.MaxHealth, Is.EqualTo(110f));
            Assert.That(character.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(character.BaseStats.AttackRate, Is.EqualTo(1.35f));
            Assert.That(character.BaseStats.Defense, Is.EqualTo(8f));
        }

        [Test]
        public void ShouldRejectMissingPlayableCharacterState()
        {
            PlayableCharacterResolver resolver = new PlayableCharacterResolver();

            Assert.That(
                () => resolver.ResolveCurrent(new PersistentGameState()),
                Throws.InvalidOperationException);
        }

        [Test]
        public void ShouldFallbackToUnlockedSelectableCharacterWhenNoPersistentCharacterIsMarkedActive()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            PlayableCharacterResolver resolver = new PlayableCharacterResolver();

            PlayableCharacterProfile character = resolver.ResolveCurrent(gameState);

            Assert.That(character.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(character.DisplayName, Is.EqualTo("Vanguard"));
        }
    }
}

