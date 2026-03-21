using System;

namespace Survivalon.Combat
{
    public sealed class CombatDirectDamageSkillEffectResolver
    {
        private const float BasicAttackAttackPowerMultiplier = 1f;

        public float ResolveAttackPowerMultiplier(
            CombatSkillDefinition directDamageSkill,
            CombatRunTimeSkillUpgradeOption runTimeSkillUpgrade = null)
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
                return CombatTriggeredActiveSkillUpgradeResolver.ResolveAttackPowerMultiplier(
                    directDamageSkill,
                    runTimeSkillUpgrade);
            }

            throw new InvalidOperationException(
                $"Unsupported direct-damage skill id '{directDamageSkill.SkillId}'.");
        }
    }
}
