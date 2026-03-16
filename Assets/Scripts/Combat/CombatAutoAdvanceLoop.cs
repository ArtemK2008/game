using System;
using Survivalon.Run;

namespace Survivalon.Combat
{
    /// <summary>
    /// Advances active combat encounters in fixed time steps based on accumulated elapsed time.
    /// </summary>
    public sealed class CombatAutoAdvanceLoop
    {
        private const float TickEpsilon = 0.0001f;
        private readonly float tickIntervalSeconds;
        private float accumulatedSeconds;

        public CombatAutoAdvanceLoop(float tickIntervalSeconds = 0.25f)
        {
            if (tickIntervalSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tickIntervalSeconds),
                    tickIntervalSeconds,
                    "Tick interval must be greater than zero.");
            }

            this.tickIntervalSeconds = tickIntervalSeconds;
        }

        public float TickIntervalSeconds => tickIntervalSeconds;

        public void Reset()
        {
            accumulatedSeconds = 0f;
        }

        public bool TryAdvance(RunLifecycleController runLifecycleController, float elapsedSeconds)
        {
            if (runLifecycleController == null)
            {
                throw new ArgumentNullException(nameof(runLifecycleController));
            }

            if (elapsedSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedSeconds),
                    elapsedSeconds,
                    "Elapsed time cannot be negative.");
            }

            if (elapsedSeconds <= 0f || !ShouldAutoAdvance(runLifecycleController))
            {
                if (!ShouldAutoAdvance(runLifecycleController))
                {
                    Reset();
                }

                return false;
            }

            accumulatedSeconds += elapsedSeconds;

            bool advanced = false;
            while (accumulatedSeconds + TickEpsilon >= tickIntervalSeconds &&
                ShouldAutoAdvance(runLifecycleController))
            {
                accumulatedSeconds = Math.Max(0f, accumulatedSeconds - tickIntervalSeconds);
                advanced |= runLifecycleController.TryAdvanceCombat(tickIntervalSeconds);
            }

            if (!ShouldAutoAdvance(runLifecycleController))
            {
                Reset();
            }

            return advanced;
        }

        private static bool ShouldAutoAdvance(RunLifecycleController runLifecycleController)
        {
            return runLifecycleController.CurrentState == RunLifecycleState.RunActive &&
                runLifecycleController.HasCombatEncounterState &&
                !runLifecycleController.CombatEncounterState.IsResolved;
        }
    }
}

