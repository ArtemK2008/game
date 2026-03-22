using System;

namespace Survivalon.Characters
{
    /// <summary>
    /// Описывает runtime-вариант skill package с текущим состоянием назначения.
    /// </summary>
    public sealed class PlayableCharacterSkillPackageOption
    {
        public PlayableCharacterSkillPackageOption(
            string characterId,
            string skillPackageId,
            string displayName,
            string summary,
            bool isAssigned)
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
            IsAssigned = isAssigned;
        }

        public string CharacterId { get; }

        public string SkillPackageId { get; }

        public string DisplayName { get; }

        public string Summary { get; }

        public bool IsAssigned { get; }
    }
}
