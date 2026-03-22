using System;

namespace Survivalon.Data.Characters
{
    /// <summary>
    /// Описывает authored skill package definition без runtime-состояния назначения.
    /// </summary>
    public sealed class PlayableCharacterSkillPackageDefinition
    {
        public PlayableCharacterSkillPackageDefinition(
            string characterId,
            string skillPackageId,
            string displayName,
            string summary)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                throw new ArgumentException("Skill package id cannot be null or whitespace.", nameof(skillPackageId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(summary))
            {
                throw new ArgumentException("Summary cannot be null or whitespace.", nameof(summary));
            }

            CharacterId = characterId;
            SkillPackageId = skillPackageId;
            DisplayName = displayName;
            Summary = summary;
        }

        public string CharacterId { get; }

        public string SkillPackageId { get; }

        public string DisplayName { get; }

        public string Summary { get; }
    }
}
