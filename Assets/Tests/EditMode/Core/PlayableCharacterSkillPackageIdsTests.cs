using NUnit.Framework;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Core
{
    public sealed class PlayableCharacterSkillPackageIdsTests
    {
        [Test]
        public void ShouldExposeStableDistinctCurrentShippedPlayableCharacterSkillPackageIds()
        {
            Assert.That(
                PlayableCharacterSkillPackageIds.VanguardDefault,
                Is.EqualTo("skill_package_vanguard_default"));
            Assert.That(
                PlayableCharacterSkillPackageIds.VanguardBurstDrill,
                Is.EqualTo("skill_package_vanguard_burst_drill"));
            Assert.That(
                PlayableCharacterSkillPackageIds.StrikerDefault,
                Is.EqualTo("skill_package_striker_default"));
            Assert.That(
                PlayableCharacterSkillPackageIds.VanguardDefault,
                Is.Not.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
            Assert.That(
                PlayableCharacterSkillPackageIds.VanguardDefault,
                Is.Not.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
            Assert.That(
                PlayableCharacterSkillPackageIds.VanguardBurstDrill,
                Is.Not.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
        }
    }
}
