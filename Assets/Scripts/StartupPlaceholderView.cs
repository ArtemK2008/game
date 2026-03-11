using UnityEngine;

namespace Survivalon.Runtime
{
    public sealed class StartupPlaceholderView : MonoBehaviour
    {
        private StartupEntryTarget activeTarget;

        public StartupEntryTarget ActiveTarget => activeTarget;

        public void Show(StartupEntryTarget target)
        {
            activeTarget = target;
            gameObject.name = target.ToString();

            Debug.Log($"Startup placeholder active: {activeTarget}.");
        }
    }
}
