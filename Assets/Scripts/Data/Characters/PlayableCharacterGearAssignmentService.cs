using System;
using System.Collections.Generic;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.Data.Characters
{
    public sealed class PlayableCharacterGearAssignmentService
    {
        private readonly PlayableCharacterSelectionService selectionService;

        public PlayableCharacterGearAssignmentService(PlayableCharacterSelectionService selectionService = null)
        {
            this.selectionService = selectionService ?? new PlayableCharacterSelectionService();
        }

        public IReadOnlyList<PlayableCharacterGearAssignmentOption> BuildOptionsForSelectedCharacter(
            PersistentGameState gameState,
            GearCategory gearCategory)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedCharacterState = selectionService.ResolveSelectedState(gameState);
            return BuildOwnedOptionsForCharacterAndCategory(
                gameState,
                selectedCharacterState,
                gearCategory);
        }

        public bool TryAssignSelectedCharacterGear(PersistentGameState gameState, string gearId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            if (!GearCatalog.Contains(gearId) || !OwnsGearId(gameState, gearId))
            {
                return false;
            }

            GearProfile gearProfile = GearCatalog.Get(gearId);
            PersistentCharacterState selectedCharacterState = selectionService.ResolveSelectedState(gameState);
            selectedCharacterState.LoadoutState.SetEquippedGearState(
                new EquippedGearState(gearProfile.GearId, gearProfile.GearCategory));
            return true;
        }

        public bool TryClearSelectedCharacterGear(PersistentGameState gameState, GearCategory gearCategory)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedCharacterState = selectionService.ResolveSelectedState(gameState);
            return selectedCharacterState.LoadoutState.ClearEquippedGearState(gearCategory);
        }

        private static bool OwnsGearId(PersistentGameState gameState, string gearId)
        {
            for (int index = 0; index < gameState.OwnedGearIds.Count; index++)
            {
                if (gameState.OwnedGearIds[index] == gearId)
                {
                    return true;
                }
            }

            return false;
        }

        private static IReadOnlyList<PlayableCharacterGearAssignmentOption> BuildOwnedOptionsForCharacterAndCategory(
            PersistentGameState gameState,
            PersistentCharacterState characterState,
            GearCategory gearCategory)
        {
            List<PlayableCharacterGearAssignmentOption> gearOptions =
                new List<PlayableCharacterGearAssignmentOption>();
            string equippedGearId = ResolveEquippedGearId(characterState, gearCategory);

            for (int index = 0; index < GearCatalog.All.Count; index++)
            {
                GearProfile gearProfile = GearCatalog.All[index];
                if (gearProfile.GearCategory != gearCategory || !OwnsGearId(gameState, gearProfile.GearId))
                {
                    continue;
                }

                gearOptions.Add(new PlayableCharacterGearAssignmentOption(
                    characterState.CharacterId,
                    gearProfile.GearId,
                    gearProfile.DisplayName,
                    gearProfile.GearCategory,
                    gearProfile.GearId == equippedGearId));
            }

            return gearOptions;
        }

        private static string ResolveEquippedGearId(
            PersistentCharacterState characterState,
            GearCategory gearCategory)
        {
            if (!characterState.LoadoutState.TryGetEquippedGearState(
                gearCategory,
                out EquippedGearState equippedGearState))
            {
                return null;
            }

            return equippedGearState.GearId;
        }
    }
}
