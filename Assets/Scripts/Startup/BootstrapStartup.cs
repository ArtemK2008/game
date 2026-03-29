using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Survivalon.Combat;
using Survivalon.Run;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.Towns;
using Survivalon.World;
using Survivalon.Core;

namespace Survivalon.Startup
{
    public sealed class BootstrapStartup : MonoBehaviour
    {
        private static readonly AccountWideProgressionEffectResolver ProgressionEffectResolver =
            new AccountWideProgressionEffectResolver();
        private WorldGraph worldGraph;
        private PersistentGameState gameState;
        private WorldNodeEntryFlowController nodeEntryFlowController;
        private SafeResumePersistenceService persistenceService;
        private BootstrapWorldMapFactory worldMapFactory;
        private BootstrapStartupStateFactory startupStateFactory;
        private BootstrapWorldContextTransitionService worldContextTransitionService;
        private SessionContextState sessionContext;
        private UiSystemFeedbackAudioHost feedbackAudioHost;
        private CombatFeedbackAudioHost combatFeedbackAudioHost;
        private MusicAudioHost musicAudioHost;
        private TownServiceBackgroundResolver townServiceBackgroundResolver;
        private IApplicationQuitService applicationQuitService;
        private UserSettingsPersistenceService userSettingsPersistenceService;
        private UserSettingsApplier userSettingsApplier;
        private IDisplaySettingsApplier displaySettingsApplier;
        private OfflineProgressClaimResolver offlineProgressClaimResolver;
        private OfflineProgressClaimService offlineProgressClaimService;
        private DateTimeOffset? configuredSessionStartUtc;
        private DateTimeOffset sessionStartUtc;
        private UserSettingsState currentUserSettings;
        private BootstrapStartupState pendingContinueStartupState;
        private OfflineProgressClaimState pendingOfflineProgressClaimState;

        public void ConfigurePersistenceStorage(IPersistentGameStateStorage storage)
        {
            persistenceService = CreatePersistenceService(
                storage ?? throw new ArgumentNullException(nameof(storage)));
        }

        public void ConfigureQuitService(IApplicationQuitService quitService)
        {
            applicationQuitService = quitService ?? throw new ArgumentNullException(nameof(quitService));
        }

        public void ConfigureUserSettingsStorage(IUserSettingsStorage storage)
        {
            userSettingsPersistenceService = new UserSettingsPersistenceService(
                storage ?? throw new ArgumentNullException(nameof(storage)));
        }

        public void ConfigureDisplaySettingsApplier(IDisplaySettingsApplier applier)
        {
            displaySettingsApplier = applier ?? throw new ArgumentNullException(nameof(applier));
        }

        public void ConfigureSessionStartUtc(DateTimeOffset startupUtcTime)
        {
            configuredSessionStartUtc = startupUtcTime;
        }

        private void Awake()
        {
            persistenceService ??= CreatePersistenceService(CreateDefaultPersistenceStorage());
            applicationQuitService ??= new ApplicationQuitService();
            userSettingsPersistenceService ??= new UserSettingsPersistenceService(CreateDefaultUserSettingsStorage());
            displaySettingsApplier ??= new UnityDisplaySettingsApplier();
            worldMapFactory = new BootstrapWorldMapFactory();
            startupStateFactory = new BootstrapStartupStateFactory(persistenceService);
            worldContextTransitionService = new BootstrapWorldContextTransitionService(persistenceService);
            feedbackAudioHost = EnsureUiSystemFeedbackAudioHost();
            combatFeedbackAudioHost = EnsureCombatFeedbackAudioHost();
            musicAudioHost = EnsureMusicAudioHost();
            townServiceBackgroundResolver = new TownServiceBackgroundResolver();
            userSettingsApplier = new UserSettingsApplier(
                feedbackAudioHost,
                combatFeedbackAudioHost,
                musicAudioHost,
                displaySettingsApplier);
            sessionStartUtc = configuredSessionStartUtc ?? DateTimeOffset.UtcNow;
            offlineProgressClaimResolver = new OfflineProgressClaimResolver(
                worldMapFactory.CreateWorldGraph(),
                () => sessionStartUtc);
            offlineProgressClaimService = new OfflineProgressClaimService(persistenceService);
            currentUserSettings = userSettingsPersistenceService.LoadOrDefault();
            userSettingsApplier.Apply(currentUserSettings);

            ShowMainMenu();
            Debug.Log("Bootstrap startup flow entered the compact main menu.");
        }

