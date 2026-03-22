using System;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class RunHudStateResolver
    {
        public RunHudState Resolve(
            NodePlaceholderState nodeContext,
            RunLifecycleState lifecycleState,
            CombatEncounterState combatEncounterState,
            PersistentWorldState persistentWorldState = null)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (combatEncounterState == null)
            {
                throw new ArgumentNullException(nameof(combatEncounterState));
            }

            bool hasTrackedProgressContext = TrackedNodeProgressRules.ShouldTrack(nodeContext.NodeType);
            int currentProgress = 0;
            int progressThreshold = 0;
            string progressGoalDisplayName = null;

            if (hasTrackedProgressContext)
            {
                ResolveTrackedProgressContext(
                    nodeContext,
                    persistentWorldState,
                    out currentProgress,
                    out progressThreshold,
                    out progressGoalDisplayName);
            }

            return new RunHudState(
                nodeContext.LocationIdentity.DisplayName,
                nodeContext.NodeDisplayName,
                nodeContext.NodeId,
                nodeContext.NodeType,
                ResolveRunStateDisplayName(lifecycleState),
                ResolveOutcomeDisplayName(combatEncounterState),
                combatEncounterState.ElapsedCombatSeconds,
                combatEncounterState.PlayerEntity.DisplayName,
                combatEncounterState.PlayerEntity.CurrentHealth,
                combatEncounterState.PlayerEntity.MaxHealth,
                combatEncounterState.EnemyEntity.DisplayName,
                combatEncounterState.EnemyEntity.CurrentHealth,
                combatEncounterState.EnemyEntity.MaxHealth,
                hasTrackedProgressContext,
                currentProgress,
                progressThreshold,
                progressGoalDisplayName);
        }

        private static string ResolveRunStateDisplayName(RunLifecycleState lifecycleState)
        {
            switch (lifecycleState)
            {
                case RunLifecycleState.RunStart:
                    return "Preparing auto-battle";
                case RunLifecycleState.RunActive:
                    return "Auto-battle active";
                case RunLifecycleState.RunResolved:
                    return "Auto-battle resolved";
                case RunLifecycleState.PostRun:
                    return "Post-run ready";
                default:
                    throw new InvalidOperationException(
                        $"Unknown run lifecycle state '{lifecycleState}'.");
            }
        }

        private static string ResolveOutcomeDisplayName(CombatEncounterState combatEncounterState)
        {
            return combatEncounterState.IsResolved
                ? combatEncounterState.Outcome.ToString()
                : "Ongoing";
        }

        private static void ResolveTrackedProgressContext(
            NodePlaceholderState nodeContext,
            PersistentWorldState persistentWorldState,
            out int currentProgress,
            out int progressThreshold,
            out string progressGoalDisplayName)
        {
            currentProgress = 0;
            progressThreshold = TrackedNodeProgressRules.GetDefaultThreshold(nodeContext.NodeType);
            progressGoalDisplayName = ResolveProgressGoalDisplayName(nodeContext);

            if (persistentWorldState == null ||
                !persistentWorldState.TryGetNodeState(nodeContext.NodeId, out PersistentNodeState nodeState))
            {
                return;
            }

            currentProgress = nodeState.UnlockProgress;
            progressThreshold = nodeState.UnlockThreshold > 0
                ? nodeState.UnlockThreshold
                : progressThreshold;
        }

        private static string ResolveProgressGoalDisplayName(NodePlaceholderState nodeContext)
        {
            return nodeContext.NodeType == NodeType.BossOrGate
                ? "gate clear"
                : "node clear";
        }
    }
}
