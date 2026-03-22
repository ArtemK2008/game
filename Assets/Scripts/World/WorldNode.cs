using System;
using Survivalon.Core;
using Survivalon.Data.Combat;

namespace Survivalon.World
{
    public sealed class WorldNode
    {
        public WorldNode(
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState state,
            CombatEncounterDefinition combatEncounter = null,
            BossProgressionGateDefinition bossProgressionGate = null)
        {
            if (combatEncounter != null && nodeType != NodeType.Combat && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Combat encounter data requires a combat-compatible node type.",
                    nameof(combatEncounter));
            }

            if (bossProgressionGate != null && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Boss progression gate data requires a boss-or-gate node type.",
                    nameof(bossProgressionGate));
            }

            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            State = state;
            CombatEncounter = combatEncounter;
            BossProgressionGate = bossProgressionGate;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState State { get; }

        public CombatEncounterDefinition CombatEncounter { get; }

        public BossProgressionGateDefinition BossProgressionGate { get; }
    }
}

