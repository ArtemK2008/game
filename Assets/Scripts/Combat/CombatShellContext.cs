using System;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Runtime.Run;

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
