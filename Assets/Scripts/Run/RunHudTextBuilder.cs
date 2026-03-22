using System;

namespace Survivalon.Run
{
    public static class RunHudTextBuilder
    {
        public static string BuildContextTitle(RunHudState runHudState)
        {
            if (runHudState == null)
            {
                throw new ArgumentNullException(nameof(runHudState));
            }

            return $"{runHudState.LocationDisplayName} | {runHudState.NodeDisplayName}";
        }

        public static string BuildSummaryText(RunHudState runHudState)
        {
            if (runHudState == null)
            {
                throw new ArgumentNullException(nameof(runHudState));
            }

            string summaryText =
                $"Run state: {runHudState.RunStateDisplayName} | Outcome: {runHudState.OutcomeDisplayName} | Elapsed: {FormatValue(runHudState.ElapsedCombatSeconds)}s\n" +
                $"Health: {runHudState.PlayerDisplayName} {FormatValue(runHudState.PlayerCurrentHealth)} / {FormatValue(runHudState.PlayerMaxHealth)} | " +
                $"{runHudState.EnemyDisplayName} {FormatValue(runHudState.EnemyCurrentHealth)} / {FormatValue(runHudState.EnemyMaxHealth)}";

            if (!runHudState.HasTrackedProgressContext)
            {
                return summaryText;
            }

            return summaryText +
                "\n" +
                $"Progress: {runHudState.CurrentProgress} / {runHudState.ProgressThreshold} toward {runHudState.ProgressGoalDisplayName}";
        }

        private static string FormatValue(float value)
        {
            return value.ToString("0.##");
        }
    }
}
