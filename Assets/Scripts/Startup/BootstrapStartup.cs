using System;
using System.IO;
using UnityEngine;
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
        private WorldGraph worldGraph;
        private PersistentGameState gameState;
        private WorldNodeEntryFlowController nodeEntryFlowController;
        private SafeResumePersistenceService persistenceService;
        private BootstrapWorldContextTransitionService worldContextTransitionService;
        private SessionContextState sessionContext;

        public void ConfigurePersistenceStorage(IPersistentGameStateStorage storage)
        {
            persistenceService = new SafeResumePersistenceService(
                storage ?? throw new ArgumentNullException(nameof(storage)));
        }

        private void Awake()
        {
            persistenceService ??= new SafeResumePersistenceService(CreateDefaultPersistenceStorage());
            worldContextTransitionService = new BootstrapWorldContextTransitionService(persistenceService);

            BootstrapStartupState startupState = new BootstrapStartupStateFactory(persistenceService)
                .Create(new BootstrapWorldMapFactory());
            worldGraph = startupState.WorldGraph;
            gameState = startupState.GameState;
            nodeEntryFlowController = startupState.NodeEntryFlowController;
            sessionContext = startupState.SessionContext;

            ShowStartupEntryTarget(startupState.EntryTarget);
            Debug.Log($"Bootstrap startup flow entered {startupState.EntryTarget}.");
        }

        private void ShowWorldMap()
        {
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
                gameState);
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
                RunPersistentContext.FromGameState(gameState));
        }

        private void ShowTownServiceScreen(NodePlaceholderState placeholderState)
        {
            SetOptionalScreenActive(FindOptionalScreen<StartupPlaceholderView>(), false);
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);

            TownServiceScreen townServiceScreen = EnsureTownServiceScreen();
            townServiceScreen.gameObject.SetActive(true);
            townServiceScreen.Show(
                placeholderState,
                gameState,
                () => HandleTownServiceReturnRequested(placeholderState.NodeId),
                () => HandleTownServiceStopRequested(placeholderState.NodeId),
                progressionInteractionService: new TownServiceProgressionInteractionService(persistenceService));
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

            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<TownServiceScreen>(), false);

            StartupPlaceholderView placeholderView = EnsurePlaceholderView();
            placeholderView.gameObject.SetActive(true);
            placeholderView.Show(entryTarget);
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

        private static IPersistentGameStateStorage CreateDefaultPersistenceStorage()
        {
            string storagePath = Path.Combine(Application.persistentDataPath, "survivalon_game_state.json");
            return new FilePersistentGameStateStorage(storagePath);
        }
    }
}

