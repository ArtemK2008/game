using System;

namespace Survivalon.Combat
{
    public enum CombatSkillCategory
    {
        BasicAttack = 0,
        Passive = 1,
    }

    public enum CombatSkillActivationType
    {
        AutomatedInterval = 0,
        AlwaysOn = 1,
    }

    public enum CombatSkillEffectType
    {
        DirectDamage = 0,
        DirectDamageModifier = 1,
    }

    public sealed class CombatSkillDefinition
    {
        public CombatSkillDefinition(
            string skillId,
            string displayName,
            CombatSkillCategory category,
            CombatSkillActivationType activationType,
            CombatSkillEffectType effectType,
            float directDamageMultiplier = 1f)
        {
            if (string.IsNullOrWhiteSpace(skillId))
            {
                throw new ArgumentException("Skill id cannot be null or whitespace.", nameof(skillId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            if (directDamageMultiplier <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(directDamageMultiplier),
                    directDamageMultiplier,
                    "Direct damage multiplier must be greater than zero.");
            }

            SkillId = skillId;
            DisplayName = displayName;
            Category = category;
            ActivationType = activationType;
            EffectType = effectType;
            DirectDamageMultiplier = directDamageMultiplier;
        }

        public string SkillId { get; }

        public string DisplayName { get; }

        public CombatSkillCategory Category { get; }

        public CombatSkillActivationType ActivationType { get; }

        public CombatSkillEffectType EffectType { get; }

        public float DirectDamageMultiplier { get; }
    }

    public static class CombatSkillCatalog
    {
        public static CombatSkillDefinition BasicAttack { get; } = new CombatSkillDefinition(
            skillId: "combat_skill_basic_attack",
            displayName: "Basic Attack",
            category: CombatSkillCategory.BasicAttack,
            activationType: CombatSkillActivationType.AutomatedInterval,
            effectType: CombatSkillEffectType.DirectDamage);

        public static CombatSkillDefinition RelentlessAssault { get; } = new CombatSkillDefinition(
            skillId: "combat_passive_relentless_assault",
            displayName: "Relentless Assault",
            category: CombatSkillCategory.Passive,
            activationType: CombatSkillActivationType.AlwaysOn,
            effectType: CombatSkillEffectType.DirectDamageModifier,
            directDamageMultiplier: 1.2f);
    }
}
