using System;
using System.Collections.Generic;

namespace Survivalon.Combat
{
    public static class CombatSkillPackageCatalog
    {
        private static readonly IReadOnlyList<CombatSkillDefinition> EmptyPassiveSkills = Array.Empty<CombatSkillDefinition>();
        private static readonly IReadOnlyList<CombatSkillDefinition> StrikerDefaultPassiveSkills = new[]
        {
            CombatSkillCatalog.RelentlessAssault,
        };
        private static readonly CombatSkillDefinition VanguardBurstDrillTriggeredActiveSkill = CombatSkillCatalog.BurstStrike;
        private static readonly CombatSkillDefinition StrikerDefaultTriggeredActiveSkill = CombatSkillCatalog.BurstStrike;

        public static IReadOnlyList<CombatSkillDefinition> GetPassiveSkills(string skillPackageId)
        {
            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                return EmptyPassiveSkills;
            }

            return skillPackageId switch
            {
                "skill_package_vanguard_default" => EmptyPassiveSkills,
                "skill_package_vanguard_burst_drill" => EmptyPassiveSkills,
                "skill_package_striker_default" => StrikerDefaultPassiveSkills,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(skillPackageId),
                    skillPackageId,
                    "Unknown combat skill package id."),
            };
        }

        public static CombatSkillDefinition GetTriggeredActiveSkill(string skillPackageId)
        {
            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                return null;
            }

            return skillPackageId switch
            {
                "skill_package_vanguard_default" => null,
                "skill_package_vanguard_burst_drill" => VanguardBurstDrillTriggeredActiveSkill,
                "skill_package_striker_default" => StrikerDefaultTriggeredActiveSkill,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(skillPackageId),
                    skillPackageId,
                    "Unknown combat skill package id."),
            };
        }
    }
}
