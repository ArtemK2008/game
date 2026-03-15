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
        private BootstrapPostRunTransitionService postRunTransitionService;
        private SessionContextState sessionContext;

        public void ConfigurePersistenceStorage(IPersistentGameStateStorage storage)
        {
            persistenceService = new SafeResumePersistenceService(
                storage ?? throw new ArgumentNullException(nameof(storage)));
        }

        private void Awake()
        {
            persistenceService ??= new SafeResumePersistenceService(CreateDefaultPersistenceStorage());
            postRunTransitionService = new BootstrapPostRunTransitionService(persistenceService);

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
                worldGraph,
                placeholderState,
                HandleReturnToWorldRequested,
                HandleStopSessionRequested,
                RunPersistentContext.FromGameState(gameState));
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
            StartupEntryTarget entryTarget = postRunTransitionService.PrepareReturnToWorld(
                gameState,
                sessionContext,
                runResult);
            Debug.Log($"Returning from post-run state to world map after {runResult.ResolutionState} on {runResult.NodeId}.");
            ShowStartupEntryTarget(entryTarget);
        }

        private void HandleStopSessionRequested(RunResult runResult)
        {
            StartupEntryTarget entryTarget = postRunTransitionService.PrepareStopSession(
                gameState,
                sessionContext,
                runResult);
            Debug.Log($"Stopping session from post-run state after {runResult.ResolutionState} on {runResult.NodeId}.");
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
