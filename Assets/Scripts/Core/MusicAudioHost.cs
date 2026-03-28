using UnityEngine;

namespace Survivalon.Core
{
    /// <summary>
    /// Plays the current prototype music context through one shared runtime-safe host.
    /// </summary>
    public sealed class MusicAudioHost : MonoBehaviour
    {
        private readonly AudioClip[] cachedClips = new AudioClip[2];
        private AudioSource audioSource;
        private bool isInitialized;
        private bool hasCurrentContext;
        private MusicContextId currentContext;

        private void Awake()
        {
            EnsureInitialized();
        }

        public void SetContext(MusicContextId contextId)
        {
            EnsureInitialized();

            AudioClip clip = cachedClips[(int)contextId];
            if (hasCurrentContext && currentContext == contextId && audioSource.clip == clip)
            {
                if (clip != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }

                return;
            }

            currentContext = contextId;
            hasCurrentContext = true;

            if (clip == null)
            {
                audioSource.Stop();
                audioSource.clip = null;
                return;
            }

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }

        private void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            audioSource = RuntimeUiSupport.GetOrAddComponent<AudioSource>(gameObject);
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;

            MusicAudioClipRegistry clipRegistry = MusicAudioClipRegistry.LoadOrNull();
            CacheClip(clipRegistry, MusicContextId.Calm);
            CacheClip(clipRegistry, MusicContextId.Gameplay);

            isInitialized = true;
        }

        private void CacheClip(MusicAudioClipRegistry clipRegistry, MusicContextId contextId)
        {
            if (clipRegistry == null || !clipRegistry.TryGetClip(contextId, out AudioClip clip))
            {
                cachedClips[(int)contextId] = null;
                return;
            }

            cachedClips[(int)contextId] = clip;
        }
    }
}
