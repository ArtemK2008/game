using NUnit.Framework;
using Survivalon.Characters;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Characters
{
    /// <summary>
    /// Verifies the runtime-safe playable-character world-icon registry resolves the current live roster icons.
    /// </summary>
    public sealed class PlayableCharacterWorldIconRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveRegistryWithCurrentLivePlayableCharacterWorldIcons()
        {
            PlayableCharacterWorldIconRegistry registry = PlayableCharacterWorldIconRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            AssertWorldIcon(registry, "character_vanguard");
            AssertWorldIcon(registry, "character_striker");
        }

        [Test]
        public void Resolver_ShouldFailClosedForUnknownPlayableCharacterId()
        {
            PlayableCharacterWorldIconResolver resolver = new PlayableCharacterWorldIconResolver(
                PlayableCharacterWorldIconRegistry.LoadOrNull());

            Assert.That(resolver.TryResolveWorldIcon("character_unknown", out Sprite worldIconSprite), Is.False);
            Assert.That(worldIconSprite, Is.Null);
        }

        private static void AssertWorldIcon(PlayableCharacterWorldIconRegistry registry, string characterId)
        {
            Assert.That(registry.TryGetWorldIcon(characterId, out Sprite worldIconSprite), Is.True);
            Assert.That(worldIconSprite, Is.Not.Null);
        }
    }
}
