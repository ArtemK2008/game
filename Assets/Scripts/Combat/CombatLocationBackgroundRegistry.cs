using System;
using UnityEngine;

namespace Survivalon.Combat
{
    /// <summary>
    /// Stores runtime-safe references to the current combat location backgrounds.
    /// </summary>
    public sealed class CombatLocationBackgroundRegistry : ScriptableObject
    {
        private const string ResourceName = "CombatLocationBackgroundRegistry";

        [SerializeField] private CombatLocationBackgroundEntry[] backgroundEntries =
            Array.Empty<CombatLocationBackgroundEntry>();

        public static CombatLocationBackgroundRegistry LoadOrNull()
        {
            return Resources.Load<CombatLocationBackgroundRegistry>(ResourceName);
        }

        public bool TryGetBackground(string locationIdentityId, out Sprite backgroundSprite)
        {
            if (string.IsNullOrWhiteSpace(locationIdentityId))
            {
                backgroundSprite = null;
                return false;
            }

            for (int index = 0; index < backgroundEntries.Length; index++)
            {
                CombatLocationBackgroundEntry backgroundEntry = backgroundEntries[index];
                if (backgroundEntry == null || !backgroundEntry.Matches(locationIdentityId))
                {
                    continue;
                }

                backgroundSprite = backgroundEntry.BackgroundSprite;
                return backgroundSprite != null;
            }

            backgroundSprite = null;
            return false;
        }
    }

    [Serializable]
    public sealed class CombatLocationBackgroundEntry
    {
        [SerializeField] private string locationIdentityId;
        [SerializeField] private Sprite backgroundSprite;

        public Sprite BackgroundSprite => backgroundSprite;

        public bool Matches(string otherLocationIdentityId)
        {
            return string.Equals(locationIdentityId, otherLocationIdentityId, StringComparison.Ordinal);
        }
    }
}
