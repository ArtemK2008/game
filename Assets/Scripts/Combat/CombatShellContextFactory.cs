using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Combat
{
    public sealed class CombatShellContextFactory
    {
        private readonly PlayableCharacterProgressionEffectResolver playableCharacterProgressionEffectResolver;

        public CombatShellContextFactory(PlayableCharacterProgressionEffectResolver playableCharacterProgressionEffectResolver = null)
        {
            this.playableCharacterProgressionEffectResolver =
                playableCharacterProgressionEffectResolver ?? new PlayableCharacterProgressionEffectResolver();
        }

        public CombatShellContext Create(
            NodePlaceholderState nodeContext,
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState,
            AccountWideProgressionEffectState progressionEffects)
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
                    triggeredActiveSkill: ResolveTriggeredActiveSkill(resolvedCharacter, playableCharacterState),
                    passiveSkills: ResolvePassiveSkills(resolvedCharacter, playableCharacterState)),
                new CombatEntityState(
                    new CombatEntityId(GetEnemyEntityIdValue(nodeContext)),
                    GetEnemyDisplayName(nodeContext),
                    CombatSide.Enemy,
                    CreateEnemyBaseStats(nodeContext)));
        }

        private CombatStatBlock CreatePlayerBaseStats(
            CombatStatBlock characterBaseStats,
            PersistentCharacterState playableCharacterState,
            AccountWideProgressionEffectState progressionEffects)
        {
            float characterProgressionMaxHealthBonus = playableCharacterState == null
                ? 0f
                : playableCharacterProgressionEffectResolver.ResolveMaxHealthBonus(playableCharacterState);

            return new CombatStatBlock(
                maxHealth: characterBaseStats.MaxHealth +
                    characterProgressionMaxHealthBonus +
                    progressionEffects.PlayerMaxHealthBonus,
                attackPower: characterBaseStats.AttackPower + progressionEffects.PlayerAttackPowerBonus,
                attackRate: characterBaseStats.AttackRate,
                defense: characterBaseStats.Defense);
        }

        private static IReadOnlyList<CombatSkillDefinition> ResolvePassiveSkills(
            PlayableCharacterProfile playableCharacter,
            PersistentCharacterState playableCharacterState)
        {
            return CombatSkillPackageCatalog.GetPassiveSkills(
                ResolveSkillPackageId(playableCharacter, playableCharacterState));
        }

        private static CombatSkillDefinition ResolveTriggeredActiveSkill(
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
            return !string.IsNullOrWhiteSpace(playableCharacterState?.SkillPackageId)
                ? playableCharacterState.SkillPackageId
                : playableCharacter.DefaultSkillPackageId;
        }

        private static CombatStatBlock CreateEnemyBaseStats(NodePlaceholderState nodeContext)
        {
            return nodeContext.NodeType == NodeType.BossOrGate
                ? new CombatStatBlock(
                    maxHealth: 180f,
                    attackPower: 16f,
                    attackRate: 0.85f,
                    defense: 18f)
                : new CombatStatBlock(
                    maxHealth: 75f,
                    attackPower: 8f,
                    attackRate: 0.9f,
                    defense: 4f);
        }

        private static string GetEnemyEntityIdValue(NodePlaceholderState nodeContext)
        {
            return nodeContext.NodeType == NodeType.BossOrGate
                ? $"{nodeContext.NodeId.Value}_boss_001"
                : $"{nodeContext.NodeId.Value}_enemy_001";
        }

        private static string GetEnemyDisplayName(NodePlaceholderState nodeContext)
        {
            return nodeContext.NodeType == NodeType.BossOrGate
                ? "Gate Enemy"
                : "Enemy Unit";
        }
    }
}

