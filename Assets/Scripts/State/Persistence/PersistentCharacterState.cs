using System;
using UnityEngine;
using Survivalon.Data.Combat;

namespace Survivalon.State.Persistence
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

        public PersistentCharacterState()
        {
        }

        public PersistentCharacterState(
            string characterId,
            bool isUnlocked,
            bool isSelectable,
            bool isActive,
            int progressionRank = 0,
            CombatStatBlockData persistentStatModifiers = null,
            string skillPackageId = "",
            PersistentLoadoutState loadoutState = null)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (progressionRank < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(progressionRank), "Progression rank cannot be negative.");
            }

            this.characterId = characterId;
            this.isUnlocked = isUnlocked;
            this.isSelectable = isSelectable;
            this.isActive = isActive;
            this.progressionRank = progressionRank;
            this.persistentStatModifiers = persistentStatModifiers ?? new CombatStatBlockData();
            this.skillPackageId = skillPackageId ?? string.Empty;
            this.loadoutState = loadoutState ?? new PersistentLoadoutState();
        }

        public string CharacterId => characterId;

        public bool IsUnlocked => isUnlocked;

        public bool IsSelectable => isSelectable;

        public bool IsActive => isActive;

        public int ProgressionRank => progressionRank;

        public CombatStatBlockData PersistentStatModifiers => persistentStatModifiers;

        public string SkillPackageId => skillPackageId;

        public PersistentLoadoutState LoadoutState => loadoutState;

        public void Unlock()
        {
            isUnlocked = true;
        }

        public void SetSelectable(bool selectable)
        {
            isSelectable = selectable;
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }

        public void IncreaseProgressionRank(int amount = 1)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Progression rank increase must be positive.");
            }

            progressionRank += amount;
        }
    }
}

