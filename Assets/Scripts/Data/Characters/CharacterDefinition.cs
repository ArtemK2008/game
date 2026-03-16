using System.Collections.Generic;
using UnityEngine;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.Data.Characters
{
    [CreateAssetMenu(
        fileName = "CharacterDefinition",
        menuName = "Survivalon/Data/Characters/Character Definition")]
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
