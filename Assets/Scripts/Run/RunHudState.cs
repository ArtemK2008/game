using System;
using Survivalon.Combat;
using Survivalon.Core;

namespace Survivalon.Run
{
    public sealed class RunHudState
    {
        public RunHudState(
            string locationDisplayName,
            NodeId nodeId,
            NodeType nodeType,
            string runStateDisplayName,
            string outcomeDisplayName,
            float elapsedCombatSeconds,
            string playerDisplayName,
            float playerCurrentHealth,
            float playerMaxHealth,
            string enemyDisplayName,
            float enemyCurrentHealth,
            float enemyMaxHealth,
            bool hasTrackedProgressContext,
            int currentProgress,
            int progressThreshold,
            string progressGoalDisplayName)
        {
            if (string.IsNullOrWhiteSpace(locationDisplayName))
            {
                throw new ArgumentException(
                    "Location display name cannot be null or whitespace.",
                    nameof(locationDisplayName));
            }

            if (string.IsNullOrWhiteSpace(runStateDisplayName))
            {
                throw new ArgumentException(
                    "Run state display name cannot be null or whitespace.",
                    nameof(runStateDisplayName));
            }

            if (string.IsNullOrWhiteSpace(outcomeDisplayName))
            {
                throw new ArgumentException(
                    "Outcome display name cannot be null or whitespace.",
                    nameof(outcomeDisplayName));
            }

            if (string.IsNullOrWhiteSpace(playerDisplayName))
            {
                throw new ArgumentException(
                    "Player display name cannot be null or whitespace.",
                    nameof(playerDisplayName));
            }

            if (string.IsNullOrWhiteSpace(enemyDisplayName))
            {
                throw new ArgumentException(
                    "Enemy display name cannot be null or whitespace.",
                    nameof(enemyDisplayName));
            }

            if (elapsedCombatSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedCombatSeconds),
                    elapsedCombatSeconds,
                    "Elapsed combat seconds cannot be negative.");
            }

            if (playerCurrentHealth < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(playerCurrentHealth),
                    playerCurrentHealth,
                    "Player current health cannot be negative.");
            }

            if (playerMaxHealth <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(playerMaxHealth),
                    playerMaxHealth,
                    "Player max health must be positive.");
            }

            if (enemyCurrentHealth < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(enemyCurrentHealth),
                    enemyCurrentHealth,
                    "Enemy current health cannot be negative.");
            }

            if (enemyMaxHealth <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(enemyMaxHealth),
                    enemyMaxHealth,
                    "Enemy max health must be positive.");
            }

            if (playerCurrentHealth > playerMaxHealth)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(playerCurrentHealth),
                    playerCurrentHealth,
                    "Player current health cannot exceed max health.");
            }

            if (enemyCurrentHealth > enemyMaxHealth)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(enemyCurrentHealth),
                    enemyCurrentHealth,
                    "Enemy current health cannot exceed max health.");
            }

            if (currentProgress < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentProgress),
                    currentProgress,
                    "Current progress cannot be negative.");
            }

            if (progressThreshold < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(progressThreshold),
                    progressThreshold,
                    "Progress threshold cannot be negative.");
            }

            if (!hasTrackedProgressContext && currentProgress != 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentProgress),
                    currentProgress,
                    "Untracked progress context must keep current progress at zero.");
            }

            if (!hasTrackedProgressContext && progressThreshold != 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(progressThreshold),
                    progressThreshold,
                    "Untracked progress context must keep threshold at zero.");
            }

            if (hasTrackedProgressContext && string.IsNullOrWhiteSpace(progressGoalDisplayName))
            {
                throw new ArgumentException(
                    "Tracked progress context requires a progress goal display name.",
                    nameof(progressGoalDisplayName));
            }

            if (!hasTrackedProgressContext && progressGoalDisplayName != null)
            {
                throw new ArgumentException(
                    "Untracked progress context cannot expose a progress goal display name.",
                    nameof(progressGoalDisplayName));
            }

            LocationDisplayName = locationDisplayName;
            NodeId = nodeId;
            NodeType = nodeType;
            RunStateDisplayName = runStateDisplayName;
            OutcomeDisplayName = outcomeDisplayName;
            ElapsedCombatSeconds = elapsedCombatSeconds;
            PlayerDisplayName = playerDisplayName;
            PlayerCurrentHealth = playerCurrentHealth;
            PlayerMaxHealth = playerMaxHealth;
            EnemyDisplayName = enemyDisplayName;
            EnemyCurrentHealth = enemyCurrentHealth;
            EnemyMaxHealth = enemyMaxHealth;
            HasTrackedProgressContext = hasTrackedProgressContext;
            CurrentProgress = currentProgress;
            ProgressThreshold = progressThreshold;
            ProgressGoalDisplayName = progressGoalDisplayName;
        }

        public string LocationDisplayName { get; }

        public NodeId NodeId { get; }

        public NodeType NodeType { get; }

        public string RunStateDisplayName { get; }

        public string OutcomeDisplayName { get; }

        public float ElapsedCombatSeconds { get; }

        public string PlayerDisplayName { get; }

        public float PlayerCurrentHealth { get; }

        public float PlayerMaxHealth { get; }

        public string EnemyDisplayName { get; }

        public float EnemyCurrentHealth { get; }

        public float EnemyMaxHealth { get; }

        public bool HasTrackedProgressContext { get; }

        public int CurrentProgress { get; }

        public int ProgressThreshold { get; }

        public string ProgressGoalDisplayName { get; }
    }
}
