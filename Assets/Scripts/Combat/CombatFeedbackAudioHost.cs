using UnityEngine;
using Survivalon.Core;

namespace Survivalon.Combat
{
    /// <summary>
    /// Plays baseline combat feedback sounds through the runtime-safe registry and stays silent when clips are unavailable.
    /// </summary>
    public sealed class CombatFeedbackAudioHost : MonoBehaviour
    {
        private readonly AudioClip[] cachedClips = new AudioClip[8];
        private AudioSource audioSource;
        private bool isInitialized;

        private void Awake()
        {
            EnsureInitialized();
        }

        public void TryPlay(CombatFeedbackSoundId soundId)
        {
            EnsureInitialized();

            AudioClip clip = cachedClips[(int)soundId];
            if (clip == null)
            {
                return;
            }

            audioSource.PlayOneShot(clip);
        }

        public void SetOutputVolume(float volume)
        {
            EnsureInitialized();
            audioSource.volume = Mathf.Clamp01(volume);
        }

        private void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            audioSource = RuntimeUiSupport.GetOrAddComponent<AudioSource>(gameObject);
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;

            CombatFeedbackAudioClipRegistry clipRegistry = CombatFeedbackAudioClipRegistry.LoadOrNull();
            CacheClip(clipRegistry, CombatFeedbackSoundId.PlayerAttack);
            CacheClip(clipRegistry, CombatFeedbackSoundId.EnemyAttack);
            CacheClip(clipRegistry, CombatFeedbackSoundId.PlayerHit);
            CacheClip(clipRegistry, CombatFeedbackSoundId.EnemyHit);
            CacheClip(clipRegistry, CombatFeedbackSoundId.EnemyDefeat);
            CacheClip(clipRegistry, CombatFeedbackSoundId.PlayerDefeat);
            CacheClip(clipRegistry, CombatFeedbackSoundId.DangerLowHealth);
            CacheClip(clipRegistry, CombatFeedbackSoundId.BurstStrike);

            isInitialized = true;
        }

        private void CacheClip(CombatFeedbackAudioClipRegistry clipRegistry, CombatFeedbackSoundId soundId)
        {
            if (clipRegistry == null || !clipRegistry.TryGetClip(soundId, out AudioClip clip))
            {
                cachedClips[(int)soundId] = null;
                return;
            }

            cachedClips[(int)soundId] = clip;
        }
    }
}
