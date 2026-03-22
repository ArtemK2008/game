using System;
using Survivalon.State.Persistence;

namespace Survivalon.Characters
{
    /// <summary>
    /// Разрешает простые runtime-эффекты прогрессии персонажа из persistent rank.
    /// </summary>
    public sealed class PlayableCharacterProgressionEffectResolver
    {
        private const float MaxHealthBonusPerRank = 5f;

        public float ResolveMaxHealthBonus(PersistentCharacterState characterState)
        {
            if (characterState == null)
            {
                throw new ArgumentNullException(nameof(characterState));
            }

            return characterState.ProgressionRank * MaxHealthBonusPerRank;
        }
    }
}
