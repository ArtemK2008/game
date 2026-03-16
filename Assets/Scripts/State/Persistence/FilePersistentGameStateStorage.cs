using System;
using System.IO;
using UnityEngine;

namespace Survivalon.Runtime.State.Persistence
{
    public sealed class FilePersistentGameStateStorage : IPersistentGameStateStorage
    {
        private readonly string storagePath;

        public FilePersistentGameStateStorage(string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
            {
                throw new ArgumentException("Storage path cannot be null or whitespace.", nameof(storagePath));
            }

            this.storagePath = storagePath;
        }

        public bool TryLoad(out PersistentGameState gameState)
        {
            if (!File.Exists(storagePath))
            {
                gameState = null;
                return false;
            }

            string json = File.ReadAllText(storagePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                gameState = null;
                return false;
            }

            gameState = JsonUtility.FromJson<PersistentGameState>(json);
            return gameState != null;
        }

        public void Save(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            string directoryPath = Path.GetDirectoryName(storagePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonUtility.ToJson(gameState, true);
            File.WriteAllText(storagePath, json);
        }
    }
}
