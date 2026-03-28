using System;
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

        public void ConfigurePersistenceStorage(IPersistentGameStateStorage storage)
        {
            persistenceService = new SafeResumePersistenceService(
                storage ?? throw new ArgumentNullException(nameof(storage)));
        }

        public void ConfigureQuitService(IApplicationQuitService quitService)
        {
            applicationQuitService = quitService ?? throw new ArgumentNullException(nameof(quitService));
        }

        private void Awake()
        {
            persistenceService ??= new SafeResumePersistenceService(CreateDefaultPersistenceStorage());
            applicationQuitService ??= new ApplicationQuitService();
            worldMapFactory = new BootstrapWorldMapFactory();
            startupStateFactory = new BootstrapStartupStateFactory(persistenceService);
            worldContextTransitionService = new BootstrapWorldContextTransitionService(persistenceService);
            feedbackAudioHost = EnsureUiSystemFeedbackAudioHost();
            combatFeedbackAudioHost = EnsureCombatFeedbackAudioHost();
            musicAudioHost = EnsureMusicAudioHost();
            townServiceBackgroundResolver = new TownServiceBackgroundResolver();

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
                feedbackAudioHost.TryPlay);
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
                combatFeedbackAudioHost.TryPlay);
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
                feedbackSoundRequested: feedbackAudioHost.TryPlay);
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
            StartupEntryTarget entryTarget = worldContextTransitionService.PrepareStopSession(
                gameState,
                sessionContext,
                nodeId);
            Debug.Log($"Stopping session from town/service context at {nodeId}.");
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
                HandleQuitRequested);
        }

        private void HandleStartRequested()
        {
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

            ApplyStartupState(startupState);
            ShowWorldMap();
        }

        private void HandleQuitRequested()
        {
            applicationQuitService.RequestQuit();
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

        private static IPersistentGameStateStorage CreateDefaultPersistenceStorage()
        {
            string storagePath = Path.Combine(Application.persistentDataPath, "survivalon_game_state.json");
            return new FilePersistentGameStateStorage(storagePath);
        }
    }
}