        private void ShowWorldMap()
        {
            musicAudioHost.SetContext(MusicContextId.Calm);
            SetOptionalScreenActive(FindOptionalScreen<StartupPlaceholderView>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<TownServiceScreen>(), false);

            WorldMapScreen worldMapScreen = EnsureWorldMapScreen();
            worldMapScreen.gameObject.SetActive(true);
            worldMapScreen.Show(
                worldGraph,
                gameState.WorldState,
                HandleNodeEntryRequested,
                sessionContext,
                gameState,
                ResolveWorldMapProgressionEffects(),
                new WorldMapBuildPreparationInteractionService(persistenceService),
                feedbackAudioHost.TryPlay,
                HandleWorldMapStopRequested,
                currentUserSettings,
                HandleSettingsChangedRequested);
        }

        private void ShowNodePlaceholder(NodePlaceholderState placeholderState)
        {
            SetOptionalScreenActive(FindOptionalScreen<StartupPlaceholderView>(), false);
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<TownServiceScreen>(), false);

            NodePlaceholderScreen nodePlaceholderScreen = EnsureNodePlaceholderScreen();
            nodePlaceholderScreen.gameObject.SetActive(true);
            nodePlaceholderScreen.Show(
                worldGraph,
                placeholderState,
                HandleReturnToWorldRequested,
                HandleStopSessionRequested,
                RunPersistentContext.FromGameState(gameState),
                HandleResolvedPostRunBoundaryReached,
                musicAudioHost.SetContext,
                feedbackAudioHost.TryPlay,
                combatFeedbackAudioHost.TryPlay,
                currentUserSettings,
                HandleSettingsChangedRequested);
        }

        private void ShowTownServiceScreen(NodePlaceholderState placeholderState)
        {
            musicAudioHost.SetContext(MusicContextId.Calm);
            SetOptionalScreenActive(FindOptionalScreen<StartupPlaceholderView>(), false);
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);

