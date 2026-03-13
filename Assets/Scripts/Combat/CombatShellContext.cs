using System;

namespace Survivalon.Runtime
{
    public sealed class CombatShellContext
    {
        public CombatShellContext(
            NodeId nodeId,
            CombatShellParticipant playerParticipant,
            CombatShellParticipant enemyParticipant)
        {
            NodeId = nodeId;
            PlayerParticipant = playerParticipant ?? throw new ArgumentNullException(nameof(playerParticipant));
            EnemyParticipant = enemyParticipant ?? throw new ArgumentNullException(nameof(enemyParticipant));
        }

        public NodeId NodeId { get; }

        public CombatShellParticipant PlayerParticipant { get; }

        public CombatShellParticipant EnemyParticipant { get; }
    }
}
