using System;

namespace Survivalon.Combat
{
    public static class CombatTriggeredActiveSkillUpgradeResolver
    {
        private const float BurstStrikeIntervalSeconds = 2.5f;
        private const float BurstTempoIntervalSeconds = 1.75f;
        private const float BurstStrikeAttackPowerMultiplier = 2f;
        private const float BurstPayloadAttackPowerMultiplier = 3f;

        public static float ResolveIntervalSeconds(
            CombatSkillDefinition triggeredActiveSkill,
            CombatRunTimeSkillUpgradeOption runTimeSkillUpgrade)
        {
            if (triggeredActiveSkill == null)
            {
                return float.PositiveInfinity;
            }

            if (triggeredActiveSkill.SkillId != CombatSkillCatalog.BurstStrike.SkillId)
            {
                throw new InvalidOperationException(
                    $"Unsupported triggered active skill id '{triggeredActiveSkill.SkillId}'.");
            }

            return runTimeSkillUpgrade?.UpgradeId == CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId
                ? BurstTempoIntervalSeconds
                : ResolveDefaultOrPayloadIntervalSeconds(runTimeSkillUpgrade);
        }

        public static float ResolveAttackPowerMultiplier(
            CombatSkillDefinition triggeredActiveSkill,
            CombatRunTimeSkillUpgradeOption runTimeSkillUpgrade)
        {
            if (triggeredActiveSkill == null)
            {
                throw new ArgumentNullException(nameof(triggeredActiveSkill));
            }

            if (triggeredActiveSkill.SkillId != CombatSkillCatalog.BurstStrike.SkillId)
            {
                throw new InvalidOperationException(
                    $"Unsupported triggered active skill id '{triggeredActiveSkill.SkillId}'.");
            }

            return runTimeSkillUpgrade?.UpgradeId == CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId
                ? BurstPayloadAttackPowerMultiplier
                : ResolveDefaultOrTempoAttackPowerMultiplier(runTimeSkillUpgrade);
        }

        private static float ResolveDefaultOrPayloadIntervalSeconds(CombatRunTimeSkillUpgradeOption runTimeSkillUpgrade)
        {
            if (runTimeSkillUpgrade == null ||
                runTimeSkillUpgrade.UpgradeId == CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId)
            {
                return BurstStrikeIntervalSeconds;
            }

            throw new InvalidOperationException(
                $"Unsupported run-time skill upgrade id '{runTimeSkillUpgrade.UpgradeId}'.");
        }

        private static float ResolveDefaultOrTempoAttackPowerMultiplier(CombatRunTimeSkillUpgradeOption runTimeSkillUpgrade)
        {
            if (runTimeSkillUpgrade == null ||
                runTimeSkillUpgrade.UpgradeId == CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId)
            {
                return BurstStrikeAttackPowerMultiplier;
            }

            throw new InvalidOperationException(
                $"Unsupported run-time skill upgrade id '{runTimeSkillUpgrade.UpgradeId}'.");
        }
    }
}
