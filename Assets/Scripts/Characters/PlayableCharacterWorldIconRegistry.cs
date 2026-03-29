using System;
using UnityEngine;

namespace Survivalon.Characters
{
    /// <summary>
    /// Stores runtime-safe references to the current playable-character world icons.
    /// </summary>
    public sealed class PlayableCharacterWorldIconRegistry : ScriptableObject
    {
        private const string ResourceName = "PlayableCharacterWorldIconRegistry";

        [SerializeField] private PlayableCharacterWorldIconEntry[] worldIconEntries =
            Array.Empty<PlayableCharacterWorldIconEntry>();

        public static PlayableCharacterWorldIconRegistry LoadOrNull()
        {
            return Resources.Load<PlayableCharacterWorldIconRegistry>(ResourceName);
        }

        public bool TryGetWorldIcon(string characterId, out Sprite worldIconSprite)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                worldIconSprite = null;
                return false;
            }

            for (int index = 0; index < worldIconEntries.Length; index++)
            {
                PlayableCharacterWorldIconEntry worldIconEntry = worldIconEntries[index];
                if (worldIconEntry == null || !worldIconEntry.Matches(characterId))
                {
                    continue;
                }

                worldIconSprite = worldIconEntry.WorldIconSprite;
                return worldIconSprite != null;
            }

            worldIconSprite = null;
            return false;
        }
    }

    [Serializable]
    public sealed class PlayableCharacterWorldIconEntry
    {
        [SerializeField] private string characterId;
        [SerializeField] private Sprite worldIconSprite;

        public Sprite WorldIconSprite => worldIconSprite;

        public bool Matches(string otherCharacterId)
        {
            return string.Equals(characterId, otherCharacterId, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Resolves playable-character world icons without pushing asset lookup into UI views.
    /// </summary>
    public sealed class PlayableCharacterWorldIconResolver
    {
        private readonly PlayableCharacterWorldIconRegistry worldIconRegistry;

        public PlayableCharacterWorldIconResolver(PlayableCharacterWorldIconRegistry worldIconRegistry = null)
        {
            this.worldIconRegistry = worldIconRegistry ?? PlayableCharacterWorldIconRegistry.LoadOrNull();
        }

        public bool TryResolveWorldIcon(string characterId, out Sprite worldIconSprite)
        {
            if (worldIconRegistry == null)
            {
                worldIconSprite = null;
                return false;
            }

            return worldIconRegistry.TryGetWorldIcon(characterId, out worldIconSprite);
        }
    }
}
