using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.Data.Rewards;
using Survivalon.Runtime.Data.World;

namespace Survivalon.Tests.EditMode.Data
{
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
                skillPackageId: "skill_package_vanguard_default"));
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
                skillPackageId: "skill_package_vanguard_default"));
            PlayableCharacterResolver resolver = new PlayableCharacterResolver();

            PlayableCharacterProfile character = resolver.ResolveCurrent(gameState);

            Assert.That(character.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(character.DisplayName, Is.EqualTo("Vanguard"));
        }
    }
}
