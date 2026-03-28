using System;
using UnityEngine;

namespace Survivalon.Core
{
    [Serializable]
    public sealed class UserSettingsState
    {
        public const float VolumeStep = 0.1f;

        public float MasterVolume = 1f;
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;
        public bool UseFullscreen = false;

        public static UserSettingsState CreateDefault()
        {
            return new UserSettingsState();
        }

        public UserSettingsState Clone()
        {
            return new UserSettingsState
            {
                MasterVolume = MasterVolume,
                MusicVolume = MusicVolume,
                SfxVolume = SfxVolume,
                UseFullscreen = UseFullscreen,
            };
        }

        public UserSettingsState Sanitize()
        {
            UserSettingsState sanitizedState = Clone();
            sanitizedState.MasterVolume = Mathf.Clamp01(sanitizedState.MasterVolume);
            sanitizedState.MusicVolume = Mathf.Clamp01(sanitizedState.MusicVolume);
            sanitizedState.SfxVolume = Mathf.Clamp01(sanitizedState.SfxVolume);
            return sanitizedState;
        }

        public UserSettingsState WithMasterVolume(float value)
        {
            UserSettingsState updatedState = Sanitize();
            updatedState.MasterVolume = Mathf.Clamp01(value);
            return updatedState;
        }

        public UserSettingsState WithMusicVolume(float value)
        {
            UserSettingsState updatedState = Sanitize();
            updatedState.MusicVolume = Mathf.Clamp01(value);
            return updatedState;
        }

        public UserSettingsState WithSfxVolume(float value)
        {
            UserSettingsState updatedState = Sanitize();
            updatedState.SfxVolume = Mathf.Clamp01(value);
            return updatedState;
        }

        public UserSettingsState WithFullscreen(bool useFullscreen)
        {
            UserSettingsState updatedState = Sanitize();
            updatedState.UseFullscreen = useFullscreen;
            return updatedState;
        }
    }
}
