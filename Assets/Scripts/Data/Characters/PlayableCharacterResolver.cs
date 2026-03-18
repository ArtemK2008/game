using Survivalon.State.Persistence;

namespace Survivalon.Data.Characters
{
    public sealed class PlayableCharacterResolver
    {
        private readonly PlayableCharacterSelectionService selectionService;

        public PlayableCharacterResolver(PlayableCharacterSelectionService selectionService = null)
        {
            this.selectionService = selectionService ?? new PlayableCharacterSelectionService();
        }

        public PlayableCharacterProfile ResolveCurrent(PersistentGameState gameState)
        {
            return PlayableCharacterCatalog.Get(ResolveCurrentState(gameState).CharacterId);
        }

        public PersistentCharacterState ResolveCurrentState(PersistentGameState gameState)
        {
            return selectionService.ResolveSelectedState(gameState);
        }
    }
}

