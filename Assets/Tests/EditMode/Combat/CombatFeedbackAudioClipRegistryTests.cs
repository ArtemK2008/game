using NUnit.Framework;
using Survivalon.Combat;
using UnityEditor;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Combat
{
    /// <summary>
    /// Проверяет runtime-safe registry и базовую корректность импорта combat feedback-клипов.
    /// </summary>
    public sealed class CombatFeedbackAudioClipRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveRegistryWithAllRequiredMilestone88Clips()
        {
            CombatFeedbackAudioClipRegistry registry = CombatFeedbackAudioClipRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            AssertClip(registry, CombatFeedbackSoundId.PlayerAttack);
            AssertClip(registry, CombatFeedbackSoundId.EnemyAttack);
            AssertClip(registry, CombatFeedbackSoundId.PlayerHit);
            AssertClip(registry, CombatFeedbackSoundId.EnemyHit);
            AssertClip(registry, CombatFeedbackSoundId.EnemyDefeat);
            AssertClip(registry, CombatFeedbackSoundId.PlayerDefeat);
            AssertClip(registry, CombatFeedbackSoundId.DangerLowHealth);
            AssertClip(registry, CombatFeedbackSoundId.BurstStrike);
        }

        [Test]
        public void Milestone88CombatClips_ShouldUseNonStreamingImportSettings()
        {
            AssertImportSettings("Assets/Audio/Combat/combat_player_attack.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_enemy_attack.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_player_hit.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_enemy_hit.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_enemy_defeat.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_player_defeat.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_danger_low_health.wav");
            AssertImportSettings("Assets/Audio/Combat/combat_skill_burst_strike.wav");
        }

        private static void AssertClip(CombatFeedbackAudioClipRegistry registry, CombatFeedbackSoundId soundId)
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
