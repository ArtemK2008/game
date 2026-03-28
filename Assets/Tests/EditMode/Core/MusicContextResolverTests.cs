using NUnit.Framework;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Verifies the minimal calm/gameplay music-context split used by the prototype.
    /// </summary>
    public sealed class MusicContextResolverTests
    {
        [Test]
        public void ResolveForCombatShell_ShouldReturnGameplayWhenCombatShellIsVisible()
        {
            Assert.That(
                MusicContextResolver.ResolveForCombatShell(isCombatShellVisible: true),
                Is.EqualTo(MusicContextId.Gameplay));
        }

        [Test]
        public void ResolveForCombatShell_ShouldReturnCalmWhenCombatShellIsNotVisible()
        {
            Assert.That(
                MusicContextResolver.ResolveForCombatShell(isCombatShellVisible: false),
                Is.EqualTo(MusicContextId.Calm));
        }
    }
}
