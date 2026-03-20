using System;
using System.Collections.Generic;

namespace Survivalon.Data.Characters
{
    public static class PlayableCharacterSkillPackageCatalog
    {
        private static readonly IReadOnlyList<PlayableCharacterSkillPackageOption> VanguardPackages = Array.AsReadOnly(new[]
        {
            new PlayableCharacterSkillPackageOption(
                "character_vanguard",
                "skill_package_vanguard_default",
                "Standard Guard",
                "No passive or active skill.",
                isAssigned: false),
            new PlayableCharacterSkillPackageOption(
                "character_vanguard",
                "skill_package_vanguard_burst_drill",
                "Burst Drill",
                "Adds Burst Strike.",
                isAssigned: false),
        });

        private static readonly IReadOnlyList<PlayableCharacterSkillPackageOption> StrikerPackages = Array.AsReadOnly(new[]
        {
            new PlayableCharacterSkillPackageOption(
                "character_striker",
                "skill_package_striker_default",
                "Relentless Burst",
                "Relentless Assault plus Burst Strike.",
                isAssigned: false),
        });

        public static IReadOnlyList<PlayableCharacterSkillPackageOption> GetOptions(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            return characterId switch
            {
                "character_vanguard" => VanguardPackages,
                "character_striker" => StrikerPackages,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(characterId),
                    characterId,
                    "Unknown playable character id."),
            };
        }

        public static bool Contains(string characterId, string skillPackageId)
        {
            if (string.IsNullOrWhiteSpace(characterId) || string.IsNullOrWhiteSpace(skillPackageId))
            {
                return false;
            }

            IReadOnlyList<PlayableCharacterSkillPackageOption> packageOptions = GetOptions(characterId);
            for (int index = 0; index < packageOptions.Count; index++)
            {
                if (packageOptions[index].SkillPackageId == skillPackageId)
                {
                    return true;
                }
            }

            return false;
        }

        public static PlayableCharacterSkillPackageOption Get(string characterId, string skillPackageId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                throw new ArgumentException("Skill package id cannot be null or whitespace.", nameof(skillPackageId));
            }

            IReadOnlyList<PlayableCharacterSkillPackageOption> packageOptions = GetOptions(characterId);
            for (int index = 0; index < packageOptions.Count; index++)
            {
                if (packageOptions[index].SkillPackageId == skillPackageId)
                {
                    return packageOptions[index];
                }
            }

            throw new ArgumentOutOfRangeException(
                nameof(skillPackageId),
                skillPackageId,
                $"Unknown skill package id '{skillPackageId}' for character '{characterId}'.");
        }
    }
}
