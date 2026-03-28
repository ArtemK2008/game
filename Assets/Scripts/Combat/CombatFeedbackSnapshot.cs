using System;

namespace Survivalon.Combat
{
    /// <summary>
    /// Captures the minimal combat snapshot needed to derive sound-feedback transitions.
    /// </summary>
    public readonly struct CombatFeedbackSnapshot
    {
        public static readonly CombatFeedbackSnapshot None = new CombatFeedbackSnapshot(
            hasEncounter: false,
            playerCurrentHealth: 0f,
            playerMaxHealth: 0f,
            enemyCurrentHealth: 0f,
            enemyMaxHealth: 0f,
            playerIsAlive: false,
            enemyIsAlive: false,
            playerBaselineAttackTimerSeconds: 0f,
            enemyBaselineAttackTimerSeconds: 0f,
            playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity,
            playerTriggeredActiveSkillId: null);

        public CombatFeedbackSnapshot(
            bool hasEncounter,
            float playerCurrentHealth,
            float playerMaxHealth,
            float enemyCurrentHealth,
            float enemyMaxHealth,
            bool playerIsAlive,
            bool enemyIsAlive,
            float playerBaselineAttackTimerSeconds,
            float enemyBaselineAttackTimerSeconds,
            float playerTriggeredActiveSkillTimerSeconds,
            string playerTriggeredActiveSkillId)
        {
            if (hasEncounter)
            {
                ValidateHealth("player", playerCurrentHealth, playerMaxHealth);
                ValidateHealth("enemy", enemyCurrentHealth, enemyMaxHealth);
            }

            HasEncounter = hasEncounter;
            PlayerCurrentHealth = playerCurrentHealth;
            PlayerMaxHealth = playerMaxHealth;
            EnemyCurrentHealth = enemyCurrentHealth;
            EnemyMaxHealth = enemyMaxHealth;
            PlayerIsAlive = playerIsAlive;
            EnemyIsAlive = enemyIsAlive;
            PlayerBaselineAttackTimerSeconds = playerBaselineAttackTimerSeconds;
            EnemyBaselineAttackTimerSeconds = enemyBaselineAttackTimerSeconds;
            PlayerTriggeredActiveSkillTimerSeconds = playerTriggeredActiveSkillTimerSeconds;
            PlayerTriggeredActiveSkillId = playerTriggeredActiveSkillId;
        }

        public bool HasEncounter { get; }

        public float PlayerCurrentHealth { get; }

        public float PlayerMaxHealth { get; }

        public float EnemyCurrentHealth { get; }

        public float EnemyMaxHealth { get; }

        public bool PlayerIsAlive { get; }

        public bool EnemyIsAlive { get; }

        public float PlayerBaselineAttackTimerSeconds { get; }

        public float EnemyBaselineAttackTimerSeconds { get; }

        public float PlayerTriggeredActiveSkillTimerSeconds { get; }

        public string PlayerTriggeredActiveSkillId { get; }

        public float PlayerHealthRatio => !HasEncounter || PlayerMaxHealth <= 0f
            ? 0f
            : PlayerCurrentHealth / PlayerMaxHealth;

        public static CombatFeedbackSnapshot FromEncounterState(CombatEncounterState encounterState)
        {
            if (encounterState == null)
            {
                return None;
            }

            return new CombatFeedbackSnapshot(
                hasEncounter: true,
                playerCurrentHealth: encounterState.PlayerEntity.CurrentHealth,
                playerMaxHealth: encounterState.PlayerEntity.MaxHealth,
                enemyCurrentHealth: encounterState.EnemyEntity.CurrentHealth,
                enemyMaxHealth: encounterState.EnemyEntity.MaxHealth,
                playerIsAlive: encounterState.PlayerEntity.IsAlive,
                enemyIsAlive: encounterState.EnemyEntity.IsAlive,
                playerBaselineAttackTimerSeconds: encounterState.PlayerEntity.TimeUntilNextBaselineAttackSeconds,
                enemyBaselineAttackTimerSeconds: encounterState.EnemyEntity.TimeUntilNextBaselineAttackSeconds,
                playerTriggeredActiveSkillTimerSeconds: encounterState.PlayerEntity.TimeUntilTriggeredActiveSkillSeconds,
                playerTriggeredActiveSkillId: encounterState.PlayerEntity.TriggeredActiveSkill?.SkillId);
        }

        private static void ValidateHealth(string label, float currentHealth, float maxHealth)
        {
            if (currentHealth < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentHealth),
                    $"{label} current health cannot be negative.");
            }

            if (maxHealth <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxHealth),
                    $"{label} max health must be positive.");
            }

            if (currentHealth > maxHealth)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentHealth),
                    $"{label} current health cannot exceed max health.");
            }
        }
    }
}
