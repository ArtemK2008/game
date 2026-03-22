using NUnit.Framework;
using Survivalon.Data.World;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class LocationIdentityCatalogTests
    {
        [Test]
        public void ShouldExposeTwoDistinctShippedLocationIdentities()
        {
            Assert.That(LocationIdentityCatalog.VerdantFrontier.LocationIdentityId, Is.Not.EqualTo(LocationIdentityCatalog.EchoCaverns.LocationIdentityId));
            Assert.That(LocationIdentityCatalog.VerdantFrontier.DisplayName, Is.EqualTo("Verdant Frontier"));
            Assert.That(LocationIdentityCatalog.VerdantFrontier.RewardSourceDisplayName, Is.EqualTo("Frontier salvage"));
            Assert.That(LocationIdentityCatalog.VerdantFrontier.RewardFocusDisplayName, Is.EqualTo("Region material farming"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.DisplayName, Is.EqualTo("Echo Caverns"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.RewardSourceDisplayName, Is.EqualTo("Cavern relic caches"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.RewardFocusDisplayName, Is.EqualTo("Persistent progression gains"));
        }
    }
}
