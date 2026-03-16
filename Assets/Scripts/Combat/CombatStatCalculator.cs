using System;

namespace Survivalon.Combat
{
    public static class CombatStatCalculator
    {
        private const float DefenseReferenceValue = 100f;

        public static float CalculateAttackIntervalSeconds(CombatStatBlock stats)
        {
            return 1f / stats.AttackRate;
        }

        public static float CalculateMitigationRatio(CombatStatBlock defenderStats)
        {
            if (defenderStats.Defense <= 0f)
            {
                return 0f;
            }

            return defenderStats.Defense / (defenderStats.Defense + DefenseReferenceValue);
        }

        public static float CalculateMitigatedDamage(float rawDamage, CombatStatBlock defenderStats)
        {
            if (rawDamage < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(rawDamage), rawDamage, "Raw damage cannot be negative.");
            }

            if (rawDamage == 0f)
            {
                return 0f;
            }

            return rawDamage * (1f - CalculateMitigationRatio(defenderStats));
        }
    }
}

