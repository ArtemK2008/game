using UnityEngine;

namespace Survivalon.Runtime
{
    public sealed class BootstrapStartup : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("Bootstrap startup flow initialized.");
        }
    }
}
