using NUnit.Framework;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Verifies the small dedicated user-settings persistence seam used by the compact menus.
    /// </summary>
    public sealed class UserSettingsPersistenceServiceTests
    {
        [Test]
        public void LoadOrDefault_ShouldReturnDefaultWhenStorageIsEmpty()
        {
            MemoryUserSettingsStorage storage = new MemoryUserSettingsStorage();
            UserSettingsPersistenceService service = new UserSettingsPersistenceService(storage);

            UserSettingsState settingsState = service.LoadOrDefault();

            Assert.That(settingsState.MasterVolume, Is.EqualTo(1f));
            Assert.That(settingsState.MusicVolume, Is.EqualTo(1f));
            Assert.That(settingsState.SfxVolume, Is.EqualTo(1f));
            Assert.That(settingsState.UseFullscreen, Is.False);
        }

        [Test]
        public void LoadOrDefaultAndSave_ShouldSanitizeSettingsState()
        {
            MemoryUserSettingsStorage storage = new MemoryUserSettingsStorage();
            storage.Seed(new UserSettingsState
            {
                MasterVolume = 1.5f,
                MusicVolume = -0.25f,
                SfxVolume = 0.35f,
                UseFullscreen = true,
            });
            UserSettingsPersistenceService service = new UserSettingsPersistenceService(storage);

            UserSettingsState loadedSettingsState = service.LoadOrDefault();

            Assert.That(loadedSettingsState.MasterVolume, Is.EqualTo(1f));
            Assert.That(loadedSettingsState.MusicVolume, Is.EqualTo(0f));
            Assert.That(loadedSettingsState.SfxVolume, Is.EqualTo(0.35f).Within(0.001f));
            Assert.That(loadedSettingsState.UseFullscreen, Is.True);

            service.Save(new UserSettingsState
            {
                MasterVolume = -1f,
                MusicVolume = 1.4f,
                SfxVolume = 0.4f,
                UseFullscreen = false,
            });

            Assert.That(storage.SavedSettingsState, Is.Not.Null);
            Assert.That(storage.SavedSettingsState.MasterVolume, Is.EqualTo(0f));
            Assert.That(storage.SavedSettingsState.MusicVolume, Is.EqualTo(1f));
            Assert.That(storage.SavedSettingsState.SfxVolume, Is.EqualTo(0.4f).Within(0.001f));
            Assert.That(storage.SavedSettingsState.UseFullscreen, Is.False);
        }

        private sealed class MemoryUserSettingsStorage : IUserSettingsStorage
        {
            public UserSettingsState SavedSettingsState { get; private set; }

            public void Seed(UserSettingsState settingsState)
            {
                SavedSettingsState = settingsState?.Clone();
            }

            public bool TryLoad(out UserSettingsState settingsState)
            {
                settingsState = SavedSettingsState?.Clone();
                return settingsState != null;
            }

            public void Save(UserSettingsState settingsState)
            {
                SavedSettingsState = settingsState?.Clone();
            }
        }
    }
}
