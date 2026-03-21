using System;

namespace Survivalon.Combat
{
    public sealed class CombatRunTimeSkillUpgradeOption
    {
        public CombatRunTimeSkillUpgradeOption(
            CombatSkillDefinition upgradedTriggeredActiveSkill,
            string description)
        {
            UpgradedTriggeredActiveSkill =
                upgradedTriggeredActiveSkill ?? throw new ArgumentNullException(nameof(upgradedTriggeredActiveSkill));

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or whitespace.", nameof(description));
            }

            Description = description;
        }

        public CombatSkillDefinition UpgradedTriggeredActiveSkill { get; }

        public string Description { get; }
    }
}
