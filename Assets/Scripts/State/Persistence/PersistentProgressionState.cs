using System;
using System.Collections.Generic;
using UnityEngine;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.State.Persistence
{
    [Serializable]
    public sealed class PersistentProgressionState
    {
        [SerializeField]
        private List<ProgressionEntryState> entries = new List<ProgressionEntryState>();

        public IReadOnlyList<ProgressionEntryState> Entries => entries;

        public bool TryGetEntry(string progressionId, out ProgressionEntryState entry)
        {
            if (string.IsNullOrWhiteSpace(progressionId))
            {
                throw new ArgumentException("Progression id cannot be null or whitespace.", nameof(progressionId));
            }

            entry = entries.Find(candidate => candidate.ProgressionId == progressionId);
            return entry != null;
        }

        public ProgressionEntryState GetOrAddEntry(string progressionId, ProgressionLayerType layerType)
        {
            if (string.IsNullOrWhiteSpace(progressionId))
            {
                throw new ArgumentException("Progression id cannot be null or whitespace.", nameof(progressionId));
            }

            if (TryGetEntry(progressionId, out ProgressionEntryState existingEntry))
            {
                if (existingEntry.LayerType != layerType)
                {
                    throw new InvalidOperationException(
                        $"Existing progression entry '{progressionId}' uses layer '{existingEntry.LayerType}' instead of '{layerType}'.");
                }

                return existingEntry;
            }

            ProgressionEntryState newEntry = new ProgressionEntryState(
                progressionId,
                layerType,
                isUnlocked: false,
                currentValue: 0);
            entries.Add(newEntry);
            return newEntry;
        }
    }
}
