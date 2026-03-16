using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class PersistentLoadoutState
    {
        [SerializeField]
        private string loadoutId = string.Empty;

        [SerializeField]
        private List<EquippedGearState> equippedGearStates = new List<EquippedGearState>();

        public string LoadoutId => loadoutId;

        public IReadOnlyList<EquippedGearState> EquippedGearStates => equippedGearStates;
    }
}

