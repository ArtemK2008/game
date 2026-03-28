using System;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Run;

namespace Survivalon.World
{
    /// <summary>
    /// Форматирует placeholder-текст экрана узла и локальные статусы run/combat flow.
    /// </summary>
    public static class NodePlaceholderScreenTextBuilder
    {
        public static string BuildTitleText(NodePlaceholderState placeholderState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            return placeholderState.NodeDisplayName;
        }

        public static string BuildCombatContextSummaryText(NodePlaceholderState placeholderState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            BossEncounterPresentationState bossPresentationState =
                BossEncounterPresentationStateResolver.Resolve(placeholderState);
            OptionalChallengePresentationState challengePresentationState =
                OptionalChallengePresentationStateResolver.Resolve(placeholderState);
            string encounterDisplayName = bossPresentationState.IsBossEncounter
                ? bossPresentationState.EncounterDisplayName
                : challengePresentationState.IsOptionalChallenge
                    ? challengePresentationState.ChallengeDisplayName
                    : PlayerFacingCoreLabelFormatter.FormatNodeType(placeholderState.NodeType);
            string summaryText =
                $"Location: {placeholderState.LocationIdentity.DisplayName}\n" +
                $"Encounter: {encounterDisplayName}";

            return bossPresentationState.HasStakesSummary
                ? summaryText + "\n" + $"Boss stakes: {bossPresentationState.StakesSummary}"
                : summaryText;
        }

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
                $"Location: {placeholderState.LocationIdentity.DisplayName}\n" +
                $"Node: {placeholderState.NodeDisplayName}\n" +
                $"Region: {placeholderState.RegionId.Value}\n" +
                $"Reward focus: {placeholderState.LocationIdentity.RewardFocusDisplayName}\n" +
                $"Enemy emphasis: {placeholderState.LocationIdentity.EnemyEmphasisDisplayName}\n" +
                BuildRegionMaterialYieldLine(placeholderState) +
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

        private static string BuildRegionMaterialYieldLine(NodePlaceholderState placeholderState)
        {
            if (placeholderState.RegionMaterialYieldContent == null ||
                !placeholderState.SupportsRegionMaterialRewards)
            {
                return string.Empty;
            }

            return $"Revisit value: Region material yield +{placeholderState.RegionMaterialYieldContent.RegionMaterialBonus}\n";
        }
    }
}

