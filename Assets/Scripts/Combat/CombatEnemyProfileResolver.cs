using System;
using Survivalon.Data.Combat;
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

            if (nodeContext.CombatEncounter == null)
            {
                throw new InvalidOperationException("Enemy profile resolver requires combat encounter content.");
            }

            return nodeContext.CombatEncounter.PrimaryEnemyProfile;
        }
    }
}
