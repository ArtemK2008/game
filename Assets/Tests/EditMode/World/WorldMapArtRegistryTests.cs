using NUnit.Framework;
using UnityEngine;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldMapArtRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveAuthoredWorldMapBackgroundAndAllNodeIconKinds()
        {
            WorldMapArtRegistry registry = WorldMapArtRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            Assert.That(registry.BackgroundSprite, Is.Not.Null);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.OrdinaryCombat, out Sprite ordinarySprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.Farm, out Sprite farmSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.Elite, out Sprite eliteSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.BossGate, out Sprite bossGateSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.Service, out Sprite serviceSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.Locked, out Sprite lockedSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.Current, out Sprite currentSprite), Is.True);
            Assert.That(registry.TryGetNodeSprite(WorldMapNodeIconKind.RegionTransition, out Sprite regionTransitionSprite), Is.True);
            Assert.That(ordinarySprite, Is.Not.Null);
            Assert.That(farmSprite, Is.Not.Null);
            Assert.That(eliteSprite, Is.Not.Null);
            Assert.That(bossGateSprite, Is.Not.Null);
            Assert.That(serviceSprite, Is.Not.Null);
            Assert.That(lockedSprite, Is.Not.Null);
            Assert.That(currentSprite, Is.Not.Null);
            Assert.That(regionTransitionSprite, Is.Not.Null);
            Assert.That(serviceSprite, Is.Not.SameAs(ordinarySprite));
            Assert.That(farmSprite, Is.Not.SameAs(ordinarySprite));
            Assert.That(eliteSprite, Is.Not.SameAs(farmSprite));
            Assert.That(currentSprite, Is.Not.SameAs(lockedSprite));
            Assert.That(regionTransitionSprite, Is.Not.SameAs(serviceSprite));
        }
    }
}
