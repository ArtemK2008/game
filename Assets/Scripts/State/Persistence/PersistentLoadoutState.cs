using System;
using System.Collections.Generic;
using UnityEngine;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
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
