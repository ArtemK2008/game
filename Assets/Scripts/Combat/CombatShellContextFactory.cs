using System;

namespace Survivalon.Runtime
{
    public sealed class CombatShellContextFactory
    {
        public CombatShellContext Create(NodePlaceholderState nodeContext)
        {
            return Create(nodeContext, default);
        }

        public CombatShellContext Create(
            NodePlaceholderState nodeContext,
            AccountWideProgressionEffectState progressionEffects)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (!nodeContext.UsesCombatShell)
            {
                throw new InvalidOperationException("Combat shell requires a combat-compatible node type.");
            }

            return new CombatShellContext(
                nodeContext.NodeId,
                new CombatEntityState(
                    new CombatEntityId("player_main"),
                    "Player Unit",
                    CombatSide.Player,
                    CreatePlayerBaseStats(progressionEffects)),
                new CombatEntityState(
                    new CombatEntityId(GetEnemyEntityIdValue(nodeContext)),
                    GetEnemyDisplayName(nodeContext),
                    CombatSide.Enemy,
                    CreateEnemyBaseStats(nodeContext)));
        }

        private static CombatStatBlock CreatePlayerBaseStats(AccountWideProgressionEffectState progressionEffects)
        {
            return new CombatStatBlock(
                maxHealth: 120f + progressionEffects.PlayerMaxHealthBonus,
                attackPower: 14f + progressionEffects.PlayerAttackPowerBonus,
                attackRate: 1.2f,
                defense: 12f);
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
