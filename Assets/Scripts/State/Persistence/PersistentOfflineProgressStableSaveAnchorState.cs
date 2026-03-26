using System;
using UnityEngine;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class PersistentOfflineProgressStableSaveAnchorState
    {
        [SerializeField]
        private bool isEligibleForOfflineProgress;

        [SerializeField]
        private long lastStableSaveUnixTimeSeconds;

        public bool HasStableSaveAnchor => isEligibleForOfflineProgress;

        public long LastStableSaveUnixTimeSeconds => lastStableSaveUnixTimeSeconds;

        public void StampStableSaveAnchor(long unixTimeSeconds)
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
