using UnityEngine;

namespace Survivalon.Core
{
    /// <summary>
    /// Проигрывает базовые UI/system feedback-звуки через runtime-safe registry и молчит, если клипы недоступны.
    /// </summary>
    public sealed class UiSystemFeedbackAudioHost : MonoBehaviour
    {
        private readonly AudioClip[] cachedClips = new AudioClip[5];
        private AudioSource audioSource;
        private bool isInitialized;

        private void Awake()
        {
            EnsureInitialized();
        }

        public void TryPlay(UiSystemFeedbackSoundId soundId)
        {
            EnsureInitialized();

            AudioClip clip = cachedClips[(int)soundId];
            if (clip == null)
            {
                return;
            }

            audioSource.PlayOneShot(clip);
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
            UiSystemFeedbackAudioClipRegistry clipRegistry = UiSystemFeedbackAudioClipRegistry.LoadOrNull();

            CacheClip(clipRegistry, UiSystemFeedbackSoundId.UiClick);
            CacheClip(clipRegistry, UiSystemFeedbackSoundId.UiConfirm);
            CacheClip(clipRegistry, UiSystemFeedbackSoundId.UiError);
            CacheClip(clipRegistry, UiSystemFeedbackSoundId.StateUnlock);
            CacheClip(clipRegistry, UiSystemFeedbackSoundId.StateBossClear);

            isInitialized = true;
        }

        private void CacheClip(UiSystemFeedbackAudioClipRegistry clipRegistry, UiSystemFeedbackSoundId soundId)
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
