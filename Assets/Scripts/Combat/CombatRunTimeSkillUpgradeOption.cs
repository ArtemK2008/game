using System;

namespace Survivalon.Combat
{
    public sealed class CombatRunTimeSkillUpgradeOption
    {
        public CombatRunTimeSkillUpgradeOption(
            string upgradeId,
            string displayName,
            string description)
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

            UpgradeId = upgradeId;
            DisplayName = displayName;
            Description = description;
        }

        public string UpgradeId { get; }

        public string DisplayName { get; }

        public string Description { get; }
    }
}
