using UnityEngine;

namespace Survivalon.Runtime
{
    public sealed class BootstrapStartup : MonoBehaviour
    {
        private void Awake()
        {
            BootstrapWorldMapFactory worldMapFactory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = worldMapFactory.CreateWorldGraph();
            PersistentGameState gameState = worldMapFactory.CreateGameState();
            GameStartupFlowResolver startupFlowResolver = new GameStartupFlowResolver();
            StartupEntryTarget entryTarget = startupFlowResolver.ResolveInitialEntryTarget(gameState.WorldState);

            if (entryTarget == StartupEntryTarget.WorldViewPlaceholder)
            {
                WorldMapScreen worldMapScreen = EnsureWorldMapScreen();
                worldMapScreen.Show(worldGraph, gameState.WorldState);
                Debug.Log($"Bootstrap startup flow entered {entryTarget}.");
                return;
            }

            StartupPlaceholderView placeholderView = EnsurePlaceholderView();
            placeholderView.Show(entryTarget);

            Debug.Log($"Bootstrap startup flow entered {entryTarget}.");
        }

        private StartupPlaceholderView EnsurePlaceholderView()
        {
            StartupPlaceholderView existingPlaceholderView = GetComponentInChildren<StartupPlaceholderView>();
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
            WorldMapScreen existingWorldMapScreen = GetComponentInChildren<WorldMapScreen>();
            if (existingWorldMapScreen != null)
            {
                return existingWorldMapScreen;
            }

            GameObject worldMapObject = new GameObject("WorldMapScreen");
            worldMapObject.transform.SetParent(transform, false);

            return worldMapObject.AddComponent<WorldMapScreen>();
        }
    }
}
