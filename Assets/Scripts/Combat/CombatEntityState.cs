using System;
using System.Collections.Generic;

namespace Survivalon.Combat
{
    public sealed class CombatEntityState
    {
        public CombatEntityState(
            CombatEntityId entityId,
            string displayName,
            CombatSide side,
            CombatStatBlock baseStats,
            CombatSkillDefinition baselineAttackSkill = null,
            IReadOnlyList<CombatSkillDefinition> passiveSkills = null,
            bool isAlive = true,
            bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            EntityId = entityId;
            DisplayName = displayName;
            Side = side;
            BaseStats = baseStats;
            BaselineAttackSkill = baselineAttackSkill ?? CombatSkillCatalog.BasicAttack;
            PassiveSkills = passiveSkills ?? Array.Empty<CombatSkillDefinition>();
            IsAlive = isAlive;
            IsActive = isActive;
        }

        public CombatEntityId EntityId { get; }

        public string DisplayName { get; }

        public CombatSide Side { get; }

        public CombatStatBlock BaseStats { get; }

        public CombatSkillDefinition BaselineAttackSkill { get; }

        public IReadOnlyList<CombatSkillDefinition> PassiveSkills { get; }

        public bool IsAlive { get; }

        public bool IsActive { get; }
    }
}

