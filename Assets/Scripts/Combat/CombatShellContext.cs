using System;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.Combat
{
    public sealed class CombatShellContext
    {
        public CombatShellContext(
            NodeId nodeId,
            CombatEntityState playerEntity,
            CombatEntityState enemyEntity)
        {
            NodeId = nodeId;
            PlayerEntity = playerEntity ?? throw new ArgumentNullException(nameof(playerEntity));
            EnemyEntity = enemyEntity ?? throw new ArgumentNullException(nameof(enemyEntity));
        }

        public NodeId NodeId { get; }

        public CombatEntityState PlayerEntity { get; }

        public CombatEntityState EnemyEntity { get; }
    }
}
