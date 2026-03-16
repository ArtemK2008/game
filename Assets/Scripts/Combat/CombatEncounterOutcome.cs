using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Runtime.Run;

namespace Survivalon.Runtime.Combat
{
    public enum CombatEncounterOutcome
    {
        None = 0,
        PlayerVictory = 1,
        EnemyVictory = 2,
    }
}
