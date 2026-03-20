using System;

namespace Survivalon.Combat
{
    public enum CombatSkillCategory
    {
        BasicAttack = 0,
    }

    public enum CombatSkillActivationType
    {
        AutomatedInterval = 0,
    }

    public enum CombatSkillEffectType
    {
        DirectDamage = 0,
    }

    public sealed class CombatSkillDefinition
    {
        public CombatSkillDefinition(
            string skillId,
            string displayName,
            CombatSkillCategory category,
            CombatSkillActivationType activationType,
            CombatSkillEffectType effectType)
        {
            if (string.IsNullOrWhiteSpace(skillId))
            {
                throw new ArgumentException("Skill id cannot be null or whitespace.", nameof(skillId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            SkillId = skillId;
            DisplayName = displayName;
            Category = category;
            ActivationType = activationType;
            EffectType = effectType;
        }

        public string SkillId { get; }

        public string DisplayName { get; }

        public CombatSkillCategory Category { get; }

        public CombatSkillActivationType ActivationType { get; }

        public CombatSkillEffectType EffectType { get; }
    }

    public static class CombatSkillCatalog
    {
        public static CombatSkillDefinition BasicAttack { get; } = new CombatSkillDefinition(
            skillId: "combat_skill_basic_attack",
            displayName: "Basic Attack",
            category: CombatSkillCategory.BasicAttack,
            activationType: CombatSkillActivationType.AutomatedInterval,
            effectType: CombatSkillEffectType.DirectDamage);
    }
}
