using System;

namespace Survivalon.Combat
{
    public sealed class CombatRunTimeSkillUpgradeOption
    {
        public CombatRunTimeSkillUpgradeOption(
            string upgradeId,
            string displayName,
            string description,
            string sourceSkillDisplayName,
            string selectionHint)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
            {
                throw new ArgumentException("Upgrade id cannot be null or whitespace.", nameof(upgradeId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or whitespace.", nameof(description));
            }

            if (string.IsNullOrWhiteSpace(sourceSkillDisplayName))
            {
                throw new ArgumentException(
                    "Source skill display name cannot be null or whitespace.",
                    nameof(sourceSkillDisplayName));
            }

            if (string.IsNullOrWhiteSpace(selectionHint))
            {
                throw new ArgumentException(
                    "Selection hint cannot be null or whitespace.",
                    nameof(selectionHint));
            }

            UpgradeId = upgradeId;
            DisplayName = displayName;
            Description = description;
            SourceSkillDisplayName = sourceSkillDisplayName;
            SelectionHint = selectionHint;
        }

        public string UpgradeId { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public string SourceSkillDisplayName { get; }

        public string SelectionHint { get; }
    }
}
