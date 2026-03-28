using UnityEngine;

namespace Survivalon.Core
{
    /// <summary>
    /// Stores runtime-safe references to the prototype's current music clips.
    /// </summary>
    public sealed class MusicAudioClipRegistry : ScriptableObject
    {
        private const string ResourceName = "MusicAudioClipRegistry";

        [SerializeField] private AudioClip calmContextClip;
        [SerializeField] private AudioClip gameplayContextClip;

        public static MusicAudioClipRegistry LoadOrNull()
        {
            return Resources.Load<MusicAudioClipRegistry>(ResourceName);
        }

        public bool TryGetClip(MusicContextId contextId, out AudioClip clip)
        {
            switch (contextId)
            {
                case MusicContextId.Calm:
                    clip = calmContextClip;
                    return clip != null;
                case MusicContextId.Gameplay:
                    clip = gameplayContextClip;
                    return clip != null;
                default:
                    clip = null;
                    return false;
            }
        }
    }
}
