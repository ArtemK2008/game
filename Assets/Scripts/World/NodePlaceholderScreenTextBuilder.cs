using System;
using System.Collections.Generic;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Run;

namespace Survivalon.World
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
            CombatEncounterState combatEncounterState = null,
            bool requiresRunTimeSkillUpgradeChoice = false)
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
                        ? requiresRunTimeSkillUpgradeChoice
                            ? "Combat shell initialized. Choose a run-only skill upgrade to start automatic combat."
                            : "Combat shell initialized. Preparing automatic combat startup."
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

        public static string BuildRunTimeSkillUpgradeText(
            IReadOnlyList<CombatRunTimeSkillUpgradeOption> upgradeOptions)
        {
            if (upgradeOptions == null)
            {
                throw new ArgumentNullException(nameof(upgradeOptions));
            }

            return
                "Run-only skill choice\n" +
                $"Choose 1 upgrade before combat auto-starts. Options: {upgradeOptions.Count}";
        }
    }
}

