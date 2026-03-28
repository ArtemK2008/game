using System;
using System.IO;
using UnityEngine;

namespace Survivalon.Core
{
    public sealed class FileUserSettingsStorage : IUserSettingsStorage
    {
        private readonly string storagePath;

        public FileUserSettingsStorage(string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
            {
                throw new ArgumentException("Storage path cannot be null or whitespace.", nameof(storagePath));
            }

            this.storagePath = storagePath;
        }

        public bool TryLoad(out UserSettingsState settingsState)
        {
            if (!File.Exists(storagePath))
            {
                settingsState = null;
                return false;
            }

            string json = File.ReadAllText(storagePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                settingsState = null;
                return false;
            }

            settingsState = JsonUtility.FromJson<UserSettingsState>(json);
            return settingsState != null;
        }

        public void Save(UserSettingsState settingsState)
        {
            if (settingsState == null)
            {
                throw new ArgumentNullException(nameof(settingsState));
            }

            string directoryPath = Path.GetDirectoryName(storagePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonUtility.ToJson(settingsState, true);
            File.WriteAllText(storagePath, json);
        }
    }
}
