using UnityEngine;

namespace Survivalon.Runtime
{
    public sealed class BootstrapStartup : MonoBehaviour
    {
        private void Awake()
        {
            PersistentGameState gameState = new PersistentGameState();
            GameStartupFlowResolver startupFlowResolver = new GameStartupFlowResolver();
            StartupEntryTarget entryTarget = startupFlowResolver.ResolveInitialEntryTarget(gameState.WorldState);

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
    }
}
