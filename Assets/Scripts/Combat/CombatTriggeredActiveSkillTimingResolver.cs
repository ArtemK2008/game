using System;

namespace Survivalon.Combat
{
    public static class CombatTriggeredActiveSkillTimingResolver
    {
        private const float BurstStrikeIntervalSeconds = 2.5f;
        private const float BurstTempoIntervalSeconds = 1.75f;

        public static float ResolveIntervalSeconds(CombatSkillDefinition triggeredActiveSkill)
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

            if (triggeredActiveSkill.SkillId == CombatSkillCatalog.BurstStrike.SkillId)
            {
                return BurstStrikeIntervalSeconds;
            }

            if (triggeredActiveSkill.SkillId == CombatSkillCatalog.BurstTempo.SkillId)
            {
                return BurstTempoIntervalSeconds;
            }

            if (triggeredActiveSkill.SkillId == CombatSkillCatalog.BurstPayload.SkillId)
            {
                return BurstStrikeIntervalSeconds;
            }

            throw new InvalidOperationException(
                $"Unsupported triggered active skill id '{triggeredActiveSkill.SkillId}'.");
        }
    }
}
