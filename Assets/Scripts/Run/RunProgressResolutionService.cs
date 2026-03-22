using System;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class RunProgressResolutionService
    {
        private readonly NodeProgressMeterService nodeProgressMeterService;
        private readonly NextNodeUnlockService nextNodeUnlockService;
        private readonly BossProgressionGateUnlockService bossProgressionGateUnlockService;

        public RunProgressResolutionService(
            NodeProgressMeterService nodeProgressMeterService = null,
            NextNodeUnlockService nextNodeUnlockService = null,
            BossProgressionGateUnlockService bossProgressionGateUnlockService = null)
        {
            this.nodeProgressMeterService = nodeProgressMeterService ?? new NodeProgressMeterService();
            this.nextNodeUnlockService = nextNodeUnlockService ?? new NextNodeUnlockService();
            this.bossProgressionGateUnlockService = bossProgressionGateUnlockService ?? new BossProgressionGateUnlockService();
        }

        public RunProgressResolution Resolve(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            CombatEncounterState combatEncounterState,
            PersistentWorldState persistentWorldState,
            WorldGraph worldGraph)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            int nodeProgressDelta = ResolveNodeProgressDelta(nodeContext, resolutionState, combatEncounterState);
            NodeProgressUpdateResult nodeProgressUpdate = ResolveNodeProgressUpdate(
                nodeContext,
                persistentWorldState,
                nodeProgressDelta);
            bool didUnlockRoute = ResolveRouteUnlock(
                nodeContext,
                worldGraph,
                persistentWorldState,
                nodeProgressUpdate);
            BossProgressionGateUnlockResult bossProgressionGateUnlock = ResolveBossProgressionGateUnlock(
                nodeContext,
                resolutionState,
                worldGraph,
                persistentWorldState);

            return new RunProgressResolution(
                nodeProgressDelta,
                nodeProgressUpdate,
                didUnlockRoute,
                bossProgressionGateUnlock);
        }

        private static int ResolveNodeProgressDelta(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            CombatEncounterState combatEncounterState)
        {
            if (!TrackedNodeProgressRules.ShouldTrack(nodeContext.NodeType) ||
                resolutionState != RunResolutionState.Succeeded ||
                combatEncounterState == null)
            {
                return 0;
            }

            return combatEncounterState.DefeatedEnemyCount;
        }

        private NodeProgressUpdateResult ResolveNodeProgressUpdate(
            NodePlaceholderState nodeContext,
            PersistentWorldState persistentWorldState,
            int nodeProgressDelta)
        {
            if (persistentWorldState == null)
            {
                return NodeProgressUpdateResult.Untracked(nodeContext.NodeState);
            }

            return nodeProgressMeterService.ApplyRunProgress(
                persistentWorldState,
                nodeContext,
                nodeProgressDelta);
        }

        private bool ResolveRouteUnlock(
            NodePlaceholderState nodeContext,
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState,
            NodeProgressUpdateResult nodeProgressUpdate)
        {
            if (worldGraph == null || persistentWorldState == null || !nodeProgressUpdate.DidReachClearThreshold)
            {
                return false;
            }

            return nextNodeUnlockService.UnlockConnectedNodesWhenSourceClears(
                worldGraph,
                persistentWorldState,
                nodeContext.NodeId) > 0;
        }

        private BossProgressionGateUnlockResult ResolveBossProgressionGateUnlock(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState)
        {
            return bossProgressionGateUnlockService.TryUnlockProgressionGate(
                nodeContext,
                resolutionState == RunResolutionState.Succeeded,
                worldGraph,
                persistentWorldState);
        }
    }
}

