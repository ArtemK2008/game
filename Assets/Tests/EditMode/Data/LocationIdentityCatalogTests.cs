using NUnit.Framework;
using Survivalon.Core;
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
            Assert.That(LocationIdentityCatalog.VerdantFrontier.EnemyEmphasisDisplayName, Is.EqualTo("Frontier raiders"));
            Assert.That(LocationIdentityCatalog.VerdantFrontier.IsFallbackIdentity, Is.False);
            Assert.That(LocationIdentityCatalog.EchoCaverns.DisplayName, Is.EqualTo("Echo Caverns"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.RewardSourceDisplayName, Is.EqualTo("Cavern relic caches"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.RewardFocusDisplayName, Is.EqualTo("Persistent progression gains"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.EnemyEmphasisDisplayName, Is.EqualTo("Gate guardians"));
            Assert.That(LocationIdentityCatalog.EchoCaverns.IsFallbackIdentity, Is.False);
        }

        [Test]
        public void ShouldMarkFallbackIdentitiesExplicitly()
        {
            LocationIdentityDefinition fallbackIdentity =
                LocationIdentityCatalog.CreateFallback(new RegionId("region_fallback"));

            Assert.That(fallbackIdentity.DisplayName, Is.EqualTo("region_fallback"));
            Assert.That(fallbackIdentity.RewardSourceDisplayName, Is.EqualTo("Regional stockpile"));
            Assert.That(fallbackIdentity.RewardFocusDisplayName, Is.EqualTo("Mixed regional value"));
            Assert.That(fallbackIdentity.EnemyEmphasisDisplayName, Is.EqualTo("Mixed local threats"));
            Assert.That(fallbackIdentity.IsFallbackIdentity, Is.True);
        }

        [Test]
        public void ShouldKeepMechanicalBossRewardBonusOutOfLocationIdentityDefinition()
        {
            Assert.That(
                typeof(LocationIdentityDefinition).GetProperty("BossPersistentProgressionMaterialBonus"),
                Is.Null);
        }
    }
}
