using System;
using UnityEngine;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
{
    [Serializable]
    public sealed class ProgressionEntryState
    {
        [SerializeField]
        private string progressionId = string.Empty;

        [SerializeField]
        private ProgressionLayerType layerType = ProgressionLayerType.AccountWide;

        [SerializeField]
        private bool isUnlocked;

        [SerializeField]
        private int currentValue;

        public ProgressionEntryState()
        {
        }

        public ProgressionEntryState(
            string progressionId,
            ProgressionLayerType layerType,
            bool isUnlocked,
            int currentValue)
        {
            if (string.IsNullOrWhiteSpace(progressionId))
            {
                throw new ArgumentException("Progression id cannot be null or whitespace.", nameof(progressionId));
            }

            if (currentValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentValue), "Progression value cannot be negative.");
            }

            this.progressionId = progressionId;
            this.layerType = layerType;
            this.isUnlocked = isUnlocked;
            this.currentValue = currentValue;
        }

        public string ProgressionId => progressionId;

        public ProgressionLayerType LayerType => layerType;

        public bool IsUnlocked => isUnlocked;

        public int CurrentValue => currentValue;

        public void Unlock()
        {
            isUnlocked = true;
        }

        public void IncreaseValue(int delta)
        {
            if (delta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delta), "Progression value delta cannot be negative.");
            }

            if (delta == 0)
            {
                return;
            }

            currentValue += delta;
        }
    }
}
