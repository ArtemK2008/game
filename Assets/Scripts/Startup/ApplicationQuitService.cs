using UnityEngine;

namespace Survivalon.Startup
{
    public sealed class ApplicationQuitService : IApplicationQuitService
    {
        public void RequestQuit()
        {
#if UNITY_EDITOR
            Debug.Log("Quit requested while running in the Unity Editor.");
#else
            Application.Quit();
#endif
        }
    }
}
