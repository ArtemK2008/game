using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatTriggeredActiveSkillUpgradeResolverTests
    {
        [Test]
        public void ShouldResolveBaseBurstStrikeValuesWhenRunTimeUpgradeIsMissing()
        {
            Assert.That(
                CombatTriggeredActiveSkillUpgradeResolver.ResolveIntervalSeconds(
                    CombatSkillCatalog.BurstStrike,
                    null),
                Is.EqualTo(2.5f).Within(0.001f));
            Assert.That(
                CombatTriggeredActiveSkillUpgradeResolver.ResolveAttackPowerMultiplier(
                    CombatSkillCatalog.BurstStrike,
                    null),
                Is.EqualTo(2f));
        }

        [Test]
        public void ShouldResolveBurstTempoUpgradeValues()
        {
            Assert.That(
                CombatTriggeredActiveSkillUpgradeResolver.ResolveIntervalSeconds(
                    CombatSkillCatalog.BurstStrike,
                    CombatRunTimeSkillUpgradeCatalog.BurstTempo),
                Is.EqualTo(1.75f).Within(0.001f));
            Assert.That(
                CombatTriggeredActiveSkillUpgradeResolver.ResolveAttackPowerMultiplier(
                    CombatSkillCatalog.BurstStrike,
                    CombatRunTimeSkillUpgradeCatalog.BurstTempo),
                Is.EqualTo(2f));
        }

        [Test]
        public void ShouldResolveBurstPayloadUpgradeValues()
        {
            Assert.That(
                CombatTriggeredActiveSkillUpgradeResolver.ResolveIntervalSeconds(
                    CombatSkillCatalog.BurstStrike,
                    CombatRunTimeSkillUpgradeCatalog.BurstPayload),
                Is.EqualTo(2.5f).Within(0.001f));
            Assert.That(
                CombatTriggeredActiveSkillUpgradeResolver.ResolveAttackPowerMultiplier(
                    CombatSkillCatalog.BurstStrike,
                    CombatRunTimeSkillUpgradeCatalog.BurstPayload),
                Is.EqualTo(3f));
        }
    }
}
