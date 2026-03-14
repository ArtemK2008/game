using System;

namespace Survivalon.Runtime
{
    public sealed class RunLifecycleController
    {
        private readonly NodePlaceholderState nodeContext;
        private readonly PersistentWorldState persistentWorldState;
        private readonly CombatShellContextFactory combatShellContextFactory;
        private readonly CombatEncounterResolver combatEncounterResolver;
        private readonly CombatAutoAdvanceLoop combatAutoAdvanceLoop;
        private readonly NodeProgressMeterService nodeProgressMeterService;
        private RunLifecycleState currentState;
        private CombatShellContext combatShellContext;
        private CombatEncounterState combatEncounterState;
        private RunResult runResult;

        public RunLifecycleController(
            NodePlaceholderState nodeContext,
            CombatShellContextFactory combatShellContextFactory = null,
            CombatEncounterResolver combatEncounterResolver = null,
            CombatAutoAdvanceLoop combatAutoAdvanceLoop = null,
            PersistentWorldState persistentWorldState = null,
            NodeProgressMeterService nodeProgressMeterService = null)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.combatShellContextFactory = combatShellContextFactory ?? new CombatShellContextFactory();
            this.combatEncounterResolver = combatEncounterResolver ?? new CombatEncounterResolver();
            this.combatAutoAdvanceLoop = combatAutoAdvanceLoop ?? new CombatAutoAdvanceLoop();
            this.persistentWorldState = persistentWorldState;
            this.nodeProgressMeterService = nodeProgressMeterService ?? new NodeProgressMeterService();
            currentState = RunLifecycleState.RunStart;
        }

        public NodePlaceholderState NodeContext => nodeContext;

        public RunLifecycleState CurrentState => currentState;

        public bool HasRunResult => runResult != null;

        public bool HasCombatContext => combatShellContext != null;

        public bool HasCombatEncounterState => combatEncounterState != null;

        public bool UsesCombatShell => nodeContext.UsesCombatShell;

        public CombatShellContext CombatContext => combatShellContext ??
            throw new InvalidOperationException("Combat context is not available until a combat-compatible run enters the active state.");

        public CombatEncounterState CombatEncounterState => combatEncounterState ??
            throw new InvalidOperationException("Combat encounter state is not available until a combat-compatible run enters the active state.");

        public RunResult RunResult => runResult ??
            throw new InvalidOperationException("Run result is not available until the run is resolved.");

        public bool TryEnterActiveState()
        {
            if (currentState != RunLifecycleState.RunStart)
            {
                return false;
            }

            combatAutoAdvanceLoop.Reset();
            combatShellContext = nodeContext.UsesCombatShell
                ? combatShellContextFactory.Create(nodeContext)
                : null;
            combatEncounterState = combatShellContext == null
                ? null
                : new CombatEncounterState(combatShellContext);
            currentState = RunLifecycleState.RunActive;
            return true;
        }

        public bool TryAdvanceTime(float elapsedSeconds)
        {
            return combatAutoAdvanceLoop.TryAdvance(this, elapsedSeconds);
        }

        public bool TryStartAutomaticFlow()
        {
            return UsesCombatShell && currentState == RunLifecycleState.RunStart && TryEnterActiveState();
        }

        public bool TryAdvanceAutomaticTime(float elapsedSeconds)
        {
            if (elapsedSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedSeconds),
                    elapsedSeconds,
                    "Elapsed time cannot be negative.");
            }

            if (elapsedSeconds == 0f)
            {
                return false;
            }

            bool changed = TryAdvanceTime(elapsedSeconds);
            if (UsesCombatShell && currentState == RunLifecycleState.RunResolved && runResult != null)
            {
                changed |= TryEnterPostRunState();
            }

            return changed;
        }

        public bool TryAdvanceCombat(float elapsedSeconds)
        {
            if (currentState != RunLifecycleState.RunActive || combatEncounterState == null)
            {
                return false;
            }

            bool advanced = combatEncounterResolver.TryAdvance(combatEncounterState, elapsedSeconds);
            if (!combatEncounterState.IsResolved)
            {
                return advanced;
            }

            runResult = CreateRunResult(
                combatEncounterState.Outcome == CombatEncounterOutcome.PlayerVictory
                    ? RunResolutionState.Succeeded
                    : RunResolutionState.Failed);
            currentState = RunLifecycleState.RunResolved;
            return advanced;
        }

        public bool TryResolveRun(RunResolutionState resolutionState)
        {
            if (currentState != RunLifecycleState.RunActive)
            {
                return false;
            }

            if (combatEncounterState != null && !combatEncounterState.IsResolved)
            {
                return false;
            }

            runResult = CreateRunResult(resolutionState);
            currentState = RunLifecycleState.RunResolved;
            return true;
        }

        public bool TryEnterPostRunState()
        {
            if (currentState != RunLifecycleState.RunResolved || runResult == null)
            {
                return false;
            }

            currentState = RunLifecycleState.PostRun;
            return true;
        }

        private RunResult CreateRunResult(RunResolutionState resolutionState)
        {
            int nodeProgressDelta = ResolveNodeProgressDelta(resolutionState);
            NodeProgressUpdateResult nodeProgressUpdate = ResolveNodeProgressUpdate(nodeProgressDelta);

            return new RunResult(
                nodeContext.NodeId,
                resolutionState,
                RunRewardPayload.Empty,
                nodeProgressDelta,
                nodeProgressUpdate.CurrentProgress,
                nodeProgressUpdate.ProgressThreshold,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
        }

        private int ResolveNodeProgressDelta(RunResolutionState resolutionState)
        {
            if (!NodeProgressMeterService.ShouldTrackProgress(nodeContext.NodeType) ||
                resolutionState != RunResolutionState.Succeeded ||
                combatEncounterState == null)
            {
                return 0;
            }

            return combatEncounterState.DefeatedEnemyCount;
        }

        private NodeProgressUpdateResult ResolveNodeProgressUpdate(int nodeProgressDelta)
        {
            if (persistentWorldState == null)
            {
                return NodeProgressUpdateResult.Untracked(nodeContext.NodeState);
            }

            return nodeProgressMeterService.ApplyRunProgress(
                persistentWorldState,
                nodeContext,
                nodeProgressDelta);
        }
    }
}
