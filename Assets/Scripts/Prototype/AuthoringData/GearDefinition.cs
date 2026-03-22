using UnityEngine;
using Survivalon.Data.Combat;
using Survivalon.Data.Gear;

namespace Survivalon.Prototype.AuthoringData
{
    /// <summary>
    /// Хранит dormant prototype-описание gear для неиспользуемого authoring slice.
    /// </summary>
    [CreateAssetMenu(
        fileName = "GearDefinition",
        menuName = "Survivalon/Prototype/Authoring/Gear Definition")]
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

