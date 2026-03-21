using System;
using System.Collections.Generic;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Combat
{
    public sealed class PlayableCharacterCombatSkillResolver
    {
        public IReadOnlyList<CombatSkillDefinition> ResolvePassiveSkills(
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState)
        {
            return CombatSkillPackageCatalog.GetPassiveSkills(
                ResolveSkillPackageId(playableCharacter, playableCharacterState));
        }

        public CombatSkillDefinition ResolveTriggeredActiveSkill(
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState)
        {
            return CombatSkillPackageCatalog.GetTriggeredActiveSkill(
                ResolveSkillPackageId(playableCharacter, playableCharacterState));
        }

        private static string ResolveSkillPackageId(
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState)
        {
            PlayableCharacterProfile resolvedCharacter = playableCharacter ?? PlayableCharacterCatalog.Default;

            return !string.IsNullOrWhiteSpace(playableCharacterState?.SkillPackageId)
                ? playableCharacterState.SkillPackageId
                : resolvedCharacter.DefaultSkillPackageId;
        }
    }
}
