using System;

namespace Survivalon.Combat
{
    public static class CombatShellTextBuilder
    {
        public static string BuildSummaryText(CombatEncounterState combatEncounterState)
        {
            if (combatEncounterState == null)
            {
                throw new ArgumentNullException(nameof(combatEncounterState));
            }

            string outcomeText = combatEncounterState.IsResolved
                ? $"Outcome: {combatEncounterState.Outcome}"
                : "Outcome: Ongoing";

            return
                $"Elapsed: {FormatStat(combatEncounterState.ElapsedCombatSeconds)}s | {outcomeText}\n" +
                $"Targeting: {combatEncounterState.PlayerEntity.DisplayName} -> {combatEncounterState.EnemyEntity.DisplayName}; " +
                $"{combatEncounterState.EnemyEntity.DisplayName} -> {combatEncounterState.PlayerEntity.DisplayName}";
        }

        public static string BuildEntityCardText(CombatEntityRuntimeState combatEntity)
        {
            if (combatEntity == null)
            {
                throw new ArgumentNullException(nameof(combatEntity));
            }

            return
                $"{combatEntity.DisplayName}\n" +
                $"{combatEntity.Side} | Alive: {FormatYesNo(combatEntity.IsAlive)} | Act: {FormatYesNo(combatEntity.IsActive)}\n" +
                $"HP: {FormatStat(combatEntity.CurrentHealth)} / {FormatStat(combatEntity.MaxHealth)} | ATK: {FormatStat(combatEntity.CombatEntity.BaseStats.AttackPower)}\n" +
                $"Rate: {FormatStat(combatEntity.CombatEntity.BaseStats.AttackRate)}/s | DEF: {FormatStat(combatEntity.CombatEntity.BaseStats.Defense)}";
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        private static string FormatStat(float value)
        {
            return value.ToString("0.##");
        }
    }
}

