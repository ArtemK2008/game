using System;
using System.Collections.Generic;
using Survivalon.Combat;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class RunLifecycleController
    {
        private readonly NodePlaceholderState nodeContext;
        private readonly WorldGraph worldGraph;
        private readonly RunPersistentContext persistentContext;
        private readonly CombatShellContextFactory combatShellContextFactory;
        private readonly PlayableCharacterCombatSkillResolver playableCharacterCombatSkillResolver;
        private readonly AccountWideProgressionEffectResolver accountWideProgressionEffectResolver;
        private readonly CombatEncounterResolver combatEncounterResolver;
        private readonly CombatAutoAdvanceLoop combatAutoAdvanceLoop;
        private readonly RunProgressResolutionService runProgressResolutionService;
        private readonly RunRewardResolutionService runRewardResolutionService;
        private readonly RunRewardGrantService runRewardGrantService;
        private readonly PlayableCharacterProgressionService playableCharacterProgressionService;
        private readonly RunTimeSkillUpgradeAutoPickResolver runTimeSkillUpgradeAutoPickResolver;
        private readonly IReadOnlyList<CombatRunTimeSkillUpgradeOption> runTimeSkillUpgradeOptions;
        private RunLifecycleState currentState;
        private CombatRunTimeSkillUpgradeOption selectedRunTimeSkillUpgradeOption;
        private CombatShellContext combatShellContext;
        private CombatEncounterState combatEncounterState;
        private RunResult runResult;

        public RunLifecycleController(
            NodePlaceholderState nodeContext,
            WorldGraph worldGraph = null,
            CombatShellContextFactory combatShellContextFactory = null,
            CombatEncounterResolver combatEncounterResolver = null,
            CombatAutoAdvanceLoop combatAutoAdvanceLoop = null,
            RunPersistentContext persistentContext = null,
            NodeProgressMeterService nodeProgressMeterService = null,
            NextNodeUnlockService nextNodeUnlockService = null,
            RunRewardResolutionService runRewardResolutionService = null,
            RunRewardGrantService runRewardGrantService = null,
            AccountWideProgressionEffectResolver accountWideProgressionEffectResolver = null,
            PlayableCharacterProgressionService playableCharacterProgressionService = null,
            PlayableCharacterCombatSkillResolver playableCharacterCombatSkillResolver = null,
            RunTimeSkillUpgradeAutoPickResolver runTimeSkillUpgradeAutoPickResolver = null)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.worldGraph = worldGraph;
            this.combatShellContextFactory = combatShellContextFactory ?? new CombatShellContextFactory();
            this.playableCharacterCombatSkillResolver =
                playableCharacterCombatSkillResolver ?? new PlayableCharacterCombatSkillResolver();
            this.combatEncounterResolver = combatEncounterResolver ?? new CombatEncounterResolver();
            this.combatAutoAdvanceLoop = combatAutoAdvanceLoop ?? new CombatAutoAdvanceLoop();
            this.persistentContext = persistentContext;
            this.accountWideProgressionEffectResolver = accountWideProgressionEffectResolver
                ?? new AccountWideProgressionEffectResolver();
            runProgressResolutionService = new RunProgressResolutionService(
                nodeProgressMeterService,
                nextNodeUnlockService);
            this.runRewardResolutionService = runRewardResolutionService ?? new RunRewardResolutionService();
            this.runRewardGrantService = runRewardGrantService ?? new RunRewardGrantService();
            this.playableCharacterProgressionService =
                playableCharacterProgressionService ?? new PlayableCharacterProgressionService();
            this.runTimeSkillUpgradeAutoPickResolver =
                runTimeSkillUpgradeAutoPickResolver ?? new RunTimeSkillUpgradeAutoPickResolver();
            currentState = RunLifecycleState.RunStart;
            runTimeSkillUpgradeOptions = ResolveRunTimeSkillUpgradeOptions();
        }

        public NodePlaceholderState NodeContext => nodeContext;

        public RunLifecycleState CurrentState => currentState;

        public bool HasRunResult => runResult != null;

        public bool HasCombatEncounterState => combatEncounterState != null;

        public bool UsesCombatShell => nodeContext.UsesCombatShell;

        public IReadOnlyList<CombatRunTimeSkillUpgradeOption> RunTimeSkillUpgradeOptions => runTimeSkillUpgradeOptions;

        public bool RequiresRunTimeSkillUpgradeChoice =>
            currentState == RunLifecycleState.RunStart &&
            runTimeSkillUpgradeOptions.Count > 0 &&
            selectedRunTimeSkillUpgradeOption == null;

        public CombatShellContext CombatContext => combatShellContext ??
            throw new InvalidOperationException("Combat context is not available until a combat-compatible run enters the active state.");

        public CombatEncounterState CombatEncounterState => combatEncounterState ??
            throw new InvalidOperationException("Combat encounter state is not available until a combat-compatible run enters the active state.");

        public RunResult RunResult => runResult ??
            throw new InvalidOperationException("Run result is not available until the run is resolved.");

        public bool TryEnterActiveState()
        {
            if (currentState != RunLifecycleState.RunStart || RequiresRunTimeSkillUpgradeChoice)
            {
                return false;
            }

            combatAutoAdvanceLoop.Reset();
            combatShellContext = nodeContext.UsesCombatShell
                ? CreateCombatShellContext()
                : null;
            combatEncounterState = combatShellContext == null
                ? null
                : new CombatEncounterState(combatShellContext);
            currentState = RunLifecycleState.RunActive;
            return true;
        }

        public bool TrySelectRunTimeSkillUpgrade(string runTimeSkillUpgradeId)
        {
            if (!RequiresRunTimeSkillUpgradeChoice || string.IsNullOrWhiteSpace(runTimeSkillUpgradeId))
            {
                return false;
            }

            for (int index = 0; index < runTimeSkillUpgradeOptions.Count; index++)
            {
                CombatRunTimeSkillUpgradeOption upgradeOption = runTimeSkillUpgradeOptions[index];
                if (upgradeOption.UpgradeId != runTimeSkillUpgradeId)
                {
                    continue;
                }

                selectedRunTimeSkillUpgradeOption = upgradeOption;
                return true;
            }

            return false;
        }

        public bool TryAdvanceTime(float elapsedSeconds)
        {
            return combatAutoAdvanceLoop.TryAdvance(this, elapsedSeconds);
        }

        public bool TryStartAutomaticFlow()
        {
            if (!UsesCombatShell || currentState != RunLifecycleState.RunStart)
            {
                return false;
            }

            AutoSelectBaselineRunTimeSkillUpgradeForAutomaticFlow();
            return TryEnterActiveState();
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
            AccountWideProgressionEffectState progressionEffects = ResolveAccountWideProgressionEffects();
            RunProgressResolution progressResolution = runProgressResolutionService.Resolve(
                nodeContext,
                resolutionState,
                combatEncounterState,
                persistentContext?.PersistentWorldState,
                worldGraph);
            RunRewardPayload rewardPayload = runRewardResolutionService.Resolve(
                nodeContext,
                resolutionState,
                worldGraph,
                progressResolution,
                progressionEffects);

            if (persistentContext?.ResourceBalancesState != null)
            {
                runRewardGrantService.Grant(persistentContext.ResourceBalancesState, rewardPayload);
            }

            ApplyPlayableCharacterProgression(resolutionState);
            return RunResultFactory.Create(nodeContext, resolutionState, progressResolution, rewardPayload);
        }

        private CombatShellContext CreateCombatShellContext()
        {
            return combatShellContextFactory.Create(
                nodeContext,
                persistentContext?.PlayableCharacter,
                persistentContext?.PlayableCharacterState,
                ResolveAccountWideProgressionEffects(),
                selectedRunTimeSkillUpgradeOption);
        }

        private void ApplyPlayableCharacterProgression(RunResolutionState resolutionState)
        {
            if (persistentContext?.PlayableCharacterState == null)
            {
                return;
            }

            playableCharacterProgressionService.TryApplyResolvedRunProgression(
                nodeContext,
                resolutionState,
                persistentContext.PlayableCharacterState);
        }

        private AccountWideProgressionEffectState ResolveAccountWideProgressionEffects()
        {
            return persistentContext?.PersistentProgressionState == null
                ? default
                : accountWideProgressionEffectResolver.Resolve(persistentContext.PersistentProgressionState);
        }

        private void AutoSelectBaselineRunTimeSkillUpgradeForAutomaticFlow()
        {
            if (selectedRunTimeSkillUpgradeOption != null || runTimeSkillUpgradeOptions.Count == 0)
            {
                return;
            }

            selectedRunTimeSkillUpgradeOption =
                runTimeSkillUpgradeAutoPickResolver.ResolveAutomaticFlowSelection(runTimeSkillUpgradeOptions);
        }

        private IReadOnlyList<CombatRunTimeSkillUpgradeOption> ResolveRunTimeSkillUpgradeOptions()
        {
            if (!UsesCombatShell)
            {
                return Array.Empty<CombatRunTimeSkillUpgradeOption>();
            }

            CombatSkillDefinition triggeredActiveSkill =
                playableCharacterCombatSkillResolver.ResolveTriggeredActiveSkill(
                    persistentContext?.PlayableCharacter,
                    persistentContext?.PlayableCharacterState);

            return CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(triggeredActiveSkill);
        }
    }
}

