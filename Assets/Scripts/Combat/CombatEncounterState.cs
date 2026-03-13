using System;

namespace Survivalon.Runtime
{
    public sealed class CombatEncounterState
    {
        public CombatEncounterState(CombatShellContext combatContext)
        {
            CombatContext = combatContext ?? throw new ArgumentNullException(nameof(combatContext));

            if (combatContext.PlayerEntity.Side != CombatSide.Player)
            {
                throw new InvalidOperationException("Combat encounter requires the player-side entity to use CombatSide.Player.");
            }

            if (combatContext.EnemyEntity.Side != CombatSide.Enemy)
            {
                throw new InvalidOperationException("Combat encounter requires the enemy-side entity to use CombatSide.Enemy.");
            }

            PlayerEntity = new CombatEntityRuntimeState(combatContext.PlayerEntity);
            EnemyEntity = new CombatEntityRuntimeState(combatContext.EnemyEntity);
            Outcome = CombatEncounterOutcome.None;
        }

        public CombatShellContext CombatContext { get; }

        public CombatEntityRuntimeState PlayerEntity { get; }

        public CombatEntityRuntimeState EnemyEntity { get; }

        public float ElapsedCombatSeconds { get; private set; }

        public CombatEncounterOutcome Outcome { get; private set; }

        public bool IsResolved => Outcome != CombatEncounterOutcome.None;

        public CombatSide? WinnerSide => Outcome switch
        {
            CombatEncounterOutcome.PlayerVictory => CombatSide.Player,
            CombatEncounterOutcome.EnemyVictory => CombatSide.Enemy,
            _ => null,
        };

        public void AdvanceElapsedTime(float elapsedSeconds)
        {
            if (elapsedSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), elapsedSeconds, "Elapsed time cannot be negative.");
            }

            ElapsedCombatSeconds += elapsedSeconds;
        }

        public void Resolve(CombatEncounterOutcome outcome)
        {
            if (outcome == CombatEncounterOutcome.None)
            {
                throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "Resolved combat outcome must identify a winner.");
            }

            Outcome = outcome;
        }
    }
}
