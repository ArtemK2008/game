using System;
using System.IO;
using NUnit.Framework;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Core
{
    public sealed class FileUserSettingsStorageTests
    {
        [Test]
        public void TryLoad_ShouldReturnFalseWhenFileIsMissing()
        {
            string storagePath = CreateStoragePath();

            try
            {
                FileUserSettingsStorage storage = new FileUserSettingsStorage(storagePath);

                Assert.That(storage.TryLoad(out UserSettingsState settingsState), Is.False);
                Assert.That(settingsState, Is.Null);
            }
            finally
            {
                DeleteStorageArtifacts(storagePath);
            }
        }

        [Test]
        public void TryLoad_ShouldReturnFalseWhenFileIsEmpty()
        {
            string storagePath = CreateStoragePath();
            EnsureParentDirectoryExists(storagePath);
            File.WriteAllText(storagePath, string.Empty);

            try
            {
                FileUserSettingsStorage storage = new FileUserSettingsStorage(storagePath);

                Assert.That(storage.TryLoad(out UserSettingsState settingsState), Is.False);
                Assert.That(settingsState, Is.Null);
            }
            finally
            {
                DeleteStorageArtifacts(storagePath);
            }
        }

        [Test]
        public void TryLoad_ShouldReturnFalseWhenFileContentIsMalformed()
        {
            string storagePath = CreateStoragePath();
            EnsureParentDirectoryExists(storagePath);
            File.WriteAllText(storagePath, "{ malformed json");

            try
            {
                FileUserSettingsStorage storage = new FileUserSettingsStorage(storagePath);

                Assert.That(storage.TryLoad(out UserSettingsState settingsState), Is.False);
                Assert.That(settingsState, Is.Null);
            }
            finally
            {
                DeleteStorageArtifacts(storagePath);
            }
        }

        [Test]
        public void TryLoad_ShouldReturnFalseWhenFileIsUnreadable()
        {
            string storagePath = CreateStoragePath();
            EnsureParentDirectoryExists(storagePath);
            File.WriteAllText(storagePath, "{\"masterVolume\":0.5}");

            try
            {
                FileUserSettingsStorage storage = new FileUserSettingsStorage(storagePath);
                using FileStream lockedStream = new FileStream(
                    storagePath,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None);

                Assert.That(storage.TryLoad(out UserSettingsState settingsState), Is.False);
                Assert.That(settingsState, Is.Null);
            }
            finally
            {
                DeleteStorageArtifacts(storagePath);
            }
        }

        [Test]
        public void Save_ShouldCreateParentDirectoryAndPersistReadableSettings()
        {
            string storagePath = CreateStoragePath();

            try
            {
                FileUserSettingsStorage storage = new FileUserSettingsStorage(storagePath);
                UserSettingsState savedState = new UserSettingsState
                {
                    MasterVolume = 0.6f,
                    MusicVolume = 0.4f,
                    SfxVolume = 0.2f,
                    UseFullscreen = true,
                };

                storage.Save(savedState);

                Assert.That(File.Exists(storagePath), Is.True);
                Assert.That(storage.TryLoad(out UserSettingsState loadedState), Is.True);
                Assert.That(loadedState, Is.Not.Null);
                Assert.That(loadedState.MasterVolume, Is.EqualTo(0.6f).Within(0.001f));
                Assert.That(loadedState.MusicVolume, Is.EqualTo(0.4f).Within(0.001f));
                Assert.That(loadedState.SfxVolume, Is.EqualTo(0.2f).Within(0.001f));
                Assert.That(loadedState.UseFullscreen, Is.True);
            }
            finally
            {
                DeleteStorageArtifacts(storagePath);
            }
        }

        private static string CreateStoragePath()
        {
            string directoryPath = Path.Combine(
                Path.GetTempPath(),
                "survivalon_user_settings_tests",
                Guid.NewGuid().ToString("N"));
            return Path.Combine(directoryPath, "user_settings.json");
        }

        private static void EnsureParentDirectoryExists(string storagePath)
        {
            string directoryPath = Path.GetDirectoryName(storagePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private static void DeleteStorageArtifacts(string storagePath)
        {
            string directoryPath = Path.GetDirectoryName(storagePath);
            if (!string.IsNullOrWhiteSpace(directoryPath) && Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }
        }
    }
}
