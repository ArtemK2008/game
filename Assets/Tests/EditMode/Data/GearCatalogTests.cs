using NUnit.Framework;
using Survivalon.Data.Gear;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class GearCatalogTests
    {
        [Test]
        public void ShouldExposeShippedStarterAndEarnedGearProfiles()
        {
            Assert.That(GearCatalog.All, Has.Count.EqualTo(3));
            Assert.That(GearCatalog.Contains(GearIds.TrainingBlade), Is.True);
            Assert.That(GearCatalog.Contains(GearIds.GatebreakerBlade), Is.True);
            Assert.That(GearCatalog.Contains(GearIds.GuardCharm), Is.True);

            GearProfile trainingBlade = GearCatalog.Get(GearIds.TrainingBlade);
            GearProfile gatebreakerBlade = GearCatalog.Get(GearIds.GatebreakerBlade);
            GearProfile guardCharm = GearCatalog.Get(GearIds.GuardCharm);

            Assert.That(trainingBlade.GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(trainingBlade.DisplayName, Is.EqualTo("Training Blade"));
            Assert.That(trainingBlade.GearCategory, Is.EqualTo(GearCategory.PrimaryCombat));
            Assert.That(trainingBlade.AttackPowerBonus, Is.EqualTo(2f));
            Assert.That(trainingBlade.MaxHealthBonus, Is.EqualTo(0f));

            Assert.That(gatebreakerBlade.GearId, Is.EqualTo(GearIds.GatebreakerBlade));
            Assert.That(gatebreakerBlade.DisplayName, Is.EqualTo("Gatebreaker Blade"));
            Assert.That(gatebreakerBlade.GearCategory, Is.EqualTo(GearCategory.PrimaryCombat));
            Assert.That(gatebreakerBlade.AttackPowerBonus, Is.EqualTo(4f));
            Assert.That(gatebreakerBlade.MaxHealthBonus, Is.EqualTo(0f));

            Assert.That(guardCharm.GearId, Is.EqualTo(GearIds.GuardCharm));
            Assert.That(guardCharm.DisplayName, Is.EqualTo("Guard Charm"));
            Assert.That(guardCharm.GearCategory, Is.EqualTo(GearCategory.SecondarySupport));
            Assert.That(guardCharm.AttackPowerBonus, Is.EqualTo(0f));
            Assert.That(guardCharm.MaxHealthBonus, Is.EqualTo(40f));
        }
    }
}
