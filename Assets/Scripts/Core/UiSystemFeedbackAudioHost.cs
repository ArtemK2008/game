using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Survivalon.Core
{
    /// <summary>
    /// РџСЂРѕРёРіСЂС‹РІР°РµС‚ Р±Р°Р·РѕРІС‹Рµ UI/system feedback-Р·РІСѓРєРё Рё Р±РµР·РѕРїР°СЃРЅРѕ РјРѕР»С‡РёС‚, РµСЃР»Рё РєР»РёРїС‹ РЅРµ РґРѕСЃС‚СѓРїРЅС‹.
    /// </summary>
    public sealed class UiSystemFeedbackAudioHost : MonoBehaviour
    {
        private readonly AudioClip[] cachedClips = new AudioClip[5];
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = RuntimeUiSupport.GetOrAddComponent<AudioSource>(gameObject);
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;

            CacheClip(UiSystemFeedbackSoundId.UiClick);
            CacheClip(UiSystemFeedbackSoundId.UiConfirm);
            CacheClip(UiSystemFeedbackSoundId.UiError);
            CacheClip(UiSystemFeedbackSoundId.StateUnlock);
            CacheClip(UiSystemFeedbackSoundId.StateBossClear);
        }

        public void TryPlay(UiSystemFeedbackSoundId soundId)
        {
            AudioClip clip = cachedClips[(int)soundId];
            if (clip == null)
            {
                return;
            }

            audioSource.PlayOneShot(clip);
        }

        private void CacheClip(UiSystemFeedbackSoundId soundId)
        {
            cachedClips[(int)soundId] = LoadClip(soundId);
        }

        private static AudioClip LoadClip(UiSystemFeedbackSoundId soundId)
        {
#if UNITY_EDITOR
            string assetPath = ResolveAssetPath(soundId);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
#else
            return null;
#endif
        }

        private static string ResolveAssetPath(UiSystemFeedbackSoundId soundId)
        {
            switch (soundId)
            {
                case UiSystemFeedbackSoundId.UiClick:
                    return "Assets/Audio/UI/ui_click.wav";
                case UiSystemFeedbackSoundId.UiConfirm:
                    return "Assets/Audio/UI/ui_confirm.wav";
                case UiSystemFeedbackSoundId.UiError:
                    return "Assets/Audio/UI/ui_error.wav";
                case UiSystemFeedbackSoundId.StateUnlock:
                    return "Assets/Audio/System/state_unlock.wav";
                case UiSystemFeedbackSoundId.StateBossClear:
                    return "Assets/Audio/System/state_boss_clear.wav";
                default:
                    return null;
            }
        }
    }
}
