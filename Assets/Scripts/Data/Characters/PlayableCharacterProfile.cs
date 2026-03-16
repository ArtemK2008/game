using System;
using Survivalon.Runtime.Combat;

namespace Survivalon.Runtime.Data.Characters
{
    public sealed class PlayableCharacterProfile
    {
        public PlayableCharacterProfile(
            string characterId,
            CombatEntityId combatEntityId,
            string displayName,
            CombatStatBlock baseStats,
            string defaultSkillPackageId = "")
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            CharacterId = characterId;
            CombatEntityId = combatEntityId;
            DisplayName = displayName;
            BaseStats = baseStats;
            DefaultSkillPackageId = defaultSkillPackageId ?? string.Empty;
        }

        public string CharacterId { get; }

        public CombatEntityId CombatEntityId { get; }

        public string DisplayName { get; }

        public CombatStatBlock BaseStats { get; }

        public string DefaultSkillPackageId { get; }
    }
}
