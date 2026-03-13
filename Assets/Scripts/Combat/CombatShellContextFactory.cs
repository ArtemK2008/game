using System;

namespace Survivalon.Runtime
{
    public sealed class CombatShellContextFactory
    {
        public CombatShellContext Create(NodePlaceholderState nodeContext)
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
                    CombatSide.Player),
                new CombatEntityState(
                    new CombatEntityId(GetEnemyEntityIdValue(nodeContext)),
                    GetEnemyDisplayName(nodeContext),
                    CombatSide.Enemy));
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
