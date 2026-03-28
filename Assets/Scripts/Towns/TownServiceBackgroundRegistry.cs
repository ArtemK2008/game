using System;
using UnityEngine;

namespace Survivalon.Towns
{
    /// <summary>
    /// Stores runtime-safe references to the current town/service backgrounds.
    /// </summary>
    public sealed class TownServiceBackgroundRegistry : ScriptableObject
    {
        private const string ResourceName = "TownServiceBackgroundRegistry";

        [SerializeField] private TownServiceBackgroundEntry[] backgroundEntries =
            Array.Empty<TownServiceBackgroundEntry>();

        public static TownServiceBackgroundRegistry LoadOrNull()
        {
            return Resources.Load<TownServiceBackgroundRegistry>(ResourceName);
        }

        public bool TryGetBackground(string contextId, out Sprite backgroundSprite)
        {
            if (string.IsNullOrWhiteSpace(contextId))
            {
                backgroundSprite = null;
                return false;
            }

            for (int index = 0; index < backgroundEntries.Length; index++)
            {
                TownServiceBackgroundEntry backgroundEntry = backgroundEntries[index];
                if (backgroundEntry == null || !backgroundEntry.Matches(contextId))
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
    public sealed class TownServiceBackgroundEntry
    {
        [SerializeField] private string contextId;
        [SerializeField] private Sprite backgroundSprite;

        public Sprite BackgroundSprite => backgroundSprite;

        public bool Matches(string otherContextId)
        {
            return string.Equals(contextId, otherContextId, StringComparison.Ordinal);
        }
    }
}
