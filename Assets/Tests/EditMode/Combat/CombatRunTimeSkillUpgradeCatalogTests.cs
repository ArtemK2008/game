using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatRunTimeSkillUpgradeCatalogTests
    {
        [Test]
        public void ShouldResolveBurstStrikeRunTimeUpgradeOptions()
        {
            var upgradeOptions = CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(
                CombatSkillCatalog.BurstStrike);

            Assert.That(upgradeOptions.Count, Is.EqualTo(2));
            Assert.That(upgradeOptions[0].UpgradedTriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstTempo));
            Assert.That(upgradeOptions[1].UpgradedTriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstPayload));
        }

        [Test]
        public void ShouldResolveNoRunTimeUpgradeOptionsWhenTriggeredActiveSkillIsMissing()
        {
            var upgradeOptions = CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(null);

            Assert.That(upgradeOptions, Is.Empty);
        }
    }
}
