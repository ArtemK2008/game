using System;

namespace Survivalon.Combat
{
    public static class CombatTriggeredActiveSkillTimingResolver
    {
        public static float ResolveIntervalSeconds(
            CombatSkillDefinition triggeredActiveSkill,
            CombatRunTimeSkillUpgradeOption runTimeSkillUpgrade = null)
        {
            if (triggeredActiveSkill == null)
            {
                return float.PositiveInfinity;
            }

            if (triggeredActiveSkill.Category != CombatSkillCategory.TriggeredActive ||
                triggeredActiveSkill.ActivationType != CombatSkillActivationType.PeriodicAutoTrigger)
            {
                throw new InvalidOperationException(
                    $"Triggered active timing resolver requires a periodic triggered active skill, but received '{triggeredActiveSkill.SkillId}'.");
            }

            return CombatTriggeredActiveSkillUpgradeResolver.ResolveIntervalSeconds(
                triggeredActiveSkill,
                runTimeSkillUpgrade);
        }
    }
}
