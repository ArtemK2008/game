using System;
using Survivalon.Runtime.Combat;

namespace Survivalon.Runtime.Data.Characters
{
    public static class PlayableCharacterCatalog
    {
        private static readonly PlayableCharacterProfile DefaultCharacterProfile =
            new PlayableCharacterProfile(
                "character_vanguard",
                new CombatEntityId("player_main"),
                "Vanguard",
                new CombatStatBlock(120f, 14f, 1.2f, 12f),
                "skill_package_vanguard_default");
        private static readonly PlayableCharacterProfile[] AllProfiles =
        {
            DefaultCharacterProfile,
        };

        public static PlayableCharacterProfile Default => DefaultCharacterProfile;

        public static bool Contains(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                return false;
            }

            for (int index = 0; index < AllProfiles.Length; index++)
            {
                if (AllProfiles[index].CharacterId == characterId)
                {
                    return true;
                }
            }

            return false;
        }

        public static PlayableCharacterProfile Get(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            for (int index = 0; index < AllProfiles.Length; index++)
            {
                if (AllProfiles[index].CharacterId == characterId)
                {
                    return AllProfiles[index];
                }
            }

            throw new ArgumentOutOfRangeException(nameof(characterId), characterId, "Unknown playable character id.");
        }
    }
}
