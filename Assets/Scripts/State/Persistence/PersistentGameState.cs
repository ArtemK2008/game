using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivalon.State.Persistence
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

        private List<string> OwnedGearIdsInternal => ownedGearIds ??= new List<string>();

        public PersistentWorldState WorldState => worldState;

        public PersistentProgressionState ProgressionState => progressionState;

        public ResourceBalancesState ResourceBalances => resourceBalances;

        public IReadOnlyList<PersistentCharacterState> CharacterStates => characterStates;

        public IReadOnlyList<string> OwnedGearIds => OwnedGearIdsInternal;

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

        public void EnsureOwnedGearId(string gearId)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            for (int index = 0; index < OwnedGearIdsInternal.Count; index++)
            {
                if (OwnedGearIdsInternal[index] == gearId)
                {
                    return;
                }
            }

            OwnedGearIdsInternal.Add(gearId);
        }

        public void ReplaceOwnedGearIds(IEnumerable<string> replacementGearIds)
        {
            if (replacementGearIds == null)
            {
                throw new ArgumentNullException(nameof(replacementGearIds));
            }

            List<string> normalizedGearIds = new List<string>();
            foreach (string replacementGearId in replacementGearIds)
            {
                if (string.IsNullOrWhiteSpace(replacementGearId))
                {
                    throw new ArgumentException(
                        "Replacement gear ids cannot contain null or whitespace entries.",
                        nameof(replacementGearIds));
                }

                normalizedGearIds.Add(replacementGearId);
            }

            ownedGearIds = normalizedGearIds;
        }
    }
}

