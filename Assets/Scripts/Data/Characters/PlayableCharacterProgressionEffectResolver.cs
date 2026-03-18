using System;
using Survivalon.State.Persistence;

namespace Survivalon.Data.Characters
{
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
