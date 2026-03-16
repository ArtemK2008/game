using UnityEngine;
using Survivalon.Runtime.Data.Combat;

namespace Survivalon.Runtime.Data.Gear
{
    [CreateAssetMenu(
        fileName = "GearDefinition",
        menuName = "Survivalon/Data/Gear/Gear Definition")]
    public sealed class GearDefinition : ScriptableObject
    {
        [SerializeField]
        private string gearId = string.Empty;

        [SerializeField]
        private string displayName = string.Empty;

        [SerializeField]
        private GearCategory gearCategory = GearCategory.PrimaryCombat;

        [SerializeField]
        private CombatStatBlockData statModifiers = new CombatStatBlockData();

        public string GearId => gearId;

        public string DisplayName => displayName;

        public GearCategory GearCategory => gearCategory;

        public CombatStatBlockData StatModifiers => statModifiers;
    }
}
