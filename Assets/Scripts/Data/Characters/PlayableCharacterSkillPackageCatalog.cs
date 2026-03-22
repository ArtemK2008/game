using System;
using System.Collections.Generic;

namespace Survivalon.Data.Characters
{
    /// <summary>
    /// Хранит authored skill package definitions для доступных играбельных персонажей.
    /// </summary>
    public static class PlayableCharacterSkillPackageCatalog
    {
        private static readonly IReadOnlyList<PlayableCharacterSkillPackageDefinition> VanguardPackages = Array.AsReadOnly(new[]
        {
            new PlayableCharacterSkillPackageDefinition(
                "character_vanguard",
                PlayableCharacterSkillPackageIds.VanguardDefault,
                "Standard Guard",
                "No passive or active skill."),
            new PlayableCharacterSkillPackageDefinition(
                "character_vanguard",
                PlayableCharacterSkillPackageIds.VanguardBurstDrill,
                "Burst Drill",
                "Adds Burst Strike."),
        });

        private static readonly IReadOnlyList<PlayableCharacterSkillPackageDefinition> StrikerPackages = Array.AsReadOnly(new[]
        {
            new PlayableCharacterSkillPackageDefinition(
                "character_striker",
                PlayableCharacterSkillPackageIds.StrikerDefault,
                "Relentless Burst",
                "Relentless Assault plus Burst Strike."),
        });

        public static IReadOnlyList<PlayableCharacterSkillPackageDefinition> GetDefinitions(string characterId)
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

            IReadOnlyList<PlayableCharacterSkillPackageDefinition> packageOptions = GetDefinitions(characterId);
            for (int index = 0; index < packageOptions.Count; index++)
            {
                if (packageOptions[index].SkillPackageId == skillPackageId)
                {
                    return true;
                }
            }

            return false;
        }

        public static PlayableCharacterSkillPackageDefinition Get(string characterId, string skillPackageId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                throw new ArgumentException("Skill package id cannot be null or whitespace.", nameof(skillPackageId));
            }

            IReadOnlyList<PlayableCharacterSkillPackageDefinition> packageOptions = GetDefinitions(characterId);
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
