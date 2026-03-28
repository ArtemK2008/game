using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Verifies that compact menu settings apply through the dedicated audio/display seam.
    /// </summary>
    public sealed class UserSettingsApplierTests
    {
        [Test]
        public void Apply_ShouldUpdateCombinedAudioVolumesAndDisplayMode()
        {
            GameObject rootObject = new GameObject("UserSettingsApplierHost");
            GameObject uiHostObject = new GameObject("UiFeedbackHost");
            GameObject combatHostObject = new GameObject("CombatFeedbackHost");
            GameObject musicHostObject = new GameObject("MusicHost");
            uiHostObject.transform.SetParent(rootObject.transform, false);
            combatHostObject.transform.SetParent(rootObject.transform, false);
            musicHostObject.transform.SetParent(rootObject.transform, false);

            try
            {
                UiSystemFeedbackAudioHost uiAudioHost = uiHostObject.AddComponent<UiSystemFeedbackAudioHost>();
                CombatFeedbackAudioHost combatAudioHost = combatHostObject.AddComponent<CombatFeedbackAudioHost>();
                MusicAudioHost musicAudioHost = musicHostObject.AddComponent<MusicAudioHost>();
                FakeDisplaySettingsApplier displaySettingsApplier = new FakeDisplaySettingsApplier();
                UserSettingsApplier applier = new UserSettingsApplier(
                    uiAudioHost,
                    combatAudioHost,
                    musicAudioHost,
                    displaySettingsApplier);

                applier.Apply(new UserSettingsState
                {
                    MasterVolume = 0.5f,
                    MusicVolume = 0.8f,
                    SfxVolume = 0.2f,
                    UseFullscreen = true,
                });

                Assert.That(uiAudioHost.GetComponent<AudioSource>().volume, Is.EqualTo(0.1f).Within(0.001f));
                Assert.That(combatAudioHost.GetComponent<AudioSource>().volume, Is.EqualTo(0.1f).Within(0.001f));
                Assert.That(musicAudioHost.GetComponent<AudioSource>().volume, Is.EqualTo(0.4f).Within(0.001f));
                Assert.That(displaySettingsApplier.ApplyCallCount, Is.EqualTo(1));
                Assert.That(displaySettingsApplier.LastAppliedFullscreen, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(rootObject);
            }
        }

        private sealed class FakeDisplaySettingsApplier : IDisplaySettingsApplier
        {
            public bool LastAppliedFullscreen { get; private set; }

            public int ApplyCallCount { get; private set; }

            public void Apply(bool useFullscreen)
            {
                LastAppliedFullscreen = useFullscreen;
                ApplyCallCount++;
            }
        }
    }
}
