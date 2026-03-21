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

        public PersistentLoadoutState()
        {
        }

        public PersistentLoadoutState(string loadoutId = "", IEnumerable<EquippedGearState> equippedGearStates = null)
        {
            this.loadoutId = loadoutId ?? string.Empty;
            ReplaceEquippedGearStates(equippedGearStates ?? Array.Empty<EquippedGearState>());
        }

        private List<EquippedGearState> EquippedGearStatesInternal => equippedGearStates ??= new List<EquippedGearState>();

        public string LoadoutId => loadoutId ?? string.Empty;

        public IReadOnlyList<EquippedGearState> EquippedGearStates => EquippedGearStatesInternal;

        public void ReplaceEquippedGearStates(IEnumerable<EquippedGearState> replacementEquippedGearStates)
        {
            if (replacementEquippedGearStates == null)
            {
                throw new ArgumentNullException(nameof(replacementEquippedGearStates));
            }

            List<EquippedGearState> normalizedEquippedGearStates = new List<EquippedGearState>();
            foreach (EquippedGearState replacementEquippedGearState in replacementEquippedGearStates)
            {
                if (replacementEquippedGearState == null)
                {
                    throw new ArgumentException(
                        "Replacement equipped gear states cannot contain null entries.",
                        nameof(replacementEquippedGearStates));
                }

                normalizedEquippedGearStates.Add(replacementEquippedGearState);
            }

            equippedGearStates = normalizedEquippedGearStates;
        }
    }
}

