using System;

namespace Survivalon.Runtime
{
    public sealed class CombatShellContextFactory
    {
        public CombatShellContext Create(NodePlaceholderState nodeContext)
        {
            return Create(nodeContext, PlayableCharacterCatalog.Default, default);
        }

        public CombatShellContext Create(
            NodePlaceholderState nodeContext,
            AccountWideProgressionEffectState progressionEffects)
        {
            return Create(nodeContext, PlayableCharacterCatalog.Default, progressionEffects);
        }

        public CombatShellContext Create(
            NodePlaceholderState nodeContext,
            PlayableCharacterProfile playableCharacter,
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
                    CreatePlayerBaseStats(resolvedCharacter.BaseStats, progressionEffects)),
                new CombatEntityState(
                    new CombatEntityId(GetEnemyEntityIdValue(nodeContext)),
                    GetEnemyDisplayName(nodeContext),
                    CombatSide.Enemy,
                    CreateEnemyBaseStats(nodeContext)));
        }

        private static CombatStatBlock CreatePlayerBaseStats(
            CombatStatBlock characterBaseStats,
            AccountWideProgressionEffectState progressionEffects)
        {
            return new CombatStatBlock(
                maxHealth: characterBaseStats.MaxHealth + progressionEffects.PlayerMaxHealthBonus,
                attackPower: characterBaseStats.AttackPower + progressionEffects.PlayerAttackPowerBonus,
                attackRate: characterBaseStats.AttackRate,
                defense: characterBaseStats.Defense);
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
