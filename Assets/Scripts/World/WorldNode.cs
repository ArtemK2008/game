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
            CombatEncounterDefinition combatEncounter = null)
        {
            if (combatEncounter != null && nodeType != NodeType.Combat && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Combat encounter data requires a combat-compatible node type.",
                    nameof(combatEncounter));
            }

            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            State = state;
            CombatEncounter = combatEncounter;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState State { get; }

        public CombatEncounterDefinition CombatEncounter { get; }
    }
}

