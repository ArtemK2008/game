using System;
using UnityEngine;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class PersistentOfflineProgressCompatibilityState
    {
        [SerializeField]
        private bool isEligibleForOfflineProgress;

        [SerializeField]
        private long lastStableSaveUnixTimeSeconds;

        public bool IsEligibleForOfflineProgress => isEligibleForOfflineProgress;

        public long LastStableSaveUnixTimeSeconds => lastStableSaveUnixTimeSeconds;

        public void MarkEligibleStableContext(long unixTimeSeconds)
        {
            if (unixTimeSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unixTimeSeconds));
            }

            isEligibleForOfflineProgress = true;
            lastStableSaveUnixTimeSeconds = unixTimeSeconds;
        }
    }
}
