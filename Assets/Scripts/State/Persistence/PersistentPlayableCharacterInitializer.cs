using System;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
{
    public sealed class PersistentPlayableCharacterInitializer
    {
        public void EnsureInitialized(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PlayableCharacterProfile defaultCharacter = PlayableCharacterCatalog.Default;
            if (!gameState.TryGetCharacterState(defaultCharacter.CharacterId, out PersistentCharacterState characterState))
            {
                characterState = new PersistentCharacterState(
                    defaultCharacter.CharacterId,
                    isUnlocked: true,
                    isSelectable: true,
                    isActive: true,
                    skillPackageId: defaultCharacter.DefaultSkillPackageId);
                gameState.AddCharacterState(characterState);
                return;
            }

            characterState.Unlock();
            characterState.SetSelectable(true);
            characterState.SetActive(true);
        }
    }
}
