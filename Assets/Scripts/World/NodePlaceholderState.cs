using System;
using Survivalon.Core;
using Survivalon.Data.Combat;

namespace Survivalon.World
{
    public sealed class NodePlaceholderState
    {
        public NodePlaceholderState(
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState nodeState,
            NodeId originNodeId,
            CombatEncounterDefinition combatEncounter = null)
        {
            if (combatEncounter != null && nodeType != NodeType.Combat && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Combat encounter data requires a combat-compatible placeholder node type.",
                    nameof(combatEncounter));
            }

            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            NodeState = nodeState;
            OriginNodeId = originNodeId;
            CombatEncounter = combatEncounter;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState NodeState { get; }

        public NodeId OriginNodeId { get; }

        public bool UsesCombatShell => NodeType == NodeType.Combat || NodeType == NodeType.BossOrGate;

        public CombatEncounterDefinition CombatEncounter { get; }
    }
}

