using UnityEngine;

namespace Survivalon.Core
{
    public sealed class UnityDisplaySettingsApplier : IDisplaySettingsApplier
    {
        public void Apply(bool useFullscreen)
        {
#if UNITY_EDITOR
            return;
#else
            Screen.fullScreenMode = useFullscreen
                ? FullScreenMode.FullScreenWindow
                : FullScreenMode.Windowed;
#endif
        }
    }
}
