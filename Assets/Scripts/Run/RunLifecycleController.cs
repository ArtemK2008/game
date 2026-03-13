using System;

namespace Survivalon.Runtime
{
    public sealed class RunLifecycleController
    {
        private readonly NodePlaceholderState nodeContext;
        private readonly CombatShellContextFactory combatShellContextFactory;
        private RunLifecycleState currentState;
        private CombatShellContext combatShellContext;
        private RunResult runResult;

        public RunLifecycleController(
            NodePlaceholderState nodeContext,
            CombatShellContextFactory combatShellContextFactory = null)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.combatShellContextFactory = combatShellContextFactory ?? new CombatShellContextFactory();
            currentState = RunLifecycleState.RunStart;
        }

        public NodePlaceholderState NodeContext => nodeContext;

        public RunLifecycleState CurrentState => currentState;

        public bool HasRunResult => runResult != null;

        public bool HasCombatContext => combatShellContext != null;

        public CombatShellContext CombatContext => combatShellContext ??
            throw new InvalidOperationException("Combat context is not available until a combat-compatible run enters the active state.");

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
            currentState = RunLifecycleState.RunActive;
            return true;
        }

        public bool TryResolveRun(RunResolutionState resolutionState)
        {
            if (currentState != RunLifecycleState.RunActive)
            {
                return false;
            }

            runResult = new RunResult(
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
    }
}
