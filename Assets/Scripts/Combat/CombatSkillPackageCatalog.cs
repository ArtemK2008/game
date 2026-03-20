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

        public static IReadOnlyList<CombatSkillDefinition> GetPassiveSkills(string skillPackageId)
        {
            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                return EmptyPassiveSkills;
            }

            return skillPackageId switch
            {
                "skill_package_vanguard_default" => EmptyPassiveSkills,
                "skill_package_striker_default" => StrikerDefaultPassiveSkills,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(skillPackageId),
                    skillPackageId,
                    "Unknown combat skill package id."),
            };
        }
    }
}
