
namespace Survivalon.State.Persistence
{
    public interface IPersistentGameStateStorage
    {
        bool TryLoad(out PersistentGameState gameState);

        void Save(PersistentGameState gameState);
    }
}

