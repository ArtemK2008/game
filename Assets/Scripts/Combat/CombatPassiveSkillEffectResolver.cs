using System;

namespace Survivalon.Combat
{
    public sealed class CombatPassiveSkillEffectResolver
    {
        public float ResolveOutgoingDirectDamageMultiplier(
            CombatEntityRuntimeState sourceEntity,
            CombatSkillDefinition executedSkill)
        {
            if (sourceEntity == null)
            {
                throw new ArgumentNullException(nameof(sourceEntity));
            }

            if (executedSkill == null)
            {
                throw new ArgumentNullException(nameof(executedSkill));
            }

            if (executedSkill.EffectType != CombatSkillEffectType.DirectDamage)
            {
                return 1f;
            }

            float directDamageMultiplier = 1f;
            for (int index = 0; index < sourceEntity.PassiveSkills.Count; index++)
            {
                CombatSkillDefinition passiveSkill = sourceEntity.PassiveSkills[index];
                if (passiveSkill.Category != CombatSkillCategory.Passive ||
                    passiveSkill.ActivationType != CombatSkillActivationType.AlwaysOn ||
                    passiveSkill.EffectType != CombatSkillEffectType.DirectDamageModifier)
                {
                    continue;
                }

                directDamageMultiplier *= passiveSkill.DirectDamageMultiplier;
            }

            return directDamageMultiplier;
        }
    }
}
