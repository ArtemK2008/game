namespace Survivalon.Core
{
    public interface IUserSettingsStorage
    {
        bool TryLoad(out UserSettingsState settingsState);

        void Save(UserSettingsState settingsState);
    }
}
