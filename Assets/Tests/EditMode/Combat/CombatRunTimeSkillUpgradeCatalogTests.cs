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
            Assert.That(upgradeOptions[0], Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
            Assert.That(upgradeOptions[1], Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstPayload));
            Assert.That(upgradeOptions[0].SourceSkillDisplayName, Is.EqualTo("Burst Strike"));
            Assert.That(upgradeOptions[0].SelectionHint, Is.EqualTo("Steadier burst pressure."));
            Assert.That(upgradeOptions[1].SourceSkillDisplayName, Is.EqualTo("Burst Strike"));
            Assert.That(upgradeOptions[1].SelectionHint, Is.EqualTo("Bigger damage spikes."));
        }

        [Test]
        public void ShouldResolveNoRunTimeUpgradeOptionsWhenTriggeredActiveSkillIsMissing()
        {
            var upgradeOptions = CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(null);

            Assert.That(upgradeOptions, Is.Empty);
        }
    }
}
