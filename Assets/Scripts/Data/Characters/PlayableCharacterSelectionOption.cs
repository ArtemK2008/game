using System;

namespace Survivalon.Data.Characters
{
    public sealed class PlayableCharacterSelectionOption
    {
        public PlayableCharacterSelectionOption(
            string characterId,
            string displayName,
            bool isSelected)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            CharacterId = characterId;
            DisplayName = displayName;
            IsSelected = isSelected;
        }

        public string CharacterId { get; }

        public string DisplayName { get; }

        public bool IsSelected { get; }
    }
}
