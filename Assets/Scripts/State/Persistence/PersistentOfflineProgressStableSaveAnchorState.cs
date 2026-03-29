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

        [SerializeField]
        private OfflineProgressEligibilityKind eligibilityKind;

        public bool HasStableSaveAnchor => lastStableSaveUnixTimeSeconds > 0 || isEligibleForOfflineProgress;

        public long LastStableSaveUnixTimeSeconds => lastStableSaveUnixTimeSeconds;

        public bool IsEligibleForOfflineProgress => eligibilityKind != OfflineProgressEligibilityKind.None;

        public OfflineProgressEligibilityKind EligibilityKind => eligibilityKind;

        public void StampStableSaveAnchor(
            long unixTimeSeconds,
            OfflineProgressEligibilityKind offlineProgressEligibilityKind = OfflineProgressEligibilityKind.None)
        {
            if (unixTimeSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unixTimeSeconds));
            }

            isEligibleForOfflineProgress = false;
            lastStableSaveUnixTimeSeconds = unixTimeSeconds;
            eligibilityKind = offlineProgressEligibilityKind;
        }
    }
}
