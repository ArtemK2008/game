using System;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class BossProgressionGateUnlockService
    {
        public string TryUnlockProgressionGate(
            NodePlaceholderState nodeContext,
            bool didDefeatBoss,
            WorldGraph worldGraph,
            PersistentWorldState worldState)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (!didDefeatBoss || worldGraph == null || worldState == null || nodeContext.BossProgressionGate == null)
            {
                return string.Empty;
            }

            BossProgressionGateDefinition bossProgressionGate = nodeContext.BossProgressionGate;
            WorldNode unlockedNode = worldGraph.GetNode(bossProgressionGate.UnlockedNodeId);

            if (ResolveNodeState(worldGraph, worldState, unlockedNode.NodeId) != NodeState.Locked)
            {
                return string.Empty;
            }

            PersistentNodeState persistentNodeState = worldState.GetOrAddNodeState(
                unlockedNode.NodeId,
                TrackedNodeProgressRules.GetDefaultThreshold(unlockedNode.NodeType),
                NodeState.Locked);

            if (persistentNodeState.State != NodeState.Locked)
            {
                return string.Empty;
            }

            persistentNodeState.MarkAvailable();
            return bossProgressionGate.UnlockSummaryText;
        }

        private static NodeState ResolveNodeState(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            NodeId nodeId)
        {
            return worldState.TryGetNodeState(nodeId, out PersistentNodeState nodeState)
                ? nodeState.State
                : worldGraph.GetNode(nodeId).State;
        }
    }
}
