using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Data.Characters
{
    public static class PlayableCharacterCatalog
    {
        private static readonly PlayableCharacterProfile DefaultCharacterProfile =
            new PlayableCharacterProfile(
                "character_vanguard",
                new CombatEntityId("player_main"),
                "Vanguard",
                new CombatStatBlock(120f, 14f, 1.2f, 12f),
                PlayableCharacterSkillPackageIds.VanguardDefault);
        private static readonly PlayableCharacterProfile StrikerCharacterProfile =
            new PlayableCharacterProfile(
                "character_striker",
                new CombatEntityId("player_striker"),
                "Striker",
                new CombatStatBlock(110f, 18f, 1.35f, 8f),
                PlayableCharacterSkillPackageIds.StrikerDefault);
        private static readonly IReadOnlyList<PlayableCharacterProfile> AllProfiles = Array.AsReadOnly(new[]
        {
            DefaultCharacterProfile,
            StrikerCharacterProfile,
        });

        public static PlayableCharacterProfile Default => DefaultCharacterProfile;

        public static IReadOnlyList<PlayableCharacterProfile> All => AllProfiles;

        public static bool Contains(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                return false;
            }

            for (int index = 0; index < AllProfiles.Count; index++)
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

            for (int index = 0; index < AllProfiles.Count; index++)
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

