using NUnit.Framework;
using UnityEngine;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatLocationBackgroundRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveVerdantEchoAndSunscorchCombatBackgrounds()
        {
            CombatLocationBackgroundRegistry registry = CombatLocationBackgroundRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            Assert.That(
                registry.TryGetBackground("location_identity_verdant_frontier", out Sprite verdantBackground),
                Is.True);
            Assert.That(
                registry.TryGetBackground("location_identity_echo_caverns", out Sprite cavernBackground),
                Is.True);
            Assert.That(
                registry.TryGetBackground("location_identity_sunscorch_ruins", out Sprite sunscorchBackground),
                Is.True);
            Assert.That(verdantBackground, Is.Not.Null);
            Assert.That(cavernBackground, Is.Not.Null);
            Assert.That(sunscorchBackground, Is.Not.Null);
            Assert.That(cavernBackground, Is.Not.SameAs(verdantBackground));
            Assert.That(sunscorchBackground, Is.Not.SameAs(cavernBackground));
            Assert.That(sunscorchBackground, Is.Not.SameAs(verdantBackground));
        }
    }
}
