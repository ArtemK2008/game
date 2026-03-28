namespace Survivalon.Core
{
    /// <summary>
    /// Resolves the prototype's minimal music-context split without adding broader music policy.
    /// </summary>
    public static class MusicContextResolver
    {
        public static MusicContextId ResolveForCombatShell(bool isCombatShellVisible)
        {
            return isCombatShellVisible
                ? MusicContextId.Gameplay
                : MusicContextId.Calm;
        }
    }
}
