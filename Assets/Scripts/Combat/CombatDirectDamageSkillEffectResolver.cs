using System;

namespace Survivalon.Combat
{
    public sealed class CombatDirectDamageSkillEffectResolver
    {
        private const float BasicAttackAttackPowerMultiplier = 1f;
        private const float BurstStrikeAttackPowerMultiplier = 2f;
        private const float BurstPayloadAttackPowerMultiplier = 3f;

        public float ResolveAttackPowerMultiplier(CombatSkillDefinition directDamageSkill)
        {
            if (directDamageSkill == null)
            {
                throw new ArgumentNullException(nameof(directDamageSkill));
            }

            if (directDamageSkill.EffectType != CombatSkillEffectType.DirectDamage)
            {
                throw new InvalidOperationException(
                    $"Direct-damage effect resolver requires a direct-damage skill, but received '{directDamageSkill.EffectType}'.");
            }

            if (directDamageSkill.SkillId == CombatSkillCatalog.BasicAttack.SkillId)
            {
                return BasicAttackAttackPowerMultiplier;
            }

            if (directDamageSkill.SkillId == CombatSkillCatalog.BurstStrike.SkillId)
            {
                return BurstStrikeAttackPowerMultiplier;
            }

            if (directDamageSkill.SkillId == CombatSkillCatalog.BurstTempo.SkillId)
            {
                return BurstStrikeAttackPowerMultiplier;
            }

            if (directDamageSkill.SkillId == CombatSkillCatalog.BurstPayload.SkillId)
            {
                return BurstPayloadAttackPowerMultiplier;
            }

            throw new InvalidOperationException(
                $"Unsupported direct-damage skill id '{directDamageSkill.SkillId}'.");
        }
    }
}
