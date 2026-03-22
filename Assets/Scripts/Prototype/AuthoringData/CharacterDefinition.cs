using System.Collections.Generic;
using UnityEngine;
using Survivalon.Data.Combat;
using Survivalon.Data.Gear;

namespace Survivalon.Prototype.AuthoringData
{
    /// <summary>
    /// Хранит dormant prototype-описание персонажа для неиспользуемого authoring slice.
    /// </summary>
    [CreateAssetMenu(
        fileName = "CharacterDefinition",
        menuName = "Survivalon/Prototype/Authoring/Character Definition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField]
        private string characterId = string.Empty;

        [SerializeField]
        private string displayName = string.Empty;

        [SerializeField]
        private CombatStatBlockData baseStats = new CombatStatBlockData();

        [SerializeField]
        private string defaultSkillPackageId = string.Empty;

        [SerializeField]
        private List<GearCategory> supportedGearCategories = new List<GearCategory>();

        public string CharacterId => characterId;

        public string DisplayName => displayName;

        public CombatStatBlockData BaseStats => baseStats;

        public string DefaultSkillPackageId => defaultSkillPackageId;

        public IReadOnlyList<GearCategory> SupportedGearCategories => supportedGearCategories;
    }
}

