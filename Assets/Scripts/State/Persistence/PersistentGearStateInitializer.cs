using System;
using System.Collections.Generic;
using Survivalon.Data.Gear;

namespace Survivalon.State.Persistence
{
    public sealed class PersistentGearStateInitializer
    {
        private static readonly string[] StarterOwnedGearIds =
        {
            GearIds.TrainingBlade,
            GearIds.GuardCharm,
        };

        public void EnsureInitialized(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            NormalizeOwnedGearIds(gameState);
            NormalizeCharacterLoadouts(gameState);
        }

        private static void NormalizeOwnedGearIds(PersistentGameState gameState)
        {
            List<string> normalizedOwnedGearIds = new List<string>();
            HashSet<string> seenGearIds = new HashSet<string>();
            IReadOnlyList<string> ownedGearIds = gameState.OwnedGearIds;

            for (int index = 0; index < ownedGearIds.Count; index++)
            {
                string ownedGearId = ownedGearIds[index];
                if (string.IsNullOrWhiteSpace(ownedGearId))
                {
                    continue;
                }

                if (seenGearIds.Add(ownedGearId))
                {
                    normalizedOwnedGearIds.Add(ownedGearId);
                }
            }

            for (int index = 0; index < StarterOwnedGearIds.Length; index++)
            {
                string shippedGearId = StarterOwnedGearIds[index];
                if (seenGearIds.Add(shippedGearId))
                {
                    normalizedOwnedGearIds.Add(shippedGearId);
                }
            }

            gameState.ReplaceOwnedGearIds(normalizedOwnedGearIds);
        }

        private static void NormalizeCharacterLoadouts(PersistentGameState gameState)
        {
            HashSet<string> ownedGearIds = new HashSet<string>(gameState.OwnedGearIds);

            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                if (characterState == null)
                {
                    continue;
                }

                NormalizeEquippedGearStates(characterState.LoadoutState, ownedGearIds);
            }
        }

        private static void NormalizeEquippedGearStates(
            PersistentLoadoutState loadoutState,
            HashSet<string> ownedGearIds)
        {
            List<EquippedGearState> normalizedEquippedGearStates = new List<EquippedGearState>();
            HashSet<GearCategory> occupiedCategories = new HashSet<GearCategory>();
            IReadOnlyList<EquippedGearState> equippedGearStates = loadoutState.EquippedGearStates;

            for (int index = 0; index < equippedGearStates.Count; index++)
            {
                EquippedGearState equippedGearState = equippedGearStates[index];
                if (equippedGearState == null)
                {
                    continue;
                }

                if (!ownedGearIds.Contains(equippedGearState.GearId))
                {
                    continue;
                }

                if (!GearCatalog.Contains(equippedGearState.GearId))
                {
                    continue;
                }

                GearProfile gearProfile = GearCatalog.Get(equippedGearState.GearId);
                if (gearProfile.GearCategory != equippedGearState.GearCategory)
                {
                    continue;
                }

                if (!occupiedCategories.Add(equippedGearState.GearCategory))
                {
                    continue;
                }

                normalizedEquippedGearStates.Add(
                    new EquippedGearState(
                        equippedGearState.GearId,
                        equippedGearState.GearCategory));
            }

            loadoutState.ReplaceEquippedGearStates(normalizedEquippedGearStates);
        }
    }
}
