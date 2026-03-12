using UnityEngine;

namespace Survivalon.Runtime
{
    public sealed class BootstrapStartup : MonoBehaviour
    {
        private WorldGraph worldGraph;
        private PersistentGameState gameState;
        private WorldNodeEntryFlowController nodeEntryFlowController;

        private void Awake()
        {
            BootstrapWorldMapFactory worldMapFactory = new BootstrapWorldMapFactory();
            worldGraph = worldMapFactory.CreateWorldGraph();
            gameState = worldMapFactory.CreateGameState();
            nodeEntryFlowController = new WorldNodeEntryFlowController(worldGraph, gameState.WorldState);
            GameStartupFlowResolver startupFlowResolver = new GameStartupFlowResolver();
            StartupEntryTarget entryTarget = startupFlowResolver.ResolveInitialEntryTarget(gameState.WorldState);

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
            worldMapScreen.Show(worldGraph, gameState.WorldState, HandleNodeEntryRequested);
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
                HandleStopSessionRequested);
        }

        private void HandleNodeEntryRequested(NodeId nodeId)
        {
            if (!nodeEntryFlowController.TryEnterNode(nodeId, out NodePlaceholderState placeholderState))
            {
                Debug.LogWarning($"World map entry rejected for node '{nodeId}'.");
                return;
            }

            Debug.Log($"Entered placeholder node flow for {nodeId}.");
            ShowNodePlaceholder(placeholderState);
        }

        private void HandleReturnToWorldRequested(RunResult runResult)
        {
            Debug.Log($"Returning from post-run state to world map after {runResult.ResolutionState} on {runResult.NodeId}.");
            ShowWorldMap();
        }

        private void HandleStopSessionRequested(RunResult runResult)
        {
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
    }
}
