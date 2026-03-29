using NUnit.Framework;
using UnityEngine;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldMapArtRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveAuthoredWorldMapBackgroundAndNodeSprites()
        {
            WorldMapArtRegistry registry = WorldMapArtRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            Assert.That(registry.BackgroundSprite, Is.Not.Null);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeArtStateId.Locked, out Sprite lockedSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeArtStateId.Available, out Sprite availableSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeArtStateId.Current, out Sprite currentSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeArtStateId.Cleared, out Sprite clearedSprite), Is.True);
            Assert.That(lockedSprite, Is.Not.Null);
            Assert.That(availableSprite, Is.Not.Null);
            Assert.That(currentSprite, Is.Not.Null);
            Assert.That(clearedSprite, Is.Not.Null);
            Assert.That(currentSprite, Is.Not.SameAs(availableSprite));
            Assert.That(clearedSprite, Is.Not.SameAs(lockedSprite));
        }
    }
}
