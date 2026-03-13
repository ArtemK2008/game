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
    }
}
