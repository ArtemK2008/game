using NUnit.Framework;
using Survivalon.Data.Gear;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class GearCatalogTests
    {
        [Test]
        public void ShouldExposeSingleStarterPrimaryCombatGearProfile()
        {
            Assert.That(GearCatalog.All, Has.Count.EqualTo(1));
            Assert.That(GearCatalog.Contains(GearIds.TrainingBlade), Is.True);

            GearProfile gearProfile = GearCatalog.Get(GearIds.TrainingBlade);

            Assert.That(gearProfile.GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(gearProfile.DisplayName, Is.EqualTo("Training Blade"));
            Assert.That(gearProfile.GearCategory, Is.EqualTo(GearCategory.PrimaryCombat));
            Assert.That(gearProfile.AttackPowerBonus, Is.EqualTo(2f));
        }
    }
}
