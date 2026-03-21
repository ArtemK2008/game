using NUnit.Framework;
using Survivalon.Data.Gear;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class GearCatalogTests
    {
        [Test]
        public void ShouldExposeShippedPrimaryAndSupportGearProfiles()
        {
            Assert.That(GearCatalog.All, Has.Count.EqualTo(2));
            Assert.That(GearCatalog.Contains(GearIds.TrainingBlade), Is.True);
            Assert.That(GearCatalog.Contains(GearIds.GuardCharm), Is.True);

            GearProfile trainingBlade = GearCatalog.Get(GearIds.TrainingBlade);
            GearProfile guardCharm = GearCatalog.Get(GearIds.GuardCharm);

            Assert.That(trainingBlade.GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(trainingBlade.DisplayName, Is.EqualTo("Training Blade"));
            Assert.That(trainingBlade.GearCategory, Is.EqualTo(GearCategory.PrimaryCombat));
            Assert.That(trainingBlade.AttackPowerBonus, Is.EqualTo(2f));
            Assert.That(trainingBlade.MaxHealthBonus, Is.EqualTo(0f));

            Assert.That(guardCharm.GearId, Is.EqualTo(GearIds.GuardCharm));
            Assert.That(guardCharm.DisplayName, Is.EqualTo("Guard Charm"));
            Assert.That(guardCharm.GearCategory, Is.EqualTo(GearCategory.SecondarySupport));
            Assert.That(guardCharm.AttackPowerBonus, Is.EqualTo(0f));
            Assert.That(guardCharm.MaxHealthBonus, Is.EqualTo(40f));
        }
    }
}
