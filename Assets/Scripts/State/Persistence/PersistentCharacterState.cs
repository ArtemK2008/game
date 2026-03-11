using System;
using UnityEngine;

namespace Survivalon.Runtime
{
    [Serializable]
    public sealed class PersistentCharacterState
    {
        [SerializeField]
        private string characterId = string.Empty;

        [SerializeField]
        private bool isUnlocked;

        [SerializeField]
        private bool isSelectable;

        [SerializeField]
        private bool isActive;

        [SerializeField]
        private int progressionRank;

        [SerializeField]
        private CombatStatBlockData persistentStatModifiers = new CombatStatBlockData();

        [SerializeField]
        private string skillPackageId = string.Empty;

        [SerializeField]
        private PersistentLoadoutState loadoutState = new PersistentLoadoutState();

        public string CharacterId => characterId;

        public bool IsUnlocked => isUnlocked;

        public bool IsSelectable => isSelectable;

        public bool IsActive => isActive;

        public int ProgressionRank => progressionRank;

        public CombatStatBlockData PersistentStatModifiers => persistentStatModifiers;

        public string SkillPackageId => skillPackageId;

        public PersistentLoadoutState LoadoutState => loadoutState;
    }
}
