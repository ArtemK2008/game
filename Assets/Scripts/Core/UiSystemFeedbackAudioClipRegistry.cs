using UnityEngine;

namespace Survivalon.Core
{
    /// <summary>
    /// Хранит runtime-safe ссылки на клипы UI/system feedback для текущего прототипа.
    /// </summary>
    public sealed class UiSystemFeedbackAudioClipRegistry : ScriptableObject
    {
        private const string ResourceName = "UiSystemFeedbackAudioClipRegistry";

        [SerializeField] private AudioClip uiClickClip;
        [SerializeField] private AudioClip uiConfirmClip;
        [SerializeField] private AudioClip uiErrorClip;
        [SerializeField] private AudioClip stateNodeClearClip;
        [SerializeField] private AudioClip stateRouteUnlockClip;
        [SerializeField] private AudioClip stateBossRewardClip;

        public static UiSystemFeedbackAudioClipRegistry LoadOrNull()
        {
            return Resources.Load<UiSystemFeedbackAudioClipRegistry>(ResourceName);
        }

        public bool TryGetClip(UiSystemFeedbackSoundId soundId, out AudioClip clip)
        {
            switch (soundId)
            {
                case UiSystemFeedbackSoundId.UiClick:
                    clip = uiClickClip;
                    return clip != null;
                case UiSystemFeedbackSoundId.UiConfirm:
                    clip = uiConfirmClip;
                    return clip != null;
                case UiSystemFeedbackSoundId.UiError:
                    clip = uiErrorClip;
                    return clip != null;
                case UiSystemFeedbackSoundId.StateNodeClear:
                    clip = stateNodeClearClip;
                    return clip != null;
                case UiSystemFeedbackSoundId.StateRouteUnlock:
                    clip = stateRouteUnlockClip;
                    return clip != null;
                case UiSystemFeedbackSoundId.StateBossReward:
                    clip = stateBossRewardClip;
                    return clip != null;
                default:
                    clip = null;
                    return false;
            }
        }
    }
}
