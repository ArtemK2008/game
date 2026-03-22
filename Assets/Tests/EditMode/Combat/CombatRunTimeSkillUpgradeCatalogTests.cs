using NUnit.Framework;
using System.Linq;
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
            Assert.That(upgradeOptions[0].DisplayName, Is.EqualTo("Burst Tempo"));
            Assert.That(upgradeOptions[0].Description, Is.EqualTo("Burst Strike triggers faster during this run."));
            Assert.That(upgradeOptions[1].DisplayName, Is.EqualTo("Burst Payload"));
            Assert.That(upgradeOptions[1].Description, Is.EqualTo("Burst Strike hits harder during this run."));
        }

        [Test]
        public void ShouldNotExposeRunChoiceUiOnlyFieldsOnCombatUpgradeOptions()
        {
            string[] propertyNames = typeof(CombatRunTimeSkillUpgradeOption)
                .GetProperties()
                .Select(property => property.Name)
                .ToArray();

            Assert.That(propertyNames, Does.Not.Contain("SourceSkillDisplayName"));
            Assert.That(propertyNames, Does.Not.Contain("SelectionHint"));
        }

        [Test]
        public void ShouldResolveNoRunTimeUpgradeOptionsWhenTriggeredActiveSkillIsMissing()
        {
            var upgradeOptions = CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(null);

            Assert.That(upgradeOptions, Is.Empty);
        }
    }
}
