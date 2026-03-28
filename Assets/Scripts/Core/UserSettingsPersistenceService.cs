using System;

namespace Survivalon.Core
{
    public sealed class UserSettingsPersistenceService
    {
        private readonly IUserSettingsStorage storage;

        public UserSettingsPersistenceService(IUserSettingsStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public UserSettingsState LoadOrDefault()
        {
            if (!storage.TryLoad(out UserSettingsState settingsState) || settingsState == null)
            {
                return UserSettingsState.CreateDefault();
            }

            return settingsState.Sanitize();
        }

        public void Save(UserSettingsState settingsState)
        {
            if (settingsState == null)
            {
                throw new ArgumentNullException(nameof(settingsState));
            }

            storage.Save(settingsState.Sanitize());
        }
    }
}
