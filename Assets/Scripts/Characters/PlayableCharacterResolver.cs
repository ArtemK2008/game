using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Characters
{
    /// <summary>
    /// Разрешает текущего играбельного персонажа из persistent state в runtime-профиль.
    /// </summary>
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

