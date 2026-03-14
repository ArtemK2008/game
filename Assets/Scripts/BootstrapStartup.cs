using System;
using System.IO;
using UnityEngine;

namespace Survivalon.Runtime
{
    public sealed class BootstrapStartup : MonoBehaviour
    {
        private WorldGraph worldGraph;
        private PersistentGameState gameState;
        private WorldNodeEntryFlowController nodeEntryFlowController;
        private SafeResumePersistenceService persistenceService;
        private SessionContextState sessionContext;

        public void ConfigurePersistenceStorage(IPersistentGameStateStorage storage)
        {
            persistenceService = new SafeResumePersistenceService(
                storage ?? throw new ArgumentNullException(nameof(storage)));
        }

        private void Awake()
        {
            BootstrapWorldMapFactory worldMapFactory = new BootstrapWorldMapFactory();
            worldGraph = worldMapFactory.CreateWorldGraph();
            persistenceService ??= new SafeResumePersistenceService(CreateDefaultPersistenceStorage());
            gameState = persistenceService.LoadOrCreate(worldMapFactory.CreateGameState());
            nodeEntryFlowController = new WorldNodeEntryFlowController(worldGraph, gameState.WorldState);
            sessionContext = new SessionContextState();
            sessionContext.SeedFromWorldState(gameState.WorldState);
            GameStartupFlowResolver startupFlowResolver = new GameStartupFlowResolver();
            StartupEntryTarget entryTarget = startupFlowResolver.ResolveInitialEntryTarget(gameState);

            if (entryTarget == StartupEntryTarget.WorldViewPlaceholder)
            {
                ShowWorldMap();
                Debug.Log($"Bootstrap startup flow entered {entryTarget}.");
                return;
            }

            StartupPlaceholderView placeholderView = EnsurePlaceholderView();
            placeholderView.Show(entryTarget);

            Debug.Log($"Bootstrap startup flow entered {entryTarget}.");
        }

        private void ShowWorldMap()
        {
            SetOptionalScreenActive(FindOptionalScreen<StartupPlaceholderView>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);

            WorldMapScreen worldMapScreen = EnsureWorldMapScreen();
            worldMapScreen.gameObject.SetActive(true);
            worldMapScreen.Show(
                worldGraph,
                gameState.WorldState,
                HandleNodeEntryRequested,
                sessionContext);
        }

        private void ShowNodePlaceholder(NodePlaceholderState placeholderState)
        {
            SetOptionalScreenActive(FindOptionalScreen<StartupPlaceholderView>(), false);
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);

            NodePlaceholderScreen nodePlaceholderScreen = EnsureNodePlaceholderScreen();
            nodePlaceholderScreen.gameObject.SetActive(true);
            nodePlaceholderScreen.Show(
                placeholderState,
                HandleReturnToWorldRequested,
                HandleStopSessionRequested,
                gameState.WorldState);
        }

        private void HandleNodeEntryRequested(NodeId nodeId)
        {
            if (!nodeEntryFlowController.TryEnterNode(nodeId, out NodePlaceholderState placeholderState))
            {
                Debug.LogWarning($"World map entry rejected for node '{nodeId}'.");
                return;
            }

            sessionContext.RecordNodeEntry(nodeId);
            Debug.Log($"Entered placeholder node flow for {nodeId}.");
            ShowNodePlaceholder(placeholderState);
        }

        private void HandleReturnToWorldRequested(RunResult runResult)
        {
            sessionContext.RecordRunReturned(runResult.NodeId);
            persistenceService.SaveResolvedWorldContext(gameState);
            Debug.Log($"Returning from post-run state to world map after {runResult.ResolutionState} on {runResult.NodeId}.");
            ShowWorldMap();
        }

        private void HandleStopSessionRequested(RunResult runResult)
        {
            sessionContext.RecordRunReturned(runResult.NodeId);
            persistenceService.SaveResolvedWorldContext(gameState);
            Debug.Log($"Stopping session from post-run state after {runResult.ResolutionState} on {runResult.NodeId}.");
            SetOptionalScreenActive(FindOptionalScreen<WorldMapScreen>(), false);
            SetOptionalScreenActive(FindOptionalScreen<NodePlaceholderScreen>(), false);

            StartupPlaceholderView placeholderView = EnsurePlaceholderView();
            placeholderView.gameObject.SetActive(true);
            placeholderView.Show(StartupEntryTarget.MainMenuPlaceholder);
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
