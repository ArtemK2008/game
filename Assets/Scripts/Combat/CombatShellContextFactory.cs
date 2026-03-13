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
                new CombatShellParticipant("Player Unit", CombatSide.Player),
                new CombatShellParticipant(GetEnemyDisplayName(nodeContext), CombatSide.Enemy));
        }

        private static string GetEnemyDisplayName(NodePlaceholderState nodeContext)
        {
            return nodeContext.NodeType == NodeType.BossOrGate
                ? "Gate Enemy"
                : "Enemy Unit";
        }
    }
}
