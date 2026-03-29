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

            return runHudState.HasBossEncounterPresentation
                ? $"Boss encounter | {runHudState.LocationDisplayName} | {runHudState.NodeDisplayName}"
                : $"{runHudState.LocationDisplayName} | {runHudState.NodeDisplayName}";
        }

        public static string BuildSummaryText(RunHudState runHudState)
        {
            if (runHudState == null)
            {
                throw new ArgumentNullException(nameof(runHudState));
            }

            string summaryText =
                $"Status: {runHudState.RunStateDisplayName} | Outcome: {runHudState.OutcomeDisplayName} | Time: {FormatValue(runHudState.ElapsedCombatSeconds)}s\n" +
                $"Health: {runHudState.PlayerDisplayName} {FormatValue(runHudState.PlayerCurrentHealth)} / {FormatValue(runHudState.PlayerMaxHealth)} | " +
                $"{runHudState.EnemyDisplayName} {FormatValue(runHudState.EnemyCurrentHealth)} / {FormatValue(runHudState.EnemyMaxHealth)}";

            if (runHudState.HasBossEncounterPresentation)
            {
                summaryText += "\n" + BuildBossEncounterSummaryLine(runHudState);
            }

            if (!runHudState.HasTrackedProgressContext)
            {
                return summaryText;
            }

            return summaryText +
                "\n" +
                $"Objective: {runHudState.CurrentProgress} / {runHudState.ProgressThreshold} toward {runHudState.ProgressGoalDisplayName}";
        }

        private static string BuildBossEncounterSummaryLine(RunHudState runHudState)
        {
            string bossSummaryLine = $"Boss: {runHudState.BossEncounterDisplayName}";
            if (!runHudState.HasBossStakeSummary)
            {
                return bossSummaryLine;
            }

            return bossSummaryLine + $" | Stakes: {runHudState.BossStakeSummary}";
        }

        private static string FormatValue(float value)
        {
            return value.ToString("0.##");
        }
    }
}
