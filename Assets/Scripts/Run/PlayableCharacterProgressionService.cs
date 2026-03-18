using System;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class PlayableCharacterProgressionService
    {
        public bool TryApplyResolvedRunProgression(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            PersistentCharacterState playableCharacterState)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (playableCharacterState == null)
            {
                throw new ArgumentNullException(nameof(playableCharacterState));
            }

            if (!nodeContext.UsesCombatShell || resolutionState != RunResolutionState.Succeeded)
            {
                return false;
            }

            playableCharacterState.IncreaseProgressionRank();
            return true;
        }
    }
}
