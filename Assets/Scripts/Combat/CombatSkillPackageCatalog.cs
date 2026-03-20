using System;
using System.Collections.Generic;
using Survivalon.Data.Characters;

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
                PlayableCharacterSkillPackageIds.VanguardDefault => EmptyPassiveSkills,
                PlayableCharacterSkillPackageIds.VanguardBurstDrill => EmptyPassiveSkills,
                PlayableCharacterSkillPackageIds.StrikerDefault => StrikerDefaultPassiveSkills,
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
                PlayableCharacterSkillPackageIds.VanguardDefault => null,
                PlayableCharacterSkillPackageIds.VanguardBurstDrill => VanguardBurstDrillTriggeredActiveSkill,
                PlayableCharacterSkillPackageIds.StrikerDefault => StrikerDefaultTriggeredActiveSkill,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(skillPackageId),
                    skillPackageId,
                    "Unknown combat skill package id."),
            };
        }
    }
}
