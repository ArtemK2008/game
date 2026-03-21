using System;
using Survivalon.Data.Combat;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Combat
{
    public sealed class CombatEnemyProfileResolver
    {
        public CombatEnemyProfile Resolve(NodePlaceholderState nodeContext)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (!nodeContext.UsesCombatShell)
            {
                throw new InvalidOperationException("Enemy profile resolver requires a combat-compatible node type.");
            }

            if (nodeContext.NodeType == NodeType.BossOrGate)
            {
                return CombatEnemyProfileCatalog.GateEnemy;
            }

            if (nodeContext.NodeId == BootstrapWorldScenario.ForestPushNodeId)
            {
                return CombatEnemyProfileCatalog.BulwarkRaider;
            }

            return CombatEnemyProfileCatalog.EnemyUnit;
        }
    }
}