            TownServiceScreen townServiceScreen = EnsureTownServiceScreen();
            townServiceScreen.gameObject.SetActive(true);
            townServiceScreen.Show(
                placeholderState,
                gameState,
                townServiceBackgroundResolver.Resolve(placeholderState.TownServiceContext),
                () => HandleTownServiceReturnRequested(placeholderState.NodeId),
                () => HandleTownServiceStopRequested(placeholderState.NodeId),
                progressionInteractionService: new TownServiceProgressionInteractionService(persistenceService),
                conversionInteractionService: new TownServiceConversionInteractionService(persistenceService),
                buildPreparationInteractionService: new TownServiceBuildPreparationInteractionService(persistenceService),
                feedbackSoundRequested: feedbackAudioHost.TryPlay,
                settingsState: currentUserSettings,
                settingsChanged: HandleSettingsChangedRequested);
        }

        private void HandleNodeEntryRequested(NodeId nodeId)
        {
            if (!nodeEntryFlowController.TryEnterNode(nodeId, out NodePlaceholderState placeholderState))
            {
                Debug.LogWarning($"World map entry rejected for node '{nodeId}'.");
                return;
            }

            sessionContext.RecordNodeEntry(nodeId);
            Debug.Log($"Entered node flow for {nodeId}.");
            ShowEnteredNodeContext(placeholderState);
        }

        private void ShowEnteredNodeContext(NodePlaceholderState placeholderState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            if (placeholderState.TownServiceContext != null)
            {
                ShowTownServiceScreen(placeholderState);
                return;
            }

            ShowNodePlaceholder(placeholderState);
        }

        private void HandleReturnToWorldRequested(RunResult runResult)
        {
            StartupEntryTarget entryTarget = worldContextTransitionService.PrepareReturnToWorld(
                gameState,
                sessionContext,
                runResult.NodeId);
            Debug.Log($"Returning from post-run state to world map after {runResult.ResolutionState} on {runResult.NodeId}.");
            ShowStartupEntryTarget(entryTarget);
        }

        private void HandleStopSessionRequested(RunResult runResult)
        {
            StartupEntryTarget entryTarget = worldContextTransitionService.PrepareStopSession(
                gameState,
                sessionContext,
                runResult.NodeId);
            Debug.Log($"Stopping session from post-run state after {runResult.ResolutionState} on {runResult.NodeId}.");
            ShowStartupEntryTarget(entryTarget);
        }

        private void HandleResolvedPostRunBoundaryReached()
        {
            worldContextTransitionService.PersistResolvedPostRunBoundary(gameState);
        }

        private void HandleTownServiceReturnRequested(NodeId nodeId)
        {
            StartupEntryTarget entryTarget = worldContextTransitionService.PrepareReturnToWorld(
                gameState,
                sessionContext,
                nodeId);
            Debug.Log($"Returning from town/service context to world map at {nodeId}.");
            ShowStartupEntryTarget(entryTarget);
        }

        private void HandleTownServiceStopRequested(NodeId nodeId)
        {
            StartupEntryTarget entryTarget = worldContextTransitionService.PrepareStopSessionFromTownService(
                gameState,
                sessionContext,
                nodeId);
            Debug.Log($"Stopping session from town/service context at {nodeId}.");
            ShowStartupEntryTarget(entryTarget);
        }

        private void HandleWorldMapStopRequested()
        {
            NodeId currentContextNodeId = ResolveCurrentWorldContextNodeId();
            StartupEntryTarget entryTarget = worldContextTransitionService.PrepareStopSession(
                gameState,
                sessionContext,
                currentContextNodeId);
            Debug.Log($"Stopping session from world-map context at {currentContextNodeId}.");
            ShowStartupEntryTarget(entryTarget);
        }

        private void ShowStartupEntryTarget(StartupEntryTarget entryTarget)
        {
            if (entryTarget == StartupEntryTarget.WorldViewPlaceholder)
            {
                ShowWorldMap();
                return;
            }

            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            ClearPendingOfflineClaim();
            musicAudioHost.SetContext(MusicContextId.Calm);
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<TownServiceScreen>(), false);

            bool canContinue = startupStateFactory.TryCreateContinue(worldMapFactory, out _);
            StartupPlaceholderView placeholderView = EnsurePlaceholderView();
            placeholderView.gameObject.SetActive(true);
            placeholderView.ShowMainMenu(
                canContinue,
                HandleStartRequested,
                canContinue ? (Action)HandleContinueRequested : null,
                HandleQuitRequested,
                currentUserSettings,
                HandleSettingsChangedRequested);
        }

        private void HandleStartRequested()
        {
            ClearPendingOfflineClaim();
            ApplyStartupState(startupStateFactory.CreateFresh(worldMapFactory));
            ShowWorldMap();
        }

        private void HandleContinueRequested()
        {
            if (!startupStateFactory.TryCreateContinue(worldMapFactory, out BootstrapStartupState startupState))
            {
                ShowMainMenu();
                return;
            }

            if (offlineProgressClaimResolver.TryResolve(
                    startupState.GameState,
                    out OfflineProgressClaimState claimState))
            {
                pendingContinueStartupState = startupState;
                pendingOfflineProgressClaimState = claimState;
                ShowOfflineProgressClaim(claimState);
                return;
            }

            ApplyStartupState(startupState);
            ResumeContinueTarget();
        }

        private void HandleQuitRequested()
        {
            applicationQuitService.RequestQuit();
        }

        private void HandleSettingsChangedRequested(UserSettingsState settingsState)
        {
            currentUserSettings = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            userSettingsApplier.Apply(currentUserSettings);
            userSettingsPersistenceService.Save(currentUserSettings);
        }

        private void HandleOfflineClaimRequested()
        {
            if (pendingContinueStartupState == null || pendingOfflineProgressClaimState == null)
            {
                ShowMainMenu();
                return;
            }

            offlineProgressClaimService.Claim(
                pendingContinueStartupState.GameState,
                pendingOfflineProgressClaimState);

            BootstrapStartupState startupState = pendingContinueStartupState;
            ClearPendingOfflineClaim();
            ApplyStartupState(startupState);
            ResumeContinueTarget();
        }

        private void ApplyStartupState(BootstrapStartupState startupState)
        {
            if (startupState == null)
            {
                throw new ArgumentNullException(nameof(startupState));
            }

            worldGraph = startupState.WorldGraph;
            gameState = startupState.GameState;
            nodeEntryFlowController = startupState.NodeEntryFlowController;
            sessionContext = startupState.SessionContext;
        }

        private void ResumeContinueTarget()
        {
            if (gameState.SafeResumeState.HasSafeResumeTarget &&
                gameState.SafeResumeState.TargetType == SafeResumeTargetType.TownService &&
                TryResumeTownServiceContext(gameState.SafeResumeState.ResumeNodeId))
            {
                return;
            }

            ShowWorldMap();
        }

        private void ShowOfflineProgressClaim(OfflineProgressClaimState claimState)
        {
            musicAudioHost.SetContext(MusicContextId.Calm);
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<TownServiceScreen>(), false);

            StartupPlaceholderView placeholderView = EnsurePlaceholderView();
            placeholderView.gameObject.SetActive(true);
            placeholderView.ShowOfflineClaim(claimState, HandleOfflineClaimRequested);
        }

        private bool TryResumeTownServiceContext(NodeId nodeId)
        {
            try
            {
                if (!nodeEntryFlowController.TryEnterNode(nodeId, out NodePlaceholderState placeholderState))
                {
                    return false;
                }

                if (placeholderState.TownServiceContext == null)
                {
                    return false;
                }

                ShowEnteredNodeContext(placeholderState);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        private StartupPlaceholderView EnsurePlaceholderView()
        {
            StartupPlaceholderView existingPlaceholderView = GetComponentInChildren<StartupPlaceholderView>(true);
            if (existingPlaceholderView != null)
            {
                return existingPlaceholderView;
            }

            GameObject placeholderObject = new GameObject("StartupPlaceholder");
            placeholderObject.transform.SetParent(transform, false);

            return placeholderObject.AddComponent<StartupPlaceholderView>();
        }

        private WorldMapScreen EnsureWorldMapScreen()
        {
            WorldMapScreen existingWorldMapScreen = GetComponentInChildren<WorldMapScreen>(true);
            if (existingWorldMapScreen != null)
            {
                return existingWorldMapScreen;
            }

            GameObject worldMapObject = new GameObject("WorldMapScreen");
            worldMapObject.transform.SetParent(transform, false);

            return worldMapObject.AddComponent<WorldMapScreen>();
        }

        private NodePlaceholderScreen EnsureNodePlaceholderScreen()
        {
            NodePlaceholderScreen existingNodePlaceholderScreen = GetComponentInChildren<NodePlaceholderScreen>(true);
            if (existingNodePlaceholderScreen != null)
            {
                return existingNodePlaceholderScreen;
            }

            GameObject nodePlaceholderObject = new GameObject("NodePlaceholderScreen");
            nodePlaceholderObject.transform.SetParent(transform, false);

            return nodePlaceholderObject.AddComponent<NodePlaceholderScreen>();
        }

        private TownServiceScreen EnsureTownServiceScreen()
        {
            TownServiceScreen existingTownServiceScreen = GetComponentInChildren<TownServiceScreen>(true);
            if (existingTownServiceScreen != null)
            {
                return existingTownServiceScreen;
            }

            GameObject townServiceObject = new GameObject("TownServiceScreen");
            townServiceObject.transform.SetParent(transform, false);

            return townServiceObject.AddComponent<TownServiceScreen>();
        }

        private UiSystemFeedbackAudioHost EnsureUiSystemFeedbackAudioHost()
        {
            UiSystemFeedbackAudioHost existingFeedbackAudioHost =
                GetComponentInChildren<UiSystemFeedbackAudioHost>(true);
            if (existingFeedbackAudioHost != null)
            {
                return existingFeedbackAudioHost;
            }

            GameObject feedbackAudioObject = new GameObject("UiSystemFeedbackAudioHost");
            feedbackAudioObject.transform.SetParent(transform, false);
            return feedbackAudioObject.AddComponent<UiSystemFeedbackAudioHost>();
        }

        private CombatFeedbackAudioHost EnsureCombatFeedbackAudioHost()
        {
            CombatFeedbackAudioHost existingFeedbackAudioHost =
                GetComponentInChildren<CombatFeedbackAudioHost>(true);
            if (existingFeedbackAudioHost != null)
            {
                return existingFeedbackAudioHost;
            }

            GameObject feedbackAudioObject = new GameObject("CombatFeedbackAudioHost");
            feedbackAudioObject.transform.SetParent(transform, false);
            return feedbackAudioObject.AddComponent<CombatFeedbackAudioHost>();
        }

        private MusicAudioHost EnsureMusicAudioHost()
        {
            MusicAudioHost existingMusicAudioHost = GetComponentInChildren<MusicAudioHost>(true);
            if (existingMusicAudioHost != null)
            {
                return existingMusicAudioHost;
            }

            GameObject musicAudioObject = new GameObject("MusicAudioHost");
            musicAudioObject.transform.SetParent(transform, false);
            return musicAudioObject.AddComponent<MusicAudioHost>();
        }

        private static void SetOptionalScreenActive(Component component, bool isActive)
        {
            if (component == null)
            {
                return;
            }

            component.gameObject.SetActive(isActive);
        }

        private T FindOptionalScreen<T>() where T : Component
        {
            return GetComponentInChildren<T>(true);
        }

        private AccountWideProgressionEffectState ResolveWorldMapProgressionEffects()
        {
            return ProgressionEffectResolver.Resolve(gameState.ProgressionState);
        }

        private NodeId ResolveCurrentWorldContextNodeId()
        {
            if (gameState.WorldState.HasCurrentNode)
            {
                return gameState.WorldState.CurrentNodeId;
            }

            if (gameState.WorldState.HasLastSafeNode)
            {
                return gameState.WorldState.LastSafeNodeId;
            }

            throw new InvalidOperationException(
                "World-map system exit requires a current node or last safe node.");
        }

        private static IPersistentGameStateStorage CreateDefaultPersistenceStorage()
        {
            string storagePath = Path.Combine(Application.persistentDataPath, "survivalon_game_state.json");
            return new FilePersistentGameStateStorage(storagePath);
        }

        private void ClearPendingOfflineClaim()
        {
            pendingContinueStartupState = null;
            pendingOfflineProgressClaimState = null;
        }

        private static SafeResumePersistenceService CreatePersistenceService(IPersistentGameStateStorage storage)
        {
            BootstrapWorldMapFactory bootstrapWorldMapFactory = new BootstrapWorldMapFactory();
            return new SafeResumePersistenceService(
                storage,
                offlineProgressEligibilityResolver: new OfflineProgressEligibilityResolver(
                    bootstrapWorldMapFactory.CreateWorldGraph()));
        }

        private static IUserSettingsStorage CreateDefaultUserSettingsStorage()
        {
            string storagePath = Path.Combine(Application.persistentDataPath, "survivalon_user_settings.json");
            return new FileUserSettingsStorage(storagePath);
        }
    }
}

