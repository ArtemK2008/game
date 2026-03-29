using System;
namespace Survivalon.Run
{
    /// <summary>
    /// Форматирует компактную post-run summary без логики выдачи наград и прогресса.
    /// </summary>
    public static class PostRunSummaryTextBuilder
    {
        private static readonly PostRunResultPresentationStateResolver PresentationStateResolver =
            new PostRunResultPresentationStateResolver();

        public static string Build(PostRunStateController postRunStateController, RunResult runResult)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            PostRunResultPresentationState presentationState =
                PresentationStateResolver.Resolve(postRunStateController, runResult);

            return
                "Run complete.\n" +
                $"Location: {postRunStateController.NodeContext.LocationIdentity.DisplayName}\n" +
                $"Node: {postRunStateController.NodeContext.NodeDisplayName}\n" +
                $"Result: {runResult.ResolutionState}\n" +
                $"Rewards: {presentationState.OrdinaryRewardSummary}\n" +
                BuildOptionalLine("Source", presentationState.RewardSourceSummary) +
                BuildOptionalLine("Clear bonus", presentationState.ClearSpikeRewardSummary) +
                BuildOptionalLine("Boss bonus", presentationState.BossSpikeRewardSummary) +
                BuildOptionalLine("Boss gear", presentationState.BossGearRewardSummary) +
                BuildOptionalLine("Unlocks", presentationState.UnlockOutcomeSummary) +
                $"Progress: {presentationState.ProgressSummary}\n";
        }

        private static string BuildOptionalLine(string label, string summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
            {
                return string.Empty;
            }

            return $"{label}: {summary}\n";
        }
    }
}

