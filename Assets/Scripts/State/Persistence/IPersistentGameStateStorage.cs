using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
{
    public interface IPersistentGameStateStorage
    {
        bool TryLoad(out PersistentGameState gameState);

        void Save(PersistentGameState gameState);
    }
}
