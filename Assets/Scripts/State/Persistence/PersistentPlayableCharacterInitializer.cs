using System;
using Survivalon.Data.Characters;

namespace Survivalon.State.Persistence
{
    public sealed class PersistentPlayableCharacterInitializer
    {
        private readonly PlayableCharacterSelectionService selectionService;

        public PersistentPlayableCharacterInitializer(PlayableCharacterSelectionService selectionService = null)
        {
            this.selectionService = selectionService ?? new PlayableCharacterSelectionService();
        }

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
                    isActive: false,
                    skillPackageId: defaultCharacter.DefaultSkillPackageId);
                gameState.AddCharacterState(characterState);
            }

            characterState.Unlock();
            characterState.SetSelectable(true);
            selectionService.EnsureValidSelection(gameState);
        }
    }
}

