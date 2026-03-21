using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatDirectDamageSkillEffectResolverTests
    {
        [Test]
        public void ShouldResolveBasicAttackDamageMultiplier()
        {
            CombatDirectDamageSkillEffectResolver resolver = new CombatDirectDamageSkillEffectResolver();

            float attackPowerMultiplier = resolver.ResolveAttackPowerMultiplier(CombatSkillCatalog.BasicAttack);

            Assert.That(attackPowerMultiplier, Is.EqualTo(1f));
        }

        [Test]
        public void ShouldResolveBurstStrikeDamageMultiplier()
        {
            CombatDirectDamageSkillEffectResolver resolver = new CombatDirectDamageSkillEffectResolver();

            float attackPowerMultiplier = resolver.ResolveAttackPowerMultiplier(CombatSkillCatalog.BurstStrike);

            Assert.That(attackPowerMultiplier, Is.EqualTo(2f));
        }

        [Test]
        public void ShouldResolveBurstTempoDamageMultiplier()
        {
            CombatDirectDamageSkillEffectResolver resolver = new CombatDirectDamageSkillEffectResolver();

            float attackPowerMultiplier = resolver.ResolveAttackPowerMultiplier(CombatSkillCatalog.BurstTempo);

            Assert.That(attackPowerMultiplier, Is.EqualTo(2f));
        }

        [Test]
        public void ShouldResolveBurstPayloadDamageMultiplier()
        {
            CombatDirectDamageSkillEffectResolver resolver = new CombatDirectDamageSkillEffectResolver();

            float attackPowerMultiplier = resolver.ResolveAttackPowerMultiplier(CombatSkillCatalog.BurstPayload);

            Assert.That(attackPowerMultiplier, Is.EqualTo(3f));
        }
    }
}
