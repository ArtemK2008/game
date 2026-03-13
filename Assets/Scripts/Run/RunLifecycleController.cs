using System;

namespace Survivalon.Runtime
{
    public sealed class RunLifecycleController
    {
        private readonly NodePlaceholderState nodeContext;
        private readonly CombatShellContextFactory combatShellContextFactory;
        private readonly CombatEncounterResolver combatEncounterResolver;
        private RunLifecycleState currentState;
        private CombatShellContext combatShellContext;
        private CombatEncounterState combatEncounterState;
        private RunResult runResult;

        public RunLifecycleController(
            NodePlaceholderState nodeContext,
            CombatShellContextFactory combatShellContextFactory = null,
            CombatEncounterResolver combatEncounterResolver = null)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.combatShellContextFactory = combatShellContextFactory ?? new CombatShellContextFactory();
            this.combatEncounterResolver = combatEncounterResolver ?? new CombatEncounterResolver();
            currentState = RunLifecycleState.RunStart;
        }

        public NodePlaceholderState NodeContext => nodeContext;

        public RunLifecycleState CurrentState => currentState;

        public bool HasRunResult => runResult != null;

        public bool HasCombatContext => combatShellContext != null;

        public bool HasCombatEncounterState => combatEncounterState != null;

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

            combatShellContext = nodeContext.UsesCombatShell
                ? combatShellContextFactory.Create(nodeContext)
                : null;
            combatEncounterState = combatShellContext == null
                ? null
                : new CombatEncounterState(combatShellContext);
            currentState = RunLifecycleState.RunActive;
            return true;
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
            return new RunResult(
                nodeContext.NodeId,
                resolutionState,
                RunRewardPayload.Empty,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
        }
    }
}
