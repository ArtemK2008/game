using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivalon.Runtime
{
    [Serializable]
    public sealed class PersistentGameState
    {
        [SerializeField]
        private PersistentWorldState worldState = new PersistentWorldState();

        [SerializeField]
        private PersistentProgressionState progressionState = new PersistentProgressionState();

        [SerializeField]
        private ResourceBalancesState resourceBalances = new ResourceBalancesState();

        [SerializeField]
        private List<PersistentCharacterState> characterStates = new List<PersistentCharacterState>();

        [SerializeField]
        private List<string> ownedGearIds = new List<string>();

        [SerializeField]
        private PersistentSafeResumeState safeResumeState = new PersistentSafeResumeState();

        public PersistentWorldState WorldState => worldState;

        public PersistentProgressionState ProgressionState => progressionState;

        public ResourceBalancesState ResourceBalances => resourceBalances;

        public IReadOnlyList<PersistentCharacterState> CharacterStates => characterStates;

        public IReadOnlyList<string> OwnedGearIds => ownedGearIds;

        public PersistentSafeResumeState SafeResumeState => safeResumeState;

        public void AddCharacterState(PersistentCharacterState characterState)
        {
            if (characterState == null)
            {
                throw new ArgumentNullException(nameof(characterState));
            }

            if (TryGetCharacterState(characterState.CharacterId, out _))
            {
                throw new InvalidOperationException($"Character state '{characterState.CharacterId}' already exists.");
            }

            characterStates.Add(characterState);
        }

        public bool TryGetCharacterState(string characterId, out PersistentCharacterState characterState)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            for (int index = 0; index < characterStates.Count; index++)
            {
                if (characterStates[index].CharacterId == characterId)
                {
                    characterState = characterStates[index];
                    return true;
                }
            }

            characterState = null;
            return false;
        }
    }
}
