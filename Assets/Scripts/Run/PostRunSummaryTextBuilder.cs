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
                "Run finished.\n" +
                $"Location: {postRunStateController.NodeContext.LocationIdentity.DisplayName}\n" +
                $"Node: {postRunStateController.NodeContext.NodeDisplayName}\n" +
                $"Resolution: {runResult.ResolutionState}\n" +
                $"Ordinary rewards: {presentationState.OrdinaryRewardSummary}\n" +
                BuildOptionalLine("Reward source", presentationState.RewardSourceSummary) +
                BuildOptionalLine("Clear spike rewards", presentationState.ClearSpikeRewardSummary) +
                BuildOptionalLine("Boss spike rewards", presentationState.BossSpikeRewardSummary) +
                BuildOptionalLine("Boss gear rewards", presentationState.BossGearRewardSummary) +
                BuildOptionalLine("Unlock outcomes", presentationState.UnlockOutcomeSummary) +
                $"Progress changes: {presentationState.ProgressSummary}\n";
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

