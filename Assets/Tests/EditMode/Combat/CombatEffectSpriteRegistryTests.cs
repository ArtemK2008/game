using NUnit.Framework;
using Survivalon.Combat;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatEffectSpriteRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveRegistryWithAllRequiredMilestone94EffectSprites()
        {
            CombatEffectSpriteRegistry registry = CombatEffectSpriteRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            AssertSprite(registry, CombatEffectVisualId.BasicImpact);
            AssertSprite(registry, CombatEffectVisualId.BurstStrike);
            AssertSprite(registry, CombatEffectVisualId.DangerPulse);
            AssertSprite(registry, CombatEffectVisualId.Defeat);
        }

        private static void AssertSprite(CombatEffectSpriteRegistry registry, CombatEffectVisualId effectVisualId)
        {
            Assert.That(registry.TryGetSprite(effectVisualId, out Sprite sprite), Is.True);
            Assert.That(sprite, Is.Not.Null);
        }
    }
}
