using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public static class NodePlaceholderScreenTextBuilder
    {
        public static string BuildSummaryText(
            NodePlaceholderState placeholderState,
            RunLifecycleState lifecycleState,
            RunResult runResult = null)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            string summary =
                $"Region: {placeholderState.RegionId.Value}\n" +
                $"Type: {placeholderState.NodeType}\n" +
                $"Node state: {placeholderState.NodeState}\n" +
                $"Lifecycle: {lifecycleState}\n" +
                $"Entered from: {placeholderState.OriginNodeId.Value}";

            if (runResult == null)
            {
                return summary;
            }

            return lifecycleState == RunLifecycleState.RunResolved
                ? summary + "\n" + $"Resolution: {runResult.ResolutionState}"
                : summary;
        }

        public static string BuildStatusText(
            NodePlaceholderState placeholderState,
            RunLifecycleState lifecycleState,
            CombatEncounterState combatEncounterState = null)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            bool usesCombatShell = placeholderState.UsesCombatShell;

            switch (lifecycleState)
            {
                case RunLifecycleState.RunStart:
                    return usesCombatShell
                        ? "Combat shell initialized. Preparing automatic combat startup."
                        : "Run shell initialized. Start the placeholder run when ready.";
                case RunLifecycleState.RunActive:
                    return combatEncounterState != null
                        ? "Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."
                        : "Run is active. Resolve the placeholder run to produce a run result.";
                case RunLifecycleState.RunResolved:
                    if (usesCombatShell && combatEncounterState != null)
                    {
                        string winnerText = combatEncounterState.WinnerSide?.ToString() ?? "Unknown";
                        return $"Combat shell resolved. Winner: {winnerText}. Preparing post-run summary.";
                    }

                    return "Run resolved. Open the post-run state to review next actions.";
                case RunLifecycleState.PostRun:
                    return "Post-run summary is active. Replay, return to the world map, or stop the session.";
                default:
                    throw new InvalidOperationException($"Unknown run lifecycle state '{lifecycleState}'.");
            }
        }

        public static string BuildPostRunSummaryText(PostRunStateController postRunStateController, RunResult runResult)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            string nodeProgressSummary = runResult.HasTrackedNodeProgress
                ? $"{runResult.NodeProgressValue} / {runResult.NodeProgressThreshold}"
                : "not tracked";

            return
                "Run finished.\n" +
                $"Node: {runResult.NodeId.Value}\n" +
                $"Resolution: {runResult.ResolutionState}\n" +
                $"Node progress total: {nodeProgressSummary}\n" +
                $"Rewards: {BuildRewardSummary(runResult.RewardPayload)}\n" +
                $"Node progress delta: {runResult.NodeProgressDelta}\n" +
                $"Persistent progression delta: {runResult.PersistentProgressionDelta}\n" +
                $"Route unlock changed: {FormatYesNo(runResult.DidUnlockRoute)}\n" +
                "Next actions:\n" +
                $"- Replay: {FormatYesNo(postRunStateController.CanReplayNode)}\n" +
                $"- Return to world: {FormatYesNo(postRunStateController.CanReturnToWorld)}\n" +
                $"- Stop: {FormatYesNo(postRunStateController.CanStopSession)}";
        }

        private static string BuildRewardSummary(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasRewards)
            {
                return "None";
            }

            List<string> rewardSummaries = new List<string>();

            foreach (RunCurrencyReward currencyReward in rewardPayload.CurrencyRewards)
            {
                rewardSummaries.Add($"{FormatResourceCategory(currencyReward.ResourceCategory)} x{currencyReward.Amount}");
            }

            foreach (RunMaterialReward materialReward in rewardPayload.MaterialRewards)
            {
                rewardSummaries.Add($"{FormatResourceCategory(materialReward.ResourceCategory)} x{materialReward.Amount}");
            }

            return string.Join(", ", rewardSummaries);
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        private static string FormatResourceCategory(ResourceCategory resourceCategory)
        {
            switch (resourceCategory)
            {
                case ResourceCategory.SoftCurrency:
                    return "Soft currency";
                case ResourceCategory.RegionMaterial:
                    return "Region material";
                case ResourceCategory.PersistentProgressionMaterial:
                    return "Persistent progression material";
                default:
                    return resourceCategory.ToString();
            }
        }
    }
}
