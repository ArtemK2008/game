using NUnit.Framework;
using Survivalon.Towns;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Towns
{
    public sealed class TownServiceBackgroundRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveCavernServiceHubBackground()
        {
            TownServiceBackgroundRegistry registry = TownServiceBackgroundRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            Assert.That(registry.TryGetBackground("town_service_cavern_hub", out Sprite backgroundSprite), Is.True);
            Assert.That(backgroundSprite, Is.Not.Null);
        }
    }
}
