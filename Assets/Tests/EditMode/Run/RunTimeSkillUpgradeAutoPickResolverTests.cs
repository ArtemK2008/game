using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Run;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunTimeSkillUpgradeAutoPickResolverTests
    {
        [Test]
        public void ShouldResolveBurstTempoAsCurrentShippedAutomaticBaseline()
        {
            RunTimeSkillUpgradeAutoPickResolver resolver = new RunTimeSkillUpgradeAutoPickResolver();

            CombatRunTimeSkillUpgradeOption resolvedOption = resolver.ResolveAutomaticFlowSelection(
                CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(CombatSkillCatalog.BurstStrike));

            Assert.That(resolvedOption, Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
        }

        [Test]
        public void ShouldResolveBurstTempoWithoutDependingOnAvailableOptionOrdering()
        {
            RunTimeSkillUpgradeAutoPickResolver resolver = new RunTimeSkillUpgradeAutoPickResolver();

            CombatRunTimeSkillUpgradeOption resolvedOption = resolver.ResolveAutomaticFlowSelection(
                new[]
                {
                    CombatRunTimeSkillUpgradeCatalog.BurstPayload,
                    CombatRunTimeSkillUpgradeCatalog.BurstTempo,
                });

            Assert.That(resolvedOption, Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
        }

        [Test]
        public void ShouldResolveNoAutomaticBaselineWhenCurrentShippedBaselineIsUnavailable()
        {
            RunTimeSkillUpgradeAutoPickResolver resolver = new RunTimeSkillUpgradeAutoPickResolver();

            CombatRunTimeSkillUpgradeOption resolvedOption = resolver.ResolveAutomaticFlowSelection(
                new[]
                {
                    CombatRunTimeSkillUpgradeCatalog.BurstPayload,
                });

            Assert.That(resolvedOption, Is.Null);
        }
    }
}
