using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Combat;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Combat
{
    public sealed class CombatShellContextFactory
    {
        private readonly PlayableCharacterProgressionEffectResolver playableCharacterProgressionEffectResolver;
        private readonly PlayableCharacterGearCombatEffectResolver playableCharacterGearCombatEffectResolver;
        private readonly PlayableCharacterCombatSkillResolver playableCharacterCombatSkillResolver;
        private readonly CombatEnemyProfileResolver combatEnemyProfileResolver;

        public CombatShellContextFactory(
            PlayableCharacterProgressionEffectResolver playableCharacterProgressionEffectResolver = null,
            PlayableCharacterGearCombatEffectResolver playableCharacterGearCombatEffectResolver = null,
            PlayableCharacterCombatSkillResolver playableCharacterCombatSkillResolver = null,
            CombatEnemyProfileResolver combatEnemyProfileResolver = null)
        {
            this.playableCharacterProgressionEffectResolver =
                playableCharacterProgressionEffectResolver ?? new PlayableCharacterProgressionEffectResolver();
            this.playableCharacterGearCombatEffectResolver =
                playableCharacterGearCombatEffectResolver ?? new PlayableCharacterGearCombatEffectResolver();
            this.playableCharacterCombatSkillResolver =
                playableCharacterCombatSkillResolver ?? new PlayableCharacterCombatSkillResolver();
            this.combatEnemyProfileResolver =
                combatEnemyProfileResolver ?? new CombatEnemyProfileResolver();
        }

        public CombatShellContext Create(
            NodePlaceholderState nodeContext,
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState,
            AccountWideProgressionEffectState progressionEffects,
            CombatRunTimeSkillUpgradeOption triggeredActiveSkillUpgrade = null)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            PlayableCharacterProfile resolvedCharacter = playableCharacter ?? PlayableCharacterCatalog.Default;

            if (!nodeContext.UsesCombatShell)
            {
                throw new InvalidOperationException("Combat shell requires a combat-compatible node type.");
            }

            return new CombatShellContext(
                nodeContext.NodeId,
                new CombatEntityState(
                    resolvedCharacter.CombatEntityId,
                    resolvedCharacter.DisplayName,
                    CombatSide.Player,
                    CreatePlayerBaseStats(
                        resolvedCharacter.BaseStats,
                        playableCharacterState,
                        progressionEffects),
                    triggeredActiveSkill: ResolveTriggeredActiveSkill(
                        resolvedCharacter,
                        playableCharacterState,
                        triggeredActiveSkillUpgrade),
                    triggeredActiveSkillUpgrade: triggeredActiveSkillUpgrade,
                    passiveSkills: ResolvePassiveSkills(resolvedCharacter, playableCharacterState)),
                CreateEnemyEntityState(nodeContext));
        }

        private CombatStatBlock CreatePlayerBaseStats(
            CombatStatBlock characterBaseStats,
            PersistentCharacterState playableCharacterState,
            AccountWideProgressionEffectState progressionEffects)
        {
            float characterProgressionMaxHealthBonus = playableCharacterState == null
                ? 0f
                : playableCharacterProgressionEffectResolver.ResolveMaxHealthBonus(playableCharacterState);
            float gearMaxHealthBonus = playableCharacterState == null
                ? 0f
                : playableCharacterGearCombatEffectResolver.ResolveMaxHealthBonus(playableCharacterState);
            float gearAttackPowerBonus = playableCharacterState == null
                ? 0f
                : playableCharacterGearCombatEffectResolver.ResolveAttackPowerBonus(playableCharacterState);

            return new CombatStatBlock(
                maxHealth: characterBaseStats.MaxHealth +
                    characterProgressionMaxHealthBonus +
                    gearMaxHealthBonus +
                    progressionEffects.PlayerMaxHealthBonus,
                attackPower: characterBaseStats.AttackPower +
                    progressionEffects.PlayerAttackPowerBonus +
                    gearAttackPowerBonus,
                attackRate: characterBaseStats.AttackRate,
                defense: characterBaseStats.Defense);
        }

        private IReadOnlyList<CombatSkillDefinition> ResolvePassiveSkills(
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState)
        {
            return playableCharacterCombatSkillResolver.ResolvePassiveSkills(
                playableCharacter,
                playableCharacterState);
        }

        private CombatSkillDefinition ResolveTriggeredActiveSkill(
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState,
            CombatRunTimeSkillUpgradeOption triggeredActiveSkillUpgrade)
        {
            CombatSkillDefinition triggeredActiveSkill = playableCharacterCombatSkillResolver.ResolveTriggeredActiveSkill(
                playableCharacter,
                playableCharacterState);

            return triggeredActiveSkillUpgrade == null
                ? triggeredActiveSkill
                : triggeredActiveSkill ?? throw new InvalidOperationException(
                    "Run-time triggered active skill upgrade requires a base triggered active skill.");
        }

        private CombatEntityState CreateEnemyEntityState(NodePlaceholderState nodeContext)
        {
            CombatEnemyProfile resolvedEnemyProfile = combatEnemyProfileResolver.Resolve(nodeContext);

            return new CombatEntityState(
                new CombatEntityId($"{nodeContext.NodeId.Value}_{resolvedEnemyProfile.EntityIdSuffix}"),
                resolvedEnemyProfile.DisplayName,
                CombatSide.Enemy,
                resolvedEnemyProfile.BaseStats);
        }
    }
}

