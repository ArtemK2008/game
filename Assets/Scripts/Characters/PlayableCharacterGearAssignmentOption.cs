using System;
using Survivalon.Data.Gear;

namespace Survivalon.Characters
{
    /// <summary>
    /// Описывает runtime-вариант экипировки для выбранного персонажа и категории gear.
    /// </summary>
    public sealed class PlayableCharacterGearAssignmentOption
    {
        public PlayableCharacterGearAssignmentOption(
            string characterId,
            string gearId,
            string displayName,
            GearCategory gearCategory,
            bool isEquipped)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            CharacterId = characterId;
            GearId = gearId;
            DisplayName = displayName;
            GearCategory = gearCategory;
            IsEquipped = isEquipped;
        }

        public string CharacterId { get; }

        public string GearId { get; }

        public string DisplayName { get; }

        public GearCategory GearCategory { get; }

        public bool IsEquipped { get; }
    }
}
