using System;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.Combat
{
    public sealed class PlayableCharacterGearCombatEffectResolver
    {
        public float ResolveAttackPowerBonus(PersistentCharacterState characterState)
        {
            if (characterState == null)
            {
                throw new ArgumentNullException(nameof(characterState));
            }

            float attackPowerBonus = 0f;
            for (int index = 0; index < characterState.LoadoutState.EquippedGearStates.Count; index++)
            {
                EquippedGearState equippedGearState = characterState.LoadoutState.EquippedGearStates[index];
                if (equippedGearState == null || !GearCatalog.Contains(equippedGearState.GearId))
                {
                    continue;
                }

                attackPowerBonus += GearCatalog.Get(equippedGearState.GearId).AttackPowerBonus;
            }

            return attackPowerBonus;
        }

        public float ResolveMaxHealthBonus(PersistentCharacterState characterState)
        {
            if (characterState == null)
            {
                throw new ArgumentNullException(nameof(characterState));
            }

            float maxHealthBonus = 0f;
            for (int index = 0; index < characterState.LoadoutState.EquippedGearStates.Count; index++)
            {
                EquippedGearState equippedGearState = characterState.LoadoutState.EquippedGearStates[index];
                if (equippedGearState == null || !GearCatalog.Contains(equippedGearState.GearId))
                {
                    continue;
                }

                maxHealthBonus += GearCatalog.Get(equippedGearState.GearId).MaxHealthBonus;
            }

            return maxHealthBonus;
        }
    }
}
