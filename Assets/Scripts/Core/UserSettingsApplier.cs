using System;
using Survivalon.Combat;

namespace Survivalon.Core
{
    public sealed class UserSettingsApplier
    {
        private readonly UiSystemFeedbackAudioHost uiFeedbackAudioHost;
        private readonly CombatFeedbackAudioHost combatFeedbackAudioHost;
        private readonly MusicAudioHost musicAudioHost;
        private readonly IDisplaySettingsApplier displaySettingsApplier;

        public UserSettingsApplier(
            UiSystemFeedbackAudioHost uiFeedbackAudioHost,
            CombatFeedbackAudioHost combatFeedbackAudioHost,
            MusicAudioHost musicAudioHost,
            IDisplaySettingsApplier displaySettingsApplier)
        {
            this.uiFeedbackAudioHost = uiFeedbackAudioHost;
            this.combatFeedbackAudioHost = combatFeedbackAudioHost;
            this.musicAudioHost = musicAudioHost;
            this.displaySettingsApplier = displaySettingsApplier ?? throw new ArgumentNullException(nameof(displaySettingsApplier));
        }

        public void Apply(UserSettingsState settingsState)
        {
            if (settingsState == null)
            {
                throw new ArgumentNullException(nameof(settingsState));
            }

            UserSettingsState sanitizedState = settingsState.Sanitize();
            float sfxVolume = sanitizedState.MasterVolume * sanitizedState.SfxVolume;
            float musicVolume = sanitizedState.MasterVolume * sanitizedState.MusicVolume;

            uiFeedbackAudioHost?.SetOutputVolume(sfxVolume);
            combatFeedbackAudioHost?.SetOutputVolume(sfxVolume);
            musicAudioHost?.SetOutputVolume(musicVolume);
            displaySettingsApplier.Apply(sanitizedState.UseFullscreen);
        }
    }
}
