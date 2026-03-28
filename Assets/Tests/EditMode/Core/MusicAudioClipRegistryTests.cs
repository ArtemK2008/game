using NUnit.Framework;
using Survivalon.Core;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Verifies the runtime-safe music registry resolves the current shipped calm and gameplay clips.
    /// </summary>
    public sealed class MusicAudioClipRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveRegistryWithBothMilestone89MusicClips()
        {
            MusicAudioClipRegistry registry = MusicAudioClipRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            AssertClip(registry, MusicContextId.Calm);
            AssertClip(registry, MusicContextId.Gameplay);
        }

        private static void AssertClip(MusicAudioClipRegistry registry, MusicContextId contextId)
        {
            Assert.That(registry.TryGetClip(contextId, out AudioClip clip), Is.True);
            Assert.That(clip, Is.Not.Null);
        }
    }
}
