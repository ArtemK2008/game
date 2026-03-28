using NUnit.Framework;
using Survivalon.Core;
using UnityEditor;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Проверяет runtime-safe registry и базовую корректность импорта UI/system feedback-клипов.
    /// </summary>
    public sealed class UiSystemFeedbackAudioClipRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveRegistryWithCurrentRequiredUiSystemFeedbackClips()
        {
            UiSystemFeedbackAudioClipRegistry registry = UiSystemFeedbackAudioClipRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            AssertClip(registry, UiSystemFeedbackSoundId.UiClick);
            AssertClip(registry, UiSystemFeedbackSoundId.UiConfirm);
            AssertClip(registry, UiSystemFeedbackSoundId.UiError);
            AssertClip(registry, UiSystemFeedbackSoundId.StateNodeClear);
            AssertClip(registry, UiSystemFeedbackSoundId.StateRouteUnlock);
            AssertClip(registry, UiSystemFeedbackSoundId.StateBossReward);
        }

        [Test]
        public void CurrentUiSystemFeedbackClips_ShouldUseNonStreamingImportSettings()
        {
            AssertImportSettings("Assets/Audio/UI/ui_click.wav");
            AssertImportSettings("Assets/Audio/UI/ui_confirm.wav");
            AssertImportSettings("Assets/Audio/UI/ui_error.wav");
            AssertImportSettings("Assets/Audio/System/state_node_clear.wav");
            AssertImportSettings("Assets/Audio/System/state_route_unlock.wav");
            AssertImportSettings("Assets/Audio/System/state_boss_reward.wav");
        }

        private static void AssertClip(UiSystemFeedbackAudioClipRegistry registry, UiSystemFeedbackSoundId soundId)
        {
            Assert.That(registry.TryGetClip(soundId, out AudioClip clip), Is.True);
            Assert.That(clip, Is.Not.Null);
        }

        private static void AssertImportSettings(string assetPath)
        {
            AudioImporter importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            AudioImporterSampleSettings sampleSettings = importer == null
                ? default
                : importer.defaultSampleSettings;

            Assert.That(importer, Is.Not.Null, assetPath);
            Assert.That(sampleSettings.loadType, Is.Not.EqualTo(AudioClipLoadType.Streaming), assetPath);
            Assert.That(importer.loadInBackground, Is.False, assetPath);
        }
    }
}
