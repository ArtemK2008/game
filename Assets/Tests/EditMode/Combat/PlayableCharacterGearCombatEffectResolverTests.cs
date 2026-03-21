using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class PlayableCharacterGearCombatEffectResolverTests
    {
        [Test]
        public void ShouldResolveTrainingBladeAttackPowerBonusWhenEquipped()
        {
            PersistentCharacterState characterState = new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                loadoutState: new PersistentLoadoutState(
                    equippedGearStates: new[]
                    {
                        new EquippedGearState(GearIds.TrainingBlade, GearCategory.PrimaryCombat),
                    }));
            PlayableCharacterGearCombatEffectResolver resolver = new PlayableCharacterGearCombatEffectResolver();

            float attackPowerBonus = resolver.ResolveAttackPowerBonus(characterState);

            Assert.That(attackPowerBonus, Is.EqualTo(2f));
        }

        [Test]
        public void ShouldReturnZeroAttackPowerBonusWhenNoGearIsEquipped()
        {
            PersistentCharacterState characterState = new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true);
            PlayableCharacterGearCombatEffectResolver resolver = new PlayableCharacterGearCombatEffectResolver();

            float attackPowerBonus = resolver.ResolveAttackPowerBonus(characterState);

            Assert.That(attackPowerBonus, Is.EqualTo(0f));
        }

        [Test]
        public void ShouldIgnoreUnknownEquippedGearWhenResolvingAttackPowerBonus()
        {
            PersistentCharacterState characterState = new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                loadoutState: new PersistentLoadoutState(
                    equippedGearStates: new[]
                    {
                        new EquippedGearState("gear_unknown_test", GearCategory.PrimaryCombat),
                    }));
            PlayableCharacterGearCombatEffectResolver resolver = new PlayableCharacterGearCombatEffectResolver();

            float attackPowerBonus = resolver.ResolveAttackPowerBonus(characterState);

            Assert.That(attackPowerBonus, Is.EqualTo(0f));
        }
    }
}
