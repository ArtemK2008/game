using NUnit.Framework;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class PlayableCharacterProgressionEffectResolverTests
    {
        [Test]
        public void ShouldReturnZeroMaxHealthBonusForUnrankedCharacter()
        {
            PersistentCharacterState characterState = new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                progressionRank: 0,
                skillPackageId: "skill_package_vanguard_default");
            PlayableCharacterProgressionEffectResolver resolver = new PlayableCharacterProgressionEffectResolver();

            float maxHealthBonus = resolver.ResolveMaxHealthBonus(characterState);

            Assert.That(maxHealthBonus, Is.EqualTo(0f));
        }

        [Test]
        public void ShouldIncreaseMaxHealthBonusByFivePerProgressionRank()
        {
            PersistentCharacterState characterState = new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                progressionRank: 3,
                skillPackageId: "skill_package_vanguard_default");
            PlayableCharacterProgressionEffectResolver resolver = new PlayableCharacterProgressionEffectResolver();

            float maxHealthBonus = resolver.ResolveMaxHealthBonus(characterState);

            Assert.That(maxHealthBonus, Is.EqualTo(15f));
        }
    }
}
