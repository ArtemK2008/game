using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivalon.Runtime
{
    [Serializable]
    public sealed class PersistentProgressionState
    {
        [SerializeField]
        private List<ProgressionEntryState> entries = new List<ProgressionEntryState>();

        public IReadOnlyList<ProgressionEntryState> Entries => entries;
    }
}
