using System;
using UnityEngine;
using Survivalon.Data.Gear;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class EquippedGearState
    {
        [SerializeField]
        private string gearId = string.Empty;

        [SerializeField]
        private GearCategory gearCategory = GearCategory.PrimaryCombat;

        public EquippedGearState()
        {
        }

        public EquippedGearState(string gearId, GearCategory gearCategory)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            this.gearId = gearId;
            this.gearCategory = gearCategory;
        }

        public string GearId => gearId;

        public GearCategory GearCategory => gearCategory;
    }
}

