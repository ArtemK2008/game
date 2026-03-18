
namespace Survivalon.Combat
{
    /// <summary>
    /// Итоговое состояние боевого столкновения.
    /// Используется для фиксации факта незавершённого боя или победы одной из сторон.
    /// </summary>
    public enum CombatEncounterOutcome
    {
        None = 0,
        PlayerVictory = 1,
        EnemyVictory = 2,
    }
}

